using DMCPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

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
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.PostAsJsonAsync("api/User/login", request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<LoginResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, request.EmailAddress),
                    new Claim("SessionId", result.SessionId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);


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
    }
}
