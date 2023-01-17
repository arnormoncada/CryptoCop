using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cryptocop.Software.API.Models.Dtos;
using Cryptocop.Software.API.Models.Entities;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Repositories.Contexts;
using Cryptocop.Software.API.Repositories.Helpers;
using Cryptocop.Software.API.Repositories.Interfaces;

namespace Cryptocop.Software.API.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly CryptoCopDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public OrderRepository(CryptoCopDbContext dbContext, IMapper mapper, IShoppingCartRepository shoppingCartRepository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _shoppingCartRepository = shoppingCartRepository;
        }

        public IEnumerable<OrderDto> GetOrders(string email)
        {

            //get all orders with given email
            var orders = _mapper.Map<IEnumerable<OrderDto>>(_dbContext.Order.Where(o => o.Email == email));

            foreach (var order in orders)
            {
                //get order items for each order
                var orderItemPerOrder = _dbContext.OrderItem.Where(o => o.Order.Id == order.Id).ToList();
                //map order items to dto
                order.OrderItems = _mapper.Map<IEnumerable<OrderItemDto>>(orderItemPerOrder);

            }
            return orders;
        }

        public OrderDto CreateNewOrder(string email, OrderInputModel order)
        {
            //get user info
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            //get address info
            var address = _dbContext.Address.FirstOrDefault(a => a.Id == order.AddressId && a.User.Id == user.Id);

            if (address == null) { throw new Exception("Address not found for this user"); }

            //get payment card info
            var paymentCard = _dbContext.PaymentCard.FirstOrDefault(p => p.Id == order.PaymentCardId && p.User.Id == user.Id);

            if (paymentCard == null) { throw new Exception("Payment card not found for this user"); }

            //get masked cardnumber
            var maskedCardNumber = PaymentCardHelper.MaskPaymentCard(paymentCard.CardNumber);

            //get the cart items for the current order
            var cartItems = _shoppingCartRepository.GetCartItems(email);

            //calculate its total price
            var totalCartPrice = cartItems.Sum(c => c.TotalPrice);

            //create order
            var newOrder = new Order
            {
                Email = email,
                FullName = user.FullName,
                StreetName = address.StreetName,
                HouseNumber = address.HouseNumber,
                ZipCode = address.ZipCode,
                Country = address.Country,
                City = address.City,
                CardholderName = paymentCard.CardholderName,
                MaskedCreditCard = maskedCardNumber,
                OrderDate = DateTime.Now,
                TotalPrice = totalCartPrice

            };

            //add to database
            _dbContext.Order.Add(newOrder);
            _dbContext.SaveChanges();

            //add the order items to database
            foreach (var item in cartItems)
            {

                _dbContext.OrderItem.Add(new OrderItem
                {
                    Order = newOrder,
                    ProductIdentifier = item.ProductIdentifier,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                });
            }
            _dbContext.SaveChanges();

            //create order dto
            var orderDto = _mapper.Map<OrderDto>(newOrder);
            orderDto.CreditCard = paymentCard.CardNumber;
            orderDto.OrderItems = _mapper.Map<IEnumerable<OrderItemDto>>(cartItems);


            //return the order to service to be sent over message queue
            return orderDto;


        }
    }
}