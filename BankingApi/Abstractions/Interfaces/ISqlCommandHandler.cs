namespace BankingApi.Abstractions.Interfaces
{
    public interface ISqlCommandHandler<T> where T : class
    {
        string Command { get; }

        IQueryable<T> Execute(IQueryable<T> query, string propertyName, string operation, string value, bool descending);

    }
}
