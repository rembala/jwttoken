namespace AspnetCoreRestApi.Helpers.Interfaces
{
    public interface IEmailService
    {
        void SendWelcomeEmail(string toEmail, string userName);

        void SendGettingStartedEmail(string toEmail, string userName);
    }
}
