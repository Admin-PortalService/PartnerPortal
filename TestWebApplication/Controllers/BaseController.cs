using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
//using TestWebApplication.Helper;
using System.Security.Claims;
namespace TestWebApplication.Controllers
{
    public class BaseController : Controller
    {
        protected const int BATCHSIZE = 100;
        //public AppUser CurrentUser => new(User);

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            string IDDecode = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            return IDDecode;
        }

    }
}

