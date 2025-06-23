using DMCPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace DMCPortal.Web.Controllers
{
    [Authorize]
    public class RegisterController : Controller
    {
        private readonly ApiSettings _apiSettings;

        public RegisterController(IOptions<ApiSettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new RegisterRequest());
        }

        [HttpPost]
        public async Task<IActionResult> Index(RegisterRequest req)
        {
            if (!ModelState.IsValid)
            {
                return View(req);
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiSettings.BaseUrl);

                // Get SessionId from logged-in user's claims
                var sessionId = User.Claims.FirstOrDefault(c => c.Type == "SessionId")?.Value;
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    ModelState.AddModelError(string.Empty, "Session expired. Please login again.");
                    return View(req);
                }

                // Add SessionId to request header
                client.DefaultRequestHeaders.Add("SessionId", sessionId);

                try
                {
                    var response = await client.PostAsJsonAsync("api/User/register", req);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<UserRegisterResponse>();

                        if (Request.Query["modal"] == "true")
                        {
                            return Json(new { success = true, message = "User registered successfully!" });
                        }

                        ViewBag.SuccessUserId = result?.UserId;
                        TempData["SuccessMessage"] = "User registered successfully!";
                        return View();
                    }

                    var apiResponse = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var errorObject = JObject.Parse(apiResponse);
                        if (errorObject["errors"] != null)
                        {
                            foreach (var prop in errorObject["errors"])
                            {
                                string field = ((JProperty)prop).Name;
                                string[] messages = prop.First().ToObject<string[]>();
                                foreach (var msg in messages)
                                {
                                    ModelState.AddModelError(field, msg);
                                }
                            }
                        }
                        else if (errorObject["message"] != null)
                        {
                            ModelState.AddModelError(string.Empty, errorObject["message"].ToString());
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, apiResponse);
                        }
                    }
                    catch
                    {
                        ModelState.AddModelError(string.Empty, apiResponse);
                    }
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError(string.Empty, $"Connection error: {ex.Message}");
                }
                catch (TaskCanceledException ex)
                {
                    ModelState.AddModelError(string.Empty, "Request timeout. Please try again.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                }

                return View(req);
            }
        }
    }

    // Response class to match API response
    public class UserRegisterResponse
    {
        public int UserId { get; set; }
    }
}