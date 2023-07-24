using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace esp32_client.Models;

public class ProductCreateModel
{
#nullable disable
    public string ProductName { get; set; }
    public List<ProcessPattern> ListProcessPattern { get; set; } = new List<ProcessPattern>();
}

public class ProcessPattern
{
    public string ProcessName { get; set; }
    public string PatternNumber { get; set; }
}

