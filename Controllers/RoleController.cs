using DMCPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace DMCPortal.Web.Controllers
{
    public class RoleController : Controller
    {
        private readonly HttpClient _client;

        public RoleController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7228/");  // Your API URL
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var response = await _client.GetAsync("api/Role");
            var data = await response.Content.ReadAsStringAsync();
            var roles = JsonConvert.DeserializeObject<List<Role>>(data);
            return Json(roles);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdate([FromBody] Role role)
        {
            role.RoleCreatedOn = DateTime.Now;

            var json = JsonConvert.SerializeObject(role);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            if (role.RoleId == 0)
            {
                response = await _client.PostAsync("api/Role", content);
            }
            else
            {
                response = await _client.PutAsync($"api/Role/{role.RoleId}", content);
            }

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true });
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = error });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _client.GetAsync($"api/Role/{id}");
            var data = await response.Content.ReadAsStringAsync();
            var role = JsonConvert.DeserializeObject<Role>(data);
            return Json(role);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _client.DeleteAsync($"api/Role/{id}");
            return Ok();
        }

        [HttpGet("GetOperationsForRole/{roleId}")]
        public async Task<IActionResult> GetOperationsForRole(int roleId)
        {
            var response = await _client.GetAsync($"api/RoleOperation/Role/{roleId}");
            var assignedIdsJson = await response.Content.ReadAsStringAsync();
            var assignedIds = JsonConvert.DeserializeObject<List<int>>(assignedIdsJson);

            // Now get all operations
            var allOpsResponse = await _client.GetAsync("api/Operation"); // You must have this in your API
            var allOpsJson = await allOpsResponse.Content.ReadAsStringAsync();
            var allOps = JsonConvert.DeserializeObject<List<Operation>>(allOpsJson);

            var result = allOps.Select(op => new {
                operationId = op.OperationId,
                operationName = op.OperationName,
                assigned = assignedIds.Contains(op.OperationId)
            });

            return Json(result);
        }
        public class Operation
        {
            public int OperationId { get; set; }
            public string OperationName { get; set; }
        }

    }
}
