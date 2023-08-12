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

        public PessoasModel Add(PessoasModel pessoa)
        {
            _db.Pessoas.Add(pessoa);
            _db.SaveChanges();

            return pessoa;
        }

        public async Task<PessoasModel> GetById(Guid id)
            => await _db.Pessoas.Include(x => x.Stacks).FirstOrDefaultAsync(x => x.Id == id);


        public async Task<IQueryable<PessoasModel>> GetByName(string nome)
            => _db.Pessoas.Where(x => x.Nome == nome);

        public async Task<bool> IsApelidoExist(string apelido)
           => await _db.Pessoas.AnyAsync(x => x.Apelido == apelido);

        public async Task<IQueryable<PessoasModel>> SearchByString(string search)
        {
            return _db.Pessoas.Include(x => x.Stacks).Where(x =>
            (EF.Functions.Like(x.Nome, $"%{search}%"))
            || (EF.Functions.Like(x.Apelido, $"%{search}%"))
            || (x.Stacks.Any(c => (EF.Functions.Like(c.Nome, $"%{search}%")))));
        }

        public async Task<int> GetTotalPessoas()
         => await _db.Pessoas.CountAsync();
    }
}
