
using System.ComponentModel.DataAnnotations;
//#nullable disable

namespace TestWebApplication.Models.ViewModels
{
    public class Incident
    {        
        [Key]
        public int IssueID { get; set; }

        [Required(ErrorMessage = "Issue Title is required.")]
        public string? IssueTitle { get; set; }
        public string? Category { get; set; }

        [Required(ErrorMessage = "Project is required.")]
        public int? ProjectID
        {
            get; set;
        }
      
        [Required(ErrorMessage = "Module is required.")]
        public int? ModuleID { get; set; }        
        
        public string? AreaPath { get; set; }
        public string? Priority{ get; set; }

        [Required(ErrorMessage = "Please fill out detail description.")]
        public string? DetailDesc{ get; set; }              

        [Required]
        public string ?CreatedBy { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOn { get; set; }
        [Required]
        public string ?ModifiedBy { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ModifiedOn { get; set; }

        public string? Status { get; set; }

        #nullable enable
        public string? UploadFile { get; set; } = string.Empty;

        public Boolean IsAssigned
        {
            get; set;
        }

        public string? Assignee
        {
            get; set;
        }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? TargetDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ActualDate { get; set; }

        public DateTime? PauseStart { get; set; }
        public DateTime? PauseEnd { get; set; }
        public string? PauseDuration { get; set; }
        public string? SLADate { get; set; }
        public bool HasBreached { get; set; }

        public string? Description { get; set; }
        public string? AltContact { get; set; }
        public string? AltPhone { get; set; }
        public string? AltEmail { get; set; }
    }
}


