using Microsoft.EntityFrameworkCore;
using RinhaBackend.Models;

namespace RinhaBackend.Data
{
    public class RinhaBackendContext : DbContext, IDisposable
    {
        //private readonly string ConnectionString;

        public RinhaBackendContext(DbContextOptions<RinhaBackendContext> option) : base(option)
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        //public RinhaBackendContext(string connectionString)
        //{
        //    ConnectionString = connectionString;
        //}

        public DbSet<PessoaModel> Pessoas { get; set; }
        public DbSet<StackModel> Stacks { get; set; }


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var configuration = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json")
        //        .Build();

        //    var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
        //    optionsBuilder.UseNpgsql(connectionString);
        //}

        public override void Dispose()
        {
            base.Dispose();
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<PessoaModel>().HasKey(c => c.Id);
            mb.Entity<PessoaModel>().Property(c => c.Nome).HasMaxLength(100).IsRequired();
            mb.Entity<PessoaModel>().Property(c => c.Apelido).HasMaxLength(32).IsRequired();
            mb.Entity<PessoaModel>().Property(c => c.Nascimento).IsRequired();
            mb.Entity<PessoaModel>()
            .HasMany(a => a.Stacks)
            .WithOne(b => b.Pessoa)
            .HasForeignKey(c => c.PessoaId);
        }
    }
}
