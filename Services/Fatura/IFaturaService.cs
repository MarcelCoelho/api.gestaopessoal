namespace api.gestaopessoal.Services.Fatura
{
    using api.gestaopessoal.Models.Fatura;
    public interface IFaturaService
    {
        List<Fatura> Get();
        List<Fatura> GetByCartao(string cartaoId);
        Fatura GetById(string id);

        Fatura? GetFaturaAtual();

        Fatura Create(Fatura fatura);
        Fatura Update(string id, Fatura fatura);
        void Remove(string id);
        void UpdateAll();
    }
}
