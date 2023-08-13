using RinhaBackend.Models;

namespace RinhaBackend.Data
{
    public interface IPessoaData
    {
        Task<PessoasModel> Add(PessoasModel pessoa);
        Task<PessoasModel> GetById(Guid id);
        Task<bool> IsApelidoExist(string apelido);
        Task<IQueryable<PessoasModel>> SearchByString(string search);
        Task<int> GetTotalPessoas();
    }
}