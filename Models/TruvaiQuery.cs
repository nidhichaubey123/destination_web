namespace DMCPortal.Web.Models
{
    public class TruvaiQuery
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime QueryDate { get; set; }
        public string Zone { get; set; }
        public string Destination { get; set; }
        public int PaxCount { get; set; }
        public decimal Budget { get; set; }
        public string? GitFit { get; set; }
        public string? Status { get; set; }
        public string? Source { get; set; }
        public string? OriginatedBy { get; set; }
        public string? AgentID { get; set; }
        public int? ConversionProbability { get; set; }
        public string? TravelPlans { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? WhyLost { get; set; }
        public string? Notes { get; set; }
        public string? QuoteUrl { get; set; }
        public DateTime? LastReplied { get; set; }
        public DateTime? ReminderDate { get; set; }
        public string? ConfirmationCode { get; set; }
        public string? FinalPax { get; set; }
        public string? CostSheetLink { get; set; }
        public string? EndClient { get; set; }
        public string? ReservationLead { get; set; }
        public string? ReminderOverdue { get; set; }
        public string? QueryCode { get; set; }
        public string? HandledBy { get; set; }
    }

}
