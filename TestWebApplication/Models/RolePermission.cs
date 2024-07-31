using System.ComponentModel.DataAnnotations;

namespace TestWebApplication.Models
{
    public class RolePermission
    {
        [Key]
        public int RolePermissionID { get; set; }

        [Required]
        public Guid RoleID { get; set; }

        [Required]
        public string ModuleName { get; set; }

        [Required]
        public bool IsCreateAccess { get; set; }

        [Required]
        public bool IsReadAccess { get; set; }

        [Required]
        public bool IsUpdateAccess { get; set; }             

        [Required]
        public bool IsDeleteAccess { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public string ModifiedBy { get; set; }

        [Required]
        public DateTime ModifiedOn { get; set; }


    }
}
