using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestWebApplication.Models.ViewModels
{
    public class ItemQuotation
    {
        [Key] public int itemQuotationId { get; set; }

        [ForeignKey("Quotation")]
        [Required]
        public int quotationId { get; set; }
        public virtual Quotation? quotaion { get; private set; }
        [Required]
        public int itemId { get; set; }
        public virtual Item item { get; private set; } = new Item();

        [RegularExpression(@"^\d+.\d{0,2}$", ErrorMessage = "Please enter a valid number.")]
        [Display(Name = "Amount ")]
        public decimal Amount { get; set; }
    }
}
