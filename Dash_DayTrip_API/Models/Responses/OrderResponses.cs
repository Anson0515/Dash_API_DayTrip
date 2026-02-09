namespace Dash_DayTrip_API.Models.Responses
{
    public class OrderCreateResponse : ApiResponse
    {
        public Order? Order { get; set; }
        public int PackagesCount { get; set; }
        public List<string> GeneratedIds { get; set; } = new();
    }

    public class OrderUpdateResponse : ApiResponse
    {
        public Order? Order { get; set; }
        public int PackagesCount { get; set; }
        public int PackagesAdded { get; set; }
        public int PackagesUpdated { get; set; }
        public int PackagesDeleted { get; set; }
    }

    public class OrderDeleteResponse : ApiResponse
    {
        public string OrderId { get; set; } = string.Empty;
        public int PackagesDeleted { get; set; }
    }

    public class OrderStatusUpdateResponse : ApiResponse
    {
        public string OrderId { get; set; } = string.Empty;
        public string PreviousStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }

    public class OrderListResponse : ApiResponse
    {
        public List<Order> Orders { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? AppliedFilters { get; set; }
    }
}