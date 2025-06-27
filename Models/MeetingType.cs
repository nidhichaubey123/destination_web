namespace DMCPortal.Web.Models
{
    public class MeetingType
    {
        public int MeetingTypeId { get; set; }
        public string MeetingTypeName { get; set; } = string.Empty;
        public string MeetingTypeDesc { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
