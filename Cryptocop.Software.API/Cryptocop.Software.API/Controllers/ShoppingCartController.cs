using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Cryptocop.Software.API.Services.Interfaces;
using Cryptocop.Software.API.Models.InputModels;

namespace Cryptocop.Software.API.Controllers
{
    [Authorize]
    [Route("api/cart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;
        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetCartItems()
        {
            var authenticatedUser = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            return Ok(_shoppingCartService.GetCartItems(authenticatedUser));
        }

        [HttpPost]
        [Route("")]
        async public Task<IActionResult> AddCartItem([FromBody] ShoppingCartItemInputModel shoppingCartItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //set the item to lowercase
            shoppingCartItem.ProductIdentifier = shoppingCartItem.ProductIdentifier.ToLower();

            var authenticatedUser = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            await _shoppingCartService.AddCartItem(authenticatedUser, shoppingCartItem);
            return NoContent();
        }


        [HttpDelete]
        [Route("{id}")]
        public IActionResult RemoveCartItem(int id)
        {
            var authenticatedUser = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            _shoppingCartService.RemoveCartItem(authenticatedUser, id);
            return NoContent();
        }

        [HttpPatch]
        [Route("{id}")]
        public IActionResult UpdateCartItemQuantity([FromBody] ShoppingCartItemInputModel inputModel, int id)
        {
            //I did not want to change the validation in the model (e.g. productidentifier required) since it needs to be required
            //according to the assignment description
            // or create a new model for this, since Arnar said it was not necessary on two occasions on piazza
            //so I catch the invalid inputs here and throw a bad request

            //try to use inputmodel.quantity
            try
            {
                //if it is indeed a number but less than 1, throw bad request
                if (inputModel.Quantity <= 0)
                {
                    return BadRequest("Quantity must be greater than 0");
                }
                var authenticatedUser = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                _shoppingCartService.UpdateCartItemQuantity(authenticatedUser, id, inputModel.Quantity.Value);
                return NoContent();
            }
            catch (Exception e)
            {
                //if it was a string or something else, throw bad request
                if (e is System.NullReferenceException) { return BadRequest("Quantity must be a number"); }
                //else throw the correct exception message
                else { return BadRequest(e.Message); }


            }



        }


        [HttpDelete]
        [Route("")]
        public IActionResult ClearCart()
        {
            var authenticatedUser = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            _shoppingCartService.ClearCart(authenticatedUser);
            return NoContent();
        }
    }
}