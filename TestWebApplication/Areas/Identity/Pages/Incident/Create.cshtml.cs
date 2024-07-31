using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Win32;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;

namespace TestWebApplication.Areas.Identity.Pages.Incident
{
    public class CreateModel : PageModel
    {
        public CreateIncident objIncident = new CreateIncident();
        public List<CreateIncident> userList = new List<CreateIncident>();
        public void OnGet()
        {
        }
        public void OnPost()
        {
            objIncident.IssueTitle = Request.Form["IssueTitle"];
            objIncident.Category = Request.Form["Category"];
            objIncident.Project = Request.Form["Project"];
            objIncident.AreaPath = Request.Form["AreaPath"];

            try
            {
                String connectionString = "Data Source =DESKTOP-RRLNR6U\\CUSCEN; Initial Catalog = TestWebAppDb; User ID = sa; Password = @dmin123";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string sql = "INSERT INTO UserRegistration(Username,Email,Password,ConfirmPsw) VALUES(@name,@email,@psw,@conPsw)";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@IssueTitle", objIncident.IssueTitle);
                        cmd.Parameters.AddWithValue("@Category", objIncident.Category);
                        cmd.Parameters.AddWithValue("@Project", objIncident.Project);
                        cmd.Parameters.AddWithValue("@AreaPath", objIncident.AreaPath);
                        cmd.ExecuteNonQuery();
                    }
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }
            //Response.Redirect("/Incident/Create");
            ViewData["Message"] = "Save Successfully!";

        }
    }

    public class CreateIncident
    {
        public string? IssueTitle;
        public string? Category;
        public string? Project;
        public string? AreaPath;
    }
}


