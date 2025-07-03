using DMCPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace DMCPortal.Web.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly ApiSettings _apiSettings;

        public LoginController(IOptions<ApiSettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginRequest());
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            using var client = new HttpClient { BaseAddress = new Uri(_apiSettings.BaseUrl) };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.PostAsJsonAsync("api/User/login", request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<LoginResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result == null || string.IsNullOrEmpty(result.SessionId.ToString()))
                {
                    ModelState.AddModelError(string.Empty, "Login failed: Invalid server response.");
                    return View(request);
                }
                var fullName = $"{result.FirstName} {result.LastName}".Trim();
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, request.EmailAddress),
                    new Claim("SessionId", result.SessionId.ToString()),
                      new Claim("FirstName", result.FirstName ?? ""),

    new Claim("LastName", result.LastName ?? ""),
        new Claim("UserId", result.UserId.ToString()),
            new Claim("FullName", fullName)
                };

                // ✅ Add each operation name as a claim
                if (result.Operations != null)
                {
                    foreach (var operation in result.Operations)
                    {
                        claims.Add(new Claim("Operation", operation));
                    }
                }

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal);

                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                var apiError = JsonSerializer.Deserialize<ApiError>(errorJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                ModelState.AddModelError(string.Empty, apiError?.Message ?? "Login failed");
                return View(request);
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Optional: Delete from DB if you store session there
            var sessionId = User.Claims.FirstOrDefault(c => c.Type == "SessionId")?.Value;

            if (!string.IsNullOrEmpty(sessionId))
            {
                using var client = new HttpClient { BaseAddress = new Uri(_apiSettings.BaseUrl) };
                await client.DeleteAsync($"api/User/DeleteSession/{sessionId}");
            }

            // Clear Auth cookie
            await HttpContext.SignOutAsync("MyCookieAuth");

            return RedirectToAction("Index", "Login");
        }

    }
}
