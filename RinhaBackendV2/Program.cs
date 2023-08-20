using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using RinhaBackendV2.Models;
using RinhaBackendV2.Repository;
using StackExchange.Redis;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Redis Configuration
//var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");
//ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionString);

//builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddSingleton<IDatabase>(_ =>
{
    return ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")).GetDatabase();
});

//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
//});
//builder.Services.AddDistributedMemoryCache();

//In memory cache
builder.Services.AddMemoryCache();

//Postgres Configuration
var connectionString = builder.Configuration.GetConnectionString("PostgreSqlConnection");
builder.Services.AddDbContext<RinhaBackendContext>(options =>
{
    options.UseNpgsql(connectionString);
});

//Mappers
IMapper mapper = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<PessoaModel, PessoaResponse>()
        .ForMember(dest => dest.Nascimento, opt => opt.MapFrom(src => src.Nascimento.ToString("yyyy-MM-dd")))
        .ForMember(dest => dest.Stacks, opt => opt.MapFrom(src => src.Stacks.Select(x => x.Nome)));

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


//Insert pessoa
app.MapPost("/pessoas", async ([FromServices] IPessoaRepository _pessoaRepository, [FromServices] IDatabase _cache, [FromServices] IHttpContextAccessor accessor, IValidator<PessoaRequest> validator, [FromBody] PessoaRequest pessoa) =>
{
    var validationResult = await validator.ValidateAsync(pessoa);

    if (validationResult.IsValid == false)
        return Results.UnprocessableEntity(validationResult.ToDictionary());

    if (await GetFromRedisCache(_cache, $"pessoa_apelido:{pessoa.Apelido}") != null)
        return Results.UnprocessableEntity("apelido existente");

    if (await _pessoaRepository.IsApelidoExist(pessoa.Apelido))
    {
        await SetFromRedisCache(_cache, $"pessoa_apelido:{pessoa.Apelido}", pessoa.Apelido);
        return Results.UnprocessableEntity("apelido existente");
    }
       

    var pessoaModel = mapper.Map<PessoaModel>(pessoa);
    pessoaModel.Id = Guid.NewGuid();

    var result = await _pessoaRepository.Add(pessoaModel);

    await SetFromRedisCache(_cache, $"pessoa_id:{pessoaModel.Id}", JsonSerializer.Serialize(pessoaModel));
    await SetFromRedisCache(_cache, $"pessoa_apelido:{pessoa.Apelido}", pessoa.Apelido);

    var httpContext = accessor.HttpContext;
    return Results.Created($"{httpContext?.Request.Scheme}://{httpContext?.Request.Host}/pessoas/{pessoaModel.Id}", pessoaModel);

});

//Get pessao by Id
app.MapGet("/pessoas/{id}", async ([FromServices] IPessoaRepository _pessoaRepository, [FromServices] IDatabase cache, String id) =>
{
    try
    {
        var cacheKey = $"pessoa_id:{id}";

        var t = await cache.PingAsync();
        var dataCache = await GetFromRedisCache(cache, cacheKey);
        if (dataCache != null)
        {
            var pessoaModel = JsonSerializer.Deserialize<PessoaModel>(dataCache);
            var pessoaResponse = mapper.Map<PessoaResponse>(pessoaModel);

            return Results.Ok(pessoaResponse);
        }

        var pessoaModelResult = await _pessoaRepository.GetById(new Guid(id));

        if (pessoaModelResult == null)
            return Results.NotFound();

        await SetFromRedisCache(cache, cacheKey, JsonSerializer.Serialize(pessoaModelResult));

        var pessoaResult = mapper.Map<PessoaResponse>(pessoaModelResult);

        return Results.Ok(pessoaResult);
    }
    catch (Exception ex) 
    {
        return Results.UnprocessableEntity(ex);
    }
}).Produces<PessoaResponse>();

//Get List of pessoas filtering by name, apelido or stacks
app.MapGet("/pessoas/", async ([FromServices] IPessoaRepository _pessoaData, [FromServices] IMemoryCache memoryCache, string t) =>
{
    if (string.IsNullOrEmpty(t))
        return Results.BadRequest();

    var cache = GetFromMemoryCache(memoryCache, t);
    if (cache != null)
        return Results.Ok(JsonSerializer.Deserialize<IEnumerable<PessoaResponse>>(cache));

    var result = await _pessoaData.SearchByString(t);

    var pessoasResponse = mapper.Map<IEnumerable<PessoaResponse>>(result);

    SetFromMemoryCache(memoryCache, t, JsonSerializer.Serialize(pessoasResponse));

    return Results.Ok(pessoasResponse);

}).Produces<PessoaResponse>();

//Count total pessoas
app.MapGet("/contagem-pessoas/", async ([FromServices] IPessoaRepository _pessoaData) =>
{
    return Results.Ok(await _pessoaData.GetTotalPessoas());
}).Produces<PessoaResponse>();



app.Run();


//Helper methods

async Task<string> GetFromRedisCache(IDatabase cache, string key)
=> await cache.StringGetAsync(key);

async Task SetFromRedisCache(IDatabase cache, string key, string content)
=> await cache.StringSetAsync(key, content, TimeSpan.FromSeconds(60));

string GetFromMemoryCache(IMemoryCache cache, string key)
=> cache.Get<string>(key);

void SetFromMemoryCache(IMemoryCache cache, string key, string content)
=> cache.Set(key, content, DateTimeOffset.Now.AddSeconds(10));


