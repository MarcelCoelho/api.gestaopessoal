using MongoDB.Driver;

namespace api.gestaopessoal.Services.Fatura
{
    using api.gestaopessoal.Models.Fatura;
    using api.gestaopessoal.Models.ConfiguracaoBD;

    public class FaturaService : IFaturaService
    {
        private readonly IMongoCollection<Fatura> _faturaCollection;
        public FaturaService(IGestaoPessoalStoreDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
           _faturaCollection = database.GetCollection<Fatura>(settings.CollectionNameFatura);
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
            return _faturaCollection.Find(tp => true).ToList();
        }

        public Fatura Get(string id)
        {
            return _faturaCollection.Find(tp => tp.Id == id).FirstOrDefault();
        }

        public void Remove(string id)
        {
           _faturaCollection.DeleteOne(tp => tp.Id == id);
        }

        public void Update(string id, Fatura Fatura)
        {
            _faturaCollection.ReplaceOne(tp => tp.Id == id, Fatura);
        }
    }
}
