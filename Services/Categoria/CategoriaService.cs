using MongoDB.Driver;

namespace api.gestaopessoal.Services.Categoria
{
    using api.gestaopessoal.Models.Categoria;
    using api.gestaopessoal.Models.ConfiguracaoBD;

    public class CategoriaService : ICategoriaService
    {
        private readonly IMongoCollection<Categoria> _categoriaCollection;
        public CategoriaService(IGestaoPessoalStoreDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _categoriaCollection = database.GetCollection<Categoria>(settings.CollectionNameCategoria);
        }

        public Categoria Create(Categoria categoria)
        {
            categoria.DataCriacao = DateTime.Now;
            categoria.DataModificacao = categoria.DataCriacao;

            _categoriaCollection.InsertOne(categoria);
            return categoria;
        }

        public List<Categoria> Get()
        {
            return _categoriaCollection.Find(tp => tp.Ativo).ToList().OrderBy(o => o.Ordem).ToList();
        }

        public List<Categoria> Get(string usuarioId)
        {
            return _categoriaCollection.Find(tp => tp.Ativo && tp.UsuarioId == usuarioId).ToList().OrderBy(o => o.Ordem).ToList();
        }

        public Categoria? GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return _categoriaCollection.Find(tp => tp.Id == id).FirstOrDefault();
        }

        public Categoria GetByCodigo(string codigo)
        {
            return _categoriaCollection.Find(tp => tp.Codigo == codigo).FirstOrDefault();
        }

        public void Remove(string id)
        {
            _categoriaCollection.DeleteOne(tp => tp.Id == id);
        }

        public Categoria Update(string id, Categoria categoria)
        {
            _categoriaCollection.ReplaceOne(tp => tp.Id == id, categoria);
            return categoria;
        }
    }
}
