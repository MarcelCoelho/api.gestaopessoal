using api.gestaopessoal.Models.ConfiguracaoBD;
using api.gestaopessoal.Services.TipoPagamento;
using api.gestaopessoal.Services.Fatura;
using api.gestaopessoal.Services.Usuario;
using api.gestaopessoal.Services.Transacao;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<GestaoPessoalStoreDatabaseSettings>(
    builder.Configuration.GetSection(nameof(GestaoPessoalStoreDatabaseSettings)));

builder.Services.AddSingleton<IGestaoPessoalStoreDatabaseSettings>(gp =>
    gp.GetRequiredService<IOptions<GestaoPessoalStoreDatabaseSettings>>().Value);

builder.Services.AddSingleton<IMongoClient>(mc =>
    new MongoClient(builder.Configuration.GetValue<string>("GestaoPessoalStoreDatabaseSettings:ConnectionString")));

builder.Services.AddScoped<ITransacaoService, TransacaoService>();
builder.Services.AddScoped<ITipoPagamentoService, TipoPagamentoService>();
builder.Services.AddScoped<IFaturaService, FaturaService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "corsPolicy",
                      builder =>
                      {
                          builder.AllowAnyOrigin();
                          builder.AllowAnyHeader();
                          builder.AllowAnyMethod();
                      });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("corsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
