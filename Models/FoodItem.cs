namespace StripePayment2.Models
{
    public class FoodItem
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int Price { get; set; }
        public required string Description { get; set; }
    }
}
