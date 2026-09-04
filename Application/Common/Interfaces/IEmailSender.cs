namespace Account.Application.Common.Interfaces;

public interface IEmailSender
{
    Task SendVerificationLinkAsync(string recipientEmail, string token, CancellationToken ct = default);
}