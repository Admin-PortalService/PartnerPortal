using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
namespace TestWebApplication.Models.ViewModels
{
    public class Module

    {
        [Key]
        public int ModuleID { get; set; }
        public int ProjectID { get; set; }

        [Display(Name = "Module Name")]
        [Required]
        public string? ModuleName { get; set; }

        [Display(Name = "Description")]
        [Required]
        public string? ModuleDesc { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOn { get; set; }

        [Required]
        public string ModifiedBy { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ModifiedOn { get; set; }

    }
}


