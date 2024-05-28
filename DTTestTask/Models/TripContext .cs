using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DTTestTask.Models
{
    public class TripContext : DbContext
    {
        public DbSet<Trip> Trips { get; set; } = default!;

        private readonly IConfiguration _configuration;

        public TripContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")!);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Trip>().ToTable("Trips");
            modelBuilder.Entity<Trip>().ToTable("Trips").HasKey(t => t.Id);
            modelBuilder.Entity<Trip>().ToTable("Trips").Property(t => t.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Trip>().HasIndex(t => t.TripDistance);
            modelBuilder.Entity<Trip>().HasIndex(t => t.FareAmount);
            modelBuilder.Entity<Trip>().HasIndex(t => t.PickupLocationID);

            modelBuilder.Entity<Trip>()
       .HasIndex(t => new { t.PickupLocationID, t.TipAmount });
        }
    }

}
