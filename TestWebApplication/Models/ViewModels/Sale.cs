using System.ComponentModel.DataAnnotations;

namespace TestWebApplication.Models.ViewModels
{
    public class Sale
    {
        [Key] public int SaleId { get; set; }
        public string Description { get; set; }
    }
}
