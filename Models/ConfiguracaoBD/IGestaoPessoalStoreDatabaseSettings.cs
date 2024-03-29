﻿namespace api.gestaopessoal.Models.ConfiguracaoBD
{
    public interface IGestaoPessoalStoreDatabaseSettings
    {
        string CollectionNameUsuario { get; set; }
        string CollectionNameTipoPagamento { get; set; }
        string CollectionNameFatura { get; set; }
        string CollectionNameTransacao { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
