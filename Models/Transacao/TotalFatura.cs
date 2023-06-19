using MongoDB.Bson.Serialization.Attributes;
using api.gestaopessoal.Models.Fatura;
using api.gestaopessoal.Models.TipoPagamento;

namespace api.gestaopessoal.Models.Transacao
{
    public class TotalFatura
    {
        public string Id { get; set; }
        public string Descricao { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFinal { get; set; }
        public bool Atual { get; set; }
        public bool Fechada { get; set; }
        public decimal Valor { get; set; }
        public int QuantidadeTransacoes { get; set; }
        public bool TransacoesVisiveis { get; set; }
        public int Ordem { get; set; }
        public List<Transacao> Transacoes { get; set; }
        public bool ModoInserir { get; set; }
    }
}
