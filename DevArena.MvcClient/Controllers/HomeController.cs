using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevArena.MvcClient.Models;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;

namespace DevArena.MvcClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult AccessAPI()
        {
            return View();
        }

        //[Authorize]
        public async Task<IActionResult> GetFullAccessResources()
        {
            return await GetResources(true);
        }

        //[Authorize]
        public async Task<IActionResult> GetLimitedAccessResources()
        {
            return await GetResources(false);
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
        }

        private async Task<IActionResult> GetResources(bool isFull)
        {
            TokenClient tokenCLient = new TokenClient("http://localhost:5001/connect/token", "mvc.clientid", "mvc.secret");
            var dClient = DiscoveryClient.GetAsync("http://localhost:5001");

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.SetBearerToken(accessToken);
            var content = await client.GetStringAsync("http://localhost:53377/" + (isFull ? "admin":"guest"));

            ViewBag.Json = JArray.Parse(content).ToString();
            return View("json");
        }

        //public IActionResult About()
        //{
        //    ViewData["Message"] = "Your application description page.";

        //    return View();
        //}

        //public IActionResult Contact()
        //{
        //    ViewData["Message"] = "Your contact page.";

        //    return View();
        //}

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
