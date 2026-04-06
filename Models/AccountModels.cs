namespace ATBM_Project.Models
{
    public class UserAccount
    {
        public string Username { get; set; }
        public string CreatedDate { get; set; }
        public string Status { get; set; }
    }

    public class RoleAccount
    {
        public string RoleName { get; set; }
        public string RoleId { get; set; }
    }
}