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
    public class PaymentRepository : IPaymentRepository
    {
        private readonly CryptoCopDbContext _dbContext;
        private readonly IMapper _mapper;
        public PaymentRepository(CryptoCopDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void AddPaymentCard(string email, PaymentCardInputModel paymentCard)
        {
            //get the user associated with the email
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            //create a new payment card object
            var paymentCardEntity = _mapper.Map<PaymentCard>(paymentCard);
            paymentCardEntity.User = user;

            //add the payment card to the database
            _dbContext.PaymentCard.Add(paymentCardEntity);
            _dbContext.SaveChanges();


        }

        public IEnumerable<PaymentCardDto> GetStoredPaymentCards(string email)
        {
            //get the user associated with the email
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            //get all the payment cards associated with the user
            var paymentCards = _dbContext.PaymentCard.Where(a => a.User == user);

            //map the payment cards to a list of PaymentCardDto objects
            return _mapper.Map<IEnumerable<PaymentCardDto>>(paymentCards);
        }
    }
}