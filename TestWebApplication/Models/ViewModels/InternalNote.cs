using System.ComponentModel.DataAnnotations;

namespace TestWebApplication.Models.ViewModels
{
    public class InternalNote
    {
        [Key] public int InterNoteID { get; set; }
        public string Subject { get; set; }
        public string Note { get; set; }

        public int IssueID { get; set; }
        public string CreatedBy { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOn { get; set; }
    }
}
