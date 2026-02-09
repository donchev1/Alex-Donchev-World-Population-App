using Microsoft.EntityFrameworkCore;
using WorldPopulationMVC.Data.Models;

namespace WorldPopulationMVC.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Example table
    public DbSet<CountryName> CountryNames { get; set; }
    public DbSet<CountryNameAlternative> CountryNameAlternatives { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<State> States { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<CountryName>(entity =>
        {
            entity.ToTable("CountryName");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                  .IsRequired()
                  .HasMaxLength(2000);
        });

        modelBuilder.Entity<CountryNameAlternative>(entity =>
        {
            entity.ToTable("CountryNameAlternative");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                  .IsRequired()
                  .HasMaxLength(2000);

            entity
            .HasOne(c => c.DefaultName)
            .WithMany(s => s.Alternatives)
            .HasForeignKey(c => c.CountryNameId)
            .OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("City"); 

            entity.HasKey(c => c.CityId);

            entity.Property(c => c.CityName)
                  .IsRequired()
                  .HasMaxLength(2000);

            entity.Property(c => c.Population);
            entity.Property(c => c.StateId);

            entity
            .HasOne(c => c.State)
            .WithMany(s => s.Cities)
            .HasForeignKey(c => c.StateId)
            .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("Country");

            entity.HasKey(c => c.CountryId);

            entity.Property(c => c.CountryName)
                  .IsRequired()
                  .HasMaxLength(2000);
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.ToTable("State");

            entity.HasKey(s => s.StateId);

            entity.Property(s => s.StateName)
                  .IsRequired()
                  .HasMaxLength(2000);

            entity.Property(s => s.CountryId);

            entity.HasOne(s => s.Country)
               .WithMany(c => c.States)
               .HasForeignKey(s => s.CountryId)
               .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
