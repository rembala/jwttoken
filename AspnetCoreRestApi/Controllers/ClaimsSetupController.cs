using AspnetCoreRestApi.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AspnetCoreRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsSetupController
    {
        private readonly ApiDbContext _context;

        private readonly UserManager<IdentityUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly ILogger<SetupController> _logger;

        public ClaimsSetupController(ApiDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<SetupController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet("GetAllClaims")]
        public async Task<IActionResult> GetAllClaims(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new NotFoundObjectResult("User not found");
            }

            var claims = await _userManager.GetClaimsAsync(user);

            return new OkObjectResult(claims);
        }

        [HttpPost("AddClaimToUser")]
        public async Task<IActionResult> AddClaimToUser(string email, string claimType, string claimValue)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new NotFoundObjectResult("User not found");
            }
            var claim = new Claim(claimType, claimValue);
            var result = await _userManager.AddClaimAsync(user, claim);
            if (result.Succeeded)
            {
                return new OkObjectResult($"Claim {claimType} added to user {email} successfully");
            }
            else
            {
                return new BadRequestObjectResult("Error adding claim to user");
            }
        }
    }
}
