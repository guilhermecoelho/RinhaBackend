using RinhaBackend.Models;

namespace RinhaBackend.Data
{
    public interface IPessoaRepository
    {
        Task<PessoaModel> Add(PessoaModel pessoa);
        Task<PessoaModel> GetById(Guid id);
        Task<bool> IsApelidoExist(string apelido);
        Task<IQueryable<PessoaModel>> SearchByString(string search);
        Task<int> GetTotalPessoas();
    }
}