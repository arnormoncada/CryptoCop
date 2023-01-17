using System.ComponentModel.DataAnnotations;

namespace Cryptocop.Software.API.Models.InputModels
{
    public class PaymentCardInputModel
    {
        [Required]
        [MinLength(3)]
        public string CardholderName { get; set; }

        [Required]
        [CreditCard]
        public string CardNumber { get; set; }

        // [RegularExpression("^(1[012]|[1-9])$")] //inclusive 1-12
        [Range(1, 12)]
        public int? Month { get; set; }

        // [RegularExpression("\b([0-9]|[1-9][0-9])\b")] //inclusive 0-99
        [Range(0, 99)]
        public int? Year { get; set; }




    }
}