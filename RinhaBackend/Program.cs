using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using RinhaBackend.Data;
using RinhaBackend.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//redis
var redisConnectionsString = builder.Configuration.GetConnectionString("RedisConnection");
builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = redisConnectionsString; });

//database
var connectionString = builder.Configuration.GetConnectionString("PostgreSqlConnection");
builder.Services.AddDbContext<RinhaBackendContext>(ServiceLifetime.Transient);

//interfaces
builder.Services.AddTransient<IPessoaData, PessoaData>();

//validators
builder.Services.AddScoped<IValidator<PessoaRequest>, PessoaRequestValidation>();

//Mappers
IMapper mapper = new MapperConfiguration(cfg =>
    {
        cfg.CreateMap<PessoasModel, PessoaResponse>()
            .ForMember(dest => dest.Nascimento, opt => opt.MapFrom(src => src.Nascimento.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.Stacks, opt => opt.MapFrom(src => src.Stacks.Select(x => x.Nome)));

        cfg.CreateMap<PessoaRequest, PessoasModel>()
           .ForMember(dest => dest.Nascimento, opt => opt.MapFrom(src => DateTime.SpecifyKind(DateTime.Parse(src.Nascimento), DateTimeKind.Utc)))
           .ForMember(dest => dest.Stacks, opt => opt.MapFrom(src => src.Stacks.Select(x => new StackModel { Nome = x })));
    }
).CreateMapper();
builder.Services.AddSingleton(mapper);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();


app.MapPost("/pessoas", async ([FromServices] IPessoaData _pessoaData, [FromServices] IDistributedCache _cache,  HttpContext httpContext, IValidator<PessoaRequest> validator, PessoaRequest pessoa) =>
{
    var validationResult = await validator.ValidateAsync(pessoa);

    if (validationResult.IsValid == false)
        return Results.UnprocessableEntity(validationResult.ToDictionary());

    if (await _cache.GetAsync($"pessoa_apelido:{pessoa.Apelido}") != null)
        return Results.UnprocessableEntity("apelido existente");

    if (await _pessoaData.IsApelidoExist(pessoa.Apelido))
        return Results.UnprocessableEntity("apelido existente");

    var result = await _pessoaData.Add(mapper.Map<PessoasModel>(pessoa));

    await _cache.SetStringAsync($"pessoa_apelido:{result.Apelido}", result.Apelido);
    await _cache.SetStringAsync($"pessoa_id:{result.Id}", JsonSerializer.Serialize(result));

    return Results.Created($"{httpContext.Request.Scheme}://{httpContext.Request.Host}/pessoas/{result.Id}", result);

}).Produces<PessoasModel>();

app.MapGet("/pessoas/{id}", async ([FromServices] IPessoaData _pessoaData, [FromServices] IDistributedCache _cache, [FromRoute] Guid id) =>
{
    try
    {
        var cache = await _cache.GetStringAsync($"pessoa_id:{id}");
        if (cache != null)
        {
            var pessoaModel = JsonSerializer.Deserialize<PessoasModel>(cache);
            var pessoaResponse = mapper.Map<PessoaResponse>(pessoaModel);

            return Results.Ok(pessoaResponse);
        }

        var result = mapper.Map<PessoaResponse>(await _pessoaData.GetById(id));

        if (result == null)
            return Results.NotFound();

        await _cache.SetStringAsync($"pessoa_id:{result.Id}", JsonSerializer.Serialize(result));

        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.Ok(ex);
    }


}).Produces<PessoaResponse>();

app.MapGet("/pessoas/", async ([FromServices] IPessoaData _pessoaData, [FromServices] IDistributedCache _cache, string t) =>
{
    if (string.IsNullOrEmpty(t))
        return Results.BadRequest();

    var result = mapper.Map<ICollection<PessoaResponse>>(await _pessoaData.SearchByString(t));

    return Results.Ok(result);

}).Produces<PessoaResponse>();

app.MapGet("/contagem-pessoas/", async ([FromServices] IPessoaData _pessoaData) =>
{
    return Results.Ok(await _pessoaData.GetTotalPessoas());
}).Produces<PessoaResponse>();


app.Run();
