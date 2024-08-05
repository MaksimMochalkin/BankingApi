namespace BankingApi.Abstractions
{
    using BankingApi.Abstractions.Interfaces;

    public abstract class SqlCommandHandler<T> : ISqlCommandHandler<T> where T : class
    {
        public virtual string Command => "unknown";

        public abstract IQueryable<T> Execute(IQueryable<T> query, string propertyName, string operation, string value, bool descending);
    }
}
