using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using DMCPortal.Web.Models;

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

        public async Task<IActionResult> Index()
        {
            var res = await _httpClient.GetAsync("SalesVisit");
            var list = new List<SalesVisit>();
            if (res.IsSuccessStatusCode)
            {
                var data = await res.Content.ReadAsStringAsync();
                list = JsonSerializer.Deserialize<List<SalesVisit>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            return View(list);
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
            var json = JsonSerializer.Serialize(visit);
            var res = await _httpClient.PostAsync("SalesVisit", new StringContent(json, Encoding.UTF8, "application/json"));
            return res.IsSuccessStatusCode ? RedirectToAction("Index") : RedirectToAction("Index");
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
}
