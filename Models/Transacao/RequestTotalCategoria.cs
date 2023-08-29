namespace api.gestaopessoal.Models.Transacao
{
    public class RequestTotalCategoria
    {
        public string cartaoId { get; set; } = string.Empty;
        public List<string> anos { get; set; } = new List<string>();
        public List<string> identificadoresFatura { get; set; } = new List<string>();
        public string usuario { get; set; } = string.Empty;
        public bool verDependentes { get; set; }
    }
}
