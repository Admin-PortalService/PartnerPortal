using System.ComponentModel.DataAnnotations;

namespace TestWebApplication.Models.ViewModels
{
    public class Reseller
    {
        [Key] public int ResellerId { get; set; }
        public string Description { get; set; }
    }
}
