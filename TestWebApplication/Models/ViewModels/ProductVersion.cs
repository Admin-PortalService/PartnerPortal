using System.ComponentModel.DataAnnotations;

namespace TestWebApplication.Models.ViewModels
{
    public class ProductVersion
    {
        [Key] public int ProductVerId { get; set; }
        public string Description { get; set; }
    }
}
