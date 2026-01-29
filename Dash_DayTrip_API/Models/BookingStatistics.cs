namespace Dash_DayTrip_API.Models
{
    public class BookingStatistics
    {
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalDeposits { get; set; }
        public decimal OutstandingBalance { get; set; }
        public int TodayBookings { get; set; }
        public decimal TodayRevenue { get; set; }
        public int PendingCount { get; set; }
        public int ConfirmedCount { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
    }
}