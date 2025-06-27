using DMCPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;

namespace DMCPortal.Web.Controllers
{
    public class MeetingTypeController : Controller
    {
        private readonly HttpClient _http;
        private const string ApiBase = "https://localhost:7228/api/MeetingType";

        public MeetingTypeController(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _http.GetAsync(ApiBase);
            if (response.IsSuccessStatusCode)
            {
                var list = await response.Content.ReadFromJsonAsync<List<MeetingType>>();
                return Json(list ?? new List<MeetingType>());
            }
            return Json(new { error = "Failed to load", statusCode = response.StatusCode });
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest(new { error = "Invalid ID" });

            var response = await _http.GetAsync($"{ApiBase}/{id}");
            if (response.IsSuccessStatusCode)
            {
                var item = await response.Content.ReadFromJsonAsync<MeetingType>();
                return Json(item);
            }
            return Json(new { error = "Failed to fetch", statusCode = response.StatusCode });
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdate([FromBody] MeetingType model)
        {
            if (string.IsNullOrWhiteSpace(model.MeetingTypeName))
                return Json(new { success = false, error = "Name is required" });

            HttpResponseMessage response = model.MeetingTypeId == 0
                ? await _http.PostAsJsonAsync(ApiBase, model)
                : await _http.PutAsJsonAsync($"{ApiBase}/{model.MeetingTypeId}", model);

            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Saved successfully" });

            var errorContent = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, error = errorContent });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return Json(new { success = false, error = "Invalid ID" });

            var response = await _http.DeleteAsync($"{ApiBase}/{id}");
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Deleted successfully" });

            var errorContent = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, error = errorContent });
        }
    }
}
