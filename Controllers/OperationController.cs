using DMCPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace DMCPortal.Web.Controllers
{
    public class OperationController : Controller
    {
        private readonly HttpClient _client;

        public OperationController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7228/");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetOperations()
        {
            var response = await _client.GetAsync("api/Operation");
            var operations = new List<Operation>();

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                operations = JsonConvert.DeserializeObject<List<Operation>>(data);
            }

            return Json(operations);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdate([FromBody] Operation operation)
        {
            operation.OperationCreatedOn = DateTime.Now;
            var json = JsonConvert.SerializeObject(operation);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            if (operation.OperationId == 0)
                response = await _client.PostAsync("api/Operation", content);
            else
                response = await _client.PutAsync($"api/Operation/{operation.OperationId}", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _client.GetAsync($"api/Operation/{id}");
            var data = await response.Content.ReadAsStringAsync();
            var operation = JsonConvert.DeserializeObject<Operation>(data);
            return Json(operation);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _client.DeleteAsync($"api/Operation/{id}");
            return RedirectToAction("Index");
        }
    }
}
