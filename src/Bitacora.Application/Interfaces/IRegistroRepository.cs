using Bitacora.Application.DTOs;
using Bitacora.Domain.Entities;

namespace Bitacora.Application.Interfaces;

/// <summary>
/// Contrato de persistencia. La capa de aplicación lo define y
/// la capa de infraestructura lo implementa (inversión de dependencias).
/// </summary>
public interface IRegistroRepository
{
    /// <summary>Lista los registros sin cargar la foto completa.</summary>
    Task<IReadOnlyList<RegistroResumenDto>> ListarResumenAsync(CancellationToken ct = default);

    Task<Registro?> ObtenerPorIdAsync(int id, CancellationToken ct = default);

    Task AgregarAsync(Registro registro, CancellationToken ct = default);

    Task ActualizarAsync(Registro registro, CancellationToken ct = default);

    Task EliminarAsync(Registro registro, CancellationToken ct = default);
}
