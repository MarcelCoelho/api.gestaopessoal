using MongoDB.Driver;

namespace api.gestaopessoal.Services.Usuario
{
    using api.gestaopessoal.Models.Usuario;
    using api.gestaopessoal.Models.ConfiguracaoBD;
    using System.Security.Cryptography;
    using api.gestaopessoal.Models.Configuration;
    using api.gestaopessoal.Services.Email;
    using api.gestaopessoal.Models.Email;

    public class UsuarioService : IUsuarioService
    {
        private readonly IMongoCollection<Usuario> _usuarioCollection;
        private readonly IConfigSettings _configSettings;
        private readonly IEmailService _emailService;

        public UsuarioService(IGestaoPessoalStoreDatabaseSettings settings, IConfigSettings configSettings, IMongoClient mongoClient, IEmailService emailService)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _usuarioCollection = database.GetCollection<Usuario>(settings.CollectionNameUsuario);
            _configSettings = configSettings;
            _emailService = emailService;
        }

        public Usuario Create(Usuario usuario)
        {

            if (string.IsNullOrEmpty(usuario.Login))
                throw new NullReferenceException("Login obrigatorio.");

            if (string.IsNullOrEmpty(usuario.Senha))
                throw new NullReferenceException("Senha obrigatorio.");

            if (string.IsNullOrEmpty(usuario.UsuarioCriacao ))
                usuario.UsuarioCriacao = "Marcel";

            if (!usuario.Ativo)
                usuario.Ativo = true;

            usuario.Login = usuario.Login.Trim().ToLower();
            usuario.DataCriacao = DateTime.Now;
            usuario.DataModificacao = usuario.DataCriacao;

            var hash = new Core.Hash(SHA512.Create());
            usuario.Senha = hash.Cripitografar(usuario.Senha);

            _usuarioCollection.InsertOne(usuario);
            return usuario;
        }

        public List<Usuario> Get()
        {
            var usuarios = _usuarioCollection.Find(tp => tp.Ativo).ToList();
            if (usuarios == null || usuarios.Count == 0)
                throw new Exception("Não existem usuários ativos cadastrados!");
            
            if (usuarios.Count > 0)
            {
                usuarios.ForEach(us =>
                {
                    SetUrlPictureDefault(ref us);
                });
            }

            return usuarios;
        }

        private void SetUrlPictureDefault(ref Usuario us)
        {
            if (string.IsNullOrEmpty(us.UrlFoto))
                us.UrlFoto = _configSettings.urlPictureDefault;
        }

        public Usuario Get(string id)
        {
            var usuario = _usuarioCollection.Find(tp => tp.Ativo && tp.Id == id).FirstOrDefault();
            if (usuario == null)
                throw new Exception("Este usuário não foi encontrado ou está inativo!");

            SetUrlPictureDefault(ref usuario);
            return usuario;
        }

        public Usuario GetByLoginEmail(string valor)
        {
            var usuario = _usuarioCollection.Find(tp => tp.Ativo && tp.Login.Equals(valor) || tp.Email.Equals(valor)).FirstOrDefault();
            if (usuario == null)
                throw new Exception("Este usuário não foi encontrado ou está inativo!");

            SetUrlPictureDefault(ref usuario);
            return usuario;
        }

        public Usuario GetByName(string login)
        {
            var usuario = _usuarioCollection.Find(tp => tp.Login.Equals(login) || tp.Email.Equals(login)).FirstOrDefault();
            if (usuario == null)
                throw new Exception("Este usuário não foi encontrado ou está inativo!");

            SetUrlPictureDefault(ref usuario);
            return usuario;
        }

        public void Remove(string id)
        {
            _usuarioCollection.DeleteOne(tp => tp.Id == id);
        }

        public void Update(string id, Usuario usuario)
        {
            _usuarioCollection.ReplaceOne(tp => tp.Id == id, usuario);
        }

        public Usuario? Login(string login, string senha)
        {
            try
            {
                var usuario = GetByName(login);

                if (usuario == null || !usuario.Ativo)
                    return null;

                //var hash = new Core.Hash(SHA512.Create());
                //usuario.Senha = hash.Cripitografar(usuario.Senha);

                var hash = new Core.Hash(SHA512.Create());
                bool validUser = hash.ValidarSenha(senha, usuario.Senha);

                _emailService.Send(new EmailTransaction
                {
                    Title = EmailType.Login,
                    Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time")),
                    Observation = string.Format("Login realizado pelo usuário: {0}", login)
                });

                return validUser ? usuario : null;
            }
            catch (Exception ex)
            {
                _emailService.Send(new EmailTransaction
                {
                    Title = EmailType.Excecao,
                    Observation = string.Format("Erro ao fazer login para o usuário: {0} - {1}", login, ex.ToString())
                });
            }

            return null;
        }
    }
}
