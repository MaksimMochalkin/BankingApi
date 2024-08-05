namespace BankingApi.Models.Requests
{
    using BankingApi.Data.Entities;

    public struct ClientFiltrationResult
    {
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<Client> Clients { get; set; }

        public ClientFiltrationResult(int totalItems,
            int page,
            int pageSize,
            List<Client> clients)
        {
            TotalItems = totalItems;
            Page = page;
            PageSize = pageSize;
            Clients = clients;
        }
    }
}
