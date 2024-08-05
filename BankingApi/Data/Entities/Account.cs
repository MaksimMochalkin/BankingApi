namespace BankingApi.Data.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
    }
}
