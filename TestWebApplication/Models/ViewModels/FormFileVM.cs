namespace TestWebApplication.Models.ViewModels
{
    public interface IFormFile
    {
        public string FilePath { get; set; }

        public string ContentType { get; set; }

        public string FileName { get; set; }

      
    }
}
