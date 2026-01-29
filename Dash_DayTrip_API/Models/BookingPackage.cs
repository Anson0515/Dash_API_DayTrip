using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Dash_DayTrip_API.Models
{
    [Table("BookingPackages")]
    public class BookingPackage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingPackageId { get; set; }
        
        public string BookingId { get; set; } = string.Empty;
        public string PackageId { get; set; } = string.Empty;
        public string PackageName { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public int NoOfPax { get; set; }
        
        public bool BoatFareEnabled { get; set; }
        public decimal? BoatFareAmount { get; set; }
        public string? BoatFareCalcType { get; set; }
        
        public bool GratuityEnabled { get; set; }
        public decimal? GratuityAmount { get; set; }
        public string? GratuityCalcType { get; set; }
        
        public decimal LineTotal { get; set; }
        
        // Navigation property
        [ForeignKey("BookingId")]
        [JsonIgnore]
        public virtual DayTripBooking? Booking { get; set; }
    }
}