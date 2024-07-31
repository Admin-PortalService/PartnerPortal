using System.ComponentModel.DataAnnotations;
namespace TestWebApplication.Models.ViewModels
{
    public class Attachment
    {
        [Key]
        public int AttachID { get; set; }

        public int IssueID { get; set; }

        public string? AttachName { get; set; } = string.Empty;
        public string CreatedBy { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOn { get; set; }

        public byte[]? Content { get; set; }
        //public virtual Incident? IssueID { get; set; }

    }
}
