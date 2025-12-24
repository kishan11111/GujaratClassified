using GujaratClassified.API.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;

namespace GujaratClassified.API.Controllers.Admin
{
    [Route("api/admin")]
    [ApiController]
   
    public class AdminController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // 1. Admin Login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> AdminLogin([FromBody] AdminLoginDto login)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var admin = await connection.QueryFirstOrDefaultAsync<AdminDto>(
                        "SP_AdminLogin",
                        new { login.Email, login.Password },
                        commandType: CommandType.StoredProcedure
                    );

                    if (admin == null)
                    {
                        return Unauthorized(new { message = "Invalid credentials" });
                    }

                    // Generate JWT token (implement your token generation logic)
                    var token = GenerateJwtToken(admin);

                    return Ok(new
                    {
                        success = true,
                        message = "Login successful",
                        data = new
                        {
                            admin,
                            token
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // 2. Get Dashboard Statistics
        [HttpGet("dashboard/stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var stats = await connection.QueryFirstAsync<DashboardStatsDto>(
                        "SP_GetAdminDashboardStats",
                        commandType: CommandType.StoredProcedure
                    );

                    return Ok(new
                    {
                        success = true,
                        data = stats
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // 3. Get Pending Posts for Approval
        [HttpGet("posts/pending")]
        public async Task<IActionResult> GetPendingPosts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var posts = await connection.QueryAsync<PostAdminViewDto>(
                        "SP_GetPendingPostsForApproval",
                        new { PageNumber = pageNumber, PageSize = pageSize },
                        commandType: CommandType.StoredProcedure
                    );

                    var firstPost = posts.FirstOrDefault();
                    var totalCount = firstPost?.TotalCount ?? 0;

                    return Ok(new
                    {
                        success = true,
                        data = posts,
                        pagination = new
                        {
                            currentPage = pageNumber,
                            pageSize,
                            totalCount,
                            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // 4. Get All Posts with Filters
        [HttpGet("posts/all")]
        public async Task<IActionResult> GetAllPosts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? status = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] string searchText = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var posts = await connection.QueryAsync<PostListDto>(
                        "SP_GetAllPostsAdmin",
                        new
                        {
                            PageNumber = pageNumber,
                            PageSize = pageSize,
                            Status = status,
                            CategoryId = categoryId,
                            SearchText = searchText,
                            DateFrom = dateFrom,
                            DateTo = dateTo
                        },
                        commandType: CommandType.StoredProcedure
                    );

                    return Ok(new
                    {
                        success = true,
                        data = posts,
                        pagination = new
                        {
                            currentPage = pageNumber,
                            pageSize,
                            totalCount = posts.Count()
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // 5. Get Today's Posts
        [HttpGet("posts/today")]
        public async Task<IActionResult> GetTodaysPosts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var posts = await connection.QueryAsync<PostListDto>(
                        "SP_GetTodaysPosts",
                        new { PageNumber = pageNumber, PageSize = pageSize },
                        commandType: CommandType.StoredProcedure
                    );

                    return Ok(new
                    {
                        success = true,
                        data = posts,
                        date = DateTime.Today.ToString("yyyy-MM-dd")
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // 6. Approve Post
        [HttpPost("posts/{postId}/approve")]
        public async Task<IActionResult> ApprovePost(
            int postId,
            [FromBody] ApproveRejectDto request)
        {
            try
            {
                var adminId = GetAdminIdFromToken(); // Implement this method to get admin ID from JWT

                using (var connection = new SqlConnection(_connectionString))
                {
                    var result = await connection.QueryFirstAsync<ActionResultDto>(
                        "SP_ApprovePost",
                        new
                        {
                            PostId = postId,
                            AdminId = adminId,
                            AdminNotes = request.Notes
                        },
                        commandType: CommandType.StoredProcedure
                    );

                    return Ok(new
                    {
                        success = result.Success == 1,
                        message = result.Message
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // 7. Reject Post
        [HttpPost("posts/{postId}/reject")]
        public async Task<IActionResult> RejectPost(
            int postId,
            [FromBody] ApproveRejectDto request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.RejectionReason))
                {
                    return BadRequest(new { message = "Rejection reason is required" });
                }

                var adminId = GetAdminIdFromToken();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var result = await connection.QueryFirstAsync<ActionResultDto>(
                        "SP_RejectPost",
                        new
                        {
                            PostId = postId,
                            AdminId = adminId,
                            RejectionReason = request.RejectionReason
                        },
                        commandType: CommandType.StoredProcedure
                    );

                    return Ok(new
                    {
                        success = result.Success == 1,
                        message = result.Message
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // 8. Get Post Details for Review
        [HttpGet("posts/{postId}/details")]
        public async Task<IActionResult> GetPostDetailsForReview(int postId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var multi = await connection.QueryMultipleAsync(
                        "SP_GetPostDetailsForReview",
                        new { PostId = postId },
                        commandType: CommandType.StoredProcedure))
                    {
                        var post = await multi.ReadFirstOrDefaultAsync<PostDetailDto>();
                        var images = await multi.ReadAsync<PostImageDto>();
                        var history = await multi.ReadAsync<AdminActionDto>();

                        if (post == null)
                        {
                            return NotFound(new { message = "Post not found" });
                        }

                        return Ok(new
                        {
                            success = true,
                            data = new
                            {
                                post,
                                images,
                                actionHistory = history
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // 9. Bulk Approve Posts
        [HttpPost("posts/bulk-approve")]
        public async Task<IActionResult> BulkApprovePosts([FromBody] BulkApproveDto request)
        {
            try
            {
                var adminId = GetAdminIdFromToken();
                var postIds = string.Join(",", request.PostIds);

                using (var connection = new SqlConnection(_connectionString))
                {
                    var result = await connection.QueryFirstAsync<BulkActionResultDto>(
                        "SP_BulkApprovePosts",
                        new
                        {
                            PostIds = postIds,
                            AdminId = adminId
                        },
                        commandType: CommandType.StoredProcedure
                    );

                    return Ok(new
                    {
                        success = result.Success == 1,
                        message = result.Message,
                        approvedCount = result.ApprovedCount
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // 10. Get Category Wise Statistics
        [HttpGet("stats/category-wise")]
        public async Task<IActionResult> GetCategoryWiseStats()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var stats = await connection.QueryAsync<CategoryStatsDto>(
                        "SP_GetCategoryWiseStats",
                        commandType: CommandType.StoredProcedure
                    );

                    return Ok(new
                    {
                        success = true,
                        data = stats
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // 11. Toggle Featured Post
        [HttpPost("posts/{postId}/toggle-featured")]
        public async Task<IActionResult> ToggleFeaturedPost(int postId)
        {
            try
            {
                var adminId = GetAdminIdFromToken();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var result = await connection.QueryFirstAsync<ToggleFeaturedResultDto>(
                        "SP_ToggleFeaturedPost",
                        new
                        {
                            PostId = postId,
                            AdminId = adminId
                        },
                        commandType: CommandType.StoredProcedure
                    );

                    return Ok(new
                    {
                        success = result.Success == 1,
                        message = result.Message,
                        isFeatured = result.NewFeaturedStatus == 1
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // 12. Delete Post (Soft Delete)
        [HttpDelete("posts/{postId}")]
        public async Task<IActionResult> DeletePost(
            int postId,
            [FromBody] DeletePostDto request)
        {
            try
            {
                var adminId = GetAdminIdFromToken();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var result = await connection.QueryFirstAsync<ActionResultDto>(
                        "SP_DeletePostAdmin",
                        new
                        {
                            PostId = postId,
                            AdminId = adminId,
                            DeleteReason = request.DeleteReason
                        },
                        commandType: CommandType.StoredProcedure
                    );

                    return Ok(new
                    {
                        success = result.Success == 1,
                        message = result.Message
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // 13. Search Posts
        [HttpGet("posts/search")]
        public async Task<IActionResult> SearchPosts(
            [FromQuery] string searchText,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var posts = await connection.QueryAsync<PostListDto>(
                        "SP_SearchPostsAdmin",
                        new
                        {
                            SearchText = searchText,
                            PageNumber = pageNumber,
                            PageSize = pageSize
                        },
                        commandType: CommandType.StoredProcedure
                    );

                    return Ok(new
                    {
                        success = true,
                        data = posts,
                        searchTerm = searchText
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // 14. Get User's Post History
        [HttpGet("users/{userId}/posts")]
        public async Task<IActionResult> GetUserPostHistory(int userId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var posts = await connection.QueryAsync<PostListDto>(
                        "SP_GetUserPostHistory",
                        new { UserId = userId },
                        commandType: CommandType.StoredProcedure
                    );

                    return Ok(new
                    {
                        success = true,
                        data = posts,
                        userId
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // Helper method to get admin ID from JWT token
        private int GetAdminIdFromToken()
        {
            // Implement your logic to extract admin ID from JWT token
            // Example:
            var adminIdClaim = User.Claims.FirstOrDefault(c => c.Type == "AdminId");
            if (adminIdClaim != null && int.TryParse(adminIdClaim.Value, out int adminId))
            {
                return adminId;
            }
            return 1; // Default for testing, replace with actual implementation
        }

        // Helper method to generate JWT token
        private string GenerateJwtToken(AdminDto admin)
        {
            // Implement your JWT token generation logic
            // This is a placeholder - implement proper JWT generation
            return "your-generated-jwt-token";
        }
    }
}
