using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;

namespace ASP_Lesson_14.Services
{
    public class EmailSenderCart : IEmailSenderCart
    {
        private readonly IConfiguration configuration;
        public EmailSenderCart(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task SendAsync(string from, string to, string subject, string body)
        {
            var email = configuration["Email:SmtpEmail"];
            var apppassword = configuration["Email:AppPassword"];
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(apppassword))
            {
                throw new InvalidOperationException("Email or AppPassword is not configured.");
            }
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(email));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };
            using var client = new SmtpClient();
            await client.ConnectAsync(configuration["Email:Host"]??"smtp.gmail.com", int.TryParse(configuration["Email:Port"], out var port)?port:465, SecureSocketOptions.SslOnConnect);
            try
            {
                await client.AuthenticateAsync(email, apppassword);
                await client.SendAsync(message);
            }
            finally
            {
                if (client.IsConnected)

                    await client.DisconnectAsync(true);

            }    
        }
    }
}
