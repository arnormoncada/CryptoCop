using System.Collections.Generic;
using Cryptocop.Software.API.Models.Dtos;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Repositories.Contexts;
using Cryptocop.Software.API.Repositories.Interfaces;
using System.Linq;
using AutoMapper;
using Cryptocop.Software.API.Models.Entities;

namespace Cryptocop.Software.API.Repositories.Implementations
{
    public class AddressRepository : IAddressRepository
    {
        private readonly CryptoCopDbContext _dbContext;
        private readonly IMapper _mapper;

        public AddressRepository(CryptoCopDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void AddAddress(string email, AddressInputModel address)
        {
            //get the user associated with the email
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            //create a new address object
            var addressEntity = _mapper.Map<Address>(address);
            addressEntity.User = user;

            //add the address to the database
            _dbContext.Address.Add(addressEntity);
            _dbContext.SaveChanges();
        }

        public IEnumerable<AddressDto> GetAllAddresses(string email)
        {
            //get the user associated with the email
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            //get all the addresses associated with the user
            var addresses = _dbContext.Address.Where(a => a.User == user);

            //map the addresses to a list of AddressDto objects
            return _mapper.Map<IEnumerable<AddressDto>>(addresses);
        }

        public void DeleteAddress(string email, int addressId)
        {
            //get the user associated with the email
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            //get the address associated with the user and the addressId
            var address = _dbContext.Address.FirstOrDefault(a => a.User == user && a.Id == addressId);

            if (address == null) { throw new System.Exception("The address does not exist for this user"); }

            //remove the address from the database
            _dbContext.Address.Remove(address);
            _dbContext.SaveChanges();
        }
    }
}