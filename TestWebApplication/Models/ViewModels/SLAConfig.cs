using System.ComponentModel.DataAnnotations;

namespace TestWebApplication.Models.ViewModels
{
    public class SLAConfig
    {
        [Key]
        public int Id { get; set; }
        public int StatusID { get; set; }
        public string Priority { get; set; }
        public string SLAName { get; set; }
        public string SLAType { get; set; }
        public int SLAmin { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOn { get; set; }
    }
}
