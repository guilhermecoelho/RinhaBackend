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
var dbBuilder = new DbContextOptionsBuilder<RinhaBackendContext>().UseNpgsql(connectionString);
var db = new RinhaBackendContext(dbBuilder.Options);

//interfaces

builder.Services.AddSingleton<IPessoaData>(new PessoaData(db));

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

var _cache = app.Services.GetRequiredService<IDistributedCache>();
var _pessoaData = app.Services.GetRequiredService<IPessoaData>();


app.MapPost("/pessoas", async (RinhaBackendContext dbContext, HttpContext httpContext,IValidator<PessoaRequest> validator, PessoaRequest pessoa) =>
{
    var validationResult = await validator.ValidateAsync(pessoa);

    if (validationResult.IsValid == false)
        return Results.UnprocessableEntity(validationResult.ToDictionary());

    if (await _cache.GetAsync($"pessoa_apelido:{pessoa.Apelido}") != null)
        return Results.UnprocessableEntity("apelido existente");

    if (await _pessoaData.IsApelidoExist(pessoa.Apelido))
        return Results.UnprocessableEntity("apelido existente");

    //var result = await _pessoaData.Add(mapper.Map<PessoasModel>(pessoa));

    var pessoalModel = mapper.Map<PessoasModel>(pessoa);
    dbContext.Pessoas.Add(pessoalModel);
    await dbContext.SaveChangesAsync();

    await _cache.SetStringAsync($"pessoa_apelido:{pessoalModel.Apelido}", pessoalModel.Apelido);
    await _cache.SetStringAsync($"pessoa_id:{pessoalModel.Id}", JsonSerializer.Serialize(pessoalModel));

    return Results.Created($"{httpContext.Request.Scheme}://{httpContext.Request.Host}/pessoas/{pessoalModel.Id}", pessoalModel);

}).Produces<PessoasModel>();

app.MapGet("/pessoas/{id}", async (RinhaBackendContext dbContext, [FromRoute] Guid id) =>
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

        var result = mapper.Map<PessoaResponse>(await dbContext.Pessoas.Include(x => x.Stacks).FirstOrDefaultAsync(x => x.Id == id));

        if (result == null)
            return Results.NotFound();

        await _cache.SetStringAsync($"pessoa_id:{result.Id}", JsonSerializer.Serialize(result));

        return Results.Ok(result);
    }
    catch(Exception ex)
    {
        return Results.Ok(ex);
    }


}).Produces<PessoaResponse>();

app.MapGet("/pessoas/", async (string t) =>
{
    if (string.IsNullOrEmpty(t))
        return Results.BadRequest();

    var cache = await _cache.GetStringAsync($"pessoa_search:{t}");
    if (cache != null)
    {
        var pessoaResponse = JsonSerializer.Deserialize<List<PessoaResponse>>(cache);
        return Results.Ok(pessoaResponse);
    }

    var result = mapper.Map<ICollection<PessoaResponse>>(await _pessoaData.SearchByString(t));

    await _cache.SetStringAsync($"pessoa_search:{t}", JsonSerializer.Serialize(result));
    return Results.Ok(result);

}).Produces<PessoaResponse>();

app.MapGet("/contagem-pessoas/", async () =>
{
    return Results.Ok(await _pessoaData.GetTotalPessoas());
}).Produces<PessoaResponse>();


app.Run();
