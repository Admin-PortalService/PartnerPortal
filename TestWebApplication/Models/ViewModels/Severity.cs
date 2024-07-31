using Microsoft.AspNetCore.Mvc;
//using System.Web.Helpers;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TestWebApplication.Models.ViewModels
{
    public class Severity
    {
        public string? Priority { get; set; }
        public string? PriorityDesc { get; set; }
        public string? IncidentStatus { get; set; }
        public string? PrjStatus { get; set; }
        public string? Level { get; set; }


        [Key]
        public int No { get; set; }
    }
}
