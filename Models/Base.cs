namespace api.gestaopessoal.Models
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using System.Text.Json.Serialization;

    public abstract class Base
    {        
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [JsonIgnore]
        [BsonElement("usuariocriacao")]
        public string UsuarioCriacao { get; set; } = string.Empty;

        [JsonIgnore]
        [BsonElement("usuariomodificacao")]
        public string UsuarioModificacao { get; set; } = string.Empty;

        [JsonIgnore]
        [BsonElement("datacriacao")]
        public DateTime DataCriacao { get; set; }

        [JsonIgnore]
        [BsonElement("datamodificacao")]
        public DateTime DataModificacao { get; set; }
    }
}
