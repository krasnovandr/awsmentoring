using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Module6.Models
{
    public class FileDto
    {
        public string Key{ get; set; }
        public IFormFile File { get; set; }
    }
}
