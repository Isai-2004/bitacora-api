using System.ComponentModel.DataAnnotations;
using Bitacora.Application.Validation;
using Bitacora.Domain.Rules;

namespace Bitacora.Application.DTOs;

/// <summary>Datos para crear un registro.</summary>
public class CrearRegistroDto
{
    /// <summary>Título del registro (3 a 100 caracteres).</summary>
    /// <example>Inspección del área norte</example>
    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(ReglasRegistro.TituloLongitudMaxima, MinimumLength = ReglasRegistro.TituloLongitudMinima,
        ErrorMessage = "El título debe tener entre {2} y {1} caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    /// <summary>Descripción (máximo 500 caracteres). En Android se obtiene por voz a texto.</summary>
    /// <example>Se revisaron los extintores y todos están vigentes.</example>
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [MaxLength(ReglasRegistro.DescripcionLongitudMaxima,
        ErrorMessage = "La descripción no puede superar {1} caracteres.")]
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>Foto opcional en Base64 (sin el prefijo data:image/...). Máximo 2 MB.</summary>
    /// <example>null</example>
    [Base64Imagen]
    public string? FotoBase64 { get; set; }
}

/// <summary>Datos para actualizar un registro. Mismas reglas que al crear.</summary>
public class ActualizarRegistroDto : CrearRegistroDto
{
}

/// <summary>Registro en el listado: sin la foto completa para aligerar la respuesta.</summary>
public record RegistroResumenDto(
    int Id,
    string Titulo,
    string Descripcion,
    bool TieneFoto,
    DateTime FechaCreacion);

/// <summary>Registro completo, incluida la foto.</summary>
public record RegistroDetalleDto(
    int Id,
    string Titulo,
    string Descripcion,
    string? FotoBase64,
    DateTime FechaCreacion);
