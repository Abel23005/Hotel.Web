using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Web.Models;

public partial class HotelContext : DbContext
{
    public HotelContext(DbContextOptions<HotelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Habitacion> Habitacions { get; set; }

    public virtual DbSet<Huesped> Huespeds { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<TipoHabitacion> TipoHabitacions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Habitacion>(entity =>
        {
            entity.HasKey(e => e.IdHabitacion).HasName("PK__Habitaci__8BBBF90143F44D84");

            entity.ToTable("Habitacion");

            entity.HasIndex(e => e.Numero, "UQ__Habitaci__7E532BC6B60B9D00").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Numero).HasMaxLength(10);
            entity.Property(e => e.PrecioPorNoche).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdTipoHabitacionNavigation).WithMany(p => p.Habitacions)
                .HasForeignKey(d => d.IdTipoHabitacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Habitacio__IdTip__4D94879B");
        });

        modelBuilder.Entity<Huesped>(entity =>
        {
            entity.HasKey(e => e.IdHuesped).HasName("PK__Huesped__939EC061831A5461");

            entity.ToTable("Huesped");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Documento).HasMaxLength(20);
            entity.Property(e => e.Nombres).HasMaxLength(100);
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.IdReserva).HasName("PK__Reserva__0E49C69DAD9A9802");

            entity.ToTable("Reserva");

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.Observaciones).HasMaxLength(200);

            entity.HasOne(d => d.IdHabitacionNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdHabitacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reserva__IdHabit__5629CD9C");

            entity.HasOne(d => d.IdHuespedNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdHuesped)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reserva__IdHuesp__5535A963");
        });

        modelBuilder.Entity<TipoHabitacion>(entity =>
        {
            entity.HasKey(e => e.IdTipoHabitacion).HasName("PK__TipoHabi__AB75E87C942D1790");

            entity.ToTable("TipoHabitacion");

            entity.Property(e => e.Nombre).HasMaxLength(60);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
