using Microsoft.AspNetCore.Mvc;
//using System.Web.Helpers;

namespace TestWebApplication.Models.ViewModels
{
    public class IncidentComment 
    {
            public Incident Incident { get; set; }
            public Comment Comment { get; set; }
        public Attachment Attachment { get; set; }

    }
}
