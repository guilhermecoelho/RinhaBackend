using Microsoft.EntityFrameworkCore;
using RinhaBackendV2.Models;
using RinhaBackendV2.Models.Requests;
using RinhaBackendV2.Models.Responses;

namespace RinhaBackendV2.Service
{
    public class TransacaoService
    {
        private RinhaContext _db;

        public TransacaoService(RinhaContext rinhaContext)
        {
            _db= rinhaContext;
        }

        public async Task<TransacoesResponse> Add(TransacoesRequest request)
        {
            var cliente = await _db.Cliente.FirstOrDefaultAsync(x => x.Id == request.ClienteId);

            if (cliente == null)
                throw new InvalidOperationException("cliente nao existe");

            if (request.Tipo == "c")
                cliente.Saldo += request.Valor;
            else if (request.Tipo == "d")
            {
                cliente.Saldo -= request.Valor;

                if (cliente.Saldo < (-cliente.Limite))
                    throw new InvalidOperationException("saldo menor que limite");
            }

            _db.Transacao.Add(new Transacao
            {
                Cliente = cliente,
                RealizadoEm = DateTime.Now,
                Tipo = request.Tipo,
                Valor= request.Valor,
                Descricao= request.Descricao,
            });

            
            await _db.SaveChangesAsync();

            return new TransacoesResponse
            {
                Limite = cliente.Limite,
                Saldo = cliente.Saldo,
            };
        }
    }
}
