using Microsoft.EntityFrameworkCore;
using RinhaBackendV2.Models;

namespace RinhaBackendV2.Repository
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

        public async Task<bool> IsApelidoExist(string apelido)
        {
            return await _db.Set<PessoaModel>().AsNoTracking().AnyAsync(x => x.Apelido == apelido);

        }

        public async Task<PessoaModel> GetById(Guid id)
        {
            return await _db.Set<PessoaModel>().Select(x => new PessoaModel
            {
                Id = x.Id,
                Nome = x.Nome,
                Apelido = x.Apelido,
                Nascimento = x.Nascimento,
                Stacks = x.Stacks
            }).FirstOrDefaultAsync(x => x.Id == id);

        }

        public async Task<IEnumerable<PessoaModel>> SearchByString(string search)
        {
            //using (var db = _db)
            //{
            //    return _db.Set<PessoaModel>().Include(x => x.Stacks).Take(50).Where(x =>
            //        (EF.Functions.Like(x.Nome, $"%{search}%"))
            //        || (EF.Functions.Like(x.Apelido, $"%{search}%"))
            //        || (x.Stacks.Any(c => (EF.Functions.Like(c.Nome, $"%{search}%")))));
            //}
            return _db.Set<PessoaModel>().Include(x => x.Stacks).Take(50).Where(x =>
                  (EF.Functions.Like(x.Nome, $"%{search}%"))
                  || (EF.Functions.Like(x.Apelido, $"%{search}%"))
                  || (x.Stacks.Any(c => (EF.Functions.Like(c.Nome, $"%{search}%")))));

        }

        public async Task<int> GetTotalPessoas()
        {

            return await _db.Set<PessoaModel>().AsNoTracking().CountAsync();

        }
    }
}
