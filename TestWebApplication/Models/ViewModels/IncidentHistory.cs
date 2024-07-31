using System.ComponentModel.DataAnnotations;

namespace TestWebApplication.Models.ViewModels
{
    public class IncidentHistory
    {
        [Key] public int Id { get; set; }
        public int IssueID { get; set; }
        public int StatusID { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOn { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ModifiedOn { get; set; }
    }
}
