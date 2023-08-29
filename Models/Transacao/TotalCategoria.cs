using api.gestaopessoal.Models;

namespace api.gestaopessoal.Models.Transacao
{
    public class TotalCategoria
    {
        public Categoria.Categoria Categoria { get; set; } = new Models.Categoria.Categoria();
        public decimal ValorTotal { get; set; }
        public int QuantidadeTransacoes { get; set; }
        public List<TotalFatura> TotalFatura { get; set; } = new List<TotalFatura>();
    }
}
