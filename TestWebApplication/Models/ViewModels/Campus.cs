using System.ComponentModel.DataAnnotations;

namespace TestWebApplication.Models.ViewModels
{
    public class Campus
    {
        [Key] public int campusId { get; set; }
        public string Description { get; set; }        
    }
}
