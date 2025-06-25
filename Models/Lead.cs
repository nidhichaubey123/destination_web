using System.ComponentModel.DataAnnotations;

namespace DMCPortal.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    using System.ComponentModel.DataAnnotations;

    namespace DMCPortal.Web.Models
    {
        public class Lead
        {
            public int Id { get; set; }
            [Required]
            public string Name { get; set; }

            [Required, EmailAddress]
            public string Email { get; set; }

            [Required, RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits")]
            public string Phone { get; set; }

            [Required]
            public DateTime? QueryDate { get; set; }

            [Required]
            public string Zone { get; set; }

            [Required]
            public string Destination { get; set; }

            [Required]
            public int PaxCount { get; set; }

            [Required]
            public string Budget { get; set; }

            // Optional properties
            public string? GitFit { get; set; }
            public string? Status { get; set; }
            public string? Source { get; set; }
            public string? OriginatedBy { get; set; }
            public string? AgentID { get; set; }
            public string? ConversionProbability { get; set; }
            public string? TravelPlan { get; set; }
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

}
