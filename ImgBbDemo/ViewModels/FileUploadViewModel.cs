using Microsoft.AspNetCore.Http;

namespace ImgBbDemo.ViewModels
{
    public class FileUploadViewModel
    {
        public IFormFile File { get; set; }
    }
}