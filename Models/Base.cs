namespace api.gestaopessoal.Models
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    public abstract class Base
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("usuariocriacao")]
        public string UsuarioCriacao { get; set; } = string.Empty;
        
        [BsonElement("usuariomodificacao")]
        public string UsuarioModificacao { get; set; } = string.Empty;

        [BsonElement("datacriacao")]
        public DateTime DataCriacao { get; set; }
        
        [BsonElement("datamodificacao")]
        public DateTime DataModificacao { get; set; }
    }
}
