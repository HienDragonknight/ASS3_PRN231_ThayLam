using IdentityAjaxClient.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace IdentityAjaxClient.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public LoginController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5232/api";
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var content = new StringContent(
                    JsonSerializer.Serialize(new { email = model.Email, password = model.Password }),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync("https://localhost:7014/api/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();


                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseContent);
                    var root = doc.RootElement;
                    var token = root.GetProperty("token").GetString();
                    var user = root.GetProperty("user");
                    var email = user.GetProperty("email").GetString();
                    var role = user.GetProperty("role").GetString();
                    if (role == "Customer")
                    {
                        ModelState.AddModelError(string.Empty, "Bạn không có quyền truy cập vào hệ thống");
                        return View(model);
                    }
                    HttpContext.Session.SetString("token", token);
                    HttpContext.Session.SetString("email", email);
                    HttpContext.Session.SetString("role", role);

                    //  Chuyển sang ProductController sau khi login thành công
                    return RedirectToAction("Index", "Product");
                }

                else
                {
                    // Try to extract error message from response
                    string errorMsg = "Login failed";
                    try
                    {
                        using var doc = JsonDocument.Parse(responseContent);
                        if (doc.RootElement.TryGetProperty("message", out var msgProp))
                        {
                            errorMsg = msgProp.GetString();
                        }
                    }
                    catch { }
                    ModelState.AddModelError(string.Empty, errorMsg);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }
    }
}

