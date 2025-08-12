using Microsoft.EntityFrameworkCore;

namespace ProyectTemplate.Data
{
    public class ProjectTemplateDbContext: DbContext
    {
        public ProjectTemplateDbContext(DbContextOptions<ProjectTemplateDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add your entity configurations here
        }
        // Define DbSets for your entities
        // public DbSet<YourEntity> YourEntities { get; set; }
    }
}
