namespace api.gestaopessoal.Services.Categoria
{
    using api.gestaopessoal.Models.Categoria;
    public interface ICategoriaService
    {
        List<Categoria> Get();
        List<Categoria> Get(string usuarioId);
        Categoria? GetById(string id);
        Categoria? GetByCodigo(string codigo);
        Categoria Create(Categoria categoria);
        Categoria Update(string id, Categoria categoria);
        void Remove(string id);
    }
}
