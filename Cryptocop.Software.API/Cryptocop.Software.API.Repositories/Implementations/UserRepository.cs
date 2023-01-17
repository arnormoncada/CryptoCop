using System;
using System.Linq;
using Cryptocop.Software.API.Models.Dtos;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Models.Entities;
using Cryptocop.Software.API.Repositories.Contexts;
using Cryptocop.Software.API.Repositories.Interfaces;
using Cryptocop.Software.API.Repositories.Helpers;
using AutoMapper;

namespace Cryptocop.Software.API.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly CryptoCopDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ITokenRepository _tokenRepository;

        public UserRepository(CryptoCopDbContext dbContext, IMapper mapper, ITokenRepository tokenRepository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _tokenRepository = tokenRepository;
        }


        public UserDto CreateUser(RegisterInputModel inputModel)
        {
            var exists = _dbContext.Users.FirstOrDefault(u => u.Email == inputModel.Email);
            //return null if user already exists
            if (exists != null) { throw new Exception("User with this email already exist"); }

            //If user does not exist, create new user
            var user = _mapper.Map<User>(inputModel);

            //hash the password and overwrite the user entity.
            user.HashedPassword = HashingHelper.HashPassword(inputModel.Password);

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            //create new shopping cart associated with user
            _dbContext.ShoppingCart.Add(new ShoppingCart { User = user });
            _dbContext.SaveChanges();

            //create new token associated with user
            var token = _tokenRepository.CreateNewToken();
            var retUser = _mapper.Map<UserDto>(user);
            retUser.TokenId = token.Id;

            return retUser;
        }

        public UserDto AuthenticateUser(LoginInputModel loginInputModel)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == loginInputModel.Email
            && u.HashedPassword == HashingHelper.HashPassword(loginInputModel.Password));

            if (user == null) { return null; }

            var token = _tokenRepository.CreateNewToken();

            var retUser = _mapper.Map<UserDto>(user);
            retUser.TokenId = token.Id;

            return retUser;
        }
    }
}