using Azure;
using Azure.Communication.Email;
using RedmineApp.Services.Interfaces;

namespace RedmineApp.Services.Implement
{
    public class EmailService : IEmailService
    {
        private readonly string _connectionString;
        public EmailService(IConfiguration config)
        {
            _connectionString = config["AzureEmailConnectionString"];
        }
        public async Task SendRandomPassword(string toEmail, string password)
        {
            var client = new EmailClient(_connectionString);
            var message = new EmailMessage(
                "",
                toEmail,
                new EmailContent("Mật khẩu mới của bạn") { Html = $"Mật khẩu: {password}" }
            );
            await client.SendAsync(WaitUntil.Started, message);
        }
    }
}
