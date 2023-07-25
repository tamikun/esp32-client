using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace esp32_client.Models
{
    public class PatternCreateModel
    {
#nullable disable
        public IFormFile File { get; set; }
        public string PatternNumber { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
    }
    
    public class PatternUpdateModel
    {
        public int Id { get; set; }
        public IFormFile File { get; set; }
        public string PatternNumber { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
    }

    public class PatternIndexPageModel
    {
        public PatternIndexPageModel()
        {
            ListDeletePatternById = new List<DeletePatternModel>();
        }

        public PatternCreateModel PatternCreateModel { get; set; }
        public List<DeletePatternModel> ListDeletePatternById { get; set; }
    }

#nullable disable
    public class DeletePatternModel
    {
        public int Id { get; set; }
        public bool IsSelected { get; set; } = false;
    }


    public class PatternResponseModel
    {
        public int Id { get; set; }
        public string PatternNumber { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
    }
}