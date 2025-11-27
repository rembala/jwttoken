namespace AspnetCoreRestApi.Configurations
{
    public class AuthResult
    {
        public string Token { get; set; }

        public bool Sucess { get; set; }

        public List<string> Errors { get; set; }
    }
}
