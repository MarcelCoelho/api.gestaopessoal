using MongoDB.Bson.Serialization.Attributes;

namespace api.gestaopessoal.Models.TipoPagamento
{
    [BsonIgnoreExtraElements]
    public class TipoPagamento : Base
    {
        [BsonElement("codigo")]
        public string Codigo { get; set; } = string.Empty;
        [BsonElement("descricao")]
        public string Descricao { get; set; } = string.Empty;
        [BsonElement("observacao")]
        public string Observacao { get; set; } = string.Empty;
    }
}
