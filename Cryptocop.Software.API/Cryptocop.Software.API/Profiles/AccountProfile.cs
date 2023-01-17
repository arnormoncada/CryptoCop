using AutoMapper;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Models.Dtos;
using Cryptocop.Software.API.Models.Entities;


public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<RegisterInputModel, User>();
        CreateMap<LoginInputModel, User>();
        CreateMap<JwtToken, JwtTokenDto>();
    }
}