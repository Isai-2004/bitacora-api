using Bitacora.Domain.Entities;
using Bitacora.Domain.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bitacora.Infrastructure.Persistence.Configurations;

public class RegistroConfiguration : IEntityTypeConfiguration<Registro>
{
    public void Configure(EntityTypeBuilder<Registro> builder)
    {
        builder.ToTable("Registros");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Titulo)
            .IsRequired()
            .HasMaxLength(ReglasRegistro.TituloLongitudMaxima);

        builder.Property(r => r.Descripcion)
            .IsRequired()
            .HasMaxLength(ReglasRegistro.DescripcionLongitudMaxima);

        builder.Property(r => r.FotoBase64);

        // SQLite guarda la fecha como texto sin zona horaria; al leerla se marca
        // de nuevo como UTC para que el JSON la devuelva con "Z" (ej. 2026-11-07T23:52:00Z).
        builder.Property(r => r.FechaCreacion)
            .IsRequired()
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
    }
}
