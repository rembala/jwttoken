using AspnetCoreRestApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreRestApi.Controllers.V2
{
    [ApiController]
    //[Route("api/v{version:apiVersion}/users")]
    [Route("api/users")]
    [ApiVersion("2.0")]
    public class UsersController : ControllerBase
    {
        [HttpGet()]
        public IActionResult AllUsers()
        {
            var users = new List<UserV2>             {
                new UserV2 { Id = Guid.NewGuid(), Name = "Alice" },
                new UserV2 { Id = Guid.NewGuid(), Name = "Bob" },
                new UserV2 { Id = Guid.NewGuid(), Name = "Charlie" }
            };

            return new OkObjectResult(users);
        }
    }
}
