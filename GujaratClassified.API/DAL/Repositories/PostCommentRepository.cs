using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class PostCommentRepository : IPostCommentRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PostCommentRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> AddCommentAsync(PostComment comment)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PostId", comment.PostId);
            parameters.Add("@UserId", comment.UserId);
            parameters.Add("@ParentCommentId", comment.ParentCommentId);
            parameters.Add("@CommentText", comment.CommentText);

            var commentId = await connection.QuerySingleAsync<int>(
                "SP_AddPostComment",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return commentId;
        }

        public async Task<PostComment?> GetCommentByIdAsync(int commentId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@CommentId", commentId);

            var comment = await connection.QueryFirstOrDefaultAsync<PostComment>(
                "SP_GetCommentById",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return comment;
        }

        public async Task<bool> UpdateCommentAsync(int commentId, int userId, string commentText)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@CommentId", commentId);
            parameters.Add("@UserId", userId);
            parameters.Add("@CommentText", commentText);

            var rowsAffected = await connection.ExecuteAsync(
                "SP_UpdatePostComment",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteCommentAsync(int commentId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@CommentId", commentId);
            parameters.Add("@UserId", userId);

            var rowsAffected = await connection.ExecuteAsync(
                "SP_DeletePostComment",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }

        public async Task<(List<PostComment> Comments, int TotalCount)> GetPostCommentsAsync(int postId, int pageSize, int pageNumber, string sortBy, bool includeReplies)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PostId", postId);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@SortBy", sortBy);
            parameters.Add("@IncludeReplies", includeReplies);

            using var multi = await connection.QueryMultipleAsync(
                "SP_GetPostComments",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var comments = (await multi.ReadAsync<PostComment>()).ToList();
            var totalCount = await multi.ReadFirstAsync<int>();

            return (comments, totalCount);
        }

        public async Task<List<PostComment>> GetCommentRepliesAsync(int parentCommentId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@ParentCommentId", parentCommentId);

            var replies = await connection.QueryAsync<PostComment>(
                "SP_GetCommentReplies",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return replies.ToList();
        }

        //public async Task<int> GetPostCommentCountAsync(int postId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var parameters = new DynamicParameters();
        //    parameters.Add("@PostId", postId);

        //    var count = await connection.QueryFirstOrDefaultAsync<int>(
        //        "SELECT COUNT(1) FROM PostComments WHERE PostId = @PostId AND IsActive = 1 AND IsBlocked = 0",
        //        parameters
        //    );

        //    return count;
        //}

        public async Task<int> GetPostCommentCountAsync(int postId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PostId", postId);

            var count = await connection.QueryFirstOrDefaultAsync<int>(
                "SP_GetPostCommentCount",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return count;
        }


        //public async Task<bool> CanUserModifyCommentAsync(int commentId, int userId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var parameters = new DynamicParameters();
        //    parameters.Add("@CommentId", commentId);
        //    parameters.Add("@UserId", userId);

        //    var result = await connection.QueryFirstOrDefaultAsync<int>(
        //        "SELECT COUNT(1) FROM PostComments WHERE CommentId = @CommentId AND UserId = @UserId AND IsActive = 1",
        //        parameters
        //    );

        //    return result > 0;
        //}

        public async Task<bool> CanUserModifyCommentAsync(int commentId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@CommentId", commentId);
            parameters.Add("@UserId", userId);

            var result = await connection.QueryFirstOrDefaultAsync<int>(
                "SP_CanUserModifyComment",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

    }
}