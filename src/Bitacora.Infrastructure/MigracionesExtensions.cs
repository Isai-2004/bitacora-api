using Bitacora.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bitacora.Infrastructure;

public static class MigracionesExtensions
{
    /// <summary>
    /// Aplica las migraciones pendientes al iniciar (equivale a "dotnet ef database update").
    /// Se usa al ejecutar con Docker, donde no está la herramienta dotnet-ef.
    /// </summary>
    public static void AplicarMigraciones(this IServiceProvider servicios)
    {
        using var scope = servicios.CreateScope();
        scope.ServiceProvider.GetRequiredService<BitacoraDbContext>().Database.Migrate();
    }
}
