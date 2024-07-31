using System.ComponentModel.DataAnnotations;

namespace TestWebApplication.Models.ViewModels
{
    public class MaintenanceLog
    {
        [Key]
        public int MaintainLogID { get; set; }
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ModifiedOn { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime LastMaintainDate { get; set; }
    }
}
