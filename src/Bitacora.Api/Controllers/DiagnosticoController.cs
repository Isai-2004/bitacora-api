using Microsoft.AspNetCore.Mvc;

namespace Bitacora.Api.Controllers;

/// <summary>
/// Endpoint de apoyo para demostrar el manejador global de errores.
/// Solo responde en el entorno Development.
/// </summary>
[ApiController]
[Route("api/diagnostico")]
public class DiagnosticoController : ControllerBase
{
    private readonly IWebHostEnvironment _entorno;

    public DiagnosticoController(IWebHostEnvironment entorno)
    {
        _entorno = entorno;
    }

    /// <summary>Lanza una excepción no controlada para comprobar la respuesta 500 con ProblemDetails.</summary>
    /// <response code="500">Error genérico sin detalles internos.</response>
    [HttpGet("error")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult ProvocarError()
    {
        if (!_entorno.IsDevelopment())
        {
            return NotFound();
        }

        throw new InvalidOperationException("Detalle interno que NO debe llegar al cliente: Data Source=bitacora.db");
    }
}
