namespace api.gestaopessoal.Services.Transacao
{
    using api.gestaopessoal.Models.Transacao;

    public interface ITransacaoService
    {
        List<Transacao> Get();
        List<Transacao> GetByUser(string usuarioId);      
       
        Transacao Get(string id);        
        Transacao Create(Transacao usuario);
        void Update(string id, Transacao usuario);
        void Remove(string id);
    }
}
