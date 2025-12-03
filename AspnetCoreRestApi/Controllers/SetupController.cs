using AspnetCoreRestApi.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetupController : ControllerBase
    {
        private readonly ApiDbContext _context;

        private readonly UserManager<IdentityUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly ILogger<SetupController> _logger;

        public SetupController(ApiDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<SetupController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet("GetAllRoles")]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.ToList();

            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string name)
        {
            var roleExist = await _roleManager.RoleExistsAsync(name);

            if (!roleExist)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return Ok($"Role {name} created successfully");
                }
                else
                {
                    return BadRequest("Error creating role");
                }
            }

            return BadRequest("Role already exists");
        }

        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            return Ok(users);
        }

        [HttpPost("AddUserToRole")]
        public async Task<IActionResult> AddUserToRole(string userEmail, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                return NotFound("Role not found");
            }
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return Ok($"User {userEmail} added to role {roleName} successfully");
            }
            else
            {
                return BadRequest("Error adding user to role");
            }
        }

        [HttpGet("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }

        [HttpPost]
        [Route("RemoveUserFromRole")]
        public async Task<IActionResult> RemoveUserFromRole(string userEmail, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                return NotFound("Role not found");
            }
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return Ok($"User {userEmail} removed from role {roleName} successfully");
            }
            else
            {
                return BadRequest("Error removing user from role");
            }
        }
    }
}