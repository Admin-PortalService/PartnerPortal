using System.ComponentModel.DataAnnotations;

namespace TestWebApplication.Models.ViewModels
{
    public class Price
    {
        [Key] public int PriceId { get; set; }
        public string Description { get; set; }
    }
}
