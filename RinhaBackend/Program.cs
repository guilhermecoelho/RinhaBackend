using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RinhaBackend.Data;
using RinhaBackend.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("PostgreSqlConnection");

builder.Services.AddDbContext<RinhaBackendContext>();

var dbBuilder = new DbContextOptionsBuilder<RinhaBackendContext>().UseNpgsql(connectionString);
var db = new RinhaBackendContext(dbBuilder.Options);

builder.Services.AddSingleton<IPessoaData>(new PessoaData(db));

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


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        return Task.CompletedTask;
    });
    await next(context);
});

var _pessoaData = app.Services.GetRequiredService<IPessoaData>();


app.MapPost("/pessoa", (PessoaRequest pessoa) =>
{
    var pessoasModel = mapper.Map<PessoasModel>(pessoa);

    var result = _pessoaData.Add(pessoasModel);

    return Results.Created(result.Id.ToString(), pessoa);
});

app.MapGet("/pessoa/{id}", async ([FromRoute] Guid id) =>
{
    var result = mapper.Map<PessoaResponse>(await _pessoaData.GetById(id));

    return Results.Ok(result);

});

app.MapGet("/pessoa/", async (string t) =>
{
    var result = await _pessoaData.GetByName(t);
    return Results.Ok(result);
});


app.Run();
