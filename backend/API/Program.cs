using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar las capas de la Clean Architecture
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar CORS para que el cliente Vue 3 pueda conectarse
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowVueClient",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5173") // URL del cliente Vue 3
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    );
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowVueClient");
app.UseAuthorization();
app.MapControllers();

app.Run();
