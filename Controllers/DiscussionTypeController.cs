using DMCPortal.API.Entities;
using DMCPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;

namespace DMCPortal.Web.Controllers
{
    public class DiscussionTypeController : Controller
    {
        private readonly HttpClient _http;
        private const string ApiBase = "https://localhost:7228/api/DiscussionType";

        public DiscussionTypeController(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
        }

        // GET /DiscussionType or /DiscussionType/Index
        public IActionResult Index() => View();

        // GET /DiscussionType/GetAll
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await _http.GetAsync(ApiBase);
                if (response.IsSuccessStatusCode)
                {
                    var list = await response.Content.ReadFromJsonAsync<List<DiscussionType>>();
                    return Json(list ?? new List<DiscussionType>());
                }
                return Json(new { error = "Failed to retrieve data", statusCode = response.StatusCode });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // GET /DiscussionType/GetById
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { error = "Invalid ID" });

                var response = await _http.GetAsync($"{ApiBase}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var item = await response.Content.ReadFromJsonAsync<DiscussionType>();
                    return Json(item);
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound(new { error = "Discussion type not found" });

                return Json(new { error = "Failed to retrieve item", statusCode = response.StatusCode });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // POST /DiscussionType/AddOrUpdate
        [HttpPost]
        public async Task<IActionResult> AddOrUpdate([FromBody] DiscussionType model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Json(new { success = false, error = "Invalid model data" });

                if (string.IsNullOrWhiteSpace(model.DiscussionTypeName))
                    return Json(new { success = false, error = "Discussion type name is required" });

                HttpResponseMessage response;

                if (model.DiscussionTypeId == 0)
                {
                    // Create new
                    response = await _http.PostAsJsonAsync(ApiBase, model);
                }
                else
                {
                    // Update existing
                    response = await _http.PutAsJsonAsync($"{ApiBase}/{model.DiscussionTypeId}", model);
                }

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Discussion type saved successfully" });
                }

                // Handle different error responses
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorMessage = "Failed to save discussion type";

                try
                {
                    var errorObj = JsonSerializer.Deserialize<JsonElement>(errorContent);
                    if (errorObj.TryGetProperty("message", out var msgElement))
                    {
                        errorMessage = msgElement.GetString() ?? errorMessage;
                    }
                }
                catch
                {
                    // If JSON parsing fails, use default message
                }

                return Json(new { success = false, error = errorMessage });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // DELETE /DiscussionType/Delete
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                    return Json(new { success = false, error = "Invalid ID" });

                var response = await _http.DeleteAsync($"{ApiBase}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Discussion type deleted successfully" });
                }

                // Handle different error responses
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorMessage = "Failed to delete discussion type";

                try
                {
                    var errorObj = JsonSerializer.Deserialize<JsonElement>(errorContent);
                    if (errorObj.TryGetProperty("message", out var msgElement))
                    {
                        errorMessage = msgElement.GetString() ?? errorMessage;
                    }
                }
                catch
                {
                    // If JSON parsing fails, use default message
                }

                return Json(new { success = false, error = errorMessage });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}