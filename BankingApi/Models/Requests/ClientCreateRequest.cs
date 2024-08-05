namespace BankingApi.Models.Requests
{
    using BankingApi.Models.Dto;

    public class ClientCreateRequest
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PersonalId { get; set; }
        public string ProfilePhoto { get; set; }
        public string MobileNumber { get; set; }
        public string Sex { get; set; }
        public AddressDto Address { get; set; }
        public ICollection<AccountDto> Accounts { get; set; }
    }
}
