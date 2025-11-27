using AspnetCoreRestApi.Configurations;
using AspnetCoreRestApi.DTOs.Requests;
using AspnetCoreRestApi.DTOs.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AspnetCoreRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthManagementController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        private readonly JwtConfig _jwtConfig;

        public AuthManagementController(UserManager<IdentityUser> userManager, IOptionsMonitor<JwtConfig> jwtConfig)
        {
            _userManager = userManager;
            _jwtConfig = jwtConfig.CurrentValue;
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
                    var jwtToken = GenerateJwtToken(newUser);

                    return Ok(new RegistrationResponse
                    {
                        Token = jwtToken,
                        Success = true,
                    });
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

                var jwtToken = GenerateJwtToken(existingUser);
                return Ok(new RegistrationResponse
                {
                    Token = jwtToken,
                    Success = true,
                });
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

        private string GenerateJwtToken(IdentityUser user)
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
               Expires = DateTime.UtcNow.AddHours(6),
               SigningCredentials = new SigningCredentials(
                   new SymmetricSecurityKey(key),
                   SecurityAlgorithms.HmacSha256Signature
                )
           };

           var token = jwtTokenHandler.CreateToken(tokenDescriptor);

           return jwtTokenHandler.WriteToken(token);
        }
    }
}