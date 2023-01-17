using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Cryptocop.Software.API.Services.Interfaces;
using Cryptocop.Software.API.Models.InputModels;

namespace Cryptocop.Software.API.Controllers
{
    [Authorize]
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        [Route("")]
        public IActionResult AddPaymentCard([FromBody] PaymentCardInputModel paymentCard)
        {
            //get the email of the user from the token
            var authenticatedUser = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //add the payment card to the database
            _paymentService.AddPaymentCard(authenticatedUser, paymentCard);

            return NoContent();

        }

        [HttpGet]
        [Route("")]
        public IActionResult GetPaymentCards()
        {
            var authenticatedUser = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            return Ok(_paymentService.GetStoredPaymentCards(authenticatedUser));
        }
    }
}