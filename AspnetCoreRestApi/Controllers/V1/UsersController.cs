using AspnetCoreRestApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreRestApi.Controllers.V1
{
    [ApiController]
    //[Route("api/v{version:apiVersion}/users")]
    [Route("api/users")]
    [ApiVersion("1.0")]
    public class UsersController
    {
        [HttpGet()]
        public IActionResult AllUsers()
        {
            var users = new List<UserV1>             {
                new UserV1 { Id = 1, Name = "Alice" },
                new UserV1 { Id = 2, Name = "Bob" },
                new UserV1 { Id = 3, Name = "Charlie" }
            };

           return new OkObjectResult(users);
        }
    }
}
