using System.Collections.Generic;
using DMCPortal.Web.Models;


namespace DMCPortal.Web.Models.ViewModels
{
    public class PaginatedLeadViewModel
    {
        public List<Lead> Leads { get; set; } = new List<Lead>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; } = "";
    }
}
