using System.ComponentModel.DataAnnotations;

namespace TestWebApplication.Models.ViewModels
{
    public class AssignLog
    {
        [Key]
        public int AssignLogID { get; set; }
        public int IssueID { get; set; }
        public int AssignID { get; set; }
        public string IssueName { get; set; }
        public string AssignBy { get; set; }
        public string? AssignTo { get; set; }
        public string ModifiedBy { get; set; }
        public string? Description { get; set; }
        public string? Remark { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ModifiedOn { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AssignDate { get; set; }
    }
}
