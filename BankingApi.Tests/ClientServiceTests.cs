namespace BankingApi.Tests
{
    using AutoMapper;
    using BankingApi.Abstractions.Interfaces;
    using BankingApi.Data;
    using BankingApi.Data.Entities;
    using BankingApi.Models.Dto;
    using BankingApi.Models.Requests;
    using BankingApi.Services;
    using BankingApi.Tests.Helpers;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using Newtonsoft.Json;
    using Shouldly;
    using System;

    public class ClientServiceTests
    {
        private const string CacheKeyPrefix = "ClientSearch_";
        private readonly BankingAppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private IClientService _clientService;

        public ClientServiceTests()
        {
            var myProfile = new AutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var mapper = new Mapper(configuration);
            _mapper = new Mapper(configuration);
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            var options = new DbContextOptionsBuilder<BankingAppDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                    .Options;
            _context = new BankingAppDbContext(options);
        }

        [Fact]
        public async Task CreateClientAsync_ValidRequest_ReturnsCreatedClient()
        {
            // Arrange
            SeedDatabase(_context);
            _clientService = new ClientService(_context, _mapper, _memoryCache);
            var request = new ClientCreateRequest
            {
                FirstName = "JohnCreate",
                LastName = "DoeCreate",
                Email = "john.doe@createexample.com",
                PersonalId = "123456789",
                MobileNumber = "1234567890",
                Sex = "Male",
                ProfilePhoto = "Photo",
                Address = new AddressDto
                {
                    Country = "country",
                    City = "city",
                    Street = "street",
                    ZipCode = "zipcode"
                },
                Accounts = new List<AccountDto>
                {
                    new AccountDto
                    {
                       AccountNumber = "1234567890",
                       Balance = 0,
                    }
                }
            };

            // Act
            var result = await _clientService.CreateClientAsync(request);

            // Assert
            result.FirstName.ShouldBe(request.FirstName);
            result.LastName.ShouldBe(request.LastName);
            result.Email.ShouldBe(request.Email);
            _context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task GetClientByIdAsync_ExistingClient_ReturnsClient()
        {
            // Arrange and Act
            SeedDatabase(_context);
            _clientService = new ClientService(_context, _mapper, _memoryCache);
            var result = await _clientService.GetClientByIdAsync(ClientGuids.JohnDoeId);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(ClientGuids.JohnDoeId);
            _context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task DeleteClientAsync_ExistingClient_ReturnsTrue()
        {
            // Arrange and Act
            SeedDatabase(_context);
            _clientService = new ClientService(_context, _mapper, _memoryCache);
            var result = await _clientService.DeleteClientAsync(ClientGuids.DeleteDoeId);

            // Assert
            result.ShouldBeTrue();
            //ClearDatabase();
            _context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task UpdateClientAsync_ExistingClient_ReturnsTrue()
        {
            // Arrange
            SeedDatabase(_context);
            _clientService = new ClientService(_context, _mapper, _memoryCache);
            var updateRequest = new ClientUpdateRequest { FirstName = "Update2" };

            // Act
            var result = await _clientService.UpdateClientAsync(ClientGuids.UpdateDoeId, updateRequest);

            // Assert
            result.ShouldBeTrue();
            //ClearDatabase();
            _context.Database.EnsureDeleted();
        }

        [Fact]
        public void GetClientsByParamsAsync_WithValidParams_ReturnsFilteredClients()
        {
            // Arrange
            SeedDatabase(_context);
            _clientService = new ClientService(_context, _mapper, _memoryCache);
            var queryParams = new ClientQueryParameters
            {
                Filters = "lastnamelike'%Doe%'",
                OrderBy = "lastname",
                Descending = true,
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = _clientService.GetClientsByParamsAsync(queryParams);

            // Assert
            result.TotalItems.ShouldBe(4);
            result.Clients.Count.ShouldBe(4);
            //ClearDatabase();
            _context.Database.EnsureDeleted();
        }

        [Fact]
        public void GetRecentQueries_ReturnsListOfQueries()
        {
            // Arrange
            _clientService = new ClientService(_context, _mapper, _memoryCache);
            var queryParamsList = new List<ClientQueryParameters> 
            {
                new ClientQueryParameters
                {
                    Filters = "lastnamelike'%Doe%'",
                    OrderBy = "lastname",
                    Descending = true,
                    Page = 1,
                    PageSize = 10
                },
                new ClientQueryParameters
                {
                    Filters = "lastnamelike'%Doe2%'",
                    OrderBy = "lastname1",
                    Descending = false,
                    Page = 1,
                    PageSize = 10
                },
                new ClientQueryParameters
                {
                    Filters = "lastnamelike'%Doe3%'",
                    OrderBy = "lastname2",
                    Descending = true,
                    Page = 10,
                    PageSize = 100
                },
                new ClientQueryParameters
                {
                    Filters = "lastnamelike'%Doe4%'",
                    OrderBy = "lastname3",
                    Descending = false,
                    Page = 11,
                    PageSize = 50
                }
            };
            queryParamsList.ForEach(CacheQuery);

            // Act
            var result = _clientService.GetRecentQueries();

            // Assert
            result.Count.ShouldBe(queryParamsList.Count - 1);
        }

        private void SeedDatabase(BankingAppDbContext context)
        {
            context.Clients.AddRange(new List<Client>
            {
                new Client
                {
                    Id = ClientGuids.JohnDoeId,
                    Email = "test1@example.com",
                    FirstName = "John",
                    LastName = "Doe",
                    PersonalId = "123456789",
                    MobileNumber = "1234567890",
                    Sex = "Male",
                    ProfilePhoto = "Photo",
                    Address = new Address
                    {
                        Id = Guid.NewGuid(),
                        Country = "country",
                        City = "city",
                        Street = "street",
                        ZipCode = "zipcode"
                    },
                    Accounts = new List<Account>
                    {
                        new Account
                        {
                            AccountNumber = "1234567890",
                            Balance = 0,
                        }
                    }
                },
                new Client
                {
                    Id = ClientGuids.JaneDoeId,
                    Email = "test2@example.com",
                    FirstName = "Jane",
                    LastName = "Doe",
                    PersonalId = "987654321",
                    MobileNumber = "0987654321",
                    Sex = "Female",
                    ProfilePhoto = "Photo",
                    Address = new Address
                    {
                        Id = Guid.NewGuid(),
                        Country = "country",
                        City = "city",
                        Street = "street",
                        ZipCode = "zipcode"
                    },
                    Accounts = new List<Account>
                    {
                        new Account
                        {
                            AccountNumber = "1234567890",
                            Balance = 0,
                        }
                    }
                },
                new Client
                {
                    Id = ClientGuids.UpdateDoeId,
                    Email = "test3@example.com",
                    FirstName = "Update",
                    LastName = "Doe",
                    PersonalId = "987654322",
                    MobileNumber = "0987654322",
                    Sex = "Female",
                    ProfilePhoto = "Photo",
                    Address = new Address
                    {
                        Id = Guid.NewGuid(),
                        Country = "country",
                        City = "city",
                        Street = "street",
                        ZipCode = "zipcode"
                    },
                    Accounts = new List<Account>
                    {
                        new Account
                        {
                            AccountNumber = "1234567890",
                            Balance = 0,
                        }
                    }
                },
                new Client
                {
                    Id = ClientGuids.DeleteDoeId,
                    Email = "test4@example.com",
                    FirstName = "Delete",
                    LastName = "Doe",
                    PersonalId = "987654323",
                    MobileNumber = "0987654323",
                    Sex = "Female",
                    ProfilePhoto = "Photo",
                    Address = new Address
                    {
                        Id = Guid.NewGuid(),
                        Country = "country",
                        City = "city",
                        Street = "street",
                        ZipCode = "zipcode"
                    },
                    Accounts = new List<Account>
                    {
                        new Account
                        {
                            AccountNumber = "1234567890",
                            Balance = 0,
                        }
                    }
                }
            });
            context.SaveChanges();
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