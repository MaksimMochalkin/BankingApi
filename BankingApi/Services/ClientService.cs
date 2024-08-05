namespace BankingApi.Services
{
    using BankingApi.Abstractions.Interfaces;
    using BankingApi.Data.Entities;
    using BankingApi.Models.Requests;
    using global::AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using Newtonsoft.Json;
    using RestaurantBooking.BusinesApi.Data;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.RegularExpressions;

    public class ClientService : IClientService
    {
        private const string CacheKeyPrefix = "ClientSearch_";
        private readonly BankingAppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public ClientService(BankingAppDbContext context,
            IMapper mapper,
            IMemoryCache memoryCache)
        {
            _context = context;
            _mapper = mapper;
            _memoryCache = memoryCache;
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
                var keys = _memoryCache.Get<List<string>>("CacheKeys") ?? new List<string>();
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

            if (!string.IsNullOrEmpty(queryParams.Filters))
            {
                var filters = queryParams.Filters.Split('&');
                foreach (var filter in filters)
                {
                    var match = Regex.Match(filter, @"(\w+)(eq|notEq|like)'(.+)'");
                    if (match.Success)
                    {
                        var propertyName = match.Groups[1].Value;
                        var operation = match.Groups[2].Value;
                        var value = match.Groups[3].Value;

                        query = ApplyFilter(query, propertyName, operation, value);
                    }
                }
            }

            if (!string.IsNullOrEmpty(queryParams.OrderBy))
            {
                var propertyInfo = typeof(Client).GetProperty(queryParams.OrderBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    query = queryParams.Descending
                        ? query.OrderByDescending(e => propertyInfo.GetValue(e, null))
                        : query.OrderBy(e => propertyInfo.GetValue(e, null));
                }
            }

            var totalItems = query.Count();
            var clients = query
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToList();

            var result = new ClientFiltrationResult(totalItems, queryParams.Page, queryParams.PageSize, clients);
            return result;
        }

        private IQueryable<Client> ApplyFilter(IQueryable<Client> query, string propertyName, string operation, string value)
        {
            var parameter = Expression.Parameter(typeof(Client), "c");
            var property = Expression.Property(parameter, propertyName);

            object constantValue;
            if (property.Type == typeof(Guid))
            {
                constantValue = Guid.Parse(value);
            }
            else
            {
                constantValue = Convert.ChangeType(value, property.Type);
            }

            var constant = Expression.Constant(Convert.ChangeType(constantValue, property.Type));
            Expression expression = default;

            switch (operation.ToLower())
            {
                case "eq":
                    expression = Expression.Equal(property, constant);
                    break;
                case "noteq":
                    expression = Expression.NotEqual(property, constant);
                    break;
                case "like":
                    if (property.Type != typeof(string))
                        throw new InvalidOperationException("LIKE operator is only applicable on string properties.");

                    var method = typeof(DbFunctionsExtensions).GetMethod("Like", new[] { typeof(DbFunctions), typeof(string), typeof(string) });
                    var functions = Expression.Constant(EF.Functions);
                    expression = Expression.Call(null, method, functions, property, constant);
                    break;
            }

            if (expression == null)
            {
                return query;
            }

            var lambda = Expression.Lambda<Func<Client, bool>>(expression, parameter);
            return query.Where(lambda);
        }

        private void CacheQuery(ClientQueryParameters queryParams)
        {
            var queryString = JsonConvert.SerializeObject(queryParams);
            var cacheKey = CacheKeyPrefix + Guid.NewGuid().ToString();
            _memoryCache.Set(cacheKey, queryString, TimeSpan.FromHours(1));

            var keys = _memoryCache.GetOrCreate("CacheKeys", entry => new List<string>());
            if (keys?.Count >= 3)
            {
                _memoryCache.Remove(keys.First());
                keys.RemoveAt(0);
            }
            keys?.Add(cacheKey);
            _memoryCache.Set("CacheKeys", keys);
        }

    }
}
