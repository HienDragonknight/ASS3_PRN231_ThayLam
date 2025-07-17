using IdentityAjaxClient.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace IdentityAjaxClient.Controllers
{
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public ProductController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7014/api";
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("api/product/GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{_apiBaseUrl}/orchid");

                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    var products = await JsonSerializer.DeserializeAsync<List<ProductViewModel>>(stream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return Ok(products);
                }

                return StatusCode((int)response.StatusCode, new { error = "Failed to retrieve products" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("api/product/GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{_apiBaseUrl}/orchid/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<ProductViewModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return Ok(data);
                }

                return response.StatusCode == System.Net.HttpStatusCode.NotFound
                    ? NotFound(new { error = "Product not found" })
                    : StatusCode((int)response.StatusCode, new { error = "Failed to retrieve product" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("api/product/Create")]
        public async Task<IActionResult> Create([FromBody] ProductViewModel model)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                var payload = new
                {
                    orchidName = model.OrchidName,
                    price = model.Price,
                    orchidUrl = model.OrchidUrl,
                    orchidDescription = model.OrchidDescription,
                    isNatural = true,
                    categoryId = model.CategoryId
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{_apiBaseUrl}/orchid", content);

                return await HandleResponse(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPut("api/product/Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductViewModel model)
        {
         

            try
            {
                var client = _httpClientFactory.CreateClient();

                var payload = new
                {
                    orchidName = model.OrchidName,
                    price = model.Price,
                    orchidUrl = model.OrchidUrl,
                    orchidDescription = model.OrchidDescription,
                    IsNatural = true,
                    categoryId = model.CategoryId
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"{_apiBaseUrl}/orchid/{id}", content);

                return await HandleResponse(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("api/product/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.DeleteAsync($"{_apiBaseUrl}/orchid/{id}");

                if (response.IsSuccessStatusCode)
                    return Ok(new { success = true });

                return response.StatusCode == System.Net.HttpStatusCode.NotFound
                    ? NotFound(new { error = "Product not found" })
                    : StatusCode((int)response.StatusCode, new { error = "Failed to delete product" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private async Task<IActionResult> HandleResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Ok(content);

            return StatusCode((int)response.StatusCode, new { error = content });
        }
    }
}
