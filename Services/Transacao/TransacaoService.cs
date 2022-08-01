using MongoDB.Driver;

namespace api.gestaopessoal.Services.Transacao
{
    using api.gestaopessoal.Models.Transacao;
    using api.gestaopessoal.Models.ConfiguracaoBD;

    using api.gestaopessoal.Services.Fatura;
    using api.gestaopessoal.Services.TipoPagamento;

    public class TransacaoService : ITransacaoService
    {
        private readonly IMongoCollection<Transacao> _transacaoCollection;
        private readonly IFaturaService _faturaService;
        private readonly ITipoPagamentoService _tipoPagamentoService;
        public TransacaoService(IGestaoPessoalStoreDatabaseSettings settings, IMongoClient mongoClient, IFaturaService faturaService, ITipoPagamentoService tipoPagamentoService)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _transacaoCollection = database.GetCollection<Transacao>(settings.CollectionNameTransacao);

            _faturaService = faturaService;
            _tipoPagamentoService = tipoPagamentoService;
        }

        public Transacao Create(Transacao transacao)
        {
            transacao.DataCriacao = DateTime.Now;
            transacao.DataModificacao = transacao.DataCriacao;

            _transacaoCollection.InsertOne(transacao);
            return transacao;
        }

        public List<Transacao> Get()
        {
            var tiposPagamento = _tipoPagamentoService.Get();
            var faturas = _faturaService.Get();

            var transacoes = _transacaoCollection.Find(tp => true).ToList();

            foreach (var transacao in transacoes)
            {
                transacao.Fatura = faturas.FirstOrDefault(f => f.Id == transacao.FaturaId);
                transacao.TipoPagamento = tiposPagamento.FirstOrDefault(t => t.Id == transacao.TipoPagamentoId);
            }

            return transacoes.OrderByDescending(order => order.DataCriacao).ToList();
        }

        public Transacao Get(string id)
        {
            var transacao = _transacaoCollection.Find(tp => tp.Id == id).FirstOrDefault();

            if (transacao is null)
                return null;

            transacao.TipoPagamento = _tipoPagamentoService.Get(transacao.TipoPagamentoId);
            transacao.Fatura = _faturaService.Get(transacao.FaturaId);

            return transacao;
        }

        public List<Transacao> GetByUser(string usuarioId)
        {
            var tiposPagamento = _tipoPagamentoService.Get();
            var faturas = _faturaService.Get();

            var transacoes = _transacaoCollection.Find(tp => tp.UsuarioId.Equals(usuarioId)).ToList();

            foreach (var transacao in transacoes)
            {
                transacao.Fatura = faturas.FirstOrDefault(f => f.Id == transacao.FaturaId);
                transacao.TipoPagamento = tiposPagamento.FirstOrDefault(t => t.Id == transacao.TipoPagamentoId);
            }

            return transacoes.OrderByDescending(order => order.DataCriacao).ToList();
        }

        public void Remove(string id)
        {
            _transacaoCollection.DeleteOne(tp => tp.Id == id);
        }

        public void Update(string id, Transacao tipoPagamento)
        {
            _transacaoCollection.ReplaceOne(tp => tp.Id == id, tipoPagamento);
        }
    }
}
