namespace api.gestaopessoal.Models.Transacao
{
    public class TotalFatura
    {
        public string Id { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Ano { get; set; } = string.Empty;
        public string Mes { get; set; } = string.Empty;
        public DateTime DataInicio { get; set; }
        public DateTime DataFinal { get; set; }
        public bool Atual { get; set; }
        public bool Fechada { get; set; }
        public decimal Valor { get; set; }
        public int QuantidadeTransacoes { get; set; }
        public int Ordem { get; set; }
        public List<Transacao> Transacoes { get; set; } = new List<Transacao>();
    }
}
