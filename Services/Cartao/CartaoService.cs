using MongoDB.Driver;

namespace api.gestaopessoal.Services.Cartao
{
    using api.gestaopessoal.Models.Cartao;
    using api.gestaopessoal.Models.ConfiguracaoBD;

    public class CartaoService : ICartaoService
    {
        private readonly IMongoCollection<Cartao> _cartaoCollection;
        public CartaoService(IGestaoPessoalStoreDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _cartaoCollection = database.GetCollection<Cartao>(settings.CollectionNameCartao);
        }

        public Cartao Create(Cartao Cartao)
        {
            Cartao.DataCriacao = DateTime.Now;
            Cartao.DataModificacao = Cartao.DataCriacao;

            _cartaoCollection.InsertOne(Cartao);
            return Cartao;
        }

        public List<Cartao> Get()
        {
            return _cartaoCollection.Find(tp => tp.Ativo).ToList().OrderByDescending(o=> o.Padrao).ToList();
        }

        public Cartao? GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return _cartaoCollection.Find(tp => tp.Id == id).FirstOrDefault();
        }

        public Cartao GetByCodigo(string codigo)
        {
            return _cartaoCollection.Find(tp => tp.Codigo == codigo).FirstOrDefault();
        }

        public void Remove(string id)
        {
            _cartaoCollection.DeleteOne(tp => tp.Id == id);
        }

        public Cartao Update(string id, Cartao Cartao)
        {
            _cartaoCollection.ReplaceOne(tp => tp.Id == id, Cartao);
            return Cartao;
        }
    }
}
