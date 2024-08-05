namespace BankingApi.Controllers
{
    using BankingApi.Abstractions.Interfaces;
    using BankingApi.Models.Requests;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]/v1")]
    [Authorize(Policy = "HasAdminRole")]
    public class ClientFiltersController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ILogger<ClientFiltersController> _logger;

        public ClientFiltersController(IClientService clientService,
            ILogger<ClientFiltersController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        [HttpGet("get-sort-clients")]
        public async Task<IActionResult> GetClients([FromQuery] ClientQueryParameters queryParams)
        {
            var result = _clientService.GetClientsByParamsAsync(queryParams);

            return Ok(result);
        }


        [HttpGet("recent-queries")]
        public IActionResult GetRecentQueries()
        {
            var recentQueries = _clientService.GetRecentQueries();

            return Ok(recentQueries);
        }
    }
}
