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

    public static string BuildHtmlTemplate(string verificationUrl)
    {
        return $@"
<!DOCTYPE html>
<html lang=""vi"">
<head>
  <meta charset=""UTF-8"">
  <style>
    body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #121316; color: #f4f4f5; margin: 0; padding: 0; }}
    .wrapper {{ max-width: 540px; margin: 30px auto; background-color: #191a1e; border: 1px solid #2d3039; border-radius: 20px; overflow: hidden; box-shadow: 0 10px 30px rgba(0,0,0,0.5); }}
    .header {{ background: linear-gradient(135deg, #23252e 0%, #17181c 100%); padding: 32px 24px; text-align: center; border-bottom: 1px solid #2d3039; }}
    .header h1 {{ margin: 0; font-size: 24px; color: #f4f4f5; font-weight: 700; letter-spacing: 0.5px; }}
    .header p {{ margin: 6px 0 0; font-size: 13px; color: #a1a1aa; }}
    .content {{ padding: 36px 28px; text-align: center; }}
    .badge {{ display: inline-block; background-color: rgba(245, 158, 11, 0.1); border: 1px solid rgba(245, 158, 11, 0.3); color: #fbbf24; font-size: 12px; font-weight: 600; padding: 4px 12px; border-radius: 9999px; margin-bottom: 16px; }}
    .btn-box {{ margin: 32px 0; text-align: center; }}
    .btn-verify {{ display: inline-block; background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%); color: #000000 !important; font-weight: 700; font-size: 14px; text-decoration: none; padding: 15px 36px; border-radius: 12px; letter-spacing: 0.5px; box-shadow: 0 4px 20px rgba(245, 158, 11, 0.35); }}
    .raw-link-box {{ background: #141518; border: 1px solid #272930; border-radius: 10px; padding: 12px; word-break: break-all; font-size: 11px; color: #a1a1aa; margin-top: 24px; }}
    .note {{ font-size: 13px; color: #9ca3af; line-height: 1.6; margin-top: 20px; }}
    .footer {{ background-color: #141518; padding: 20px 24px; text-align: center; border-top: 1px solid #252730; font-size: 11px; color: #71717a; }}
  </style>
</head>
<body>
  <div class=""wrapper"">
    <div class=""header"">
      <h1>✦ NYXORIS TAROT ✦</h1>
      <p>Nen Tang Boc Bai & Luan Giai Chiem Tinh Hoc 3D</p>
    </div>
    <div class=""content"">
      <div class=""badge"">XAC THUC EMAIL TAI KHOAN</div>
      <h2 style=""font-size: 20px; color: #ffffff; margin: 0 0 10px; font-weight: 700;"">Kich Hoat Tai Khoan Cua Ban</h2>
      <p style=""font-size: 13px; color: #d4d4d8; margin: 0; line-height: 1.6;"">Nhan vao nut ben duoi de xac thuc email va mo khoa toan quyen tro chuyen truc tiep cung AI Reader:</p>
      
      <div class=""btn-box"">
        <a href=""{verificationUrl}"" class=""btn-verify"" target=""_blank"">✦ KICH HOAT TAI KHOAN NGAY ✦</a>
      </div>

      <p class=""note"">Lien ket nay co hieu luc trong vong <strong>24 gio</strong>.<br>Neu nut tren khong hoat dong, ban co the sao chep lien ket ben duoi:</p>
      <div class=""raw-link-box"">{verificationUrl}</div>
    </div>
    <div class=""footer"">
      &copy; 2026 Nyxoris Tarot. Bao luu moi quyen.<br>Day la email tu dong, vui long khong tra loi thu nay.
    </div>
  </div>
</body>
</html>";
    }
}
