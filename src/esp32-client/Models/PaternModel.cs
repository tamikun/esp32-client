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

    public class PaternIndexPageModel
    {
        public PaternIndexPageModel()
        {
            ListDeletePaternById = new List<DeletePaternModel>();
        }

        public PaternCreateModel PaternCreateModel { get; set; }
        public List<DeletePaternModel> ListDeletePaternById { get; set; }
    }

#nullable disable
    public class DeletePaternModel
    {
        public int Id { get; set; }
        public bool IsSelected { get; set; } = false;
    }


    public class PaternResponseModel
    {
        public int Id { get; set; }
        public string PaternNumber { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
    }
}