using Microsoft.AspNetCore.Http;

namespace Module6.ApiModels
{
    public class CreateFileDto
    {
        public IFormFile File { get; set; }
    }
}
