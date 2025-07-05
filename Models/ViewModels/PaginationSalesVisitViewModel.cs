namespace DMCPortal.Web.Models.ViewModels
{
    public class PaginatedSalesVisitViewModel
    {
        public List<SalesVisitViewModel> SalesVisits { get; set; } = new();
        public string SearchTerm { get; set; } = "";
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
