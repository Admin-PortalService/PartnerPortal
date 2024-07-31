using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestWebApplication.Models.ViewModels
{
    public class Contract
    {
        [Key]
        public int ContractId { get; set; }
        public int CustomerId { get; set; }
        public int ProjectId { get; set; }
        [Required]
        public Boolean IsActive { get; set; }
        public string Description { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime SetupDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ExpireDate { get; set; }
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
        [Required]
        public int Margin { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? attachFile { get; set; }
        [NotMapped]
        public IFormFile uploadFile { get; set; }

    }
}
