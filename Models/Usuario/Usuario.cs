using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace api.gestaopessoal.Models.Usuario
{
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

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("dependentes")]
        public string Dependentes { get; set; } = string.Empty;       
        
    }
}
