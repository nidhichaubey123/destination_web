using Microsoft.AspNetCore.Mvc;
using DMCPortal.Web.Models;
using System.Text.Json;
using System.Text;

namespace DMCPortal.Web.Controllers
{
    public class ZoneController : Controller
    {
        private readonly HttpClient _httpClient;

        public ZoneController(IHttpClientFactory factory, IConfiguration config)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri(config["ApiSettings:BaseUrl"]);
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Zone");
            var zones = response.IsSuccessStatusCode
                ? JsonSerializer.Deserialize<List<Zone>>(await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                : new List<Zone>();

            return View(zones);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string ZoneName)
        {
            var payload = JsonSerializer.Serialize(new { ZoneName = ZoneName.Trim(), ZoneCreatedBy = User.Identity?.Name ?? "Admin" });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Zone", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Zone created successfully!";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                TempData["ErrorMessage"] = "This Zone already exists.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error creating Zone.";
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ZoneEditModel model)
        {
            try
            {
                var apiPayload = new
                {
                    ZoneName = model.ZoneName.Trim(),
                    ZoneUpdatedBy = model.UpdatedBy
                };

                var json = JsonSerializer.Serialize(apiPayload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/Zone/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Zone updated successfully!";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    TempData["ErrorMessage"] = "This Zone name already exists.";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to update zone: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating zone: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Zone/{id}");
            TempData[response.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
                response.IsSuccessStatusCode ? "Zone deleted successfully!" : "Error deleting zone";

            return RedirectToAction(nameof(Index));
        }


    }
    public class ZoneEditModel
    {
        public int ZoneId { get; set; }

        public string ZoneName { get; set; } = string.Empty;

        public string UpdatedBy { get; set; } = string.Empty;
    }

}
