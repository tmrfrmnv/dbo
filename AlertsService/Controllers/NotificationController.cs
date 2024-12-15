using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using AlertsService.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace AlertsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly EmailSettings _emailSettings;  // Добавляем поле для настроек почты

        public NotificationController(IEmailService emailService, IOptions<EmailSettings> emailSettings)
        {
            _emailService = emailService;
            _emailSettings = emailSettings.Value; // Получаем настройки почты
        }

        [HttpPost("SendEmailWithAttachment")]
        [SwaggerOperation(
        Summary = "Уведомление с вложением",
        Description = "Отправляет письмо на указанный адрес, с пользовательской темой сообщения и вложенными файлами")]
        public async Task<IActionResult> SendEmailWithAttachment(string email, string subject, string message, IFormFile file)
        {
            try
            {
                Response.Cookies.Append("LastEmail", email, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(1), // Пример срока действия куки — 1 день
                    HttpOnly = true,  // Это сделает куки доступными только через HTTP (не через JavaScript)
                    Secure = true     // Только через HTTPS
                });
                // Проверка, если файл прикреплен
                if (file != null && file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        var attachment = new Attachment(new MemoryStream(memoryStream.ToArray()), file.FileName);
                        await _emailService.SendEmailWithAttachmentAsync(email, subject, message, attachment);
                    }
                }
                else
                {
                    // Если файла нет, отправляем просто текстовое письмо
                    await _emailService.SendEmailAsync(email, subject, message);
                }

                return Ok("Сообщение отправлено.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при отправке сообщения: {ex.Message}");
            }
        }

        [HttpGet("GetEmailSettings")]
        [SwaggerOperation(
        Summary = "Вывод настроек отправителя",
        Description = "Вывод текущих настроек почты, с которой посылаются запросы")]
        public IActionResult GetEmailSettings()
        {
            // Возвращаем текущие настройки почты
            var emailSettings = new
            {
                SmtpServer = _emailSettings.SmtpServer,
                Port = _emailSettings.Port,
                UseSSL = _emailSettings.UseSSL,
                SenderEmail = _emailSettings.SenderEmail,
                SenderName = _emailSettings.SenderName
            };

            return Ok(emailSettings);
        }
        [HttpPost("UpdateEmailSettings")]
        [SwaggerOperation(
        Summary = "Изменение данных отправителя",
        Description = "Обновляет данные аккаунта почты, с которого посылаются запросы")]
        public IActionResult UpdateEmailSettings([FromBody] EmailSettings newSettings)
        {
            try
            {
                // Обновляем настройки почты
                _emailSettings.SmtpServer = newSettings.SmtpServer;
                _emailSettings.Port = newSettings.Port;
                _emailSettings.UseSSL = newSettings.UseSSL;
                _emailSettings.SenderEmail = newSettings.SenderEmail;
                _emailSettings.SenderName = newSettings.SenderName;
                _emailSettings.Username = newSettings.Username;
                _emailSettings.Password = newSettings.Password;



                return Ok("Настройки почты успешно обновлены.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при обновлении настроек: {ex.Message}");
            }
        }
        [HttpPost("SendNotification")]
        [SwaggerOperation(
        Summary = "Уведомление",
        Description = "Отправка обычного сообщения с заранее подготовленной темой в самом сервисе")]
        public async Task<IActionResult> SendNotification(string email, string message)
        {
            string subject = "Уведомление";

            // Сохраняем email в куки
            Response.Cookies.Append("LastEmail", email, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(1), // Пример срока действия куки — 1 день
                HttpOnly = true,  // Это сделает куки доступными только через HTTP (не через JavaScript)
                Secure = true     // Только через HTTPS
            });

            // Отправляем email
            await _emailService.SendEmailAsync(email, subject, message);

            return Ok("Сообщение отправлено.");
        }

        [HttpGet("GetLastEmail")]
        [SwaggerOperation(
        Summary = "Последний получатель",
        Description = "Вывод последнего Email, на которое было отправлено письмо")]
        public IActionResult GetLastEmail()
        {
            // Получаем LastEmail из куки
            var lastEmail = Request.Cookies["LastEmail"];

            // Если LastEmail не найден в куки, возвращаем "Empty"
            if (string.IsNullOrEmpty(lastEmail))
            {
                return Ok("Empty");
            }

            return Ok(lastEmail);
        }

        [HttpGet("GetSentEmailsCount")]
        [SwaggerOperation(
        Summary = "Всего уведомлений",
        Description = "Вывод общего числа отправленных писем за всё время")]
        public IActionResult GetSentEmailsCount()
        {
            // Получаем количество отправленных сообщений из EmailService
            var sentEmailsCount = _emailService.GetSentEmailsCount();

            return Ok(sentEmailsCount);
        }
    }
}
