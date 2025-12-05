using AspnetCoreRestApi.Helpers.Interfaces;

namespace AspnetCoreRestApi.Helpers
{
    public class EmailService : IEmailService
    {
        public void SendGettingStartedEmail(string toEmail, string userName)
        {
            Console.WriteLine($"This will send getting started email to {toEmail} welcoming {userName} to the platform.");
        }

        public void SendWelcomeEmail(string toEmail, string userName)
        {
            Console.WriteLine($"This will send an email to {toEmail} welcoming {userName} to the platform.");
        }
    }
}
