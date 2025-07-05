namespace DMCPortal.Web.Models
{
    public class Zone
    {
        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
        public string ZoneCreatedBy { get; set; }
        public DateTime ZoneCreatedOn { get; set; }
        public string ZoneUpdatedBy { get; set; }
        public DateTime? ZoneUpdatedOn { get; set; }
    }
}
