using AspnetCoreRestApi.Core.IConfiguration;
using AspnetCoreRestApi.Models.DbSetModels;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private ILogger<UserController> _logger;

        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork, ILogger<UserController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            try
            {
                user.Id = Guid.NewGuid();
                var createdUser = await _unitOfWork.UserRepository.AddAsync(user);
                await _unitOfWork.SaveAsync();
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new user.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the user with ID: {UserId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _unitOfWork.UserRepository.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all users.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            try
            {
                var updatedUser = await _unitOfWork.UserRepository.UpdateAsync(user);
                await _unitOfWork.SaveAsync();
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user with ID: {UserId}", user.Id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var result = await _unitOfWork.UserRepository.DeleteAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                await _unitOfWork.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the user with ID: {UserId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
