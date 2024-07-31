using System.ComponentModel.DataAnnotations;

namespace TestWebApplication.Models.ViewModels
{
    public class ImplementationAssist
    {
        [Key] public int AssistId { get; set; }
        public string Description { get; set; }
    }
}
