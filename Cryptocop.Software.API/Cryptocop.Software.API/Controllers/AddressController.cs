using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Cryptocop.Software.API.Services.Interfaces;
using Cryptocop.Software.API.Models.InputModels;

namespace Cryptocop.Software.API.Controllers
{
    [Authorize]
    [Route("api/addresses")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        [Route("")]
        public ActionResult GetAllAddresses()
        {
            var authenticatedUser = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            return Ok(_addressService.GetAllAddresses(authenticatedUser));
        }

        [HttpPost]
        [Route("")]
        public ActionResult AddAddress([FromBody] AddressInputModel address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var authenticatedUser = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            _addressService.AddAddress(authenticatedUser, address);

            return NoContent();
        }


        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteAddress(int id)
        {
            var authenticatedUser = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            _addressService.DeleteAddress(authenticatedUser, id);

            return NoContent();
        }
    }
}