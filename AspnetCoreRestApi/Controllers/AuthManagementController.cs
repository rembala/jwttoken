using AspnetCoreRestApi.Configurations;
using AspnetCoreRestApi.Data;
using AspnetCoreRestApi.DTOs.Requests;
using AspnetCoreRestApi.DTOs.Responses;
using AspnetCoreRestApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AspnetCoreRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthManagementController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        private readonly JwtConfig _jwtConfig;

        private readonly TokenValidationParameters _tokenValidationParameters;

        private readonly ApiDbContext _context;

        public AuthManagementController(
            UserManager<IdentityUser> userManager,
            IOptionsMonitor<JwtConfig> jwtConfig, 
            TokenValidationParameters tokenValidationParameters,
            ApiDbContext apiDbContext)
        {
            _userManager = userManager;
            _jwtConfig = jwtConfig.CurrentValue;
            _tokenValidationParameters = tokenValidationParameters;
            _context = apiDbContext;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTo user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);
                if (existingUser != null)
                {
                    return BadRequest(new RegistrationResponse
                    {
                        Success = true,
                        Errors = new List<string>
                        {
                            "Email already in use"
                        }
                    });
                }
                var newUser = new IdentityUser
                {
                    Email = user.Email,
                    UserName = user.UserName
                };
                var isCreated = await _userManager.CreateAsync(newUser, user.Password);
                if (isCreated.Succeeded)
                {
                    var jwtToken = await GenerateJwtToken(newUser);

                    return Ok(jwtToken);
                }
                else
                {
                    return BadRequest(new RegistrationResponse
                    {
                        Success = false,
                        Errors = isCreated.Errors.Select(x => x.Description).ToList()
                    });
                }
            }

            return BadRequest(new RegistrationResponse
            {
                Success = false,
                Errors = new List<string>
                {
                    "Invalid payload"
                }
            });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);

                if (existingUser == null)
                {
                     return BadRequest(new RegistrationResponse
                     {
                          Success = false,
                          Errors = new List<string>
                          {
                            "Invalid login request"
                          }
                     });
                }

                var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);

                if (!isCorrect)
                {
                    return BadRequest(new RegistrationResponse
                    {
                        Success = false,
                        Errors = new List<string>
                        {
                            "Invalid login request"
                        }
                    });
                }

                var jwtToken = await GenerateJwtToken(existingUser);

                return Ok(jwtToken);
            }
            return BadRequest(new RegistrationResponse
            {
                Success = false,
                Errors = new List<string>
                {
                    "Invalid payload"
                }
            });
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            if (ModelState.IsValid)
            {
               var result = await VerifyAndGenerateToken(tokenRequest);
               if (result == null)
               {
                   return BadRequest(new RegistrationResponse
                   {
                       Success = false,
                       Errors = new List<string>
                       {
                           "Something went wrong"
                       }
                   });
                }

                return Ok(result);
            }

            return BadRequest(new RegistrationResponse
            {
                Success = false,
                Errors = new List<string>
                {
                    "Invalid payload"
                }
            });
        }

        private async Task<AuthResult> VerifyAndGenerateToken(TokenRequest tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                _tokenValidationParameters.ValidateLifetime = false;
                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (result == false)
                    {
                        return null;
                    }
                }
                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

                if (expiryDate > DateTime.UtcNow)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new List<string>
                        {
                            "Token has not yet expired"
                        }
                    };
                }
                var storedRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);
                if (storedRefreshToken == null)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new List<string>
                        {
                            "Refresh token does not exist"
                        }
                    };
                }

                if (storedRefreshToken.IsUsed)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new List<string>
                        {
                            "Refresh token has been used"
                        }
                    };
                }

                if (storedRefreshToken.IsRevoked)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new List<string>
                        {
                            "Refresh token has been revoked"
                        }
                    };
                }
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedRefreshToken.JwtId != jti)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new List<string>
                        {
                            "Token doesn't match refresh token"
                        }
                    };
                }
                storedRefreshToken.IsUsed = true;
                _context.RefreshTokens.Update(storedRefreshToken);
                await _context.SaveChangesAsync();
                var user = await _userManager.FindByIdAsync(storedRefreshToken.UserId);
                return await GenerateJwtToken(user);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private DateTime UnixTimeStampToDateTime(long utcExpiryDate)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(utcExpiryDate).ToUniversalTime();

            return dateTimeVal;
        }

        private async Task<AuthResult> GenerateJwtToken(IdentityUser user)
        {
           var jwtTokenHandler = new JwtSecurityTokenHandler();
           var key = System.Text.Encoding.ASCII.GetBytes(_jwtConfig.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                   new Claim("Id", user.Id),
                   new Claim(JwtRegisteredClaimNames.Email, user.Email),
                   new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                   new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
               }),
                Expires = DateTime.UtcNow.AddSeconds(30),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                 )
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevoked = false,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Token = RandomString(35) + Guid.NewGuid()
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResult
            {
                Token = jwtToken,
                Success = true,
                RefreshToken = refreshToken.Token
            };
        }

        private string RandomString(int length)
        {
           var random = new Random();
           const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
           return new string(Enumerable.Repeat(chars, length)
                 .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}