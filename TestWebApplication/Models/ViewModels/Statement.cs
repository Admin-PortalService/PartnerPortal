using System.ComponentModel.DataAnnotations;

namespace TestWebApplication.Models.ViewModels
{
    public class Statement
    {
        [Key] public int StatementId { get; set; }

        [Display(Name = "Document Type : ")]
        [Required(ErrorMessage = "Document Type is required")]
        public string DocType { get; set; }

        [Display(Name = "Organization : ")]
        [Required(ErrorMessage = "Organization is required")]
        public int Organization { get; set; }

        [Display(Name = "Project : ")]
        [Required(ErrorMessage = "Project is required")]
        public int Project { get; set; }

        [Display(Name = "Description : ")]
        [Required(ErrorMessage = "Description is required")]
        public string DocDes { get; set; }

        [Display(Name = "Reference : ")]
        [Required(ErrorMessage = "Reference is required")]
        public string Ref { get; set; }

        [Display(Name = "Created Date : ")]
        [Required(ErrorMessage = "Created Date is required")]
        public DateTime? Date { get; set; }

        public virtual List<item_statement> ItemStatement { get; set; } = new List<item_statement>();
    }
}
