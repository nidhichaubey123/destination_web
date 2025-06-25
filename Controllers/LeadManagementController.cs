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
        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            try
            {
                int pageSize = 10;
                var apiUrl = $"{_apiSettings.BaseUrl}/api/leads?search={search}&page={page}&pageSize={pageSize}";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                    var result = JsonSerializer.Deserialize<LeadApiResponse>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    var allLeads = result.Leads.Select(q => new Lead
                    {
                        Id = q.Id,
                        Name = q.Name,
                        Email = q.Email,
                        Phone = q.Phone,
                        QueryDate = q.QueryDate,
                        Zone = q.Zone,
                        Destination = q.Destination,
                        PaxCount = q.PaxCount,
                        Budget = q.Budget.ToString(),
                        GitFit = q.GitFit,
                        Status = q.Status,
                        Source = q.Source,
                        OriginatedBy = q.OriginatedBy,
                        AgentID = q.AgentID,
                        ConversionProbability = q.ConversionProbability?.ToString(),
                        TravelPlan = q.TravelPlans,
                        StartDate = q.StartDate,
                        EndDate = q.EndDate,
                        WhyLost = q.WhyLost,
                        Notes = q.Notes,
                        QuoteUrl = q.QuoteUrl,
                        LastReplied = q.LastReplied,
                        ReminderDate = q.ReminderDate,
                        ConfirmationCode = q.ConfirmationCode,
                        FinalPax = q.FinalPax,
                        CostSheetLink = q.CostSheetLink,
                        EndClient = q.EndClient,
                        ReservationLead = q.ReservationLead,
                        ReminderOverdue = q.ReminderOverdue,
                        QueryCode = q.QueryCode,
                        HandledBy = q.HandledBy
                    }).ToList();

                    int totalPages = (int)Math.Ceiling(result.TotalRecords / (double)pageSize);

                    var viewModel = new PaginatedLeadViewModel
                    {
                        Leads = allLeads,
                        CurrentPage = page,
                        TotalPages = totalPages,
                        SearchTerm = search
                    };

                    return View(viewModel);
                }
                else
                {
                    TempData["Error"] = "Unable to load leads.";
                    return View(new PaginatedLeadViewModel());
                }
            }
            catch
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
                    GitFit = model.GitFit,
                    Destination = model.Destination,
                    ConversionProbability = model.ConversionProbability,
                    PaxCount = model.PaxCount,
                    TravelPlan = model.TravelPlan,
                    StartDate = model.StartDate?.ToString("yyyy-MM-ddTHH:mm:ss"),
                    EndDate = model.EndDate?.ToString("yyyy-MM-ddTHH:mm:ss"),
                    Budget = model.Budget,
                    Status = model.Status,
                    Source = model.Source,
                    QueryCode = model.QueryCode,
                    Notes = model.Notes
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


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var apiUrl = $"{_apiSettings.BaseUrl}/api/leads/{id}";

                var response = await _httpClient.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(500, "Deletion failed. API responded with error.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Lead model)
        {
            if (model == null || model.Id <= 0)
                return BadRequest("Invalid data");

            try
            {
                var apiPayload = new
                {
                    Id = model.Id,
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    Zone = model.Zone,
                    Destination = model.Destination,
                    PaxCount = model.PaxCount,
                    Budget = string.IsNullOrEmpty(model.Budget) ? (decimal?)null : decimal.Parse(model.Budget),
                    Status = model.Status,
                    Source = model.Source,
                    QueryCode = model.QueryCode,
                    Notes = model.Notes,

                    GitFit = model.GitFit
                };

                var json = JsonSerializer.Serialize(apiPayload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var apiUrl = $"{_apiSettings.BaseUrl}/api/leads/update";
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                    return Ok();

                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode(500, "API update failed: " + errorContent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server error: " + ex.Message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetLeadById(int id)
        {
            var apiUrl = $"{_apiSettings.BaseUrl}/api/leads/{id}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                var apiLead = JsonSerializer.Deserialize<TruvaiQuery>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiLead == null)
                    return StatusCode(500, "Lead not found");

                // Map to lowercase properties
                return Json(new
                {
                    id = apiLead.Id,
                    name = apiLead.Name,
                    email = apiLead.Email,
                    phone = apiLead.Phone,
                    queryDate = apiLead.QueryDate.ToString("yyyy-MM-dd"),
                    zone = apiLead.Zone,
                    gitFit = apiLead.GitFit,
                    destination = apiLead.Destination,
                    conversionProbability = apiLead.ConversionProbability,
                    paxCount = apiLead.PaxCount,
                    travelPlan = apiLead.TravelPlans,
                    startDate = apiLead.StartDate?.ToString("yyyy-MM-dd"),
                    endDate = apiLead.EndDate?.ToString("yyyy-MM-dd"),
                    budget = apiLead.Budget,
                    status = apiLead.Status,
                    source = apiLead.Source,
                    queryCode = apiLead.QueryCode,
                    notes = apiLead.Notes
                });
            }

            return StatusCode(500, "Failed to load lead details");
        }



    }
}