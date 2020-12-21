using Stripe;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace TestWebApp.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        public ActionResult Index()
        {
            return View();
        }

        // POST: Payment/Stripe
        [HttpPost]
        public JsonResult Stripe(string order_id, string pi_id)
        {
            try
            {
                StripeConfiguration.ApiKey = "sk_test_4eC39HqLyjWDarjtT1zdp7dc";

                if (String.IsNullOrEmpty(pi_id))
                {
                    var options = new PaymentIntentCreateOptions
                    {
                        Amount = 2000,
                        Currency = "usd",
                        //CaptureMethod = "manual",
                        PaymentMethodTypes = new List<string>
                        {
                            "card",
                        },
                        PaymentMethodOptions = new PaymentIntentPaymentMethodOptionsOptions
                        {
                            Card = new PaymentIntentPaymentMethodOptionsCardOptions
                            {
                                RequestThreeDSecure = "any"
                            }
                        },
                        ReceiptEmail = "aa@aa.com"
                    };
                    var service = new PaymentIntentService();
                    var create = service.Create(options);

                    return Json(new { status = "Success", response = create });
                }
                else
                {
                    /*
                    var service = new PaymentIntentService();
                    var options = new PaymentIntentCaptureOptions
                    {
                        //ReturnUrl = "https://example.com/return_url",
                    };
                    var intent = service.Capture(pi_id, options);
                    return Json(new { status = "Success", response = intent });
                    */
                    var options = new ChargeListOptions { Limit = 100, PaymentIntent = pi_id };
                    var service = new ChargeService();
                    StripeList<Charge> charges = service.List(options);

                    return Json(new { status = "Success", response = charges });
                }
            }
            catch (StripeException se)
            {
                Response.StatusCode = (int)se.HttpStatusCode;
                return Json(new
                {
                    status = "Failure",
                    code = se.HttpStatusCode,
                    message = se.Message,
                    data = se.Data,
                    error = se.StripeError,
                    response = se.StripeResponse
                });
            }
        }
    }
}
