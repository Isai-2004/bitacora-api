namespace Bitacora.Application.Exceptions;

/// <summary>Se lanza cuando el recurso solicitado no existe (se traduce a HTTP 404).</summary>
public class RecursoNoEncontradoException : Exception
{
    public RecursoNoEncontradoException(string recurso, object id)
        : base($"No existe un {recurso} con id {id}.")
    {
    }
}
