using AlertsService.Models;
using AlertsService;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private static int _sentEmailsCount = 0; // Переменная для подсчета отправленных сообщений

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(string recipientEmail, string subject, string message)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };

        mailMessage.To.Add(recipientEmail);

        using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
        {
            Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
            EnableSsl = _emailSettings.UseSSL
        };

        try
        {
            await smtpClient.SendMailAsync(mailMessage);

            // Увеличиваем счетчик отправленных сообщений после успешной отправки
            _sentEmailsCount++;
        }
        catch (Exception ex)
        {
            // Обработка ошибок
            Console.WriteLine($"Ошибка отправки почты: {ex.Message}");
            throw;
        }
    }
    public async Task SendEmailWithAttachmentAsync(string recipientEmail, string subject, string message, Attachment attachment)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };

        mailMessage.To.Add(recipientEmail);

        // Добавляем вложение, если оно есть
        if (attachment != null)
        {
            mailMessage.Attachments.Add(attachment);
        }

        using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
        {
            Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
            EnableSsl = _emailSettings.UseSSL
        };

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
            _sentEmailsCount++;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка отправки почты: {ex.Message}");
            throw;
        }
    }

    public int GetSentEmailsCount()
    {
        return _sentEmailsCount;
    }
}
