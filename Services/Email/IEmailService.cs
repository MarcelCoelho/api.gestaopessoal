using api.gestaopessoal.Models.Email;

namespace api.gestaopessoal.Services.Email
{
    public interface IEmailService
    {       
        void Send(EmailTransaction transaction);
    }
}
