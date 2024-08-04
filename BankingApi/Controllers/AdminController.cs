namespace RestaurantBooking.BusinesApi.Controllers
{
    using BankingApi.Abstractions.Interfaces;
    using BankingApi.Controllers;
    using BankingApi.Models.Requests;
    using IdentityModel.Client;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [ApiController]
    [Route("[controller]/v1")]
    [Authorize(Policy = "HasAdminRole")]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IClientService _clientService;

        public AdminController(ILogger<AdminController> logger,
            IHttpClientFactory httpClientFactory,
            IClientService clientService)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _clientService = clientService;
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetClient(Guid id)
        {
            var client = await _clientService.GetClientByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }

        [HttpPost("client-create")]
        public async Task<IActionResult> CreateClient([FromBody] ClientCreateRequest model)
        {
            try
            {
                // todo: think about photo field
                _logger.LogInformation($"CreateClient request model: {model}");

                var client = await _clientService.CreateClientAsync(model);

                if (client is not null)
                {
                    _logger.LogInformation("Client created successfully");

                    return Ok("Client created successfully");
                }

                return BadRequest(client);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("client-update")]
        public async Task<IActionResult> UpdateClient(Guid id, [FromBody] ClientUpdateRequest model)
        {
            try
            {
                var result = await _clientService.UpdateClientAsync(id, model);
                if (!result)
                {
                    return NotFound();
                }

                _logger.LogInformation("Client updated successfully");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("client-delete")]
        public async Task<IActionResult> DeleteClient(Guid id)
        {
            try
            {
                var result = await _clientService.DeleteClientAsync(id);
                if (!result)
                {
                    _logger.LogInformation($"Client with Id: {id} not found.");

                    return NotFound($"Client with Id: {id} not found.");
                }

                return Ok($"Client with Id: {id} deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
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
