namespace BankingApi.Models.Requests
{
    public class QueryParametersBase
    {
        public string Filters { get; set; }
        public string? OrderBy { get; set; } = null;
        public bool Descending { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
