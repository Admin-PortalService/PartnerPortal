using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace TestWebApplication.Models.ViewModels
{
    public class Item
    {
        [Key] public int ItemId { get; set; }

        [Display(Name = "Item Name")]
        public string? ItemName { get; set; }

        [Display(Name = "Description")]
        public string ItemDes { get; set; }

        [Display(Name = "Remark ")]
        public string? Remark { get; set; }
    }
}
