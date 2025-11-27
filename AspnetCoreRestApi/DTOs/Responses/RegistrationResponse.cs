using AspnetCoreRestApi.Configurations;

namespace AspnetCoreRestApi.DTOs.Responses
{
    public class RegistrationResponse : AuthResult
    {
        public bool Success { get; internal set; }
    }
}
