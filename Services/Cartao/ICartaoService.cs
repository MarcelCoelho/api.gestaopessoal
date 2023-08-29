namespace api.gestaopessoal.Services.Cartao
{
    using api.gestaopessoal.Models.Cartao;
    public interface ICartaoService
    {
        List<Cartao> Get();
        Cartao? GetById(string id);
        Cartao? GetByCodigo(string codigo);
        Cartao Create(Cartao categoria);
        Cartao Update(string id, Cartao categoria);
        void Remove(string id);
    }
}
