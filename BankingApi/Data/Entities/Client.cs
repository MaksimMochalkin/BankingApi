using System.Net;

namespace BankingApi.Data.Entyties
{
    public class Client
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PersonalId { get; set; }
        public string ProfilePhoto { get; set; }
        public string MobileNumber { get; set; }
        public string Sex { get; set; }
        public Address Address { get; set; }
        public ICollection<Account> Accounts { get; set; }
    }
}
