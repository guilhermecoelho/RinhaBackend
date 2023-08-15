using Microsoft.EntityFrameworkCore;
using RinhaBackend.Models;
using System.Reflection.Emit;

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
            //mb.Entity<StackModel>().HasNoKey();


            mb.Entity<PessoasModel>().HasKey(c => c.Id);
            mb.Entity<PessoasModel>().Property(c => c.Nome).HasMaxLength(100).IsRequired();
            mb.Entity<PessoasModel>().Property(c => c.Apelido).HasMaxLength(32).IsRequired();
            mb.Entity<PessoasModel>().Property(c => c.Nascimento).IsRequired();
            mb.Entity<PessoasModel>()
            .HasMany(a => a.Stacks)
            .WithOne(b => b.Pessoa)
            .HasForeignKey(c => c.PessoaId);

            //var pessoaId = Guid.NewGuid();

            //var pessoaId2 = Guid.NewGuid();

            //mb.Entity<PessoasModel>().ToTable("Pessoas")
            //   .HasData(
            //   new PessoasModel
            //   {
            //       Id = pessoaId,
            //       Nome = "Nome test",
            //       Apelido = "Apelido teste",
            //       Nascimento = DateTime.UtcNow
            //   },
            //    new PessoasModel
            //    {
            //        Id = pessoaId2,
            //        Nome = "Nome test 2",
            //        Apelido = "Apelido teste 2",
            //        Nascimento = DateTime.UtcNow
            //    }
            //   );

            //mb.Entity<StackModel>().ToTable("Stacks")
            //  .HasData(
            //  new StackModel
            //  {
            //      Id = Guid.NewGuid(),
            //      Nome = "c#",
            //      PessoaId = pessoaId,
            //  },
            //   new StackModel
            //   {
            //       Id = Guid.NewGuid(),
            //       Nome = "go",
            //       PessoaId = pessoaId,
            //   },

            //    new StackModel
            //    {
            //        Id = Guid.NewGuid(),
            //        Nome = "python",
            //        PessoaId = pessoaId2,
            //    },
            //   new StackModel
            //   {
            //       Id = Guid.NewGuid(),
            //       Nome = "c++",
            //       PessoaId = pessoaId2,
            //   }
            //  );
        }
    }
}
