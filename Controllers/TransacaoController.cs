
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

        [HttpGet("TransacoesPorFaturaAtual")]
        public ActionResult<List<Transacao>> TransacoesPorFaturaAtual()
        {
            return _transacaoService.TransacoesPorFaturaAtual();
        }


        [HttpPost("TransacoesPorFaturas")]
        public ActionResult<List<TotalFatura>> TransacoesPorFaturas(List<string> usuarios)
        {
            try
            {
                var transacoes = _transacaoService.TransacoesPorFaturas(usuarios);
                return Ok(transacoes);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("TransacoesPorFaturasNaoSelecionadas")]
        public ActionResult<List<TotalFatura>> TransacoesPorFaturasNaoSelecionadas(List<string> ids)
        {
            var transacoes = _transacaoService.TransacoesPorFaturasNaoSelecionadas(ids);

            return transacoes;
        }

        [HttpPost("totalPorFatura")]
        public ActionResult<List<TotalFatura>> TotalPorFatura([FromBody] List<string> usuarios)
        {
            try
            {
                var result = _transacaoService.TotalPorFatura(usuarios);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("totalFaturaPorUsuario")]
        public ActionResult<List<UsuarioFatura>> TotalFaturaPorUsuarios([FromBody] RequestTotalFatura request)
        {
            try
            {
                var result = _transacaoService.TotalFaturaPorUsuarios(request.ano, request.usuarios);
                if (result != null && result.Any())
                    return Ok(result);

                return NotFound();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("totalPorCategoria")]
        public ActionResult<List<UsuarioFatura>> TotalPorCategoria([FromBody] RequestTotalCategoria request)
        {
            try
            {
                var result = _transacaoService.TotalTransacoesPorCategoria(request.cartaoId, request.anos, request.identificadoresFatura, request.usuario, request.verDependentes);
                if (result != null && result.Any())
                    return Ok(result);

                return NotFound();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET api/<TransacaoController>/5
        [HttpGet("{id}")]
        public ActionResult<Transacao> Get(string id)
        {
            var transacao = _transacaoService.Get(id);

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
        public ActionResult<List<Transacao>> Post([FromBody] Transacao Transacao)
        {            
            var transacoes = _transacaoService.Create(Transacao);

            return Ok(transacoes);
        }

        // PUT api/<TransacaoController>/5
        [HttpPut("{id}")]
        public ActionResult<Transacao> Put(string id, [FromBody] Transacao Transacao)
        {
            var transacaoEncontrado = _transacaoService.Get(id);
            if (transacaoEncontrado == null)
                return NotFound($"Transacao com Id = {id} não encontrado.");

            Transacao.DataCriacao = transacaoEncontrado.DataCriacao;
            Transacao.DataModificacao = DateTime.Now;

            var transacao = _transacaoService.Update(id, Transacao);
            return Ok(transacao);
        }

        [HttpPut]
        public ActionResult Put(string obs1, string obs2)
        {
            _transacaoService.UpdateAll(obs1, obs2);
            return NoContent();
        }
               
        // DELETE api/<TransacaoController>/5
        [HttpDelete("{id}")]
        public ActionResult<bool> Delete(string id)
        {
            var transacao = _transacaoService.Get(id);
            if (transacao == null)
                return NotFound($"Transacao com Id = {id} não encontrado.");

            _transacaoService.Remove(id);
            return true;
        }

        // GET: api/<TransacaoController>
        [HttpGet("MaxTransacao")]
        public ActionResult<int> GetMaxTransacao()
        {
            return _transacaoService.GetMaxTransacao();
        }

        [HttpGet("GetData")]
        public ActionResult<string> GetData()
        {
            return _transacaoService.GetData();
        }

        [HttpPost("GravarTransacoes")]
        public ActionResult<List<Transacao>> GravarTransacoes([FromBody] List<Transacao> transacoes)
        {
            return _transacaoService.GravarTransacoes(transacoes);
        }
    }
}
