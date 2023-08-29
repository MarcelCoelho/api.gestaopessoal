
using api.gestaopessoal.Models.Configuration;
using api.gestaopessoal.Models.Email;
using MongoDB.Bson.IO;
using System.Net;
using System.Net.Mail;

namespace api.gestaopessoal.Services.Email
{
    public class EmailService : IEmailService
    {

        private readonly IConfigSettings _configSettings;

        public EmailService(IConfigSettings configSettings)
        {
            _configSettings = configSettings;
        }

        public void Send(EmailTransaction transaction)
        {
            var task = Task.Run(() => Execute(transaction));
        }

        private void Execute(EmailTransaction transaction)
        {
            // Configurações do servidor SMTP
            string smtpHost = _configSettings.smtpHost;
            int smtpPort = _configSettings.smtpPort;
            string smtpUsername = _configSettings.smtpUsername;
            string smtpPassword = _configSettings.smtpSecret;

            // Configurações do e-mail
            string loginDate = transaction.Title == EmailType.Login ? transaction.Date.ToString("dd/MM/yyyy HH:mm:ss") : "";

            string fromEmail = _configSettings.smtpUsername;
            string toEmail = _configSettings.toEmail;
            string subject = "Financa - " + GetTitleEmail(transaction.Title);
            string body;

            if (string.IsNullOrEmpty(transaction.Observation))
            {
                body =
                "<html>" +
                "           <body>" +
                "             <p>Transação realizada por:&nbsp<strong>" + transaction.Owner + "</strong></p></br>" +
                "             <p>Produto: <strong>" + transaction.Product + "</strong></p>" +
                "             <p>Data: <strong>" + transaction.Date.ToString("dd/MM/yyyy") + "</strong></p>" +
                "             <p>Fatura: <strong>" + transaction.Bill + "</strong></p>" +
                "             <p>Tipo: <strong>" + transaction.Type + "</strong></p>" +
                "             <p>Qnt. Parcelas: <strong>" + transaction.InstallmentsCount + "</strong></p>" +
                "             <p>Valor: <strong>R$ " + transaction.Amount.ToString("N2") + "</strong></p></br>" +
                "           </body>" +
                "         </html>";
            }
            else
            {
                body =
                    "<html>" +
                    "       <body>" +
                    "             <p>Mensagem:&nbsp<strong>" + transaction.Observation + " - " + loginDate + "</strong></p>" +
                    "       </body>" +
                    "</html>";
            }


            // Configuração do cliente SMTP
            SmtpClient client = new SmtpClient(smtpHost, smtpPort);
            client.EnableSsl = true; // Use SSL, se necessário
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);            

            // Construção da mensagem de e-mail
            MailMessage messageMail = new MailMessage(fromEmail, toEmail, subject, body);
            messageMail.IsBodyHtml = true;

            try
            {
                client.Send(messageMail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private string GetTitleEmail(EmailType emailType)
        {
            switch (emailType)
            {
                case EmailType.Login:
                    return "Login";
                case EmailType.NovaTransacao:
                    return "Transação Criada";
                case EmailType.AtualizaTransacao:
                    return "Transação Atualizada";
                case EmailType.ExcluirTransacao:
                    return "Transação Excluída";
                case EmailType.Validacao:
                    return "Validação";
                case EmailType.Excecao:
                    return "Erro de Aplicação";

                default:
                    return "Opção não identificada";
            }
        }

    }
}
