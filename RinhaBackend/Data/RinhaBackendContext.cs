using Microsoft.EntityFrameworkCore;
using RinhaBackend.Models;

namespace RinhaBackend.Data
{
    public class RinhaBackendContext : DbContext
    {
        private readonly string ConnectionString;

        public RinhaBackendContext(DbContextOptions<RinhaBackendContext> option) : base(option)
        {

        }

        public RinhaBackendContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public DbSet<PessoasModel> Pessoas { get; set; }
        public DbSet<StackModel> Stacks { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {

            mb.Entity<PessoasModel>().HasKey(c => c.Id);
            mb.Entity<PessoasModel>().Property(c => c.Nome).HasMaxLength(100).IsRequired();
            mb.Entity<PessoasModel>().Property(c => c.Apelido).HasMaxLength(32).IsRequired();
            mb.Entity<PessoasModel>().Property(c => c.Nascimento).IsRequired();
            mb.Entity<PessoasModel>()
            .HasMany(a => a.Stacks)
            .WithOne(b => b.Pessoa)
            .HasForeignKey(c => c.PessoaId);
        }
    }
}
