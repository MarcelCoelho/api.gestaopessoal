namespace api.gestaopessoal.Models.Transacao
{
    public class RequestTotalFatura
    {
        public string ano { get; set; } = string.Empty;
        public List<string> usuarios { get; set; } = new List<string>();
    }
}
