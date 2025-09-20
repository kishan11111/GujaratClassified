using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Common;
using GujaratClassified.API.Models.Entity;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;
using GujaratClassified.API.Services.Interfaces;
using System.Text.Json;

namespace GujaratClassified.API.Services.Implementations
{
    public class AgriFieldService : IAgriFieldService
    {
        private readonly IAgriFieldRepository _agriFieldRepository;
        private readonly IAgriFieldImageRepository _imageRepository;
        private readonly IAgriFieldVideoRepository _videoRepository;
        private readonly IAgriFieldCommentRepository _commentRepository;
        private readonly IAgriFieldLikeRepository _likeRepository;
        private readonly IAgriFieldFollowRepository _followRepository;
        private readonly IFarmerProfileRepository _farmerProfileRepository;
        private readonly ILogger<AgriFieldService> _logger;

        public AgriFieldService(
            IAgriFieldRepository agriFieldRepository,
            IAgriFieldImageRepository imageRepository,
            IAgriFieldVideoRepository videoRepository,
            IAgriFieldCommentRepository commentRepository,
            IAgriFieldLikeRepository likeRepository,
            IAgriFieldFollowRepository followRepository,
            IFarmerProfileRepository farmerProfileRepository,
            ILogger<AgriFieldService> logger)
        {
            _agriFieldRepository = agriFieldRepository;
            _imageRepository = imageRepository;
            _videoRepository = videoRepository;
            _commentRepository = commentRepository;
            _likeRepository = likeRepository;
            _followRepository = followRepository;
            _farmerProfileRepository = farmerProfileRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<AgriFieldResponse>> CreateAgriFieldAsync(int userId, CreateAgriFieldRequest request)
        {
            try
            {
                // Create the main AgriField record
                var agriField = new AgriField
                {
                    UserId = userId,
                    FarmName = request.FarmName,
                    DistrictId = request.DistrictId,
                    TalukaId = request.TalukaId,
                    VillageId = request.VillageId,
                    Address = request.Address,
                    CropType = request.CropType,
                    FarmingMethod = request.FarmingMethod,
                    Season = request.Season,
                    Title = request.Title,
                    Description = request.Description,
                    FarmSizeAcres = request.FarmSizeAcres,
                    SoilType = request.SoilType,
                    WaterSource = request.WaterSource,
                    PlantingDate = request.PlantingDate,
                    ExpectedHarvestDate = request.ExpectedHarvestDate,
                    Tags = request.Tags != null ? JsonSerializer.Serialize(request.Tags) : null
                };

                var agriFieldId = await _agriFieldRepository.CreateAgriFieldAsync(agriField);

                // Add images if provided
                if (request.Images != null && request.Images.Any())
                {
                    foreach (var imageRequest in request.Images)
                    {
                        var image = new AgriFieldImage
                        {
                            AgriFieldId = agriFieldId,
                            ImageUrl = imageRequest.ImageUrl,
                            Caption = imageRequest.Caption,
                            ImageType = imageRequest.ImageType,
                            IsMain = imageRequest.IsMain,
                            SortOrder = imageRequest.SortOrder
                        };

                        await _imageRepository.AddAgriFieldImageAsync(image);
                    }
                }

                // Add videos if provided
                if (request.Videos != null && request.Videos.Any())
                {
                    foreach (var videoRequest in request.Videos)
                    {
                        var video = new AgriFieldVideo
                        {
                            AgriFieldId = agriFieldId,
                            VideoUrl = videoRequest.VideoUrl,
                            ThumbnailUrl = videoRequest.ThumbnailUrl,
                            Caption = videoRequest.Caption,
                            VideoType = videoRequest.VideoType,
                            SortOrder = videoRequest.SortOrder,
                            DurationSeconds = videoRequest.DurationSeconds
                        };

                        await _videoRepository.AddAgriFieldVideoAsync(video);
                    }
                }

                // Update farmer profile stats
                await _farmerProfileRepository.UpdateFarmerStatsAsync(userId, "TotalPosts", 1);

                // Get the created agri field with all details
                var createdAgriField = await _agriFieldRepository.GetAgriFieldByIdAsync(agriFieldId, userId);
                var response = MapToAgriFieldResponse(createdAgriField);

                return ApiResponse<AgriFieldResponse>.SuccessResponse(response, "Farm post created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating agri field for user {UserId}", userId);
                return ApiResponse<AgriFieldResponse>.ErrorResponse("An error occurred while creating the farm post",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<AgriFieldResponse>> GetAgriFieldByIdAsync(int agriFieldId, int? currentUserId = null)
        {
            try
            {
                var agriField = await _agriFieldRepository.GetAgriFieldByIdAsync(agriFieldId, currentUserId);

                if (agriField == null)
                {
                    return ApiResponse<AgriFieldResponse>.ErrorResponse("Farm post not found");
                }

                // Increment view count
                await _agriFieldRepository.IncrementViewCountAsync(agriFieldId);

                // Get comments
                var comments = await _commentRepository.GetCommentsAsync(agriFieldId, 1, 10);

                var response = MapToAgriFieldResponse(agriField);
                response.Comments = comments.Select(MapToCommentResponse).ToList();

                return ApiResponse<AgriFieldResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting agri field {AgriFieldId}", agriFieldId);
                return ApiResponse<AgriFieldResponse>.ErrorResponse("An error occurred while fetching the farm post",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<PagedResponse<AgriFieldResponse>>> GetAgriFieldsAsync(AgriFieldSearchRequest request, int? currentUserId = null)
        {
            try
            {
                var (agriFields, totalCount) = await _agriFieldRepository.GetAgriFieldsAsync(request, currentUserId);

                var responses = agriFields.Select(MapToAgriFieldResponse).ToList();

                var pagedResponse = new PagedResponse<AgriFieldResponse>
                {
                    Data = responses,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
                };

                return ApiResponse<PagedResponse<AgriFieldResponse>>.SuccessResponse(pagedResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting agri fields with search parameters");
                return ApiResponse<PagedResponse<AgriFieldResponse>>.ErrorResponse("An error occurred while fetching farm posts",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<PagedResponse<AgriFieldResponse>>> GetUserAgriFieldsAsync(int userId, int pageNumber = 1, int pageSize = 20, string? status = null)
        {
            try
            {
                var (agriFields, totalCount) = await _agriFieldRepository.GetUserAgriFieldsAsync(userId, pageNumber, pageSize, status);

                var responses = agriFields.Select(MapToAgriFieldResponse).ToList();

                var pagedResponse = new PagedResponse<AgriFieldResponse>
                {
                    Data = responses,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return ApiResponse<PagedResponse<AgriFieldResponse>>.SuccessResponse(pagedResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user agri fields for user {UserId}", userId);
                return ApiResponse<PagedResponse<AgriFieldResponse>>.ErrorResponse("An error occurred while fetching user's farm posts",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<AgriFieldResponse>> UpdateAgriFieldAsync(int agriFieldId, int userId, UpdateAgriFieldRequest request)
        {
            try
            {
                var existingAgriField = await _agriFieldRepository.GetAgriFieldByIdAsync(agriFieldId);

                if (existingAgriField == null || existingAgriField.UserId != userId)
                {
                    return ApiResponse<AgriFieldResponse>.ErrorResponse("Farm post not found or unauthorized");
                }

                // Update only provided fields
                if (request.FarmName != null) existingAgriField.FarmName = request.FarmName;
                if (request.Title != null) existingAgriField.Title = request.Title;
                if (request.Description != null) existingAgriField.Description = request.Description;
                if (request.DistrictId.HasValue) existingAgriField.DistrictId = request.DistrictId.Value;
                if (request.TalukaId.HasValue) existingAgriField.TalukaId = request.TalukaId.Value;
                if (request.VillageId.HasValue) existingAgriField.VillageId = request.VillageId.Value;
                if (request.Address != null) existingAgriField.Address = request.Address;
                if (request.CropType != null) existingAgriField.CropType = request.CropType;
                if (request.FarmingMethod != null) existingAgriField.FarmingMethod = request.FarmingMethod;
                if (request.Season != null) existingAgriField.Season = request.Season;
                if (request.FarmSizeAcres.HasValue) existingAgriField.FarmSizeAcres = request.FarmSizeAcres;
                if (request.SoilType != null) existingAgriField.SoilType = request.SoilType;
                if (request.WaterSource != null) existingAgriField.WaterSource = request.WaterSource;
                if (request.PlantingDate.HasValue) existingAgriField.PlantingDate = request.PlantingDate;
                if (request.ExpectedHarvestDate.HasValue) existingAgriField.ExpectedHarvestDate = request.ExpectedHarvestDate;
                if (request.Tags != null) existingAgriField.Tags = JsonSerializer.Serialize(request.Tags);

                var success = await _agriFieldRepository.UpdateAgriFieldAsync(agriFieldId, existingAgriField);

                if (!success)
                {
                    return ApiResponse<AgriFieldResponse>.ErrorResponse("Failed to update farm post");
                }

                var updatedAgriField = await _agriFieldRepository.GetAgriFieldByIdAsync(agriFieldId, userId);
                var response = MapToAgriFieldResponse(updatedAgriField);

                return ApiResponse<AgriFieldResponse>.SuccessResponse(response, "Farm post updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating agri field {AgriFieldId} for user {UserId}", agriFieldId, userId);
                return ApiResponse<AgriFieldResponse>.ErrorResponse("An error occurred while updating the farm post",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> DeleteAgriFieldAsync(int agriFieldId, int userId)
        {
            try
            {
                var success = await _agriFieldRepository.DeleteAgriFieldAsync(agriFieldId, userId);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to delete farm post or unauthorized");
                }

                // Update farmer profile stats
                await _farmerProfileRepository.UpdateFarmerStatsAsync(userId, "TotalPosts", -1);

                return ApiResponse<bool>.SuccessResponse(true, "Farm post deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting agri field {AgriFieldId} for user {UserId}", agriFieldId, userId);
                return ApiResponse<bool>.ErrorResponse("An error occurred while deleting the farm post",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> LikeAgriFieldAsync(int agriFieldId, int userId, string? reactionType = "LIKE")
        {
            try
            {
                var success = await _likeRepository.LikeAgriFieldAsync(agriFieldId, userId, reactionType);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to like farm post or already liked");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Farm post liked successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error liking agri field {AgriFieldId} for user {UserId}", agriFieldId, userId);
                return ApiResponse<bool>.ErrorResponse("An error occurred while liking the farm post",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> UnlikeAgriFieldAsync(int agriFieldId, int userId)
        {
            try
            {
                var success = await _likeRepository.UnlikeAgriFieldAsync(agriFieldId, userId);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to unlike farm post");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Farm post unliked successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unliking agri field {AgriFieldId} for user {UserId}", agriFieldId, userId);
                return ApiResponse<bool>.ErrorResponse("An error occurred while unliking the farm post",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> FollowAgriFieldAsync(int agriFieldId, int userId)
        {
            try
            {
                var success = await _followRepository.FollowAgriFieldAsync(agriFieldId, userId);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to follow farm or already following");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Farm followed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error following agri field {AgriFieldId} for user {UserId}", agriFieldId, userId);
                return ApiResponse<bool>.ErrorResponse("An error occurred while following the farm",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> UnfollowAgriFieldAsync(int agriFieldId, int userId)
        {
            try
            {
                var success = await _followRepository.UnfollowAgriFieldAsync(agriFieldId, userId);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to unfollow farm");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Farm unfollowed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unfollowing agri field {AgriFieldId} for user {UserId}", agriFieldId, userId);
                return ApiResponse<bool>.ErrorResponse("An error occurred while unfollowing the farm",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<int>> AddCommentAsync(int agriFieldId, int userId, CreateAgriFieldCommentRequest request)
        {
            try
            {
                var comment = new AgriFieldComment
                {
                    AgriFieldId = agriFieldId,
                    UserId = userId,
                    ParentCommentId = request.ParentCommentId,
                    CommentText = request.CommentText,
                    CommentType = request.CommentType
                };

                var commentId = await _commentRepository.AddCommentAsync(comment);

                return ApiResponse<int>.SuccessResponse(commentId, "Comment added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding comment to agri field {AgriFieldId} for user {UserId}", agriFieldId, userId);
                return ApiResponse<int>.ErrorResponse("An error occurred while adding the comment",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<AgriFieldCommentResponse>>> GetCommentsAsync(int agriFieldId, int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                var comments = await _commentRepository.GetCommentsAsync(agriFieldId, pageNumber, pageSize);
                var responses = comments.Select(MapToCommentResponse).ToList();

                return ApiResponse<List<AgriFieldCommentResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments for agri field {AgriFieldId}", agriFieldId);
                return ApiResponse<List<AgriFieldCommentResponse>>.ErrorResponse("An error occurred while fetching comments",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> UpdateCommentAsync(int commentId, int userId, string commentText)
        {
            try
            {
                var success = await _commentRepository.UpdateCommentAsync(commentId, userId, commentText);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to update comment or unauthorized");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Comment updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating comment {CommentId} for user {UserId}", commentId, userId);
                return ApiResponse<bool>.ErrorResponse("An error occurred while updating the comment",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> DeleteCommentAsync(int commentId, int userId)
        {
            try
            {
                var success = await _commentRepository.DeleteCommentAsync(commentId, userId);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to delete comment or unauthorized");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Comment deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment {CommentId} for user {UserId}", commentId, userId);
                return ApiResponse<bool>.ErrorResponse("An error occurred while deleting the comment",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<AgriFieldStatsResponse>> GetDashboardStatsAsync()
        {
            try
            {
                // This would require additional repository methods for stats
                // For now, return placeholder data
                var stats = new AgriFieldStatsResponse
                {
                    TotalFarmPosts = 156,
                    ActiveFarmers = 89,
                    TotalComments = 324,
                    TotalDistrictsActive = 15,
                    TrendingTopics = new List<TrendingTopicResponse>
                    {
                        new() { Tag = "OrganicFarming", PostCount = 45 },
                        new() { Tag = "RabiSeason2024", PostCount = 32 },
                        new() { Tag = "PestControl", PostCount = 28 },
                        new() { Tag = "AhmedabadFarms", PostCount = 21 }
                    }
                };

                return ApiResponse<AgriFieldStatsResponse>.SuccessResponse(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard stats");
                return ApiResponse<AgriFieldStatsResponse>.ErrorResponse("An error occurred while fetching dashboard stats",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<AgriFieldResponse>>> GetFeaturedAgriFieldsAsync(int limit = 10)
        {
            try
            {
                var agriFields = await _agriFieldRepository.GetFeaturedAgriFieldsAsync(limit);
                var responses = agriFields.Select(MapToAgriFieldResponse).ToList();

                return ApiResponse<List<AgriFieldResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured agri fields");
                return ApiResponse<List<AgriFieldResponse>>.ErrorResponse("An error occurred while fetching featured farm posts",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<AgriFieldResponse>>> GetNearbyAgriFieldsAsync(int districtId, int? talukaId = null, int limit = 20)
        {
            try
            {
                var agriFields = await _agriFieldRepository.GetNearbyAgriFieldsAsync(districtId, talukaId, limit);
                var responses = agriFields.Select(MapToAgriFieldResponse).ToList();

                return ApiResponse<List<AgriFieldResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting nearby agri fields for district {DistrictId}", districtId);
                return ApiResponse<List<AgriFieldResponse>>.ErrorResponse("An error occurred while fetching nearby farm posts",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<AgriFieldResponse>>> GetTrendingAgriFieldsAsync(int days = 7, int limit = 20)
        {
            try
            {
                var agriFields = await _agriFieldRepository.GetTrendingAgriFieldsAsync(days, limit);
                var responses = agriFields.Select(MapToAgriFieldResponse).ToList();

                return ApiResponse<List<AgriFieldResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting trending agri fields");
                return ApiResponse<List<AgriFieldResponse>>.ErrorResponse("An error occurred while fetching trending farm posts",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<AgriFieldResponse>>> GetFollowedAgriFieldsAsync(int userId, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var agriFields = await _followRepository.GetFollowedAgriFieldsAsync(userId, pageNumber, pageSize);
                var responses = agriFields.Select(MapToAgriFieldResponse).ToList();

                return ApiResponse<List<AgriFieldResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting followed agri fields for user {UserId}", userId);
                return ApiResponse<List<AgriFieldResponse>>.ErrorResponse("An error occurred while fetching followed farm posts",
                    new List<string> { ex.Message });
            }
        }

        #region Private Helper Methods

        private AgriFieldResponse MapToAgriFieldResponse(AgriField agriField)
        {
            if (agriField == null) return null;

            var response = new AgriFieldResponse
            {
                AgriFieldId = agriField.AgriFieldId,
                UserId = agriField.UserId,
                FarmName = agriField.FarmName,
                DistrictId = agriField.DistrictId,
                TalukaId = agriField.TalukaId,
                VillageId = agriField.VillageId,
                Address = agriField.Address,
                CropType = agriField.CropType,
                FarmingMethod = agriField.FarmingMethod,
                Season = agriField.Season,
                Title = agriField.Title,
                Description = agriField.Description,
                FarmSizeAcres = agriField.FarmSizeAcres,
                SoilType = agriField.SoilType,
                WaterSource = agriField.WaterSource,
                PlantingDate = agriField.PlantingDate,
                ExpectedHarvestDate = agriField.ExpectedHarvestDate,
                IsFeatured = agriField.IsFeatured,
                CreatedAt = agriField.CreatedAt,
                UpdatedAt = agriField.UpdatedAt,
                ViewCount = agriField.ViewCount,
                LikeCount = agriField.LikeCount,
                CommentCount = agriField.CommentCount,
                FollowerCount = agriField.FollowerCount,
                FarmerName = agriField.FarmerName,
                FarmerMobile = agriField.FarmerMobile,
                FarmerProfileImage = agriField.FarmerProfileImage,
                FarmerVerified = agriField.FarmerVerified,
                DistrictName = agriField.DistrictName,
                TalukaName = agriField.TalukaName,
                VillageName = agriField.VillageName,
                IsLiked = agriField.IsLiked,
                IsFollowed = agriField.IsFollowed
            };

            // Parse tags
            if (!string.IsNullOrEmpty(agriField.Tags))
            {
                try
                {
                    response.Tags = JsonSerializer.Deserialize<List<string>>(agriField.Tags);
                }
                catch
                {
                    response.Tags = new List<string>();
                }
            }

            // Map images
            if (agriField.Images != null)
            {
                response.Images = agriField.Images.Select(img => new AgriFieldImageResponse
                {
                    AgriImageId = img.AgriImageId,
                    ImageUrl = img.ImageUrl,
                    Caption = img.Caption,
                    ImageType = img.ImageType,
                    IsMain = img.IsMain,
                    SortOrder = img.SortOrder,
                    CreatedAt = img.CreatedAt
                }).ToList();
            }

            // Map videos
            if (agriField.Videos != null)
            {
                response.Videos = agriField.Videos.Select(vid => new AgriFieldVideoResponse
                {
                    AgriVideoId = vid.AgriVideoId,
                    VideoUrl = vid.VideoUrl,
                    ThumbnailUrl = vid.ThumbnailUrl,
                    Caption = vid.Caption,
                    VideoType = vid.VideoType,
                    SortOrder = vid.SortOrder,
                    DurationSeconds = vid.DurationSeconds,
                    CreatedAt = vid.CreatedAt
                }).ToList();
            }

            return response;
        }

        private AgriFieldCommentResponse MapToCommentResponse(AgriFieldComment comment)
        {
            if (comment == null) return null;

            var response = new AgriFieldCommentResponse
            {
                CommentId = comment.CommentId,
                UserId = comment.UserId,
                ParentCommentId = comment.ParentCommentId,
                CommentText = comment.CommentText,
                CommentType = comment.CommentType,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                UserName = comment.UserName,
                UserProfileImage = comment.UserProfileImage,
                UserVerified = comment.UserVerified
            };

            if (comment.Replies != null)
            {
                response.Replies = comment.Replies.Select(MapToCommentResponse).ToList();
            }

            return response;
        }

        #endregion
    }
}