using Bitacora.Application.DTOs;
using Bitacora.Application.Exceptions;
using Bitacora.Application.Interfaces;
using Bitacora.Application.Mappings;
using Bitacora.Domain.Entities;

namespace Bitacora.Application.Services;

/// <summary>Casos de uso de la bitácora.</summary>
public interface IRegistroService
{
    Task<IReadOnlyList<RegistroResumenDto>> ListarAsync(CancellationToken ct = default);
    Task<RegistroDetalleDto> ObtenerAsync(int id, CancellationToken ct = default);
    Task<RegistroDetalleDto> CrearAsync(CrearRegistroDto dto, CancellationToken ct = default);
    Task<RegistroDetalleDto> ActualizarAsync(int id, ActualizarRegistroDto dto, CancellationToken ct = default);
    Task EliminarAsync(int id, CancellationToken ct = default);
}

public class RegistroService : IRegistroService
{
    private const string Recurso = "registro";
    private readonly IRegistroRepository _repositorio;

    public RegistroService(IRegistroRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<IReadOnlyList<RegistroResumenDto>> ListarAsync(CancellationToken ct = default) =>
        _repositorio.ListarResumenAsync(ct);

    public async Task<RegistroDetalleDto> ObtenerAsync(int id, CancellationToken ct = default)
    {
        var registro = await ObtenerExistenteAsync(id, ct);
        return registro.ADetalle();
    }

    public async Task<RegistroDetalleDto> CrearAsync(CrearRegistroDto dto, CancellationToken ct = default)
    {
        // La entidad aplica las reglas y asigna FechaCreacion.
        var registro = Registro.Crear(dto.Titulo, dto.Descripcion, dto.FotoBase64);
        await _repositorio.AgregarAsync(registro, ct);
        return registro.ADetalle();
    }

    public async Task<RegistroDetalleDto> ActualizarAsync(int id, ActualizarRegistroDto dto, CancellationToken ct = default)
    {
        var registro = await ObtenerExistenteAsync(id, ct);
        registro.Actualizar(dto.Titulo, dto.Descripcion, dto.FotoBase64);
        await _repositorio.ActualizarAsync(registro, ct);
        return registro.ADetalle();
    }

    public async Task EliminarAsync(int id, CancellationToken ct = default)
    {
        var registro = await ObtenerExistenteAsync(id, ct);
        await _repositorio.EliminarAsync(registro, ct);
    }

    private async Task<Registro> ObtenerExistenteAsync(int id, CancellationToken ct) =>
        await _repositorio.ObtenerPorIdAsync(id, ct)
        ?? throw new RecursoNoEncontradoException(Recurso, id);
}
