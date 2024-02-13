using Microsoft.EntityFrameworkCore;
using RinhaBackendV2.Models;
using RinhaBackendV2.Models.Requests;
using RinhaBackendV2.Models.Responses;
using RinhaBackendV2.Service;

namespace RinhaBackendV2.Tests
{
    public class TransacaoTest
    {
        private RinhaContext _db;
        private TransacaoService _service;

        public TransacaoTest()
        {
            var dbBuilder = new DbContextOptionsBuilder<RinhaContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());

            _db = new RinhaContext(dbBuilder.Options);
            

            _service = new TransacaoService(_db);

            _db.Cliente.Add(new Cliente
            {
                Id = 1,
                Nome = "o barato sai caro",
                Limite = 1000 * 100,
                Saldo = 0
            });
            _db.Cliente.Add(new Cliente
            {
                Id = 2,
                Nome = "zan corp ltda",
                Limite = 800 * 100,
                Saldo = 0
            });
            _db.SaveChanges();
        }

        [Fact]
        public async Task Cliente_Invalido()
        {
            //Arrange
            var request = new TransacoesRequest
            {
                Descricao = "test",
                Tipo = "c",
                Valor = 100,
                ClienteId = 5
            };

            //act

            var result =  () => _service.Add(request);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(result);

            //Assert
            Assert.Equal("cliente nao existe", ex.Message);
        }

        [Fact]
        public async Task Saldo_Menor_limite()
        {
            //Arrange
            var request = new TransacoesRequest
            {
                Descricao = "test",
                Tipo = "d",
                Valor = 100001,
                ClienteId = 1
            };

            //act

            var result = () => _service.Add(request);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(result);

            //Assert
            Assert.Equal("saldo menor que limite", ex.Message);
        }

        [Fact]
        public async Task Tipo_credito()
        {
            //Arrange
            var request = new TransacoesRequest
            {
                Descricao = "test",
                Tipo = "c",
                Valor = 100,
                ClienteId = 1
            };

            var EXPECTED_RESULT = new TransacoesResponse
            {
                Limite = 100000,
                Saldo = 100
            };

            //act

            var result = await _service.Add(request);

            //Assert
            Assert.Equal(EXPECTED_RESULT.Limite, result.Limite);
            Assert.Equal(EXPECTED_RESULT.Saldo, result.Saldo);
        }

        [Fact]
        public async Task Tipo_Debito()
        {
            //Arrange
            var request = new TransacoesRequest
            {
                Descricao = "test",
                Tipo = "d",
                Valor = 100,
                ClienteId = 1
            };

            var EXPECTED_RESULT = new TransacoesResponse
            {
                Limite = 100000,
                Saldo = -100
            };

            //act

            var result = await _service.Add(request);

            //Assert
            Assert.Equal(EXPECTED_RESULT.Limite, result.Limite);
            Assert.Equal(EXPECTED_RESULT.Saldo, result.Saldo);
        }
    }
}