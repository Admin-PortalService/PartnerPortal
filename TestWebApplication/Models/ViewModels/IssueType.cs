using System.ComponentModel.DataAnnotations;
namespace TestWebApplication.Models.ViewModels
{
    public class IssueType 
    {
        [Key]
        public int IssueTypeID { get; set; }

        [Display(Name = "Issue Type")]
        [Required]
        public string IssueTypeName { get; set; }

        [Display(Name = "Description")]
        [Required]
        public string IssueTypeDesc { get; set; }

        [Display(Name = "Issue Code")]
        [Required]
        public string IssueCode { get; set; }

    }
}
