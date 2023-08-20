using Microsoft.EntityFrameworkCore;
using RinhaBackendV2.Models;

namespace RinhaBackendV2.Repository
{
    public class RinhaBackendContext : DbContext
    {

        public RinhaBackendContext(DbContextOptions<RinhaBackendContext> option) : base(option)
        {
            
        }

        public DbSet<PessoaModel> Pessoas { get; set; }
        public DbSet<StackModel> Stacks { get; set; }

        //public override void Dispose()
        //{
        //    base.Dispose();
        //}
    }
}
