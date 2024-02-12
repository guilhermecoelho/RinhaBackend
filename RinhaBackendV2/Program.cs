using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
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

    cfg.CreateMap<PessoaRequest, PessoaModel>()
       .ForMember(dest => dest.Nascimento, opt => opt.MapFrom(src => DateTime.SpecifyKind(DateTime.Parse(src.Nascimento), DateTimeKind.Utc)))
       .ForMember(dest => dest.Stacks, opt => opt.MapFrom(src => src.Stacks.Select(x => new StackModel { Nome = x })));
}
).CreateMapper();
builder.Services.AddSingleton(mapper);

//interfaces
builder.Services.AddTransient<IPessoaRepository, PessoaRepository>();

//validators
builder.Services.AddTransient<IValidator<PessoaRequest>, PessoaRequestValidation>();

//others
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//global variables

app.MapPost("/clientes/{id}/transacoes", async ([FromRoute] int id, TransacoesRequest request) =>
{
    var validationResult = await validator.ValidateAsync(pessoa);

    var result = new TransacoesResponse();

    return Results.Ok(result);

}).Produces<TransacoesResponse>();

app.MapGet("/clientes/{id}/extrato", async ([FromRoute] int id) =>
{
    if (string.IsNullOrEmpty(t))
        return Results.BadRequest();

    var cache = GetFromMemoryCache(memoryCache, t);
    if (cache != null)
        return Results.Ok(JsonSerializer.Deserialize<IEnumerable<PessoaModel>>(cache));

    var result = await _pessoaData.SearchByString(t);

    var result = new TransacoesResponse();

    return Results.Ok(result);

}).Produces<TransacoesResponse>();

app.Run();


//Helper methods

async Task<string> GetFromRedisCache(IConnectionMultiplexerPool cache, string key)
{
    var pool = await cache.GetAsync();
    var db = pool.Connection.GetDatabase();

    return await db.StringGetAsync(key);
}


async Task SetFromRedisCache(IConnectionMultiplexerPool cache, string key, string content)
{
    var pool = await cache.GetAsync();
    var db = pool.Connection.GetDatabase();
    var sub = pool.Connection.GetSubscriber();

    await db.StringSetAsync(key, content, TimeSpan.FromSeconds(60));
    await sub.PublishAsync("added-record", content);
}

string GetFromMemoryCache(IMemoryCache cache, string key)
=> cache.Get<string>(key);

void SetFromMemoryCache(IMemoryCache cache, string key, string content)
=> cache.Set(key, content, DateTimeOffset.Now.AddSeconds(10));


