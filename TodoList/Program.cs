using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TodoList.Interfaces;
using TodoList.Models;
using TodoList.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar MongoDbSettings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Registrar o cliente MongoDB
builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

// Registrar o banco de dados MongoDB
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

// Registrar o servi�o TaskService
builder.Services.AddScoped<ITaskService, TaskService>();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

// Adicionar servi�os ao cont�iner
builder.Services.AddControllers();

// Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplicar CORS
app.UseCors("AllowAllOrigins");

// Habilitar Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TodoList API V1");
    });
}

// Configura��o de roteamento e autoriza��o
app.UseRouting();
app.UseAuthorization();

// Mapear os controladores
app.MapControllers();

// Executar o aplicativo
app.Run();
