
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
        public ActionResult<List<TotalFatura>> TransacoesPorFaturas(List<string> ids)
        {
            var transacoes = _transacaoService.TransacoesPorFaturas(ids);

            return transacoes;
        }

        [HttpPost("TransacoesPorFaturasNaoSelecionadas")]
        public ActionResult<List<TotalFatura>> TransacoesPorFaturasNaoSelecionadas(List<string> ids)
        {
            var transacoes = _transacaoService.TransacoesPorFaturasNaoSelecionadas(ids);

            return transacoes;
        }

        [HttpGet("totalPorFatura")]
        public ActionResult<List<TotalFatura>> TotalPorFatura()
        {
            return _transacaoService.TotalPorFatura();
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
        public ActionResult Put()
        {
            _transacaoService.UpdateAll();
            return NoContent();
        }

        //[HttpPut("novaColuna")]
        //public ActionResult PutGeralNovaColuna()
        //{
        //    _transacaoService.UpdateAllNovaColuna();
        //    return NoContent();
        //}



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
