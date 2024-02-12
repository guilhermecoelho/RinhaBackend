using Microsoft.EntityFrameworkCore;
using RinhaBackendV2.Models;

namespace RinhaBackendV2.Repository
{
    public class RinhaBackendContext : DbContext
    {

        public RinhaBackendContext(DbContextOptions<RinhaBackendContext> option) : base(option)
        {
            
        }

        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Transacao> Transacao { get; set; }

        //public override void Dispose()
        //{
        //    base.Dispose();
        //}
    }
}
