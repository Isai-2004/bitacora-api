using Bitacora.Application.DTOs;
using Bitacora.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bitacora.Api.Controllers;

/// <summary>Administración de los registros de la bitácora.</summary>
[ApiController]
[Route("api/registros")]
public class RegistrosController : ControllerBase
{
    private readonly IRegistroService _servicio;

    public RegistrosController(IRegistroService servicio)
    {
        _servicio = servicio;
    }

    /// <summary>Lista los registros (sin la foto completa).</summary>
    /// <response code="200">Listado de registros, del más reciente al más antiguo.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RegistroResumenDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RegistroResumenDto>>> Listar(CancellationToken ct)
    {
        return Ok(await _servicio.ListarAsync(ct));
    }

    /// <summary>Consulta un registro con su foto.</summary>
    /// <param name="id">Identificador del registro.</param>
    /// <response code="200">Registro encontrado.</response>
    /// <response code="404">No existe un registro con ese id.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RegistroDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RegistroDetalleDto>> ObtenerPorId(int id, CancellationToken ct)
    {
        return Ok(await _servicio.ObtenerAsync(id, ct));
    }

    /// <summary>Crea un registro. La fecha de creación la asigna el servidor.</summary>
    /// <response code="201">Registro creado; el encabezado Location apunta al nuevo recurso.</response>
    /// <response code="400">Datos inválidos; se detallan los campos con error.</response>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(RegistroDetalleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RegistroDetalleDto>> Crear([FromBody] CrearRegistroDto dto, CancellationToken ct)
    {
        var creado = await _servicio.CrearAsync(dto, ct);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
    }

    /// <summary>Actualiza el título, la descripción y la foto de un registro.</summary>
    /// <param name="id">Identificador del registro.</param>
    /// <param name="dto">Nuevos datos.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <response code="200">Registro actualizado.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="404">No existe un registro con ese id.</response>
    [HttpPut("{id:int}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(RegistroDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RegistroDetalleDto>> Actualizar(int id, [FromBody] ActualizarRegistroDto dto, CancellationToken ct)
    {
        return Ok(await _servicio.ActualizarAsync(id, dto, ct));
    }

    /// <summary>Elimina un registro.</summary>
    /// <param name="id">Identificador del registro.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <response code="204">Registro eliminado.</response>
    /// <response code="404">No existe un registro con ese id.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
    {
        await _servicio.EliminarAsync(id, ct);
        return NoContent();
    }
}
