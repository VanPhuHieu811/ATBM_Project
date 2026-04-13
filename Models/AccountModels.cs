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

    public class PrivilegeInfo
    {
        public string PrivilegeName { get; set; }
        public string Type { get; set; }
        public string TableName { get; set; }
    }
}