using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
//#nullable disable
namespace TestWebApplication.Models.ViewModels
{
    public class Comment 
    {
        [Key]
        public int CmtID { get; set; }
        public string Subject { get; set; }
        public string CmtMsg { get; set; }
      
        public int IssueID { get; set; }
        public string CreatedBy { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOn { get; set; }
        
    }
}
