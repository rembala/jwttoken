using System.ComponentModel.DataAnnotations;

namespace AspnetCoreRestApi.DTOs.Requests
{
    public class UserRegistrationDTo
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
