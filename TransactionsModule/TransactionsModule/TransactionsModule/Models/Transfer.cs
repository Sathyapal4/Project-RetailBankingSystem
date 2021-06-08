using System.ComponentModel.DataAnnotations;

namespace TransactionsModule.Models
{
    public class Transfer
    {
        [Required]
        public int Source_AccountId { get; set; }
        [Required]
        public int Target_AccountId { get; set; }
        [Range(0, double.MaxValue)]
        [Required]
        public double Amount { get; set; }
    }
}
