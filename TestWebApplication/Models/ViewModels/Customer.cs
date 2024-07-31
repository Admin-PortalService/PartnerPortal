using System.ComponentModel.DataAnnotations;

namespace TestWebApplication.Models.ViewModels
{
    public class Customer 
    {
        [Key]
        public int CustomerID { get; set; }

        [Display(Name = "CustomerCode")]
        public string? CustomerCode { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "CustomerName is required.")]
        public string? CustomerName { get; set; }

        [Display(Name = "Email")]
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        //[RegularExpression(@"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$", ErrorMessage = "Invalid pattern.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$", ErrorMessage = "Invalid pattern.")]
        public string? CustomerEmail { get; set; }

        public string? CustomerAddress1 { get; set; }
        public string? CustomerAddress2 { get; set; }
        //public string? City { get; set; }
        //public string? State { get; set; }
        //public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactInfo { get; set; }
        public string? ContactEmail { get; set; }
        public Boolean? IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

     
        public string ModifiedBy { get; set; }
    }
}
