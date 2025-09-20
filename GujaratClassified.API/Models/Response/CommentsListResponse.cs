namespace GujaratClassified.API.Models.Response
{
    public class CommentsListResponse
    {
        public List<CommentResponse> Comments { get; set; } = new();
        public int TotalComments { get; set; }
        public PaginationResponse Pagination { get; set; }
    }
}