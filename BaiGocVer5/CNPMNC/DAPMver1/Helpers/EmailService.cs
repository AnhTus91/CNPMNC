using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace DAPMver1.Services
{
    public class EmailService
    {
        private readonly string _email;
        private readonly string _password;

        public EmailService(IConfiguration configuration)
        {
            _email = configuration["EmailSettings:Email"];
            _password = configuration["EmailSettings:PasswordEmail"];
        }

        public bool SendConfirmationEmail(string toEmail, string token)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(_email, _password),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_email),
                    Subject = "Xác nhận đăng ký tài khoản",
                    Body = $"Vui lòng sử dụng mã xác nhận của bạn: {token}",
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(toEmail);

                smtpClient.Send(mailMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
