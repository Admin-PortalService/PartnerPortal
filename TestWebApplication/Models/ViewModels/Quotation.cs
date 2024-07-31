using System.ComponentModel.DataAnnotations;

namespace TestWebApplication.Models.ViewModels
{
    public class Quotation
    {
        [Key] public int QuotationId { get; set; }

        [Display(Name = "Quotation Type : ")]
        [Required(ErrorMessage = "Quotation Type is required")]
        public string QuotationType { get; set; }
        [Display(Name = "Organization : ")]
        [Required(ErrorMessage = "Organization is required")]
        public int Organization { get; set; }

        [Display(Name = "Project : ")]
        [Required(ErrorMessage = "Project is required")]
        public int Project { get; set; }

        [Display(Name = "Description : ")]
        [Required(ErrorMessage = "Description is required")]
        public string Desc { get; set; }

        [Display(Name = "Created Date : ")]
        [Required(ErrorMessage = "Created Date is required")]
        public DateTime? Date { get; set; }

        public virtual List<ItemQuotation> ItemQuotation { get; set; } = new List<ItemQuotation>();
    }
}
