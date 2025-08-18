using Microsoft.EntityFrameworkCore;
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
                b.Property(h => h.Nombre).HasMaxLength(200).IsRequired();
                b.Property(h => h.Direccion).HasMaxLength(250).IsRequired();
                b.Property(h => h.Ciudad).HasMaxLength(120).IsRequired();
                b.Property(h => h.Pais).HasMaxLength(120).IsRequired();
                b.Property(h => h.Estrellas).IsRequired();
                b.HasIndex(h => new { h.Ciudad, h.Pais });
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
