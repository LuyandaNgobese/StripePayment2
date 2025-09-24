using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using StripePayment2.Models; // <-- add this    


namespace StripePayment2.Controllers // namespace should match your project name
{
    public class PaymentController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IConfiguration config, ILogger<PaymentController> logger)
        {
            _config = config;
            _logger = logger;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("api/payments/create-checkout-session")]
        public IActionResult CreateCheckoutSession([FromBody] CreateCheckoutSessionRequest request)
        {
            /* var options = new SessionCreateOptions
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
             };*/

            try
            {
                // Create line items from cart items
                var lineItems = new List<SessionLineItemOptions>();

                foreach (var item in request.Items)
                {
                    lineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = item.UnitAmount, // Already in cents
                            Currency = "zar",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.ProductName,
                                Description = $"Product ID: {item.ProductId}"
                            }
                        },
                        Quantity = item.Quantity
                    });
                }

                  var options = new SessionCreateOptions
                  {
                      PaymentMethodTypes = new List<string> { "card" },
                      Mode = "payment",
                      Currency = "zar",
                      // ✅ FIXED URL - Use direct URL with proper placeholder
                      SuccessUrl = "https://localhost:7028/payment/success?session_id={CHECKOUT_SESSION_ID}",
                      CancelUrl = "https://localhost:7028/payment/cancel",
                      LineItems = lineItems
                  };

                var service = new SessionService();
                var session = service.Create(options);

                return Ok(new { url = session.Url });      
            }

            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error creating checkout session");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating checkout session");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }


         // public IActionResult Success() => View();     created below
         // public IActionResult Cancel() => View();

        [HttpGet("payment/success")]
        public async Task<IActionResult> Success(string session_id)
        {
            if (string.IsNullOrEmpty(session_id) || session_id.Contains("CHECK"))
            {
                // If no valid session ID, show basic success
                ViewData["Paid"] = true;
                ViewData["Message"] = "Payment completed successfully!";
                return View();
            }

            try
            {
                var service = new SessionService();
                var session = await service.GetAsync(session_id);

                ViewData["Paid"] = session.PaymentStatus == "paid";
                ViewData["SessionId"] = session.Id;
                ViewData["AmountTotal"] = session.AmountTotal / 100.0; // Convert to Rands
                ViewData["Currency"] = session.Currency.ToUpper();

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving session");
                ViewData["Error"] = "Unable to retrieve payment details";
                return View();
            }
        }

        [HttpGet("payment/cancel")]
        public IActionResult Cancel()
        {
            return View();
        }
    }
}
    

