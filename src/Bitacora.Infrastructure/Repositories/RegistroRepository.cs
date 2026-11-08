using Bitacora.Application.DTOs;
using Bitacora.Application.Interfaces;
using Bitacora.Domain.Entities;
using Bitacora.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bitacora.Infrastructure.Repositories;

public class RegistroRepository : IRegistroRepository
{
    private readonly BitacoraDbContext _db;

    public RegistroRepository(BitacoraDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<RegistroResumenDto>> ListarResumenAsync(CancellationToken ct = default)
    {
        // La proyección evita traer la columna FotoBase64 completa desde la base de datos.
        return await _db.Registros
            .AsNoTracking()
            .OrderByDescending(r => r.Id)
            .Select(r => new RegistroResumenDto(
                r.Id,
                r.Titulo,
                r.Descripcion,
                r.FotoBase64 != null,
                r.FechaCreacion))
            .ToListAsync(ct);
    }

    public Task<Registro?> ObtenerPorIdAsync(int id, CancellationToken ct = default) =>
        _db.Registros.FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task AgregarAsync(Registro registro, CancellationToken ct = default)
    {
        _db.Registros.Add(registro);
        await _db.SaveChangesAsync(ct);
    }

    public async Task ActualizarAsync(Registro registro, CancellationToken ct = default)
    {
        // La entidad ya está rastreada por el contexto; basta con guardar.
        await _db.SaveChangesAsync(ct);
    }

    public async Task EliminarAsync(Registro registro, CancellationToken ct = default)
    {
        _db.Registros.Remove(registro);
        await _db.SaveChangesAsync(ct);
    }
}
