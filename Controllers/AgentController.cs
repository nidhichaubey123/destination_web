using DMCPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            var agents = new List<Agent>();
            var zones = new List<Zone>(); // Your Zone model

            var agentResponse = await _httpClient.GetAsync("api/Agent");
            var zoneResponse = await _httpClient.GetAsync("api/Zone"); // Adjust endpoint if needed

            if (agentResponse.IsSuccessStatusCode)
            {
                var json = await agentResponse.Content.ReadAsStringAsync();
                agents = JsonSerializer.Deserialize<List<Agent>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            if (zoneResponse.IsSuccessStatusCode)
            {
                var json = await zoneResponse.Content.ReadAsStringAsync();
                zones = JsonSerializer.Deserialize<List<Zone>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                ViewBag.ZoneList = zones
                    .Select(z => new SelectListItem { Text = z.ZoneName, Value = z.ZoneName }) // or use ZoneId if preferred
                    .ToList();
            }

            return View(agents);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Agent agent)
        {
            try
            {
                // Step 1: Fetch existing agents
                var getResponse = await _httpClient.GetAsync("api/Agent");
                if (getResponse.IsSuccessStatusCode)
                {
                    var jsonData = await getResponse.Content.ReadAsStringAsync();
                    var existingAgents = JsonSerializer.Deserialize<List<Agent>>(jsonData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    // Step 2: Check for duplicates
                    bool duplicate = existingAgents.Any(a =>
                        a.AgentName.Equals(agent.AgentName, StringComparison.OrdinalIgnoreCase) ||
                        a.emailAddress.Equals(agent.emailAddress, StringComparison.OrdinalIgnoreCase));

                    if (duplicate)
                    {
                        TempData["ErrorMessage"] = "Duplicate Agent Name or Email Address is not allowed.";
                        return RedirectToAction("Index");
                    }
                }

                // Step 3: Proceed to save
                var json = JsonSerializer.Serialize(agent);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Agent", content);

                if (response.IsSuccessStatusCode)
                {
                    await SendAgentToAppSheet(agent);
                    TempData["SuccessMessage"] = "Agent added successfully.";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to add agent. Status: {response.StatusCode}. Error: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Exception occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Agent agent)
        {
            try
            {
                // Step 1: Fetch all agents
                var getResponse = await _httpClient.GetAsync("api/Agent");
                if (getResponse.IsSuccessStatusCode)
                {
                    var jsonData = await getResponse.Content.ReadAsStringAsync();
                    var existingAgents = JsonSerializer.Deserialize<List<Agent>>(jsonData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    // Step 2: Check for duplicates excluding current agent
                    bool duplicate = existingAgents.Any(a =>
                        a.AgentId != agent.AgentId && (
                            a.AgentName.Equals(agent.AgentName, StringComparison.OrdinalIgnoreCase) ||
                            a.emailAddress.Equals(agent.emailAddress, StringComparison.OrdinalIgnoreCase)
                        )
                    );

                    if (duplicate)
                    {
                        TempData["ErrorMessage"] = "Duplicate Agent Name or Email Address is not allowed.";
                        return RedirectToAction("Index");
                    }
                }

                // Step 3: Proceed to update
                var json = JsonSerializer.Serialize(agent);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/Agent/{agent.AgentId}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Agent updated successfully.";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to update agent. Status: {response.StatusCode}. Error: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Exception occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }



        private async Task SendAgentToAppSheet(Agent agent)
        {
            var appId = "af9afc6d-2f67-4d93-a122-06b5eb261d22";
            var apiKey = "V2-7PseE-MsP0e-11oP9-JPY4t-53I87-0gtaN-hliEm-xiAEN";
            var tableName = "row1";
            var url = $"https://api.appsheet.com/api/v2/apps/{appId}/tables/{tableName}/Action";

            var payload = new
            {
                Action = "Add",
                Properties = new { Locale = "en-US" },
                Rows = new[]
                {
            new
            {
              AppSheetId = Guid.NewGuid().ToString(),
                AgentName = agent.AgentName,
                AgentPoc1 = agent.AgentPoc1,
                Agency_Company = agent.Agency_Company,
                phoneno = agent.phoneno,
                emailAddress = agent.emailAddress,
                Zone = agent.Zone,
                AgentAddress = agent.AgentAddress
            }
        }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            content.Headers.Add("ApplicationAccessKey", apiKey);

            using var client = new HttpClient();
            var response = await client.PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine("📢 AppSheet Response:");
            Console.WriteLine(result);  // 🔍 This will show success or error message

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"AppSheet insert failed: {response.StatusCode} - {result}");
            }
        }

    }
}