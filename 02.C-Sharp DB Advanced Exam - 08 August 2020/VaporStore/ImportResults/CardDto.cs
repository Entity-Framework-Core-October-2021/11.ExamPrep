using System.ComponentModel.DataAnnotations;

namespace VaporStore.ImportResults
{
    public class CardDto
    {
        [Required]
        [RegularExpression(GlobalConstants.CardNumberRegex)]
        public string Number { get; set; }

        [Required]
        [MaxLength(GlobalConstants.CardCvcMaxLength)]
        public string Cvc { get; set; }

        [Required]
        public string Type { get; set; }
    }
}