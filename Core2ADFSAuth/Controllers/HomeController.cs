using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Core2ADFSAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Core2ADFSAuth.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private static readonly HttpClient Client = new HttpClient();
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _config;

        public HomeController(IDistributedCache cache, IConfiguration config)
        {
            _cache = cache;
            _config = config;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UserClaims() => View();
       
    }
}
