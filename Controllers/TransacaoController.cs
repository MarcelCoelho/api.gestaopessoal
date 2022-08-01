
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.gestaopessoal.Controllers
{
    using api.gestaopessoal.Models.Transacao;
    using api.gestaopessoal.Services.Transacao;

    [Route("api/[controller]")]
    [ApiController]
    public class TransacaoController : ControllerBase
    {
        private readonly ITransacaoService _transacaoService;

        public TransacaoController(ITransacaoService TransacaoService)
        {
            _transacaoService = TransacaoService;
        }

        // GET: api/<TransacaoController>
        [HttpGet]
        public ActionResult<List<Transacao>> Get()
        {
            return _transacaoService.Get();
        }

        // GET api/<TransacaoController>/5
        [HttpGet("{id}")]
        public ActionResult<Transacao> Get(string id)
        {
            var transacao  = _transacaoService.Get(id);

            if (transacao == null)            
                return NotFound($"Transacao com Id = {id} não encontrado.");
            
            return transacao;
        }

        // GET api/<TransacaoController>/5
        [HttpGet("PorUsuario/{usuarioId}")]
        public ActionResult<List<Transacao>> GetPorUsuario(string usuarioId)
        {
            var transacoes = _transacaoService.GetByUser(usuarioId);

            if (transacoes == null)
                return NotFound($"Transações para o Usuario = {usuarioId} não foram encontradas.");

            return transacoes;
        }

        // POST api/<TransacaoController>
        [HttpPost]
        public ActionResult<Transacao> Post([FromBody] Transacao Transacao)
        {
            _transacaoService.Create(Transacao);

            return CreatedAtAction(nameof(Get), new { id = Transacao.Id }, Transacao);
        }

        // PUT api/<TransacaoController>/5
        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] Transacao Transacao)
        {
            var transacaoEncontrado = _transacaoService.Get(id);
            if (transacaoEncontrado == null)
                return NotFound($"Transacao com Id = {id} não encontrado.");

            _transacaoService.Update(id, Transacao);
            return NoContent();
        }

        // DELETE api/<TransacaoController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var transacao = _transacaoService.Get(id);
            if (transacao == null)
                return NotFound($"Transacao com Id = {id} não encontrado.");

            _transacaoService.Remove(id);
            return Ok($"Transacao com Id = {id} removido.");
        }
    }
}
