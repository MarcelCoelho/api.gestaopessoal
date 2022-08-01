namespace api.gestaopessoal.Services.TipoPagamento
{
    using api.gestaopessoal.Models.TipoPagamento;

    public interface ITipoPagamentoService
    {
        List<TipoPagamento> Get();
        TipoPagamento Get(string id);
        TipoPagamento Create(TipoPagamento tipoPagamento);
        void Update(string id, TipoPagamento tipoPagamento);
        void Remove(string id);
    }
}
