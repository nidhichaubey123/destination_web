namespace DMCPortal.Web.Models
{
    public class SalesVisit
    {
        public int SalesVisitId { get; set; }
        public int UserId { get; set; }
        public int UserName { get; set; }

        public DateTime VisitDate { get; set; }
        public TimeSpan VisitTime { get; set; }
        public int? AgentId { get; set; }
        public int? DiscussionTypeId { get; set; }
        public int? MeetingTypeId { get; set; }
        public string MeetingVenueName { get; set; }
        public decimal? MeetingLatitude { get; set; }
        public decimal? MeetingLongitude { get; set; }

        public decimal? EntryLatitude { get; set; }
        public decimal? EntryLongitude { get; set; }
        public string MeetingNotes { get; set; }
        public string SalesVisitCode { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public int? DeletedBy { get; set; }



    }
}
