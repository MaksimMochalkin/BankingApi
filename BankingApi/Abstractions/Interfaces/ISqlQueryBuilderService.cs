namespace BankingApi.Abstractions.Interfaces
{
    using BankingApi.Models.Requests;

    public interface ISqlQueryBuilderService<T> where T : class
    {
        IQueryable<T> GenerateSqlQuery(IQueryable<T> query, QueryParametersBase queryParams);
    }
}
