using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace esp32_client.Models
{
    public class PaternCreateModel
    {
#nullable disable
        public IFormFile File { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
    }


    public class PaternResponseModel
    {
        public string PaternNumber { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
    }
}