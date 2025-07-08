using DMCPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DMCPortal.Web.Controllers
{
    public class AgentController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public AgentController(IHttpClientFactory factory, IConfiguration config)
        {
            _httpClient = factory.CreateClient();
            _apiBaseUrl = config["ApiSettings:BaseUrl"];
            _httpClient.BaseAddress = new Uri(_apiBaseUrl);
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Agent");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var agents = JsonSerializer.Deserialize<List<Agent>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(agents);
            }

            return View(new List<Agent>());
        }

        [HttpGet]
        public async Task<IActionResult> GetAgents()
        {
            var response = await _httpClient.GetAsync("api/Agent");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var agents = JsonSerializer.Deserialize<List<Agent>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return Json(agents);
            }
            return Json(new List<Agent>());
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _httpClient.GetAsync($"api/Agent/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var agent = JsonSerializer.Deserialize<Agent>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return Json(agent);
            }
            return NotFound();
        }

        // ✅ FIXED: Added [ValidateAntiForgeryToken] and proper error handling
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Agent agent)
        {
            try
            {
                var json = JsonSerializer.Serialize(agent);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Agent", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Agent added successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    // ✅ FIXED: Log the error response for debugging
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to add agent. Status: {response.StatusCode}. Error: {errorContent}";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Exception occurred: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Agent/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Agent deleted successfully.";
                }
                else
                {
                    // ✅ FIXED: Better error handling
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to delete agent. Status: {response.StatusCode}. Error: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Exception occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // ✅ FIXED: This method was missing the route parameter handling
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Agent agent)
        {
            try
            {
                var json = JsonSerializer.Serialize(agent);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // ✅ FIXED: Make sure AgentId is being passed correctly
                var response = await _httpClient.PutAsync($"api/Agent/{agent.AgentId}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Agent updated successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    // ✅ FIXED: Better error handling
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to update agent. Status: {response.StatusCode}. Error: {errorContent}";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Exception occurred: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}