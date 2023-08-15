using Microsoft.EntityFrameworkCore;
using RinhaBackend.Models;

namespace RinhaBackend.Data
{
    public class PessoaRepository : IPessoaRepository
    {
        private readonly RinhaBackendContext _db;

        public PessoaRepository(RinhaBackendContext db)
        {
            _db = db;
        }

        public async Task<PessoaModel> Add(PessoaModel pessoa)
        {
            _db.Pessoas.Add(pessoa);
            await _db.SaveChangesAsync();

            return pessoa;
        }

        public async Task<PessoaModel> GetById(Guid id)
            => await _db.Pessoas.Include(x => x.Stacks).AsSplitQuery().FirstOrDefaultAsync(x => x.Id == id);

        public async Task<bool> IsApelidoExist(string apelido)
           => await _db.Pessoas.AsSplitQuery().AnyAsync(x => x.Apelido == apelido);

        public async Task<IQueryable<PessoaModel>> SearchByString(string search)
        {
            return _db.Pessoas.Include(x => x.Stacks).AsSplitQuery().Take(50).Where(x =>
            (EF.Functions.Like(x.Nome, $"%{search}%"))
            || (EF.Functions.Like(x.Apelido, $"%{search}%"))
            || (x.Stacks.Any(c => (EF.Functions.Like(c.Nome, $"%{search}%")))));
        }

        public async Task<int> GetTotalPessoas()
         => await _db.Pessoas.CountAsync();
    }
}
