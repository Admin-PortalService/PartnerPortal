using Microsoft.AspNetCore.Mvc;
using TestWebApplication.Data;
using TestWebApplication.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using IronPdf;
using System.Linq;
using Microsoft.CodeAnalysis;
using static System.Reflection.Metadata.BlobBuilder;
using Microsoft.AspNetCore.Authorization;

namespace TestWebApplication.Controllers
{
    [Authorize]
    public class IncReportController : Controller
    {
        private readonly ApplicationDbContext context;
        private IWebHostEnvironment environment;
        public IncReportController(ApplicationDbContext context,IWebHostEnvironment _environment)
        {
            this.context = context;
            environment = _environment;
        }
        public IActionResult IncRptSelect()
        {
            ViewBag.ProjectList = context.Project.ToList();
            return View();
        }
        //public IActionResult IncidentDtlReport(FilterMdl FilterModel, int Project) List <int> Project
		 public IActionResult IncidentDtlReport(FilterMdl FilterModel)
        {
            // var listOfIds = (from n in context.Incident where n.ProjectID == Project select n.ProjectID);
            // var itemEntity = (from m in context.Incident where listOfIds.Contains(m.ProjectID) select m);
            var result = new List<Incident>();
            DateTime defaultDate = default(DateTime);
            var inputDT = defaultDate.ToString().Replace("12:00:00 AM", ""); //output date only
            if (FilterModel.Status == "All" && FilterModel.FromDate == defaultDate && FilterModel.ToDate == defaultDate ) //&& Project == 0
            {
                result = context.Incident
                .OrderBy(incident =>
                incident.Priority == "High" ? 0 :
                incident.Priority == "Medium" ? 1 :
                incident.Priority == "Low" ? 2 : 3).ToList();
            }
            else if (FilterModel.Status == "All" && FilterModel.FromDate != defaultDate && FilterModel.ToDate != defaultDate ) //&& Project != 0
            {
                //result = context.Incident.Where(t => t.CreatedOn >= FilterModel.FromDate && t.CreatedOn <= FilterModel.ToDate).ToList();
                result = context.Incident.Where(t => t.CreatedOn >= FilterModel.FromDate && t.CreatedOn <= FilterModel.ToDate)
               .OrderBy(incident =>
                incident.Priority == "High" ? 0 :
                incident.Priority == "Medium" ? 1 :
                incident.Priority == "Low" ? 2 : 3).ToList();
            }
            else if (FilterModel.Status == "All" && FilterModel.FromDate != defaultDate && FilterModel.ToDate == defaultDate)
            {
                result = context.Incident.Where(t => t.CreatedOn >= FilterModel.FromDate)
                .OrderBy(incident =>
                incident.Priority == "High" ? 0 :
                incident.Priority == "Medium" ? 1 :
                incident.Priority == "Low" ? 2 : 3).ToList();
            }
            else if (FilterModel.Status == "All" && FilterModel.FromDate == defaultDate && FilterModel.ToDate != defaultDate)
            {
                result = context.Incident.Where(t => t.CreatedOn <= FilterModel.ToDate)
                .OrderBy(incident =>
                incident.Priority == "High" ? 0 :
                incident.Priority == "Medium" ? 1 :
                incident.Priority == "Low" ? 2 : 3).ToList();
            }
            else if (FilterModel.Status != "All" && FilterModel.FromDate != defaultDate && FilterModel.ToDate == defaultDate)
            {
                result = context.Incident.Where(t => t.Status == FilterModel.Status && t.CreatedOn >= FilterModel.FromDate)
                    .OrderBy(incident => 
                    incident.Priority == "High" ? 0 : 
                    incident.Priority == "Medium" ? 1 : 
                    incident.Priority == "Low" ? 2 : 3).ToList();
            }
            else if (FilterModel.Status != "All" && FilterModel.FromDate == defaultDate && FilterModel.ToDate != defaultDate)
            {
                result = context.Incident.Where(t => t.Status == FilterModel.Status && t.CreatedOn <= FilterModel.ToDate)
                .OrderBy(incident =>
                incident.Priority == "High" ? 0 :
                incident.Priority == "Medium" ? 1 :
                incident.Priority == "Low" ? 2 : 3).ToList();
            }
            else
            {
                //result = context.Incident.Where(t => t.ProjectID == Project).ToList(); 
				result = context.Incident.Where(t => t.Status == FilterModel.Status)
                .OrderBy(incident => 
                incident.Priority == "High" ? 0 : 
                incident.Priority == "Medium" ? 1 :
                incident.Priority == "Low" ? 2 : 3).ToList();
            }
            
            ViewBag.project = context.Project.ToList();
            ViewBag.module = context.Module.ToList();
            return View(result);
        }
        public void GeneratePDF()
        {
            {
                string dest_path = Path.Combine(this.environment.WebRootPath, "PDF");

                if (!Directory.Exists(dest_path))
                {

                    Directory.CreateDirectory(dest_path);
                }
                var Renderer = new HtmlToPdf();
                var PDF = Renderer.RenderHtmlFileAsPdf("Views/IncReport/Index.cshtml");
                PDF.SaveAs("IncidentReport.pdf");
            }
        }
       
        public async Task<IActionResult> Detail(int Id)
        {
            ViewBag.Incident = await context.Incident.Where(t => t.IssueID == Id).ToListAsync();
            ViewBag.Comment = await context.Comment.Where(t => t.IssueID == Id).OrderBy(t => t.CreatedOn).ToListAsync();
            ViewBag.module = context.Module.ToList();
            ViewBag.project = context.Project.ToList();
            return View();
        }
        public IActionResult Viewer()
        {
            return View();
        }
    }
}
