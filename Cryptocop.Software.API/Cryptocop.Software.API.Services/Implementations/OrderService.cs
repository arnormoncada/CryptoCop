using System;
using System.Collections.Generic;
using Cryptocop.Software.API.Models.Dtos;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Repositories.Interfaces;
using Cryptocop.Software.API.Services.Interfaces;

namespace Cryptocop.Software.API.Services.Implementations
{

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IQueueService _queueService;

        public OrderService(IOrderRepository orderRepository, IShoppingCartService shoppingCartService, IQueueService queueService)
        {
            _orderRepository = orderRepository;
            _shoppingCartService = shoppingCartService;
            _queueService = queueService;
        }


        public IEnumerable<OrderDto> GetOrders(string email)
        {
            return _orderRepository.GetOrders(email);
        }

        public void CreateNewOrder(string email, OrderInputModel order)
        {
            //create order within db
            var createdOrder = _orderRepository.CreateNewOrder(email, order);

            //remove items from shopping cart
            _shoppingCartService.ClearCart(email);

            //send order to queue
            _queueService.PublishMessage("create-order", createdOrder);

        }
    }
}