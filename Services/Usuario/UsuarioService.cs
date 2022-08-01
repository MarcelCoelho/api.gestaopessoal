using MongoDB.Driver;

namespace api.gestaopessoal.Services.Usuario
{
    using api.gestaopessoal.Models.Usuario;
    using api.gestaopessoal.Models.ConfiguracaoBD;
    using System.Security.Cryptography;

    public class UsuarioService : IUsuarioService
    {
        private readonly IMongoCollection<Usuario> _usuarioCollection;
        public UsuarioService(IGestaoPessoalStoreDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
           _usuarioCollection = database.GetCollection<Usuario>(settings.CollectionNameUsuario);
        }

        public Usuario Create(Usuario usuario)
        {

            if (usuario.Login == null)
                throw new NullReferenceException("Login obrigatorio.");

            if (usuario.Senha == null)
                throw new NullReferenceException("Senha obrigatorio.");

            usuario.DataCriacao = DateTime.Now;
            usuario.DataModificacao = usuario.DataCriacao;                       

            var hash = new Core.Hash(SHA512.Create());
            usuario.Senha = hash.Cripitografar(usuario.Senha);

            _usuarioCollection.InsertOne(usuario);
            return usuario;
        }

        public List<Usuario> Get()
        {
            return _usuarioCollection.Find(tp => true).ToList();
        }

        public Usuario Get(string id)
        {
            return _usuarioCollection.Find(tp => tp.Id == id).FirstOrDefault();
        }

        public Usuario GetByName(string login)
        {
            return _usuarioCollection.Find(tp => tp.Login == login).FirstOrDefault();
        }

        public void Remove(string id)
        {
            _usuarioCollection.DeleteOne(tp => tp.Id == id);
        }

        public void Update(string id, Usuario tipoPagamento)
        {
            _usuarioCollection.ReplaceOne(tp => tp.Id == id, tipoPagamento);
        }

        public bool ValidarLogin(string login, string senha)
        {
            var usuario = GetByName(login);
            
            if (usuario == null || !usuario.Ativo)
                return false;

            var hash = new Core.Hash(SHA512.Create());
            bool usuarioValido = hash.ValidarSenha(senha, usuario.Senha);

            return usuarioValido;
        }
    }
}
