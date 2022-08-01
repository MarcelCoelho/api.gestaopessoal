using MongoDB.Bson.Serialization.Attributes;

namespace api.gestaopessoal.Models.Fatura
{
    [BsonIgnoreExtraElements]
    public class Fatura : Base
    {
        [BsonElement("mes")]
        public string Mes { get; set; } = string.Empty;        
        [BsonElement("ano")]
        public string Ano { get; set; } = string.Empty;        
        
        [BsonElement("datainicio")]
        public DateTime DataInicio { get; set; }
        [BsonElement("datafinal")]
        public DateTime DataFinal { get; set; }
        [BsonElement("observacao")]
        public string Observacao { get; set; } = string.Empty;
        [BsonElement("ordem")]
        public int Ordem { get; set; }
        [BsonElement("fechada")]
        public bool Fechada { get; set; }
        [BsonElement("atual")]
        public bool Atual { get; set; }
    }
}
