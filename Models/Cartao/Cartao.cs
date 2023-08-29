using MongoDB.Bson.Serialization.Attributes;

namespace api.gestaopessoal.Models.Cartao
{
    [Serializable]
    [BsonIgnoreExtraElements]
    public class Cartao : Base
    {
        [BsonElement("codigo")]
        public string Codigo { get; set; } = string.Empty;        
        [BsonElement("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [BsonElement("ativo")]
        public bool Ativo { get; set; }
        [BsonElement("padrao")]
        public bool Padrao { get; set; }
    }
}
