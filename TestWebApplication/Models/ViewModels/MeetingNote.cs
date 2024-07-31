using System.ComponentModel.DataAnnotations;

namespace TestWebApplication.Models.ViewModels
{
    public class MeetingNote
    {
        [Key] public int MeetingNoteId { get; set; }

        [Display(Name = "Organization : ")]
        [Required(ErrorMessage = "Organization is required")]
        public int Organization { get; set; }

        [Display(Name = "Project : ")]
        [Required(ErrorMessage = "Project is required")]
        public int Project { get; set; }

        [Display(Name = "Meeting Title : ")]
        [Required(ErrorMessage = "Meeting Title is required")]
        public string Title { get; set; }

        [Display(Name = "Meeting Note : ")]
        [Required(ErrorMessage = "Meeting Note is required")]
        public string Note { get; set; }

        [Display(Name = "Attendees : ")]
        public string? Attendance { get; set; }        

        [Display(Name = "Meeting Date : ")]
        [Required(ErrorMessage = "Meeting Date is required")]
        public DateTime? Date { get; set; }
    }
}
