using Bitacora.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bitacora.Application;

public static class DependencyInjection
{
    /// <summary>Registra los casos de uso de la capa de aplicación.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IRegistroService, RegistroService>();
        return services;
    }
}
