using MongoDB.Driver;

namespace api.gestaopessoal.Services.Fatura
{
    using api.gestaopessoal.Models.Fatura;
    using api.gestaopessoal.Models.ConfiguracaoBD;
    using api.gestaopessoal.Services.Cartao;

    public class FaturaService : IFaturaService
    {
        private readonly IMongoCollection<Fatura> _faturaCollection;
        private readonly ICartaoService _cartaoService;
        public FaturaService(IGestaoPessoalStoreDatabaseSettings settings, IMongoClient mongoClient, ICartaoService cartaoService)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _faturaCollection = database.GetCollection<Fatura>(settings.CollectionNameFatura);
            _cartaoService = cartaoService;
        }

        public Fatura Create(Fatura Fatura)
        {
            Fatura.DataCriacao = DateTime.Now;
            Fatura.DataModificacao = Fatura.DataCriacao;

            _faturaCollection.InsertOne(Fatura);
            return Fatura;
        }

        public List<Fatura> Get()
        {
            return _faturaCollection.Find(tp => true).ToList().OrderByDescending(o => o.Ordem).ToList();
        }

        public List<Fatura> GetByCartao(string cartaoId)
        {
            return _faturaCollection.Find(tp => tp.CartaoId == cartaoId).ToList().OrderByDescending(o => o.Ordem).ToList();
        }

        public Fatura? GetFaturaAtual()
        {
            var fatura = _faturaCollection.Find(tp => tp.Atual).ToList();
            if (fatura != null && fatura.Any())
                return fatura[0];

            return null;
        }

        public Fatura GetById(string id)
        {
            return _faturaCollection.Find(tp => tp.Id == id).FirstOrDefault();
        }

        public void Remove(string id)
        {
            _faturaCollection.DeleteOne(tp => tp.Id == id);
        }

        public Fatura Update(string id, Fatura Fatura)
        {
            _faturaCollection.ReplaceOne(tp => tp.Id == id, Fatura);
            return Fatura;
        }
        public void UpdateAll()
        {
            var cartao = _cartaoService.GetByCodigo("01");

            var faturas = Get();

            foreach (var fatura in faturas)
            {
                fatura.CartaoId = cartao.Id;
                _faturaCollection.ReplaceOne(tp => tp.Id == fatura.Id, fatura);
            }
        }

    }
}
