
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.gestaopessoal.Controllers
{
    using api.gestaopessoal.Models.TipoPagamento;
    using api.gestaopessoal.Services.TipoPagamento;

    [Route("api/[controller]")]
    [ApiController]
    public class TipoPagamentoController : ControllerBase
    {
        private readonly ITipoPagamentoService _tipoPagamentoService;
        public TipoPagamentoController(ITipoPagamentoService tipoPagamentoService)
        {
            _tipoPagamentoService = tipoPagamentoService;
        }

        // GET: api/<TipoPagamentoController>
        [HttpGet]
        public ActionResult<List<TipoPagamento>> Get()
        {
            return _tipoPagamentoService.Get();
        }

        // GET api/<TipoPagamentoController>/5
        [HttpGet("{id}")]
        public ActionResult<TipoPagamento> Get(string id)
        {
            var tipoPagamento  = _tipoPagamentoService.GetById(id);

            if (tipoPagamento == null)            
                return NotFound($"Tipo Pagamento com Id = {id} não encontrado.");
            
            return tipoPagamento;
        }

        // POST api/<TipoPagamentoController>
        [HttpPost]
        public ActionResult<TipoPagamento> Post([FromBody] TipoPagamento tipoPagamento)
        {
            _tipoPagamentoService.Create(tipoPagamento);
            return CreatedAtAction(nameof(Get), new { id = tipoPagamento.Id }, tipoPagamento);
        }

        // PUT api/<TipoPagamentoController>/5
        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] TipoPagamento tipoPagamento)
        {
            var tipoPagamentoEncontrado = _tipoPagamentoService.GetById(id);
            if (tipoPagamentoEncontrado == null)
                return NotFound($"Tipo Pagamento com Id = {id} não encontrado.");

            _tipoPagamentoService.Update(id, tipoPagamento);
            return NoContent();
        }

        // DELETE api/<TipoPagamentoController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var tipoPagamento = _tipoPagamentoService.GetById(id);
            if (tipoPagamento == null)
                return NotFound($"Tipo Pagamento com Id = {id} não encontrado.");

            _tipoPagamentoService.Remove(id);
            return Ok($"Tipo Pagamento com Id = {id} removido.");
        }
    }
}
