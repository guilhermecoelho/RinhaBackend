using Alachisoft.NCache.EntityFrameworkCore;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RinhaBackend.Models;
using System.Text;

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

            using var conn = new NpgsqlConnection("Host=host.docker.internal;Port=5432;Pooling=true;Database=RinhaBackend;User Id=postgres;Password=Postgres2018!;");

            var sql = new StringBuilder();

            sql.AppendLine($"  INSERT INTO \"Pessoas\" (\"Id\", \"Apelido\", \"Nascimento\", \"Nome\")\r\n VALUES ('{pessoa.Id}','{pessoa.Apelido}', '{pessoa.Nascimento}','{pessoa.Nome}');");

            foreach(var stack in pessoa.Stacks)
            {
                stack.Id = Guid.NewGuid();
                stack.PessoaId = pessoa.Id;
                sql.AppendLine($"  INSERT INTO \"Stacks\" (\"Id\", \"Nome\", \"PessoaId\")\r\n VALUES ('{stack.Id}','{stack.Nome}', '{pessoa.Id}');");
            }

            await conn.ExecuteAsync(sql.ToString(), pessoa).ConfigureAwait(false);

            //await _db.Pessoas.AddAsync(pessoa);
            //await _db.SaveChangesAsync();

            return pessoa;
        }

        public async Task<PessoaModel> GetById(Guid id)
            => await _db.Pessoas.AsNoTracking().Include(x => x.Stacks).AsSplitQuery().FirstOrDefaultAsync(x => x.Id == id);

        public async Task<bool> IsApelidoExist(string apelido)
        => await _db.Pessoas.AsNoTracking().AsSplitQuery().AnyAsync(x => x.Apelido == apelido);


        public async Task<IEnumerable<PessoaModel>> SearchByString(string search)
        {
            return await _db.Pessoas.AsNoTracking().Include(x => x.Stacks).AsSplitQuery().Take(50).Where(x =>
                (EF.Functions.Like(x.Nome, $"%{search}%"))
                || (EF.Functions.Like(x.Apelido, $"%{search}%"))
                || (x.Stacks.Any(c => (EF.Functions.Like(c.Nome, $"%{search}%"))))).ToListAsync();//FromCacheAsync(new CachingOptions() { StoreAs = StoreAs.SeperateEntities });
        }

        public async Task<int> GetTotalPessoas()
         => await _db.Pessoas.AsNoTracking().CountAsync();
    }
}
