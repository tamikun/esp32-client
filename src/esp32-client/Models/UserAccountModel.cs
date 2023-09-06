namespace esp32_client.Models
{
    public class UserAccountCreateModel
    {

        public string LoginName { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public int RoleId { get; set; }
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
        public string ConfirmedPassword { get; set; }
    }
}