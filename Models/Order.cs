namespace StripePayment2.Models
{
    public class Order
    {
        public int Id { get; set; }
        public required List<FoodItem> Items { get; set; }
        public int TotalAmount { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? StripeSessionId { get; set; } // Track Stripe session
        public string? PaymentIntentId { get; set; } // Track payment intent
    }
}
