namespace api.gestaopessoal.Services.Transacao
{
    using api.gestaopessoal.Models.Transacao;

    public interface ITransacaoService
    {
        List<Transacao> Get();
        List<Transacao> GetByUser(string usuarioId);

        List<Transacao> TransacoesPorFaturaAtual();
        List<TotalFatura> TotalPorFatura();
        List<TotalFatura> TransacoesPorFaturas(List<string> ids);
        List<TotalFatura> TransacoesPorFaturasNaoSelecionadas(List<string> ids);

        Transacao Get(string id);        
        List<Transacao> Create(Transacao usuario);
        Transacao Update(string id, Transacao usuario);

        void UpdateAll();
        //void UpdateAllNovaColuna();
        int GetMaxTransacao();
        void Remove(string id);

        string GetData();

        List<Transacao> GravarTransacoes(List<Transacao> transacoes);
    }
}
