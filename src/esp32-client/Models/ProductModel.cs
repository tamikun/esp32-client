using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace esp32_client.Models;

public class ProductCreateModel
{
#nullable disable
    public string ProductName { get; set; }
}

public class ProductUpdateModel
{
#nullable disable
    public int Id { get; set; }
    public string ProductName { get; set; }
}
