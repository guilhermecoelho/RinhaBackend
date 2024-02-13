
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
            await _db.Set<PessoaModel>().AddAsync(pessoa);
            await _db.SaveChangesAsync();

            return pessoa;
        }

        public async Task<PessoaModel> GetById(Guid id)
        => await _db.Set<PessoaModel>().AsNoTracking().Include(x => x.Stacks).FirstOrDefaultAsync(x => x.Id == id);

        public async Task<bool> IsApelidoExist(string apelido)
        => await _db.Set<PessoaModel>().AsNoTracking().AnyAsync(x => x.Apelido == apelido);


        public async Task<IQueryable<PessoaModel>> SearchByString(string search)
        {
            return _db.Set<PessoaModel>().AsNoTracking().Include(x => x.Stacks).Take(50).Where(x =>
                (EF.Functions.Like(x.Nome, $"%{search}%"))
                || (EF.Functions.Like(x.Apelido, $"%{search}%"))
                || (x.Stacks.Any(c => (EF.Functions.Like(c.Nome, $"%{search}%")))));//FromCacheAsync(new CachingOptions() { StoreAs = StoreAs.SeperateEntities });
        }

        public async Task<int> GetTotalPessoas()
         => await _db.Set<PessoaModel>().AsNoTracking().CountAsync();
    }
}
