
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.gestaopessoal.Controllers
{
    using api.gestaopessoal.Models.Categoria;
    using api.gestaopessoal.Services.Categoria;

    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;
        public CategoriaController(ICategoriaService CategoriaService)
        {
            _categoriaService = CategoriaService;
        }

        // GET: api/<CategoriaController>
        [HttpGet()]
        public ActionResult<List<Categoria>> Get(string usuarioId)
        {
            return _categoriaService.Get(usuarioId);
        }

        // GET api/<CategoriaController>/5
        [HttpGet("{id}")]
        public ActionResult<Categoria> GetById(string id)
        {
            var Categoria = _categoriaService.GetById(id);

            if (Categoria == null)
                return NotFound($"Categoria com Id = {id} não encontrada.");

            return Categoria;
        }

        [HttpGet("{codigo}")]
        public ActionResult<Categoria> GetByCodigo(string codigo)
        {
            var Categoria = _categoriaService.GetByCodigo(codigo);

            if (Categoria == null)
                return NotFound($"Categoria com Codigo = {codigo} não encontrada.");

            return Categoria;
        }

        // POST api/<CategoriaController>
        [HttpPost]
        public ActionResult<Categoria> Post([FromBody] Categoria Categoria)
        {
            _categoriaService.Create(Categoria);
            return CreatedAtAction(nameof(Get), new { id = Categoria.Id }, Categoria);
        }

        // PUT api/<CategoriaController>/5
        [HttpPut]
        public ActionResult<Categoria> Put(Categoria Categoria)
        {
            string id = Categoria.Id;
            var CategoriaEncontrada = _categoriaService.GetById(Categoria.Id);
            if (CategoriaEncontrada == null)
                return NotFound($"Categoria com Id = {Categoria.Id} não encontrada.");

            _categoriaService.Update(id, Categoria);
            return CreatedAtAction(nameof(Get), new { id = Categoria.Id }, Categoria);
        }

        // DELETE api/<CategoriaController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var Categoria = _categoriaService.GetById(id);
            if (Categoria == null)
                return NotFound($"Categoria com Id = {id} não encontrada.");

            _categoriaService.Remove(id);
            return Ok($"Categoria com Id = {id} removida.");
        }
    }
}
