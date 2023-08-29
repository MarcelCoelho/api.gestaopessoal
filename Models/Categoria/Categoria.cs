using MongoDB.Bson.Serialization.Attributes;

namespace api.gestaopessoal.Models.Categoria
{
    [Serializable]
    [BsonIgnoreExtraElements]
    public class Categoria : Base
    {
        [BsonElement("codigo")]
        public string Codigo { get; set; } = string.Empty;        
        [BsonElement("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [BsonElement("ordem")]
        public int Ordem { get; set; }

        [BsonElement("ativo")]
        public bool Ativo { get; set; }
        [BsonElement("usuarioId")]
        public string UsuarioId { get; set; } = string.Empty;
    }
}
