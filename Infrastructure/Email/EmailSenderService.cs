using System.Net;
using System.Net.Mail;
using Account.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Account.Infrastructure.Email;

public class EmailSenderService : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailSenderService> _logger;

    public EmailSenderService(IConfiguration configuration, ILogger<EmailSenderService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendVerificationLinkAsync(string recipientEmail, string token, CancellationToken ct = default)
    {
        var clientUrl = _configuration["Mail:ClientUrl"] ?? "http://localhost:3000";
        var verificationUrl = clientUrl.TrimEnd('/') + "/verify-email?token=" + token;
        var smtpUser = _configuration["Mail:Username"];
        var smtpPass = _configuration["Mail:Password"];
        var host = _configuration["Mail:Host"] ?? "smtp.gmail.com";
        var port = int.TryParse(_configuration["Mail:Port"], out var p) ? p : 587;

        if (string.IsNullOrWhiteSpace(smtpUser) || string.IsNullOrWhiteSpace(smtpPass))
        {
            EmailVerificationTemplate.LogDevFallback(_logger, recipientEmail, verificationUrl, "No SMTP username or password configured");
            return;
        }

        try
        {
            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(smtpUser, "Nyxoris Tarot"),
                Subject = "✦ Kích hoạt tài khoản Nyxoris Tarot của bạn ✦",
                Body = EmailVerificationTemplate.BuildHtmlTemplate(verificationUrl),
                IsBodyHtml = true
            };
            mail.To.Add(recipientEmail);

            await client.SendMailAsync(mail, ct);
            _logger.LogInformation("Verification email sent to {Email}", recipientEmail);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send email via SMTP ({Message}). Falling back to console log.", ex.Message);
            EmailVerificationTemplate.LogDevFallback(_logger, recipientEmail, verificationUrl, ex.Message);
        }
    }
}
