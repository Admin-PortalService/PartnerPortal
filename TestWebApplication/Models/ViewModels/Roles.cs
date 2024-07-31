using System.ComponentModel.DataAnnotations;
namespace TestWebApplication.Models.ViewModels
{
    public class Roles
    {
        [Key]
        public Guid RoleID { get; set; }


        [Display(Name = "Role Type")]
        [Required]
        public string RoleType { get; set; }


        [Display(Name = "Description")]
        [Required]
        public string RoleDesc { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOn { get; set; }

        [Required]
        public String CreatedBy { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ModifiedOn { get; set; }

        [Required]
        public string ModifiedBy { get; set; }

        [Required]
        public Boolean IsInternal { get; set; }

        [Required]
        public Boolean IsActive { get; set; }

    }
}
