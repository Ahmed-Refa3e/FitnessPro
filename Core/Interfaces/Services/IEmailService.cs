namespace Core.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string body);
        public string GenerateVerificatonCode(int length = 6);
    }
}
