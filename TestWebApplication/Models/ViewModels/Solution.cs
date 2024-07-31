using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
//#nullable disable
namespace TestWebApplication.Models.ViewModels
{
    public class Solution
    {
        [Key]
        public int SolutionID { get; set; }
        public int IssueID { get; set; }
        public string SolTitle { get; set; }
        public string SolDesc { get; set; }
        public string? Note { get; set; }

        public string CreatedBy { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOn { get; set; }

    }
}
