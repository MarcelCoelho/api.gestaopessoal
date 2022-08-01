
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.gestaopessoal.Controllers
{
    using api.gestaopessoal.Models.Usuario;
    using api.gestaopessoal.Services.Usuario;

    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET: api/<UsuarioController>
        [HttpGet]
        public ActionResult<List<Usuario>> Get()
        {
            return _usuarioService.Get();
        }

        // GET api/<UsuarioController>/5
        [HttpGet("{id}")]
        public ActionResult<Usuario> Get(string id)
        {
            var usuario = _usuarioService.Get(id);

            if (usuario == null)
                return NotFound($"Usuario com Id = {id} não encontrado.");

            return usuario;
        }

        // POST api/<UsuarioController>
        [HttpPost]
        public ActionResult<Usuario> Post([FromBody] Usuario usuario)
        {
            _usuarioService.Create(usuario);
            return CreatedAtAction(nameof(Get), new { id = usuario.Id }, usuario);
        }

        // PUT api/<UsuarioController>/5
        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] Usuario usuario)
        {
            var usuarioEncontrado = _usuarioService.Get(id);
            if (usuarioEncontrado == null)
                return NotFound($"Usuario com Id = {id} não encontrado.");

            _usuarioService.Update(id, usuario);
            return NoContent();
        }

        // DELETE api/<UsuarioController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var usuario = _usuarioService.Get(id);
            if (usuario == null)
                return NotFound($"Usuario com Id = {id} não encontrado.");

            _usuarioService.Remove(id);
            return Ok($"Usuario com Id = {id} removido.");
        }

        [HttpPost("ValidarLogin")]
        public ActionResult ValidarLogin(string login, string senha)
        {
            bool validacaoOK = _usuarioService.ValidarLogin(login, senha);

            if (validacaoOK)
                return Ok($"Usuario {login} validado com sucesso.");
            else
                return NotFound($"Usuario e Senha inválidos.");
        }
    }
}
