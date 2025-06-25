namespace DMCPortal.Web.Models
{
    public class LeadApiResponse
    {
        public int TotalRecords { get; set; }
        public List<TruvaiQuery> Leads { get; set; }
    }
}
