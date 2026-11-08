namespace Bitacora.Domain.Exceptions;

/// <summary>
/// Se lanza cuando una operación incumple una regla del negocio.
/// Incluye el nombre del campo afectado para poder informarlo al cliente.
/// </summary>
public class ReglaNegocioException : Exception
{
    public string Campo { get; }

    public ReglaNegocioException(string campo, string mensaje) : base(mensaje)
    {
        Campo = campo;
    }
}
