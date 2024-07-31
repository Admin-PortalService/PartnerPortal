
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
namespace TestWebApplication.Views.Incident
{
    public class CreateModel : PageModel
    {

        public string? IssueTitle
        {
            get;
            set;
        }
        [Display(Name = "Category")]
        public string? Category
        {
            get; set;

        }
        [Display(Name = "Project ID")]
        [Required(ErrorMessage = "ProjectID is required.")]
        public int? ProjectID
        {
            get; set;
        }
        [Display(Name = "Project Name")]
        [Required(ErrorMessage = "Project Name is required.")]
        public string? ProjectName
        {
            get; set;
        }

        [Display(Name = "Project Description")]
        [Required(ErrorMessage = "Project Description is required.")]
        public string? ProjectDesc
        {
            get; set;
        }

        [Display(Name = "Area Path")]
        //[DataType(DataType.EmailAddress, ErrorMessage = "Invalid AreaPath")]
        [Required(ErrorMessage = "AreaPath is required.")]
        public string? AreaPath
        {
            get; set;
        }


    }
}
