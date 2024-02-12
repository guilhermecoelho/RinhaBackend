using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;

namespace RinhaBackendV2.Models
{
    public class RinhaContext : DbContext
    {
        public RinhaContext(DbContextOptions<RinhaContext> options)
          : base(options)
        { }

        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Transacao> Transacao { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Cliente>().HasKey(c => c.Id);

        //    modelBuilder.Entity<Transacao>().HasKey(c => c.I);
        //}
    }
}
