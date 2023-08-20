using RinhaBackendV2.Models;

namespace RinhaBackendV2.Repository
{
    public interface IPessoaRepository
    {
        Task<PessoaModel> Add(PessoaModel pessoa);
        Task<bool> IsApelidoExist(string apelido);
        Task<PessoaModel> GetById(Guid id);
        Task<IEnumerable<PessoaModel>> SearchByString(string search);
        Task<int> GetTotalPessoas();
    }
}