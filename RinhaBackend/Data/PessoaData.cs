using Microsoft.EntityFrameworkCore;
using RinhaBackend.Models;

namespace RinhaBackend.Data
{
    public class PessoaData : IPessoaData
    {
        private readonly RinhaBackendContext _db;

        public PessoaData(RinhaBackendContext db)
        {
            _db = db;
        }

        public async Task<PessoasModel> Add(PessoasModel pessoa)
        {
            _db.Pessoas.Add(pessoa);
            await _db.SaveChangesAsync();

            return pessoa;
        }

        public async Task<PessoasModel> GetById(Guid id)
            => await _db.Pessoas.Include(x => x.Stacks).AsSplitQuery().FirstOrDefaultAsync(x => x.Id == id);

        public async Task<bool> IsApelidoExist(string apelido)
           => await _db.Pessoas.AsSplitQuery().AnyAsync(x => x.Apelido == apelido);

        public async Task<IQueryable<PessoasModel>> SearchByString(string search)
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
