namespace api.gestaopessoal.Services.Fatura
{
    using api.gestaopessoal.Models.Fatura;
    public interface IFaturaService
    {
        List<Fatura> Get();
        Fatura Get(string id);
        Fatura Create(Fatura fatura);
        void Update(string id, Fatura fatura);
        void Remove(string id);
    }
}
