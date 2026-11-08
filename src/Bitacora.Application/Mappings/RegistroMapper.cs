using Bitacora.Application.DTOs;
using Bitacora.Domain.Entities;

namespace Bitacora.Application.Mappings;

/// <summary>Convierte la entidad en DTO de salida (nunca se expone la entidad).</summary>
public static class RegistroMapper
{
    public static RegistroDetalleDto ADetalle(this Registro r) =>
        new(r.Id, r.Titulo, r.Descripcion, r.FotoBase64, r.FechaCreacion);
}
