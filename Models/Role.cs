namespace DMCPortal.Web.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string? RoleDescription { get; set; }
        public DateTime RoleCreatedOn { get; set; }

        public int? RoleCreatedBy { get; set; }

        public DateTime? RoleUpdatedOn { get; set; }
        public int? RoleUpdatedBy { get; set; }
    }
}
