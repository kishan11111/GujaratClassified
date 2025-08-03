namespace GujaratClassified.API.Models.Response
{
    public class PaginationResponse
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public int PageSize { get; set; }
        public bool HasNext => CurrentPage < TotalPages;
        public bool HasPrevious => CurrentPage > 1;
    }
}
