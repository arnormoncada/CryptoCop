using Cryptocop.Software.API.Models.Dtos;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Repositories.Interfaces;
using Cryptocop.Software.API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Cryptocop.Software.API.Services.Implementations
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ICryptoCurrencyService _cryptoCurrencyService;

        public ShoppingCartService(IShoppingCartRepository shoppingCartRepository, ICryptoCurrencyService cryptoCurrencyService)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _cryptoCurrencyService = cryptoCurrencyService;
        }


        public IEnumerable<ShoppingCartItemDto> GetCartItems(string email)
        {
            return _shoppingCartRepository.GetCartItems(email);
        }

        public async Task AddCartItem(string email, ShoppingCartItemInputModel shoppingCartItemItem)
        {
            //get the available cryptocurrencies
            var cryptoCurrencies = await _cryptoCurrencyService.GetAvailableCryptocurrencies();

            //get the cryptocurrency from the api given by the input model
            var cryptoCurrency = cryptoCurrencies.FirstOrDefault(c => c.Symbol == shoppingCartItemItem.ProductIdentifier
            .ToUpper());

            if (cryptoCurrency == null) { throw new Exception("The cryptocurrency cannot be added to cart. Available crypto's are: btc, eth, usdt and xmr"); }

            _shoppingCartRepository.AddCartItem(email, shoppingCartItemItem, cryptoCurrency.PriceInUsd);


        }

        public void RemoveCartItem(string email, int id)
        {
            _shoppingCartRepository.RemoveCartItem(email, id);
        }

        public void UpdateCartItemQuantity(string email, int id, float quantity)
        {
            _shoppingCartRepository.UpdateCartItemQuantity(email, id, quantity);
        }

        public void ClearCart(string email)
        {
            _shoppingCartRepository.ClearCart(email);
        }
    }
}
