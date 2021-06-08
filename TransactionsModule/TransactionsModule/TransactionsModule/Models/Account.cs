using System.ComponentModel.DataAnnotations;

namespace TransactionsModule.Models
{
    public class Account
    {
        [Required]
        public int AccountId { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public double Amount { get; set; }
        public string Narration { get; set; }
    }
}
