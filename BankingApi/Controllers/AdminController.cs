namespace RestaurantBooking.BusinesApi.Controllers
{
    using BankingApi.Abstractions.Interfaces;
    using BankingApi.Models.Requests;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]/v1")]
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
    }
}
