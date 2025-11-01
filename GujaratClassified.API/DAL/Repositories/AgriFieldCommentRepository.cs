using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class AgriFieldCommentRepository : IAgriFieldCommentRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AgriFieldCommentRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> AddCommentAsync(AgriFieldComment comment)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AgriFieldId", comment.AgriFieldId);
            parameters.Add("@UserId", comment.UserId);
            parameters.Add("@ParentCommentId", comment.ParentCommentId);
            parameters.Add("@CommentText", comment.CommentText);
            parameters.Add("@CommentType", comment.CommentType);

            var commentId = await connection.QuerySingleAsync<int>(
                "SP_AddAgriFieldComment",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            // Increment comment count
            await IncrementCommentCountAsync(comment.AgriFieldId);

            return commentId;
        }

        //public async Task<List<AgriFieldComment>> GetCommentsAsync(int agriFieldId, int pageNumber = 1, int pageSize = 50)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var offset = (pageNumber - 1) * pageSize;

        //    var comments = await connection.QueryAsync<AgriFieldComment>(
        //        @"SELECT c.*, u.FirstName + ' ' + ISNULL(u.LastName, '') as UserName, 
        //                 u.ProfileImage as UserProfileImage, u.IsVerified as UserVerified
        //          FROM AgriFieldComments c
        //          INNER JOIN Users u ON c.UserId = u.UserId
        //          WHERE c.AgriFieldId = @AgriFieldId AND c.IsActive = 1 AND c.ParentCommentId IS NULL
        //          ORDER BY c.CreatedAt DESC
        //          OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY",
        //        new { AgriFieldId = agriFieldId, Offset = offset, PageSize = pageSize }
        //    );

        //    var commentsList = comments.ToList();

        //    // Get replies for each comment
        //    foreach (var comment in commentsList)
        //    {
        //        var replies = await connection.QueryAsync<AgriFieldComment>(
        //            @"SELECT c.*, u.FirstName + ' ' + ISNULL(u.LastName, '') as UserName, 
        //                     u.ProfileImage as UserProfileImage, u.IsVerified as UserVerified
        //              FROM AgriFieldComments c
        //              INNER JOIN Users u ON c.UserId = u.UserId
        //              WHERE c.ParentCommentId = @CommentId AND c.IsActive = 1
        //              ORDER BY c.CreatedAt ASC",
        //            new { CommentId = comment.CommentId }
        //        );

        //        comment.Replies = replies.ToList();
        //    }

        //    return commentsList;
        //}


        public async Task<List<AgriFieldComment>> GetCommentsAsync(int agriFieldId, int pageNumber = 1, int pageSize = 50)
        {
            using var connection = _connectionFactory.CreateConnection();

            var offset = (pageNumber - 1) * pageSize;

            var comments = await connection.QueryAsync<AgriFieldComment>(
                "SP_GetAgriFieldComments",
                new { AgriFieldId = agriFieldId, Offset = offset, PageSize = pageSize },
                commandType: CommandType.StoredProcedure
            );

            var commentsList = comments.ToList();

            // Get replies for each comment
            foreach (var comment in commentsList)
            {
                var replies = await connection.QueryAsync<AgriFieldComment>(
                    "SP_GetAgriFieldCommentReplies",
                    new { CommentId = comment.CommentId },
                    commandType: CommandType.StoredProcedure
                );

                comment.Replies = replies.ToList();
            }

            return commentsList;
        }


        //public async Task<bool> UpdateCommentAsync(int commentId, int userId, string commentText)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var result = await connection.ExecuteAsync(
        //        @"UPDATE AgriFieldComments 
        //          SET CommentText = @CommentText, UpdatedAt = @UpdatedAt 
        //          WHERE CommentId = @CommentId AND UserId = @UserId",
        //        new
        //        {
        //            CommentId = commentId,
        //            UserId = userId,
        //            CommentText = commentText,
        //            UpdatedAt = DateTime.UtcNow
        //        }
        //    );

        //    return result > 0;
        //}

        public async Task<bool> UpdateCommentAsync(int commentId, int userId, string commentText)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QuerySingleAsync<int>(
                "SP_UpdateAgriFieldComment",
                new
                {
                    CommentId = commentId,
                    UserId = userId,
                    CommentText = commentText
                },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

        //public async Task<bool> DeleteCommentAsync(int commentId, int userId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var result = await connection.ExecuteAsync(
        //        "UPDATE AgriFieldComments SET IsActive = 0 WHERE CommentId = @CommentId AND UserId = @UserId",
        //        new { CommentId = commentId, UserId = userId }
        //    );

        //    return result > 0;
        //}

        public async Task<bool> DeleteCommentAsync(int commentId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QuerySingleAsync<int>(
                "SP_DeleteAgriFieldComment",
                new { CommentId = commentId, UserId = userId },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }


        //public async Task<AgriFieldComment?> GetCommentByIdAsync(int commentId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var comment = await connection.QueryFirstOrDefaultAsync<AgriFieldComment>(
        //        "SELECT * FROM AgriFieldComments WHERE CommentId = @CommentId",
        //        new { CommentId = commentId }
        //    );

        //    return comment;
        //}

        public async Task<AgriFieldComment?> GetCommentByIdAsync(int commentId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var comment = await connection.QueryFirstOrDefaultAsync<AgriFieldComment>(
                "SP_GetAgriFieldCommentById",
                new { CommentId = commentId },
                commandType: CommandType.StoredProcedure
            );

            return comment;
        }

        //public async Task<bool> IncrementCommentCountAsync(int agriFieldId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var result = await connection.ExecuteAsync(
        //        "UPDATE AgriFields SET CommentCount = CommentCount + 1 WHERE AgriFieldId = @AgriFieldId",
        //        new { AgriFieldId = agriFieldId }
        //    );

        //    return result > 0;
        //}


        public async Task<bool> IncrementCommentCountAsync(int agriFieldId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QuerySingleAsync<int>(
                "SP_IncrementAgriFieldCommentCount",
                new { AgriFieldId = agriFieldId },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }


        //public async Task<bool> DecrementCommentCountAsync(int agriFieldId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var result = await connection.ExecuteAsync(
        //        "UPDATE AgriFields SET CommentCount = CASE WHEN CommentCount > 0 THEN CommentCount - 1 ELSE 0 END WHERE AgriFieldId = @AgriFieldId",
        //        new { AgriFieldId = agriFieldId }
        //    );

        //    return result > 0;
        //}

        public async Task<bool> DecrementCommentCountAsync(int agriFieldId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QuerySingleAsync<int>(
                "SP_DecrementAgriFieldCommentCount",
                new { AgriFieldId = agriFieldId },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

    }
}