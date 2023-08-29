namespace api.gestaopessoal.Models.Transacao
{
    using api.gestaopessoal.Models.Usuario;

    public class UsuarioFatura
    {
        public Usuario? Usuario { get; set; }
        public List<TotalFatura>? TotalFaturas { get; set; }

    }
}
