namespace Bitacora.Domain.Rules;

/// <summary>
/// Reglas de negocio propias de un registro de la bitácora.
/// Se centralizan aquí para que todas las capas usen los mismos límites.
/// </summary>
public static class ReglasRegistro
{
    public const int TituloLongitudMinima = 3;
    public const int TituloLongitudMaxima = 100;
    public const int DescripcionLongitudMaxima = 500;

    /// <summary>Tamaño máximo de la foto ya decodificada: 2 MB.</summary>
    public const int FotoTamanoMaximoBytes = 2 * 1024 * 1024;

    /// <summary>
    /// Indica si el texto es Base64 válido y devuelve el tamaño en bytes
    /// de su contenido decodificado.
    /// </summary>
    public static bool EsBase64Valido(string valor, out int bytesDecodificados)
    {
        bytesDecodificados = 0;

        if (string.IsNullOrWhiteSpace(valor) || valor.Length % 4 != 0)
        {
            return false;
        }

        // Tamaño real = (longitud / 4) * 3 menos el relleno '='.
        var relleno = valor.EndsWith("==") ? 2 : valor.EndsWith('=') ? 1 : 0;
        var tamanoEstimado = (valor.Length / 4 * 3) - relleno;

        var buffer = new byte[tamanoEstimado];
        if (!Convert.TryFromBase64String(valor, buffer, out bytesDecodificados))
        {
            return false;
        }

        return true;
    }
}
