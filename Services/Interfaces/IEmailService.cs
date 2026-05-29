namespace RedmineApp.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendRandomPassword(string toEmail, string password);
    }
}
