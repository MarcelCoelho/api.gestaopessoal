using MongoDB.Bson.Serialization.Attributes;

namespace api.gestaopessoal.Models.Usuario
{
    [BsonIgnoreExtraElements]
    public class Usuario : Base
    {
        [BsonElement("nomecompleto")]
        public string NomeCompleto { get; set; } = string.Empty;
        
        [BsonElement("login")]
        public string Login { get; set; } = string.Empty;

        [BsonElement("senha")]
        public string Senha { get; set; } = string.Empty;

        [BsonElement("urlfoto")]
        public string UrlFoto { get; set; } = string.Empty;

        [BsonElement("ativo")]
        public bool Ativo { get; set; }
    }
}
