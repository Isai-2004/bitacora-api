using System.ComponentModel.DataAnnotations;
using Bitacora.Domain.Rules;

namespace Bitacora.Application.Validation;

/// <summary>
/// Valida que el campo, si viene, sea Base64 válido y no supere 2 MB decodificado.
/// Un valor nulo o vacío se considera válido porque la foto es opcional.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class Base64ImagenAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string texto || string.IsNullOrWhiteSpace(texto))
        {
            return ValidationResult.Success;
        }

        var miembro = validationContext.MemberName is null ? null : new[] { validationContext.MemberName };

        if (!ReglasRegistro.EsBase64Valido(texto.Trim(), out var bytes))
        {
            return new ValidationResult("La foto no es un texto Base64 válido.", miembro);
        }

        if (bytes > ReglasRegistro.FotoTamanoMaximoBytes)
        {
            return new ValidationResult("La foto no puede superar 2 MB.", miembro);
        }

        return ValidationResult.Success;
    }
}
