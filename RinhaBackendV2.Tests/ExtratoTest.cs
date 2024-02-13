using Microsoft.EntityFrameworkCore;
using RinhaBackendV2.Models;
using RinhaBackendV2.Models.Requests;
using RinhaBackendV2.Models.Responses;
using RinhaBackendV2.Service;

namespace RinhaBackendV2.Tests
{
    public class ExtratoTest
    {
        private RinhaContext _db;
        private ExtratoService _service;

        private List<Cliente> _cliente;
        private List<Transacao> _transacao;

        public ExtratoTest()
        {
            var dbBuilder = new DbContextOptionsBuilder<RinhaContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());

            _db = new RinhaContext(dbBuilder.Options);


            _service = new ExtratoService(_db);

            _cliente = new List<Cliente>
            {
                new Cliente
                {
                    Id = 1,
                    Nome = "o barato sai caro",
                    Limite = 1000 * 100,
                    Saldo = 200
                },
                new Cliente
                {
                    Id = 2,
                    Nome = "zan corp ltda",
                    Limite = 800 * 100,
                    Saldo = 100
                }
            };
            _db.Cliente.AddRange(_cliente);
            _db.SaveChanges();

            _transacao = new List<Transacao>
            {
                new Transacao
                {
                    Id = 1,
                    Cliente = _db.Cliente.FirstOrDefault(x => x.Id == 1),
                    Descricao = "descricao",
                    RealizadoEm = DateTime.Now,
                    Tipo = "c",
                    Valor = 100
                },
                new Transacao
                {
                    Id = 2,
                    Cliente = _db.Cliente.FirstOrDefault(x => x.Id == 1),
                    Descricao = "descricao 2",
                    RealizadoEm = DateTime.Now,
                    Tipo = "c",
                    Valor = 100
                },
                new Transacao
                {
                    Id = 3,
                    Cliente = _db.Cliente.FirstOrDefault(x => x.Id == 2),
                    Descricao = "descricao 2",
                    RealizadoEm = DateTime.Now,
                    Tipo = "c",
                    Valor = 100
                }
            };
            _db.Transacao.AddRange(_transacao);

            _db.SaveChanges();
        }

        [Fact]
        public async Task Extrato_simples()
        {
            //Arrange
            var clientId = 1;

            var EXPECTED_RESULT = new ExtratoResponse
            {
                Saldo = new Saldo
                {
                    Limite = 100000,
                    Total = 200
                },
                Transacao = new List<Transacao>()
            };

            EXPECTED_RESULT.Transacao = _transacao.Where(x => x.Cliente.Id == 1).ToList();



            //act

            var result = await _service.GetByClient(clientId);

            //Assert
            Assert.Equal(EXPECTED_RESULT.Saldo.Limite, result.Saldo.Limite);
            Assert.Equal(EXPECTED_RESULT.Saldo.Total, result.Saldo.Total);
            Assert.Equal(EXPECTED_RESULT.Transacao.Count(), result.Transacao.Count());
        }
    }
}