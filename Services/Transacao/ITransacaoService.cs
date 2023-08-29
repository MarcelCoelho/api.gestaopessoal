namespace api.gestaopessoal.Services.Transacao
{
    using api.gestaopessoal.Models.Transacao;

    public interface ITransacaoService
    {
        List<Transacao> Get();
        List<Transacao> GetByUser(string usuarioId);
        List<Transacao> TransacoesPorFaturaAtual();
        List<TotalFatura> TotalPorFatura(List<string> usuarios);
        List<TotalFatura> TransacoesPorFaturas(List<string> usuarios);
        List<UsuarioFatura>? TotalFaturaPorUsuarios(string ano, List<string> usuarios);
        List<TotalFatura> TransacoesPorFaturasNaoSelecionadas(List<string> ids);
        Transacao Get(string id);        
        List<Transacao>? Create(Transacao usuario);
        Transacao Update(string id, Transacao usuario);

        void UpdateAll(string obs1, string obs2);
        int GetMaxTransacao();
        void Remove(string id);

        string GetData();

        List<Transacao> GravarTransacoes(List<Transacao> transacoes);
        List<TotalCategoria>? TotalTransacoesPorCategoria(string cartaoId, List<string> anos, List<string> identificadoresFatura, string usuario, bool verDependentes);
    }
}
