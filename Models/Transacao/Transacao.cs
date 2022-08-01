using MongoDB.Bson.Serialization.Attributes;
using api.gestaopessoal.Models.Fatura;
using api.gestaopessoal.Models.TipoPagamento;

namespace api.gestaopessoal.Models.Transacao
{
    public class Transacao: Base
    {
        [BsonElement("data")]
        public DateTime Data { get; set; }

        [BsonElement("produto")]
        public string Produto { get; set; } = string.Empty;

        [BsonElement("loja")]
        public string Loja { get; set; } = string.Empty;

        [BsonElement("local")]
        public string Local { get; set; } = string.Empty;

        [BsonElement("numeroParcela")]
        public int NumeroParcela { get; set; }

        [BsonElement("quantidadeParcelas")]
        public int QuantidadeParcelas { get; set; }

        [BsonElement("valor")]
        public decimal Valor { get; set; }

        [BsonElement("observacao")]
        public string Observacao { get; set; } = string.Empty;

        [BsonElement("faturaid")]
        public string FaturaId { get; set; } = string.Empty;

        [BsonElement("tipopagamentoid")]
        public string TipoPagamentoId { get; set; } = string.Empty;

        [BsonElement("usuarioid")]
        public string UsuarioId { get; set; } = string.Empty;

        [BsonElement("estaselecionado")]
        public bool EstaSelecionado { get; set; } = false;

        [BsonIgnore]
        public Fatura.Fatura? Fatura { get; set; }
        [BsonIgnore]
        public TipoPagamento.TipoPagamento? TipoPagamento { get; set; }
    }
}
