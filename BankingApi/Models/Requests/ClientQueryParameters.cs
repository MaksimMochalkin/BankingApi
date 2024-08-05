namespace BankingApi.Models.Requests
{
    public class ClientQueryParameters
    {
        public string Filters { get; set; } // Строка для фильтров
        public string? OrderBy { get; set; } = null; // Поле для сортировки
        public bool Descending { get; set; } // Направление сортировки
        public int Page { get; set; } = 1; // Номер страницы
        public int PageSize { get; set; } = 10; // Размер страницы
    }
}
