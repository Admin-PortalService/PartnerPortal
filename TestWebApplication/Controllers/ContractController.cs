using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Security.Claims;
using TestWebApplication.Common;
using TestWebApplication.Data;
using TestWebApplication.Helpers;
using TestWebApplication.Models.ViewModels;

namespace TestWebApplication.Controllers
{
    [Authorize]
    public class ContractController : BaseController
    {
        private readonly ApplicationDbContext context;
        public ContractController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [RolePermissionAuthorize("Read")]
        public IActionResult Index()
        {
            try
            {
                var user = HttpContext.User;
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRoleId = context.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).Distinct().SingleOrDefault();
                var isInternal = context.Roles.Where(t => t.RoleID.ToString() == userRoleId.ToUpper()).Select(t => t.IsInternal).Distinct().SingleOrDefault();

                if (isInternal == false)
                {
                    string Name = User.Identity?.Name;
                    var usrId = context.UserAccess.Where(t => t.UserMail == Name).Select(t => t.UserID).FirstOrDefault();
                    var cusIds = context.User_Customer.Where(u => u.UserID == usrId).Select(u => u.CustomerID).ToList();
                    foreach (var cusId in cusIds)
                    {
                        ViewBag.contractList = context.Contract.Where(u => u.CustomerId == cusId).ToList();
                    }
                }

                else
                {
                    ViewBag.contractList = context.Contract.ToList();
                }

                ViewBag.IsInternal = isInternal;
                ViewBag.CustomerList = context.Customer.Where(t => t.CustomerID != 0 && t.IsActive == true).ToList();
                if (TempData["Success"] != null)
                {
                    ViewBag.msg = TempData["Success"];
                }
                return View();
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [RolePermissionAuthorize("Create")]
        public IActionResult Create()
        {
            try
            {
                ViewBag.CustomerList = context.Customer.Where(t => t.CustomerID != 0 && t.IsActive == true).OrderBy(t => t.CustomerName).ToList();
                ViewBag.ProjectList = context.Project.Where(p => p.ProjectID != 0).OrderBy(p => p.ProjectName).ToList();
                return View();
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [RolePermissionAuthorize("Create")]
        public IActionResult Create(Contract model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var fileName = "";
                    fileName = model.uploadFile.FileName;
                    fileName = Path.GetFileName(fileName);
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/contractFile", fileName);
                    var stream = new FileStream(uploadPath, FileMode.Create);
                    model.uploadFile.CopyToAsync(stream);
                    stream.Dispose();

                    Contract contract = new()
                    {
                        CustomerId = model.CustomerId,
                        ProjectId = model.ProjectId,
                        IsActive = model.IsActive,
                        Description = model.Description,
                        SetupDate = model.SetupDate,
                        ExpireDate = model.ExpireDate,
                        CreatedBy = model.CreatedBy,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        ModifiedBy = model.ModifiedBy,
                        Margin = model.Margin,
                        Country = model.Country,
                        State = model.State,
                        attachFile = fileName
                    };
                    context.Contract.Add(contract);
                    context.SaveChanges();
                }
                else
                {
                    var errors = new Dictionary<string, IEnumerable<string>>();
                    List<string> message = new List<string>();
                    foreach (var keyValue in ModelState)
                    {
                        if (keyValue.Value.Errors.Count > 0)
                        {
                            errors[keyValue.Key] = keyValue.Value.Errors.Select(e => e.ErrorMessage);//ValidationState
                            var msg1 = keyValue.Key;
                            var msg2 = keyValue.Value.ValidationState;
                            string msg = msg1 + new string(' ', 3) + "is" + new string(' ', 3) + msg2;
                            message.Add(msg);
                        }
                    }
                    var strings = (from o in message
                                   select o.ToString()).ToList();
                    ViewBag.msgList = strings;
                    ViewBag.CustomerList = context.Customer.Where(t => t.CustomerID != 0 && t.IsActive == true).OrderBy(t => t.CustomerName).ToList();
                    ViewBag.ProjectList = context.Project.Where(p => p.ProjectID != 0).OrderBy(p => p.ProjectName).ToList();
                    return View(model);
                }
                TempData["Success"] = "Contract Successfully Created";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [RolePermissionAuthorize("Update")]
        public IActionResult Edit(int id)
        {
            try
            {
                var obj = context.Contract.SingleOrDefault(x => x.ContractId == id);
                var result = new Contract()
                {
                    ContractId = obj.ContractId,
                    CustomerId = obj.CustomerId,
                    ProjectId = obj.ProjectId,
                    IsActive = obj.IsActive,
                    Description = obj.Description,
                    SetupDate = obj.SetupDate,
                    ExpireDate = obj.ExpireDate,
                    CreatedBy = obj.CreatedBy,
                    CreatedOn = obj.CreatedOn,
                    ModifiedBy = obj.ModifiedBy,
                    ModifiedOn = obj.ModifiedOn,
                    Margin = obj.Margin,
                    Country = obj.Country,
                    State = obj.State,
                    attachFile = obj.attachFile
                };

                ViewBag.CustomerList = context.Customer.Where(t => t.CustomerID != 0 && t.IsActive == true).OrderBy(t => t.CustomerName).ToList();
                ViewBag.Customer = context.Customer.SingleOrDefault(t => t.CustomerID == result.CustomerId);

                ViewBag.ProjectList = context.Project.Where(p => p.ProjectID != 0).OrderBy(p => p.ProjectName).ToList();
                ViewBag.Project = context.Project.SingleOrDefault(p => p.ProjectID == result.ProjectId);
                return View(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [RolePermissionAuthorize("Update")]
        public IActionResult Edit(Contract model)
        {
            try
            {
                if (model.uploadFile != null)
                {
                    model.attachFile = model.uploadFile.FileName;
                }

                Contract contract = new()
                {
                    ContractId = model.ContractId,
                    CustomerId = model.CustomerId,
                    ProjectId = model.ProjectId,
                    IsActive = model.IsActive,
                    Description = model.Description,
                    CreatedOn = model.CreatedOn,
                    CreatedBy = model.CreatedBy,
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = model.ModifiedBy,
                    SetupDate = model.SetupDate,
                    ExpireDate = model.ExpireDate,
                    Margin = model.Margin,
                    Country = model.Country,
                    State = model.State,
                    attachFile = model.attachFile
                };
                context.Contract.Update(contract);
                context.SaveChanges();
                TempData["Success"] = "Contract Successfully Modified.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public JsonResult GetProjectsByOrgId(int orgId)
        {
            try
            {
                List<Projects> projectList = new List<Projects>();
                if (orgId != null)
                    projectList = context.Project.Where(p => p.CustomerID == orgId).ToList();

                return Json(projectList);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<IActionResult> DownloadFile(string fileName)
        {
            try
            {
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/contractFile", fileName);
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(uploadPath, out var contenttype))
                {
                    contenttype = "application/octet-stream";
                }
                var bytes = System.IO.File.ReadAllBytes(uploadPath);
                return File(bytes, contenttype, Path.GetFileName(uploadPath));
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<JsonResult> CheckUploadDocumentFile()
        {
            try
            {
                Dictionary<string, string> dt1 = new Dictionary<string, string>();
                string directory = string.Empty, filePathAndName = string.Empty;
                List<string> uploadedFiles = new List<string>();

                if (Request.Form?.Files != null && Request.Form?.Files.Count != 0)
                {
                    var files = Request.Form?.Files;
                    IFormFile file = files[0];
                    if (!(file.FileName.EndsWith(".pdf")))
                    {
                        dt1.Add(UploadConstants.MessageKey, "InvalidFileExtension");
                        return Json(dt1);
                    }
                    else if (file.Length > 20971520)
                    {
                        dt1.Add(UploadConstants.MessageKey, "FileSize");
                        return Json(dt1);
                    }
                    else
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        dt1.Add(UploadConstants.MessageKey, "Success");
                        return Json(dt1);
                    }
                }
                else
                {
                    dt1.Add(UploadConstants.MessageKey, "EmptyFile");
                    return Json(dt1);
                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> dt = new Dictionary<string, string>();
                dt.Add(UploadConstants.MessageKey, "Error");
                dt.Add(UploadConstants.ErrorMessage, ex.Message);
                return Json(dt);
            }
        }
    }
}
