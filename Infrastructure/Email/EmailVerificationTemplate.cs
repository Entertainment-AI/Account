using Microsoft.Extensions.Logging;

namespace Account.Infrastructure.Email;

public static class EmailVerificationTemplate
{
    public static void LogDevFallback(ILogger logger, string email, string url, string reason)
    {
        logger.LogInformation(@"
================================================================================
[NYXORIS ACCOUNT DEV EMAIL VERIFICATION LINK]
To: {Email}
Reason: {Reason}
--------------------------------------------------------------------------------
🔗 CLICK LINK TO VERIFY ACCOUNT DIRECTLY:
{Url}
================================================================================
", email, reason, url);
    }

    public static string BuildPlainText(string verificationUrl)
    {
        return $@"Xác thực tài khoản Nyxoris

Xin chào,

Bạn nhận được email này vì đã đăng ký tài khoản tại Nyxoris. Vui lòng truy cập liên kết sau để xác thực tài khoản của bạn:

{verificationUrl}

Liên kết này có hiệu lực trong vòng 24 giờ.
Nếu bạn không yêu cầu tạo tài khoản Nyxoris, vui lòng bỏ qua email này.

---
Đây là email tự động, vui lòng không phản hồi lại thư này.";
    }

    public static string BuildHtmlTemplate(string verificationUrl)
    {
        return $@"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"" lang=""vi"">
<head>
  <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
  <title>Xác thực tài khoản Nyxoris</title>
</head>
<body style=""margin:0; padding:0; background-color:#ffffff; font-family:-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif; -webkit-font-smoothing:antialiased; color:#1f2937;"">
  <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color:#ffffff; padding:40px 20px;"">
    <tr>
      <td align=""center"">
        <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width:520px; text-align:left;"">
          
          <!-- Heading -->
          <tr>
            <td style=""padding-bottom:16px;"">
              <h1 style=""margin:0; font-size:22px; font-weight:600; color:#111827; letter-spacing:-0.3px; line-height:1.3;"">
                Xác thực tài khoản
              </h1>
            </td>
          </tr>

          <!-- Body Text -->
          <tr>
            <td style=""padding-bottom:28px; font-size:15px; line-height:1.6; color:#374151;"">
              Xin chào,<br/><br/>
              Bạn nhận được email này vì đã đăng ký tài khoản tại <strong style=""color:#111827;"">Nyxoris</strong>. Vui lòng xác thực địa chỉ email để hoàn tất quá trình thiết lập tài khoản:
            </td>
          </tr>

          <!-- CTA Button -->
          <tr>
            <td style=""padding-bottom:28px;"">
              <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                  <td align=""center"" style=""border-radius:6px; background-color:#111827;"">
                    <a href=""{verificationUrl}"" target=""_blank"" style=""display:inline-block; padding:12px 24px; font-size:14px; font-weight:500; color:#ffffff; text-decoration:none; border-radius:6px;"">
                      Xác thực tài khoản
                    </a>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Expiry Note -->
          <tr>
            <td style=""padding-bottom:28px; font-size:13px; line-height:1.6; color:#6b7280;"">
              Liên kết xác thực có hiệu lực trong vòng 24 giờ.<br/>
              Nếu bạn không yêu cầu tạo tài khoản Nyxoris, vui lòng bỏ qua email này.
            </td>
          </tr>

          <!-- Divider & Link -->
          <tr>
            <td style=""border-top:1px solid #e5e7eb; padding-top:20px;"">
              <p style=""margin:0 0 8px; font-size:12px; color:#6b7280; line-height:1.5;"">
                Nếu không thể nhấp vào nút bên trên, bạn có thể sao chép và dán liên kết sau vào trình duyệt:
              </p>
              <p style=""margin:0; font-size:12px; line-height:1.5; word-break:break-all;"">
                <a href=""{verificationUrl}"" target=""_blank"" style=""color:#2563eb; text-decoration:none;"">{verificationUrl}</a>
              </p>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style=""padding-top:28px; font-size:12px; color:#9ca3af; line-height:1.5;"">
              Đây là email tự động, vui lòng không phản hồi lại thư này.
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
    }
}
