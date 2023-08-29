
using api.gestaopessoal.Models.Email;
using api.gestaopessoal.Services.Email;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.gestaopessoal.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public ActionResult Post(EmailTransaction transaction)
        {
            _emailService.Send(transaction);
            return Ok();
        }
    }
}
