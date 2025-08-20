﻿using Microsoft.EntityFrameworkCore;
using ProjectTemplate.Data.Entities;
using ProjectTemplate.Utils;

namespace ProyectTemplate.Data
{
    public class ProjectTemplateDbContext: DbContext
    {
        public ProjectTemplateDbContext(DbContextOptions<ProjectTemplateDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; } = default!;
        public DbSet<Rol> Roles { get; set; } = default!;
        public DbSet<Hotel> Hoteles { get; set; } = default!;
        public DbSet<Alojamiento> Alojamientos { get; set; } = default!;
        public DbSet<Reserva> Reservas { get; set; } = default!;
        public DbSet<ReservaHotel> ReservasHotel { get; set; } = default!;
        public DbSet<ReservaAlojamiento> ReservasAlojamiento { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---- Config Rol ----
            modelBuilder.Entity<Rol>(e =>
            {
                e.ToTable("Roles");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nombre).IsRequired().HasMaxLength(64);
                e.Property(x => x.Normalizado).IsRequired().HasMaxLength(64);
                e.HasIndex(x => x.Normalizado).IsUnique();
            });

            // ---- Config Usuario ----
            modelBuilder.Entity<Usuario>(e =>
            {
                e.ToTable("Usuarios");
                e.HasKey(x => x.Id);

                e.Property(x => x.Username).IsRequired().HasMaxLength(64);
                e.Property(x => x.Email).IsRequired().HasMaxLength(256);

                e.HasIndex(x => x.Username).IsUnique();
                e.HasIndex(x => x.Email).IsUnique();

                e.Property(x => x.PasswordHash).IsRequired();
                e.Property(x => x.PasswordSalt).IsRequired();

                e.HasOne(x => x.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(x => x.RolId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            // ---- Config Hotel ----
            modelBuilder.Entity<Hotel>(b =>
            {
                b.ToTable("Hoteles");
                b.HasKey(h => h.Id);
                b.Property(h => h.Nombre).IsRequired().HasMaxLength(200);
                b.Property(h => h.Ubicacion).IsRequired().HasMaxLength(250);
                b.Property(h => h.CantidadHabitaciones).IsRequired();
                b.Property(h => h.PrecioPorNoche).HasColumnType("decimal(18,2)").IsRequired();
            });

            // ---- Config Alojamiento ---- 
            modelBuilder.Entity<Alojamiento>(b =>
            {
                b.ToTable("Alojamientos");
                b.HasKey(a => a.Id);
                b.Property(a => a.Nombre).IsRequired().HasMaxLength(200);
                b.Property(a => a.Ubicacion).IsRequired().HasMaxLength(250);
                b.Property(a => a.CapacidadMaxima).IsRequired();
                b.Property(a => a.PrecioPorNoche).HasColumnType("decimal(18,2)").IsRequired();
            });

            // ---- Config Reserva Base ----   
            modelBuilder.Entity<Reserva>(b =>
            {
                b.ToTable("Reservas");               
                b.HasKey(r => r.Id);

                b.Property(r => r.Estado)
                .IsRequired()
                .HasMaxLength(20); 

                b.Property(r => r.FechaInicio).IsRequired();
                b.Property(r => r.FechaFin).IsRequired();

                b.HasOne(r => r.Usuario)
                .WithMany() 
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

                // Índices útiles para filtros
                b.HasIndex(r => r.UsuarioId);
                b.HasIndex(r => r.Estado);
                b.HasIndex(r => new { r.FechaInicio, r.FechaFin });
            });

            // ---- Config ReservaHotel ----
            modelBuilder.Entity<ReservaHotel>(b =>
            {
                b.ToTable("ReservasHotel");         
                b.Property(rh => rh.CantidadHabitaciones).IsRequired();

                b.HasOne(rh => rh.Hotel)
                .WithMany()
                .HasForeignKey(rh => rh.HotelId)
                .OnDelete(DeleteBehavior.Restrict);

                b.HasIndex(rh => rh.HotelId);
            });

            // ---- Config ReservaAlojamiento ----
            modelBuilder.Entity<ReservaAlojamiento>(b =>
            {
                b.ToTable("ReservasAlojamiento");    
                b.Property(ra => ra.CantidadPersonas).IsRequired();

                b.HasOne(ra => ra.Alojamiento)
                .WithMany()
                .HasForeignKey(ra => ra.AlojamientoId)
                .OnDelete(DeleteBehavior.Restrict);

                b.HasIndex(ra => ra.AlojamientoId);
            });

            // ---- Seed de Rol + Admin ----
            var adminRolId = new Guid("00000000-0000-0000-0000-000000000001");
            var adminUserId = new Guid("00000000-0000-0000-0000-0000000000AD");
            const string adminUsername = "admin";
            const string adminEmail = "admin@local";
            const string adminPlain = "BackNetAdmin123";
            const string adminSalt = "ZHVtbXlTdGF0aWNTYWx0MTIzNDU2";
            var adminHash = Password.Hash(adminPlain, adminSalt);

            modelBuilder.Entity<Rol>().HasData(new Rol
            {
                Id = adminRolId,
                Nombre = "Admin",
                Normalizado = "ADMIN"
            });

            modelBuilder.Entity<Usuario>().HasData(new Usuario
            {
                Id = adminUserId,
                Username = adminUsername,
                Email = adminEmail,
                PasswordSalt = adminSalt,
                PasswordHash = adminHash,
                RolId = adminRolId,
                CreatedAtUtc = DateTime.SpecifyKind(new DateTime(2025, 1, 1), DateTimeKind.Utc)
            });
        }
    }
}
