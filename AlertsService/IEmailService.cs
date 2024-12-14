using System.Net.Mail;

namespace AlertsService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string recipientEmail, string subject, string message);
        int GetSentEmailsCount();
        Task SendEmailWithAttachmentAsync(string recipientEmail, string subject, string message, Attachment attachment);
    }
}
