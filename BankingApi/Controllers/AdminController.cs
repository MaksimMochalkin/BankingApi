using BankingApi.Controllers;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.IdentityModel.Tokens.Jwt;

namespace RestaurantBooking.BusinesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(ILogger<AdminController> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;

        }

       
        [HttpGet]
        [Authorize(Policy = "HasAdminRole")]
        [Route("create-admin")]
        public async Task<IActionResult> CreateAdmin()
        {
            var model = new ClaimManager(HttpContext, User);
            var jsonToken = await HttpContext.GetTokenAsync("access_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            var json = JsonConvert.SerializeObject(new { Email = "mm@gmail.com", FirstName = "sdfsdfs", Password = "123qweAqws!!", ConfirmPassword = "123qweAqws!!", Role = "Administrator" });
            var content1 = new StringContent(json);
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.SetBearerToken(jsonToken);
                var result = await client.PostAsync("https://localhost:7293/Auth/admin-registration", content1);
                var content = await result.Content.ReadAsStringAsync();
                return Ok(content);
            }
            catch (Exception ex)
            {
                await RefreshToken(refreshToken);
                var client = _httpClientFactory.CreateClient();
                jsonToken = await HttpContext.GetTokenAsync("access_token");
                client.SetBearerToken(jsonToken);
                var result = await client.GetAsync("https://localhost:44350/User/GetUserInfoForBusinesApi");
                var content = await result.Content.ReadAsStringAsync();
                return Ok(content);
            }
        }

        private async Task RefreshToken(string refreshToken)
        {
            var refreshClient = _httpClientFactory.CreateClient();
            var response = await refreshClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                ClientId = "BusinesApiClient_id",
                ClientSecret = "BusinesApiClient_secret",
                Address = "https://localhost:7273/connect/token",
                RefreshToken = refreshToken,
                Scope = "openid RestaurantBooking.BusinesApi offline_access"
            });

            await UpdateAuthTokens(response.AccessToken, response.RefreshToken);
        }

        private async Task UpdateAuthTokens(string accessToken, string refreshToken)
        {
            var authenticate = await HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

            authenticate.Properties.UpdateTokenValue("access_token", accessToken);
            authenticate.Properties.UpdateTokenValue("refresh_token", refreshToken);

            await HttpContext.SignInAsync(authenticate.Principal, authenticate.Properties);
        }
    }
}
