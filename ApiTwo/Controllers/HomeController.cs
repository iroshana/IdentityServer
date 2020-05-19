using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiTwo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [Route("/")]
        public async Task<IActionResult> Index()
        {
            var serverClient = _httpClientFactory.CreateClient();

            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync("https://localhost:44332/");

            var tokenResponce = await serverClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest 
                { 
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = "client_id",
                    ClientSecret = "client_secret",
                    Scope = "ApiOne"
                });

            var apiClient = _httpClientFactory.CreateClient();

            apiClient.SetBearerToken(tokenResponce.AccessToken);

            var response = await apiClient.GetAsync("https://localhost:44331/secret");

            var content = response.Content.ReadAsStringAsync();

            return Ok(new 
            {
                access_token = tokenResponce.AccessToken,
                message = content
            });
        }

    }
}
