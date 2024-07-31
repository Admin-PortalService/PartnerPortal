using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestWebApplication.Models.ViewModels
{
    public class User_Customer
    {
        [Key] public int Id { get; set; }

        [ForeignKey("UserAccess")]
        public int UserID { get; set; }
        public virtual UserAccess? user { get; set; }

        [ForeignKey("Customer")]
        public int CustomerID { get; set; }
        public virtual Customer? customer { get; set; } 
    }
}
