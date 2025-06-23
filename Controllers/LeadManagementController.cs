using DMCPortal.Web.Models;
using DMCPortal.Web.Models.DMCPortal.Web.Models;
using DMCPortal.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace DMCPortal.Web.Controllers
{
    public class LeadManagementController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;

        public LeadManagementController(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                var apiUrl = $"{_apiSettings.BaseUrl}/api/leads";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var allLeads = JsonSerializer.Deserialize<List<Lead>>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    int pageSize = 10;
                    int totalLeads = allLeads?.Count ?? 0;
                    int totalPages = (int)Math.Ceiling(totalLeads / (double)pageSize);

                    var paginatedLeads = allLeads
                        .OrderByDescending(l => l.QueryDate)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    var viewModel = new PaginatedLeadViewModel
                    {
                        Leads = paginatedLeads,
                        CurrentPage = page,
                        TotalPages = totalPages
                    };

                    return View(viewModel);
                }
                else
                {
                    TempData["Error"] = "Unable to load leads.";
                    return View(new PaginatedLeadViewModel());
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Something went wrong!";
                return View(new PaginatedLeadViewModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Lead model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" | ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction("Index");
            }

            try
            {
                var leadData = new
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    QueryDate = model.QueryDate?.ToString("yyyy-MM-ddTHH:mm:ss"),
                    Zone = model.Zone,
                    Destination = model.Destination,
                    PaxCount = model.PaxCount,
                    Budget = model.Budget?.ToString(),

                    GitFit = model.GitFit,
                    Status = model.Status,
                    Source = model.Source,
                    OriginatedBy = model.OriginatedBy,
                    AgentID = model.AgentID,
                    ConversionProbability = model.ConversionProbability?.ToString(),
                    TravelPlans = model.TravelPlan,
                    StartDate = model.StartDate?.ToString("yyyy-MM-ddTHH:mm:ss"),
                    EndDate = model.EndDate?.ToString("yyyy-MM-ddTHH:mm:ss"),
                    WhyLost = model.WhyLost,
                    Notes = model.Notes,
                    QuoteUrl = model.QuoteUrl,
                    LastReplied = model.LastReplied?.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ReminderDate = model.ReminderDate?.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ConfirmationCode = model.ConfirmationCode,
                    FinalPax = model.FinalPax,
                    CostSheetLink = model.CostSheetLink,
                    EndClient = model.EndClient,
                    ReservationLead = model.ReservationLead,
                    ReminderOverdue = model.ReminderOverdue,
                    QueryCode = model.QueryCode
                };

                var json = JsonSerializer.Serialize(leadData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var apiUrl = $"{_apiSettings.BaseUrl}/api/leads";
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Lead created successfully!";
                }
                else
                {
                    var err = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"API Error: {response.StatusCode} - {err}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
