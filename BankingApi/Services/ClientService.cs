namespace BankingApi.Services
{
    using BankingApi.Abstractions.Interfaces;
    using BankingApi.Data.Entyties;
    using BankingApi.Models.Requests;
    using global::AutoMapper;
    using RestaurantBooking.BusinesApi.Data;

    public class ClientService : IClientService
    {
        private readonly BankingAppDbContext _context;
        private readonly IMapper _mapper;

        public ClientService(BankingAppDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Client> CreateClientAsync(ClientCreateRequest request)
        {
            var client = _mapper.Map<Client>(request);
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return client;
        }

        public async Task<bool> DeleteClientAsync(Guid id)
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

        public async Task<Client> GetClientByIdAsync(Guid id)
        {
            return await _context.Clients.FindAsync(id);
        }

        public async Task<bool> UpdateClientAsync(Guid id, ClientUpdateRequest clientDto)
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
    }
}
