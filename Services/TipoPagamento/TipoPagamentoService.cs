using MongoDB.Driver;

namespace api.gestaopessoal.Services.TipoPagamento
{
    using api.gestaopessoal.Models.TipoPagamento;
    using api.gestaopessoal.Models.ConfiguracaoBD;

    public class TipoPagamentoService : ITipoPagamentoService
    {
        private readonly IMongoCollection<TipoPagamento> _tipoPagamentoCollection;
        public TipoPagamentoService(IGestaoPessoalStoreDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
           _tipoPagamentoCollection = database.GetCollection<TipoPagamento>(settings.CollectionNameTipoPagamento);
        }

        public TipoPagamento Create(TipoPagamento tipoPagamento)
        {
            tipoPagamento.DataCriacao = DateTime.Now;
            tipoPagamento.DataModificacao = tipoPagamento.DataCriacao;

            _tipoPagamentoCollection.InsertOne(tipoPagamento);
            return tipoPagamento;
        }

        public List<TipoPagamento> Get()
        {
            return _tipoPagamentoCollection.Find(tp => true).ToList();
        }

        public TipoPagamento Get(string id)
        {
            return _tipoPagamentoCollection.Find(tp => tp.Id == id).FirstOrDefault();
        }

        public void Remove(string id)
        {
           _tipoPagamentoCollection.DeleteOne(tp => tp.Id == id);
        }

        public void Update(string id, TipoPagamento tipoPagamento)
        {
            _tipoPagamentoCollection.ReplaceOne(tp => tp.Id == id, tipoPagamento);
        }
    }
}
