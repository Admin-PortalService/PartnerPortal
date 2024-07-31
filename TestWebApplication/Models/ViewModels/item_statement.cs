using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestWebApplication.Models.ViewModels
{
    public class item_statement
    {
        [Key] public int itemStateId { get; set; }

        [ForeignKey("Statement")]
        public int statementId { get; set; }
        public virtual Statement? statement { get; private set; }
        public int itemId { get; set; }
        public virtual Item item { get; private set; } = new Item();

        [RegularExpression(@"^\d+.\d{0,2}$", ErrorMessage = "Please enter a valid number.")]
        [Display(Name = "Amount ")]
        public decimal Amount { get; set; }
    }
}
