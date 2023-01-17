using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Cryptocop.Software.API.Services.Interfaces;

namespace Cryptocop.Software.API.Controllers
{

    [Authorize]
    [Route("api/cryptocurrencies")]
    [ApiController]

    public class CryptoCurrencyController : ControllerBase
    {
        private readonly ICryptoCurrencyService _cryptoCurrencyService;

        public CryptoCurrencyController(ICryptoCurrencyService cryptoCurrencyService)
        {
            _cryptoCurrencyService = cryptoCurrencyService;
        }

        [Route("")]
        async public Task<IActionResult> GetCryptoCurrencies()
        {
            var cryptoCurrencies = await _cryptoCurrencyService.GetAvailableCryptocurrencies();

            return Ok(cryptoCurrencies);
        }
    }
}
