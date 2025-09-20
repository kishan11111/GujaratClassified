// Models/Response/PagedResponse.cs
namespace GujaratClassified.API.Models.Response
{
    public class PagedResponse<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;
        public int? NextPage => HasNextPage ? PageNumber + 1 : null;
        public int? PreviousPage => HasPreviousPage ? PageNumber - 1 : null;

        public PagedResponse()
        {
        }

        public PagedResponse(List<T> data, int pageNumber, int pageSize, int totalCount)
        {
            Data = data ?? new List<T>();
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = totalCount > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;
        }

        public static PagedResponse<T> Create(List<T> data, int pageNumber, int pageSize, int totalCount)
        {
            return new PagedResponse<T>(data, pageNumber, pageSize, totalCount);
        }

        public static PagedResponse<T> Empty(int pageNumber = 1, int pageSize = 20)
        {
            return new PagedResponse<T>(new List<T>(), pageNumber, pageSize, 0);
        }
    }
}