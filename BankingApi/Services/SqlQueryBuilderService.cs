namespace BankingApi.Services
{
    using BankingApi.Abstractions.Interfaces;
    using BankingApi.Models.Requests;
    using BankingApi.SqlCommandHandlers;
    using System.Text.RegularExpressions;

    public class SqlQueryBuilderService<T> : ISqlQueryBuilderService<T> where T : class
    {
        private static Dictionary<string, ISqlCommandHandler<T>> _sqlCommandHandlers => InitCommandHandlers();

        public SqlQueryBuilderService()
        {
        }

        public IQueryable<T> GenerateSqlQuery(IQueryable<T> query, QueryParametersBase queryParams)
        {
            if (!string.IsNullOrEmpty(queryParams.Filters))
            {
                var filters = queryParams.Filters.Split('&');
                foreach (var filter in filters)
                {
                    var match = Regex.Match(filter, @"(\w+)(eq|notEq|like)'(.+)'");
                    if (match.Success)
                    {
                        var propertyName = match.Groups[1].Value;
                        var operation = match.Groups[2].Value;
                        var value = match.Groups[3].Value;

                        query = ApplyFilter(query, propertyName, operation, value);
                    }
                }
            }

            if (!string.IsNullOrEmpty(queryParams.OrderBy))
            {
                query = ApplyFilter(query, queryParams.OrderBy, "orderby", null, queryParams.Descending);
            }

            return query;
        }

        private static Dictionary<string, ISqlCommandHandler<T>> InitCommandHandlers()
        {
            return new Dictionary<string, ISqlCommandHandler<T>>
            {
                { "eq", new EqualSqlCommandHandler<T>() },
                { "noteq", new NotEqualSqlCommandHandler<T>() },
                { "like", new LikeSqlCommandHandler<T>() },
                { "orderby", new OrderBySqlCommandHandler<T>() }
            };
        }

        private IQueryable<T> ApplyFilter(IQueryable<T> query, string propertyName, string operation, string value, bool descending = false)
        {
            if (!_sqlCommandHandlers.TryGetValue(operation.ToLower(), out var sqlCommandHandler))
            {
                throw new NotImplementedException("Sql handler not found");
            }

            var finalQuery = sqlCommandHandler.Execute(query, propertyName, operation, value, descending);

            return finalQuery;
        }

    }

}
