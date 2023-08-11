using RinhaBackend.Models;

namespace RinhaBackend.Data
{
    public interface IPessoaData
    {
        PessoasModel Add(PessoasModel pessoa);
        Task<PessoasModel> GetById(Guid id);
        Task<IQueryable<PessoasModel>> GetByName(string nome);
    }
}