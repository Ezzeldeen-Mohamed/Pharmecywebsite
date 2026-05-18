using System.Collections.Generic;

namespace ePharma_asp_mvc.Data.ViewModels
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}