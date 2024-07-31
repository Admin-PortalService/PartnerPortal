using System.ComponentModel.DataAnnotations;

namespace TestWebApplication.Models.ViewModels
{
    public class Projects
    {
        [Key]
        public int ProjectID { get; set; }

        [Display(Name = "Customer/Organization")]
        public int? CustomerID { get; set; }


        [Display(Name = "Project Name")]
        [Required]
        public string ProjectName { get; set; }

        public string ProjectDesc { get; set; }

        [Display(Name = "Valid From Date")]
        [Required(ErrorMessage = "Valid From Date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ValidFrom { get; set; }


        [Display(Name = "Valid To Date")]
        [Required(ErrorMessage = "Valid To Date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ValidTo { get; set; }
        public DateTime LastDate { get; set; }
        public string? Status { get; set; }
        public string ProjectCode { get; set; }
        [Required]
        public string CreatedBy { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOn { get; set;}

        [Required]
        public string ModifiedBy { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ModifiedOn { get; set; }
        public Boolean? IsActive { get; set; }
    }
}
