using AutoMapper;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Models.Dtos;
using Cryptocop.Software.API.Models.Entities;


public class PaymentProfile : Profile
{
    public PaymentProfile()
    {
        CreateMap<PaymentCard, PaymentCardDto>();
        CreateMap<PaymentCardInputModel, PaymentCard>();
    }
}