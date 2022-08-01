namespace api.gestaopessoal.Services.Usuario
{
    using api.gestaopessoal.Models.Usuario;

    public interface IUsuarioService
    {
        List<Usuario> Get();
        Usuario Get(string id);
        Usuario GetByName(string login);
        Usuario Create(Usuario usuario);
        void Update(string id, Usuario usuario);
        void Remove(string id);
        bool ValidarLogin(string login, string senha);
    }
}
