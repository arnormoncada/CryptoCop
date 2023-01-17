using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Cryptocop.Software.API.Services.Interfaces;
using Cryptocop.Software.API.Models.InputModels;

namespace Cryptocop.Software.API.Controllers
{
    [Authorize]
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetAllOrders()
        {
            var authenticatedUser = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            return Ok(_orderService.GetOrders(authenticatedUser));
        }

        [HttpPost]
        [Route("")]
        public IActionResult AddNewOrder([FromBody] OrderInputModel inputModel)
        {
            var authenticatedUser = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            _orderService.CreateNewOrder(authenticatedUser, inputModel);

            return NoContent();
        }
    }
}