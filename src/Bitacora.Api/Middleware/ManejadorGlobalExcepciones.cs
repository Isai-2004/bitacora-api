using System.Diagnostics;
using Bitacora.Application.Exceptions;
using Bitacora.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Bitacora.Api.Middleware;

/// <summary>
/// Manejador global de excepciones. Traduce cada excepción a un ProblemDetails:
/// - ReglaNegocioException        → 400 con el campo inválido
/// - RecursoNoEncontradoException → 404
/// - cualquier otra               → 500 genérico (el detalle solo va al log)
/// </summary>
public sealed class ManejadorGlobalExcepciones : IExceptionHandler
{
    private readonly ILogger<ManejadorGlobalExcepciones> _logger;

    public ManejadorGlobalExcepciones(ILogger<ManejadorGlobalExcepciones> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problema = exception switch
        {
            ReglaNegocioException ex => new ValidationProblemDetails(
                new Dictionary<string, string[]> { [ex.Campo] = new[] { ex.Message } })
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "La solicitud no cumple las reglas del negocio.",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            },

            RecursoNoEncontradoException ex => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Recurso no encontrado.",
                Detail = ex.Message,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5"
            },

            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Ocurrió un error inesperado en el servidor.",
                Detail = "Intente de nuevo más tarde. Si el problema persiste, comparta el traceId con el equipo de soporte.",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1"
            }
        };

        if (problema.Status == StatusCodes.Status500InternalServerError)
        {
            // El detalle completo queda en el log del servidor, nunca en la respuesta.
            _logger.LogError(exception, "Error no controlado en {Metodo} {Ruta}",
                httpContext.Request.Method, httpContext.Request.Path);
        }

        problema.Instance = httpContext.Request.Path;
        problema.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = problema.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(
            problema, problema.GetType(), options: null, contentType: "application/problem+json",
            cancellationToken: cancellationToken);

        return true;
    }
}
