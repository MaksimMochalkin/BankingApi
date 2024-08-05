namespace BankingApi.Services
{
    using BankingApi.Abstractions.Interfaces;
    using BankingApi.Data;
    using BankingApi.Data.Entities;
    using BankingApi.Models.Requests;
    using global::AutoMapper;
    using Microsoft.Extensions.Caching.Memory;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class ClientService : IClientService
    {
        private const string CacheKeyPrefix = "ClientSearch_";
        private const string CacheKeys = "CacheKeys";
        private readonly BankingAppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly ISqlQueryBuilderService<Client> _sqlQueryBuilderService;

        public ClientService(BankingAppDbContext context,
            IMapper mapper,
            IMemoryCache memoryCache,
            ISqlQueryBuilderService<Client> sqlQueryBuilderService)
        {
            _context = context;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _sqlQueryBuilderService = sqlQueryBuilderService;
        }

        public async Task<Client> CreateClientAsync(ClientCreateRequest request)
        {
            try
            {
                var client = _mapper.Map<Client>(request);
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
                return client;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<bool> DeleteClientAsync(Guid id)
        {
            try
            {
                var client = await _context.Clients.FindAsync(id);
                if (client == null)
                {
                    return false;
                }

                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Client> GetClientByIdAsync(Guid id)
        {
            try
            {
                return await _context.Clients.FindAsync(id);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<bool> UpdateClientAsync(Guid id, ClientUpdateRequest clientDto)
        {
            try
            {
                var client = await _context.Clients.FindAsync(id);
                if (client == null)
                {
                    return false;
                }

                _mapper.Map(clientDto, client);
                _context.Clients.Update(client);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public ClientFiltrationResult GetClientsByParamsAsync(ClientQueryParameters queryParams)
        {
            try
            {
                var result = GetClientsAsync(queryParams);
                CacheQuery(queryParams);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public List<string> GetRecentQueries()
        {
            try
            {
                var keys = _memoryCache.Get<List<string>>(CacheKeys) ?? new List<string>();
                var recentQueries = keys.Select(_memoryCache.Get<string>).ToList();

                return recentQueries;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private ClientFiltrationResult GetClientsAsync(ClientQueryParameters queryParams)
        {
            var query = _context.Clients.AsQueryable();
            query = _sqlQueryBuilderService.GenerateSqlQuery(query, queryParams);

            // todo: make async
            var totalItems = query.Count();
            var clients = query
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToList();

            var result = new ClientFiltrationResult(totalItems, queryParams.Page, queryParams.PageSize, clients);
            return result;
        }

        private void CacheQuery(ClientQueryParameters queryParams)
        {
            var queryString = JsonConvert.SerializeObject(queryParams);
            var cacheKey = CacheKeyPrefix + Guid.NewGuid().ToString();
            _memoryCache.Set(cacheKey, queryString, TimeSpan.FromHours(1));

            var keys = _memoryCache.GetOrCreate(CacheKeys, entry => new List<string>());
            if (keys?.Count >= 3)
            {
                _memoryCache.Remove(keys.First());
                keys.RemoveAt(0);
            }
            keys?.Add(cacheKey);
            _memoryCache.Set(CacheKeys, keys);
        }

    }
}
