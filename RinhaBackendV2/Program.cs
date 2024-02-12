using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RinhaBackendV2.Models;
using RinhaBackendV2.Models.Requests;
using RinhaBackendV2.Models.Responses;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dbBuilder = new DbContextOptionsBuilder<RinhaContext>().UseInMemoryDatabase("testing");

var db = new RinhaContext(dbBuilder.Options);

db.Cliente.Add(new Cliente
{
    Id = 1,
    Limite = 1,
    SaldoInicial = 1
});
db.SaveChanges();

db.Transacao.Add(new Transacao
{
    Id = 1,
    Valor = 1,
    Tipo = "c",
    Descricao = "descricao",
    RealizadoEm = new DateTime(),
    Cliente = db.Cliente.FirstOrDefault(x => x.Id == 1)
});
db.SaveChanges();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/clientes/{id}/transacoes", async ([FromRoute] int id, TransacoesRequest request) =>
{

    var result = new TransacoesResponse();

    return Results.Ok(result);

}).Produces<TransacoesResponse>();

app.MapGet("/clientes/{id}/extrato", async ([FromRoute] int id) =>
{

    var result = new TransacoesResponse();

    return Results.Ok(result);

}).Produces<TransacoesResponse>();

app.Run();

