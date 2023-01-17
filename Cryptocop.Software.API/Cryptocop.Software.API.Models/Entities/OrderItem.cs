using System;

namespace Cryptocop.Software.API.Models.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        public Order Order { get; set; }

        public string ProductIdentifier { get; set; }

        public float Quantity { get; set; }

        public float UnitPrice { get; set; }

        public float TotalPrice { get; set; }



    }
}