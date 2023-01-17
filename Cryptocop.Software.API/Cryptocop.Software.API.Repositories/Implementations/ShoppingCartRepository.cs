using System.Collections.Generic;
using Cryptocop.Software.API.Models.Dtos;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Repositories.Contexts;
using Cryptocop.Software.API.Repositories.Interfaces;
using Cryptocop.Software.API.Models.Entities;
using AutoMapper;
using System.Linq;
using System;

namespace Cryptocop.Software.API.Repositories.Implementations
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly CryptoCopDbContext _dbContext;
        private readonly IMapper _mapper;

        public ShoppingCartRepository(CryptoCopDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public IEnumerable<ShoppingCartItemDto> GetCartItems(string email)
        {
            //get user
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            //get shopping cart linked to user
            var cart = _dbContext.ShoppingCart.FirstOrDefault(c => c.User.Id == user.Id);


            //get shopping cart items linked to shopping cart
            var cartItems = _mapper.Map<IEnumerable<ShoppingCartItemDto>>(_dbContext.ShoppingCartItem.Where(s => s.ShoppingCart.Id == cart.Id));

            return cartItems;
        }

        public void AddCartItem(string email, ShoppingCartItemInputModel shoppingCartItemItem, float priceInUsd)
        {
            //get user
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            //get shopping cart linked to user
            var cart = _dbContext.ShoppingCart.FirstOrDefault(c => c.User.Id == user.Id);

            //get shopping cart items linked to shopping cart
            var cartItems = _dbContext.ShoppingCartItem.Where(s => s.ShoppingCart.Id == cart.Id);

            //check if the item is already in the cart
            var item = cartItems.FirstOrDefault(c => c.ProductIdentifier == shoppingCartItemItem.ProductIdentifier);


            if (item != null)
            {
                //update quantity if item is already in cart
                item.Quantity += shoppingCartItemItem.Quantity.Value;
            }
            else
            {
                //add new item
                var cartItem = new ShoppingCartItem
                {
                    ShoppingCart = cart,
                    ProductIdentifier = shoppingCartItemItem.ProductIdentifier,
                    Quantity = shoppingCartItemItem.Quantity.Value,
                    UnitPrice = priceInUsd
                };

                _dbContext.ShoppingCartItem.Add(cartItem);
            }

            _dbContext.SaveChanges();
        }

        public void RemoveCartItem(string email, int id)
        {


            //check if the cartitem belongs to the user or exists in general
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            var cart = _dbContext.ShoppingCart.FirstOrDefault(s => s.User.Id == user.Id);
            var cartItem = _dbContext.ShoppingCartItem.FirstOrDefault(s => s.ShoppingCart.Id == cart.Id && s.Id == id);

            if (cartItem == null)
            {
                throw new System.Exception("Item does not exist in cart");
            }

            //remove the cartitem
            _dbContext.ShoppingCartItem.Remove(cartItem);
            _dbContext.SaveChanges();
        }


        public void UpdateCartItemQuantity(string email, int id, float quantity)
        {
            //check if the cartitem belongs to the user or exists in general
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            var cart = _dbContext.ShoppingCart.FirstOrDefault(s => s.User.Id == user.Id);
            var cartItem = _dbContext.ShoppingCartItem.FirstOrDefault(s => s.ShoppingCart.Id == cart.Id && s.Id == id);

            if (cartItem == null)
            {
                throw new System.Exception("Item does not exist in cart");
            }

            //update the quantity
            cartItem.Quantity = quantity;
            _dbContext.SaveChanges();
        }

        public void ClearCart(string email)
        {
            //get user with email
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email).Id;

            //get shopping cart linked to user
            var cartId = _dbContext.ShoppingCart.FirstOrDefault(u => u.User.Id == user).Id;

            //get shopping cart items linked to shopping cart
            var cartItems = _dbContext.ShoppingCartItem.Where(s => s.ShoppingCart.Id == cartId);

            //remove all items
            _dbContext.ShoppingCartItem.RemoveRange(cartItems);
            _dbContext.SaveChanges();
        }

        public void DeleteCart(string email)
        {
            //this method is not used, It was here with the template but
            //get user with email
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email).Id;

            //get shopping cart linked to user
            var cart = _dbContext.ShoppingCart.FirstOrDefault(u => u.User.Id == user);

            //get shopping cart items linked to shopping cart
            var cartItems = _dbContext.ShoppingCartItem.Where(s => s.ShoppingCart.Id == cart.Id);

            //remove all items
            _dbContext.ShoppingCartItem.RemoveRange(cartItems);

            //remove the cart
            _dbContext.ShoppingCart.Remove(cart);
            _dbContext.SaveChanges();
        }
    }
}