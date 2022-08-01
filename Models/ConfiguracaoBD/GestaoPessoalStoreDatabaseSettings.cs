namespace api.gestaopessoal.Models.ConfiguracaoBD
{
    public class GestaoPessoalStoreDatabaseSettings : IGestaoPessoalStoreDatabaseSettings
    {
        public string CollectionNameUsuario { get; set; } = string.Empty;
        public string CollectionNameTipoPagamento { get; set; } = string.Empty;
        public string CollectionNameFatura { get; set; } = string.Empty;
        public string CollectionNameTransacao { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }
}
