using Cryptocop.Software.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cryptocop.Software.API.Repositories.Contexts
{
    public class CryptoCopDbContext : DbContext
    {
        public CryptoCopDbContext(DbContextOptions<CryptoCopDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<PaymentCard> PaymentCard { get; set; }
        public DbSet<ShoppingCart> ShoppingCart { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItem { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<JwtToken> JwtTokens { get; set; }
    }
}