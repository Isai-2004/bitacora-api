using System.Reflection;
using Bitacora.Api.Middleware;
using Bitacora.Application;
using Bitacora.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Capas de la solución (inyección de dependencias).
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Controladores: [ApiController] responde 400 automáticamente con los campos inválidos.
builder.Services.AddControllers();

// Manejo centralizado de errores con ProblemDetails.
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ManejadorGlobalExcepciones>();

// OpenAPI + Swagger UI.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bitácora API",
        Version = "v1",
        Description = "API REST para administrar los registros de la bitácora (arquitectura limpia)."
    });

    // Comentarios XML de la API y de los DTO (capa de aplicación).
    foreach (var ensamblado in new[] { Assembly.GetExecutingAssembly(), typeof(Bitacora.Application.DependencyInjection).Assembly })
    {
        var xml = Path.Combine(AppContext.BaseDirectory, $"{ensamblado.GetName().Name}.xml");
        if (File.Exists(xml))
        {
            c.IncludeXmlComments(xml);
        }
    }
});

var app = builder.Build();

// Con Docker se activa esta opción para crear/actualizar la base de datos al arrancar.
if (app.Configuration.GetValue<bool>("BaseDeDatos:AplicarMigracionesAlIniciar"))
{
    app.Services.AplicarMigraciones();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bitácora API v1");
    c.DocumentTitle = "Bitácora API";
});

app.MapControllers();

app.Run();

// Permite referenciar Program desde pruebas de integración.
public partial class Program { }
