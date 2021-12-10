using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var s = new KeyvaultSecret();
           var result =  s.GetSecret();
            Console.WriteLine("result: "+ result);
            ViewBag.secretvalue = result;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }

    public class KeyvaultSecret
    {
        public string GetSecret( )
        {
            try
            {
                Uri kvurl = new Uri("https://kvname100.vault.azure.net/secret1");
                //scheme = protocol
                string kvUri = kvurl.Scheme.ToString() + "://" + kvurl.Host.ToString();
                //second section of url which is after the domain name
                string secretName = kvurl.Segments[1];
                SecretClientOptions options = new SecretClientOptions()
                {
                    Retry =
                        {
                            Delay= TimeSpan.FromSeconds(1),
                            MaxDelay = TimeSpan.FromSeconds(16),
                            MaxRetries = 2,
                            Mode = RetryMode.Exponential
                        }
                };

                var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential(), options);
                KeyVaultSecret secretBundle = client.GetSecret(secretName);
                string secret = secretBundle.Value;
                Console.WriteLine(secret);

                return "secret from vaault: "+secret;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return "no secret due to error";
        }
    }
}
