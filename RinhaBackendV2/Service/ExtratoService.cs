using Microsoft.EntityFrameworkCore;
using RinhaBackendV2.Models;
using RinhaBackendV2.Models.Responses;

namespace RinhaBackendV2.Service
{
    public class ExtratoService
    {
        private RinhaContext _db;

        public ExtratoService(RinhaContext rinhaContext)
        {
            _db = rinhaContext;
        }

        public async Task<ExtratoResponse> GetByClient(int clientId)
        {
            var cliente = await _db.Cliente.FirstOrDefaultAsync(x => x.Id== clientId);

            if (cliente == null)
                throw new InvalidOperationException("cliente nao existe");

            var transacoes = await _db.Transacao.Where(x => x.Cliente.Id== clientId).OrderByDescending(x => x.RealizadoEm).Take(10).ToListAsync();

            var result = new ExtratoResponse
            {
                Saldo = new Saldo
                {
                    DataExtrato = DateTime.Now,
                    Limite = cliente.Limite,
                    Total = cliente.Saldo
                },
               Transacao = transacoes
            };

            return result;
        }
    }
}
