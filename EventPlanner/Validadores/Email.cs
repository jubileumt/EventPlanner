using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace EventPlanner.Validadores
{
    public sealed class Email
    {
        public string Provedor { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }

        public Email(string provedor, string userName, string password)
        {
            Provedor = provedor ?? throw new ArgumentNullException(nameof(provedor));
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }

        public void SendEmail(List<string> emaillist, string subject, string body)
        {

            var message = PrepareMessage(emaillist, subject, body);
            SendEmailBySmtp(message);
        }

        private MailMessage PrepareMessage(List<string> emaillist, string subject, string body)
        {
            var mail = new MailMessage();
            mail.From = new MailAddress(UserName);

            foreach (var email in emaillist)
            {
                if (ValidateEmail(email))
                {
                    mail.To.Add(email);
                }
            }
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            return mail;
        }

        private bool ValidateEmail(string email)
        {
            Regex expression = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            if (expression.IsMatch(email))
            {
                return true;
            }
            return false;
        }

        private void SendEmailBySmtp(MailMessage message)
        {
            SmtpClient smtpCliente = new SmtpClient();
            smtpCliente.Host = Provedor;
            smtpCliente.Port = 587;
            smtpCliente.EnableSsl = true;
            smtpCliente.Timeout = 50000;
            smtpCliente.UseDefaultCredentials = false;
            smtpCliente.Credentials = new NetworkCredential(UserName, Password);
            smtpCliente.Send(message);
            smtpCliente.Dispose();

        }
    }
}