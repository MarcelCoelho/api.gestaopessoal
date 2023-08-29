
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.gestaopessoal.Controllers
{
    using api.gestaopessoal.Models.Cartao;
    using api.gestaopessoal.Services.Cartao;

    [Route("api/[controller]")]
    [ApiController]
    public class CartaoController : ControllerBase
    {
        private readonly ICartaoService _cartaoService;
        public CartaoController(ICartaoService CartaoService)
        {
            _cartaoService = CartaoService;
        }

        [HttpGet()]
        public ActionResult<List<Cartao>> Get()
        {
            return _cartaoService.Get();            
        }

        // GET api/<CartaoController>/5
        [HttpGet("{id}")]
        public ActionResult<Cartao> GetById(string id)
        {
            var Cartao = _cartaoService.GetById(id);

            if (Cartao == null)
                return NotFound($"Cartao com Id = {id} não encontrada.");

            return Cartao;
        }

        [HttpGet("{codigo}")]
        public ActionResult<Cartao> GetByCodigo(string codigo)
        {
            var Cartao = _cartaoService.GetByCodigo(codigo);

            if (Cartao == null)
                return NotFound($"Cartao com Codigo = {codigo} não encontrada.");

            return Cartao;
        }

        // POST api/<CartaoController>
        [HttpPost]
        public ActionResult<Cartao> Post([FromBody] Cartao Cartao)
        {
            _cartaoService.Create(Cartao);
            return CreatedAtAction(nameof(GetById), new { id = Cartao.Id }, Cartao);
        }

        // PUT api/<CartaoController>/5
        [HttpPut]
        public ActionResult<Cartao> Put(Cartao Cartao)
        {
            string id = Cartao.Id;
            var CartaoEncontrada = _cartaoService.GetById(Cartao.Id);
            if (CartaoEncontrada == null)
                return NotFound($"Cartao com Id = {Cartao.Id} não encontrada.");

            _cartaoService.Update(id, Cartao);
            return CreatedAtAction(nameof(GetById), new { id = Cartao.Id }, Cartao);
        }

        // DELETE api/<CartaoController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var Cartao = _cartaoService.GetById(id);
            if (Cartao == null)
                return NotFound($"Cartao com Id = {id} não encontrada.");

            _cartaoService.Remove(id);
            return Ok($"Cartao com Id = {id} removida.");
        }
    }
}
