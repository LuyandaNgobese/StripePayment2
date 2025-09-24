
/*namespace FoodDelivery.Controllers
{
    public class CreateCheckoutSessionRequest
    {
        public IEnumerable<object> Items { get; internal set; }
    }
}*/

namespace StripePayment2.Models
{
    public class CreateCheckoutSessionRequest
    {
        public List<CartItem> Items { get; set; } = new();
    }

    public class CartItem
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int UnitAmount { get; set; } // Price in cents
        public int Quantity { get; set; }
    }
}