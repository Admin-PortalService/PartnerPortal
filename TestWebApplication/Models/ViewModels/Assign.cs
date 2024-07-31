using Microsoft.AspNetCore.Mvc;
//using System.Web.Helpers;
using System.ComponentModel.DataAnnotations;

namespace TestWebApplication.Models.ViewModels
{
    public class Assign 
    {
        [Key]
        public int AssignID { get; set; }

        public int IssueID { get; set; }
        public int UserID { get; set; }
        public string? UserName { get; set; }
        //public string? AssignDesc { get; set; }

        public string? UserMail { get; set; }

        public DateTime AssignOn { get; set; }

        public string? AssignBy { get; set; }

        public string? Description { get; set; }
    }
}

