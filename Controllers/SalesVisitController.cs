using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using DMCPortal.Web.Models;
using DMCPortal.Web.Models.ViewModels;

namespace DMCPortal.Web.Controllers
{
    public class SalesVisitController : Controller
    {
        private readonly HttpClient _httpClient;

        public SalesVisitController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7228/api/");
        }

        public async Task<IActionResult> Index(string search = "", int page = 1, int pageSize = 10)
        {
            var userId = User?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Index", "Login");

            var url = $"SalesVisit/Paged?page={page}&pageSize={pageSize}&search={search}&userId={userId}";
            var model = new PaginatedSalesVisitViewModel
            {
                SearchTerm = search,
                CurrentPage = page,
                SalesVisits = new List<SalesVisitViewModel>(),
                TotalPages = 0
            };

            var res = await _httpClient.GetAsync(url);

            if (res.IsSuccessStatusCode)
            {
                var data = await res.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PagedResult<SalesVisitViewModel>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result != null && result.Items != null)
                {
                    model.SalesVisits = result.Items;
                    model.TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesVisit(int id)
        {
            var res = await _httpClient.GetAsync($"SalesVisit/{id}");
            if (!res.IsSuccessStatusCode)
                return NotFound();

            var data = await res.Content.ReadAsStringAsync();
            var visit = JsonSerializer.Deserialize<SalesVisit>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Json(visit);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SalesVisit visit)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Index", "Login");

            visit.UserId = int.Parse(userId);

            var json = JsonSerializer.Serialize(visit);
            var res = await _httpClient.PostAsync("SalesVisit", new StringContent(json, Encoding.UTF8, "application/json"));

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SalesVisit visit)
        {
            var json = JsonSerializer.Serialize(visit);
            var res = await _httpClient.PutAsync($"SalesVisit/{visit.SalesVisitId}", new StringContent(json, Encoding.UTF8, "application/json"));
            return res.IsSuccessStatusCode ? Json(new { success = true }) : Json(new { success = false });
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _httpClient.DeleteAsync($"SalesVisit/{id}");
            return RedirectToAction("Index");
        }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }

  
}
