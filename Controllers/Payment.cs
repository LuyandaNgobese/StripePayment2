using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace payments.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IConfiguration _config;

        public PaymentController(IConfiguration config)
        {
            _config = config;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCheckoutSession()
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                Currency = "zar",
                SuccessUrl = Url.Action("Success", "Payment", null, Request.Scheme),
                CancelUrl = Url.Action("Cancel", "Payment", null, Request.Scheme),
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = 6000,
                            Currency = "zar",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Sample Product"
                            }
                        },
                        Quantity = 1
                    }
                }
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Redirect(session.Url);
        }

        public IActionResult Success() => View();
        public IActionResult Cancel() => View();
    }
}
