using RinhaBackend.Models;

namespace RinhaBackend.Data
{
    public interface IPessoaData
    {
        PessoasModel Add(PessoasModel pessoa);
        Task<PessoasModel> GetById(Guid id);
        Task<IQueryable<PessoasModel>> GetByName(string nome);
        Task<bool> IsApelidoExist(string apelido);
        Task<IQueryable<PessoasModel>> SearchByString(string search);
        Task<int> GetTotalPessoas();
    }
}