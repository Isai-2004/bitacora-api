using Bitacora.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bitacora.Infrastructure.Persistence;

public class BitacoraDbContext : DbContext
{
    public BitacoraDbContext(DbContextOptions<BitacoraDbContext> options) : base(options)
    {
    }

    public DbSet<Registro> Registros => Set<Registro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BitacoraDbContext).Assembly);
    }
}
