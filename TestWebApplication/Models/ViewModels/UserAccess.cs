using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestWebApplication.Models.ViewModels
{
    public class UserAccess 
    {
        [Key]
        public int UserID { get; set; }


        [Display(Name = "UserName")]
        [Required(ErrorMessage = "UserName is required.")]
        public string UserName { get; set; }


        [Display(Name = "User Description")]
        public string? UserDesc { get; set; }

        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Start Date is required.")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [Required(ErrorMessage = "End Date is required.")]
        public DateTime EndDate { get; set; }

        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOn { get; set; }

    
        public string ModifiedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ModifiedOn { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        //[RegularExpression(@"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$", ErrorMessage = "Invalid pattern.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$", ErrorMessage = "Invalid pattern.")]

        public string? UserMail { get; set; }

        [NotMapped]
        public List<int> CustomerID { get; set; }

        [Display(Name = "RoleName")]
        public string? RoleName { get; set; }

    }
}
