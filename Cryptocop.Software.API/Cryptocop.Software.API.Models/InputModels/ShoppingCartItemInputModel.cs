using System.ComponentModel.DataAnnotations;

namespace Cryptocop.Software.API.Models.InputModels
{
    public class ShoppingCartItemInputModel
    {
        [Required]
        public string ProductIdentifier { get; set; }

        [Required]
        [Range(0.01, float.MaxValue)] //inclusive 0.01 to maxval
        public float? Quantity { get; set; }

    }
}