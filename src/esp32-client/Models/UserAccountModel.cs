using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace esp32_client.Models
{
    public class UserAccountCreateModel
    {
#nullable disable
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
    }

    public class UserAccountUpdateModel
    {
        public int Id { get; set; }
        public string LoginName { get; set; }
        public string UserName { get; set; }
    }

    public class UserAccountChangePasswordModel
    {
        public int Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}