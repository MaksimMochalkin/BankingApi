﻿namespace BankingApi.Abstractions.Interfaces
{
    using BankingApi.Data.Entyties;
    using BankingApi.Models.Requests;

    public interface IClientService
    {
        Task<Client> GetClientByIdAsync(Guid id);
        Task<Client> CreateClientAsync(ClientCreateRequest clientDto);
        Task<bool> UpdateClientAsync(Guid id, ClientUpdateRequest clientDto);
        Task<bool> DeleteClientAsync(Guid id);
    }
}
