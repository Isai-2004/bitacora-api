using Bitacora.Application.Interfaces;
using Bitacora.Infrastructure.Persistence;
using Bitacora.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bitacora.Infrastructure;

public static class DependencyInjection
{
    /// <summary>Registra el DbContext (SQLite) y la implementación del repositorio.</summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var cadena = configuration.GetConnectionString("DefaultConnection")
                     ?? "Data Source=bitacora.db";

        services.AddDbContext<BitacoraDbContext>(options => options.UseSqlite(cadena));
        services.AddScoped<IRegistroRepository, RegistroRepository>();

        return services;
    }
}
