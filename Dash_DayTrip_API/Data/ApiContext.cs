using Microsoft.EntityFrameworkCore;
using Dash_DayTrip_API.Models;

namespace Dash_DayTrip_API.Data
{
    public class ApiContext : DbContext
    {
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormSettings> FormSettings { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<DayTripBooking> Bookings { get; set; }
        public DbSet<BookingPackage> BookingPackages { get; set; }
        
        public ApiContext(DbContextOptions<ApiContext> options) 
            : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Form configuration
            modelBuilder.Entity<Form>()
                .HasIndex(f => f.Status);
            modelBuilder.Entity<Form>()
                .HasIndex(f => f.IsDefault);
            
            // FormSettings - One-to-One with Form
            modelBuilder.Entity<FormSettings>()
                .HasOne(fs => fs.Form)
                .WithOne(f => f.FormSettings)
                .HasForeignKey<FormSettings>(fs => fs.FormId);
            
            // Package configuration
            modelBuilder.Entity<Package>()
                .HasOne(p => p.Form)
                .WithMany(f => f.Packages)
                .HasForeignKey(p => p.FormId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Booking configuration
            modelBuilder.Entity<DayTripBooking>()
                .HasIndex(b => b.Status);
            modelBuilder.Entity<DayTripBooking>()
                .HasIndex(b => b.ReferenceNumber);
            
            // BookingPackage configuration
            modelBuilder.Entity<BookingPackage>()
                .HasOne(bp => bp.Booking)
                .WithMany(b => b.BookingPackages)
                .HasForeignKey(bp => bp.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Configure decimal precision for financial fields
            ConfigureDecimalPrecision(modelBuilder);
        }
        
        private void ConfigureDecimalPrecision(ModelBuilder modelBuilder)
        {
            // FormSettings decimals
            modelBuilder.Entity<FormSettings>()
                .Property(fs => fs.DepositAmount).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<FormSettings>()
                .Property(fs => fs.SSTPercentage).HasColumnType("decimal(5,2)");
            
            // Package decimals
            modelBuilder.Entity<Package>()
                .Property(p => p.Price).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Package>()
                .Property(p => p.BoatFareAmount).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Package>()
                .Property(p => p.GratuityAmount).HasColumnType("decimal(10,2)");
            
            // Booking decimals
            modelBuilder.Entity<DayTripBooking>()
                .Property(b => b.Subtotal).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<DayTripBooking>()
                .Property(b => b.TotalBoatFare).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<DayTripBooking>()
                .Property(b => b.TotalGratuity).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<DayTripBooking>()
                .Property(b => b.GrandTotal).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<DayTripBooking>()
                .Property(b => b.DepositPaid).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<DayTripBooking>()
                .Property(b => b.BalanceDue).HasColumnType("decimal(10,2)");
            
            // BookingPackage decimals
            modelBuilder.Entity<BookingPackage>()
                .Property(bp => bp.UnitPrice).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<BookingPackage>()
                .Property(bp => bp.LineTotal).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<BookingPackage>()
                .Property(bp => bp.BoatFareAmount).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<BookingPackage>()
                .Property(bp => bp.GratuityAmount).HasColumnType("decimal(10,2)");
        }
    }
}
