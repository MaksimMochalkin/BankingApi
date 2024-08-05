namespace BankingApi.SqlCommandHandlers
{
    using BankingApi.Abstractions;
    using Microsoft.EntityFrameworkCore;
    using System.Linq.Expressions;

    public class LikeSqlCommandHandler<T> : SqlCommandHandler<T> where T : class
    {
        public override string Command => "like";

        public override IQueryable<T> Execute(IQueryable<T> query, string propertyName, string operation, string value, bool descending)
        {
            var parameter = Expression.Parameter(typeof(T), "c");
            var property = Expression.Property(parameter, propertyName);

            if (property.Type != typeof(string))
                throw new InvalidOperationException("LIKE operator is only applicable on string properties.");

            object constantValue;
            if (property.Type == typeof(Guid))
            {
                constantValue = Guid.Parse(value);
            }
            else
            {
                constantValue = Convert.ChangeType(value, property.Type);
            }

            var constant = Expression.Constant(Convert.ChangeType(constantValue, property.Type));
            var method = typeof(DbFunctionsExtensions).GetMethod("Like", new[] { typeof(DbFunctions), typeof(string), typeof(string) });
            var functions = Expression.Constant(EF.Functions);
            var expression = Expression.Call(null, method, functions, property, constant);
            if (expression == null)
            {
                return query;
            }

            var lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
            return query.Where(lambda);
        }
    }
}
