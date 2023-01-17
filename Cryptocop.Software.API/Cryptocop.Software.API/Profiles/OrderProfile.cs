using AutoMapper;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Models.Dtos;
using Cryptocop.Software.API.Models.Entities;


public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDto>()
        .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate.ToString("dd-MM-yyyy")))
        .ForMember(dest => dest.CreditCard, opt => opt.MapFrom(src => src.MaskedCreditCard));

        CreateMap<OrderDto, Order>()
        .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)));


        CreateMap<OrderInputModel, Order>();
        CreateMap<OrderInputModel, OrderItem>();
        CreateMap<OrderItem, OrderItemDto>();
        CreateMap<ShoppingCartItemDto, OrderItemDto>()
        .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.UnitPrice * src.Quantity));
    }
}