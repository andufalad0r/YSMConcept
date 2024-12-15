using Microsoft.EntityFrameworkCore;
using YSMConcept.Domain.Entities;

namespace YSMConcept.Infrastructure.Data
{
    public class YsmDbContext : DbContext
    {
        public YsmDbContext(DbContextOptions<YsmDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasMany(p => p.CollectionImages) 
                .WithOne(i => i.Project)         
                .HasForeignKey(i => i.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>().
                OwnsOne(a => a.Address, address =>
                {
                    address.Property(s => s.City).HasColumnName("city");
                    address.Property(s => s.Street).HasColumnName("street");
                });

            modelBuilder.Entity<Project>().
                OwnsOne(a => a.Date, date =>
                {
                    date.Property(s => s.Year).HasColumnName("year");
                    date.Property(s => s.Month).HasColumnName("month");
                });
            base.OnModelCreating(modelBuilder);
        }
        
        public DbSet<ImageEntity> Images { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
    }
}
