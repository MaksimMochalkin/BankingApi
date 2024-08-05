namespace BankingApi.SqlCommandHandlers
{
    using BankingApi.Abstractions;
    using System.Linq.Expressions;

    public class OrderBySqlCommandHandler<T> : SqlCommandHandler<T> where T : class
    {
        public override string Command => "orderby";

        public override IQueryable<T> Execute(IQueryable<T> query, string propertyName, string operation, string value, bool descending)
        {
            var parameter = Expression.Parameter(typeof(T), "c");
            var property = Expression.Property(parameter, propertyName);

            var keySelector = Expression.Lambda<Func<T, object>>(
                Expression.Convert(property, typeof(object)),
                parameter);

            if (descending)
            {
                query = query.OrderByDescending(keySelector);
            }
            else
            {
                query = query.OrderBy(keySelector);
            }

            return query;
        }
    }
}
