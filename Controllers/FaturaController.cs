
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.gestaopessoal.Controllers
{
    using api.gestaopessoal.Models.Fatura;
    using api.gestaopessoal.Services.Fatura;

    [Route("api/[controller]")]
    [ApiController]
    public class FaturaController : ControllerBase
    {
        private readonly IFaturaService _faturaService;
        public FaturaController(IFaturaService FaturaService)
        {
            _faturaService = FaturaService;
        }

        // GET: api/<FaturaController>
        [HttpGet]
        public ActionResult<List<Fatura>> Get()
        {
            return _faturaService.Get();
        }

        // GET api/<FaturaController>/5
        [HttpGet("{id}")]
        public ActionResult<Fatura> Get(string id)
        {
            var fatura = _faturaService.Get(id);

            if (fatura == null)
                return NotFound($"Fatura com Id = {id} não encontrada.");

            return fatura;
        }

        // POST api/<FaturaController>
        [HttpPost]
        public ActionResult<Fatura> Post([FromBody] Fatura Fatura)
        {
            _faturaService.Create(Fatura);
            return CreatedAtAction(nameof(Get), new { id = Fatura.Id }, Fatura);
        }

        // PUT api/<FaturaController>/5
        [HttpPut]
        public ActionResult<Fatura> Put(Fatura Fatura)
        {
            string id = Fatura.Id;
            var faturaEncontrada = _faturaService.Get(Fatura.Id);
            if (faturaEncontrada == null)
                return NotFound($"Fatura com Id = {Fatura.Id} não encontrada.");

            _faturaService.Update(id, Fatura);
            return CreatedAtAction(nameof(Get), new { id = Fatura.Id }, Fatura);
        }

        [HttpPut("Fechada/{id}/{fechada}")]
        public ActionResult PutFechada(string id, bool fechada)
        {            
            var faturaEncontrada = _faturaService.Get(id);
            if (faturaEncontrada == null)
                return NotFound($"Fatura com Id = {id} não encontrada.");

            faturaEncontrada.Fechada = fechada;

            _faturaService.Update(id, faturaEncontrada);
            return NoContent();
        }

        [HttpPut("Atual/{id}/{atual}")]
        public ActionResult PutAtual(string id, bool atual)
        {
            var faturaEncontrada = _faturaService.Get(id);
            if (faturaEncontrada == null)
                return NotFound($"Fatura com Id = {id} não encontrada.");

            faturaEncontrada.Atual = atual;

            _faturaService.Update(id, faturaEncontrada);
            return NoContent();
        }

        // DELETE api/<FaturaController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var fatura = _faturaService.Get(id);
            if (fatura == null)
                return NotFound($"Fatura com Id = {id} não encontrada.");

            _faturaService.Remove(id);
            return Ok($"Fatura com Id = {id} removida.");
        }
    }
}
