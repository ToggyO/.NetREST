using System.Collections.Generic;

namespace NetREST.Common.Models.Pagination
{
    public class PaginationModel
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PaginationModel<T> : PaginationModel
    {
        public int Total { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}