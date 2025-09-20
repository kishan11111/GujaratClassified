// Services/Implementations/FarmerProfileService.cs
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Common;
using GujaratClassified.API.Models.Entity;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;
using GujaratClassified.API.Services.Interfaces;
using System.Text.Json;

namespace GujaratClassified.API.Services.Implementations
{
    public class FarmerProfileService : IFarmerProfileService
    {
        private readonly IFarmerProfileRepository _farmerProfileRepository;
        private readonly IAgriFieldRepository _agriFieldRepository;
        private readonly ILogger<FarmerProfileService> _logger;

        public FarmerProfileService(
            IFarmerProfileRepository farmerProfileRepository,
            IAgriFieldRepository agriFieldRepository,
            ILogger<FarmerProfileService> logger)
        {
            _farmerProfileRepository = farmerProfileRepository;
            _agriFieldRepository = agriFieldRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<FarmerProfileResponse>> CreateFarmerProfileAsync(int userId, CreateFarmerProfileRequest request)
        {
            try
            {
                // Check if profile already exists
                var existingProfile = await _farmerProfileRepository.GetFarmerProfileByUserIdAsync(userId);
                if (existingProfile != null)
                {
                    return ApiResponse<FarmerProfileResponse>.ErrorResponse("Farmer profile already exists for this user");
                }

                var profile = new FarmerProfile
                {
                    UserId = userId,
                    FarmName = request.FarmName,
                    TotalFarmArea = request.TotalFarmArea,
                    MainCrops = request.MainCrops != null ? JsonSerializer.Serialize(request.MainCrops) : null,
                    FarmingExperience = request.FarmingExperience,
                    SpecialtyAreas = request.SpecialtyAreas != null ? JsonSerializer.Serialize(request.SpecialtyAreas) : null,
                    Bio = request.Bio,
                    Achievements = request.Achievements != null ? JsonSerializer.Serialize(request.Achievements) : null
                };

                var profileId = await _farmerProfileRepository.CreateFarmerProfileAsync(profile);

                var createdProfile = await _farmerProfileRepository.GetFarmerProfileByUserIdAsync(userId);
                var response = MapToFarmerProfileResponse(createdProfile);

                return ApiResponse<FarmerProfileResponse>.SuccessResponse(response, "Farmer profile created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating farmer profile for user {UserId}", userId);
                return ApiResponse<FarmerProfileResponse>.ErrorResponse("An error occurred while creating the farmer profile",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<FarmerProfileResponse>> GetFarmerProfileAsync(int userId)
        {
            try
            {
                var profile = await _farmerProfileRepository.GetFarmerProfileByUserIdAsync(userId);

                if (profile == null)
                {
                    return ApiResponse<FarmerProfileResponse>.ErrorResponse("Farmer profile not found");
                }

                // Get recent posts for this farmer
                var (recentPosts, _) = await _agriFieldRepository.GetUserAgriFieldsAsync(userId, 1, 5, "ACTIVE");

                var response = MapToFarmerProfileResponse(profile);
                response.RecentPosts = recentPosts.Select(MapToAgriFieldResponse).ToList();

                return ApiResponse<FarmerProfileResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting farmer profile for user {UserId}", userId);
                return ApiResponse<FarmerProfileResponse>.ErrorResponse("An error occurred while fetching the farmer profile",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<FarmerProfileResponse>> UpdateFarmerProfileAsync(int userId, CreateFarmerProfileRequest request)
        {
            try
            {
                var existingProfile = await _farmerProfileRepository.GetFarmerProfileByUserIdAsync(userId);

                if (existingProfile == null)
                {
                    return ApiResponse<FarmerProfileResponse>.ErrorResponse("Farmer profile not found");
                }

                // Update the profile
                existingProfile.FarmName = request.FarmName ?? existingProfile.FarmName;
                existingProfile.TotalFarmArea = request.TotalFarmArea ?? existingProfile.TotalFarmArea;
                existingProfile.MainCrops = request.MainCrops != null ? JsonSerializer.Serialize(request.MainCrops) : existingProfile.MainCrops;
                existingProfile.FarmingExperience = request.FarmingExperience ?? existingProfile.FarmingExperience;
                existingProfile.SpecialtyAreas = request.SpecialtyAreas != null ? JsonSerializer.Serialize(request.SpecialtyAreas) : existingProfile.SpecialtyAreas;
                existingProfile.Bio = request.Bio ?? existingProfile.Bio;
                existingProfile.Achievements = request.Achievements != null ? JsonSerializer.Serialize(request.Achievements) : existingProfile.Achievements;
                existingProfile.UpdatedAt = DateTime.UtcNow;

                var success = await _farmerProfileRepository.UpdateFarmerProfileAsync(existingProfile);

                if (!success)
                {
                    return ApiResponse<FarmerProfileResponse>.ErrorResponse("Failed to update farmer profile");
                }

                var updatedProfile = await _farmerProfileRepository.GetFarmerProfileByUserIdAsync(userId);
                var response = MapToFarmerProfileResponse(updatedProfile);

                return ApiResponse<FarmerProfileResponse>.SuccessResponse(response, "Farmer profile updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating farmer profile for user {UserId}", userId);
                return ApiResponse<FarmerProfileResponse>.ErrorResponse("An error occurred while updating the farmer profile",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<TopFarmerResponse>>> GetTopFarmersAsync(int limit = 10, string orderBy = "TotalLikes")
        {
            try
            {
                var farmers = await _farmerProfileRepository.GetTopFarmersAsync(limit, orderBy);

                var responses = farmers.Select(farmer => new TopFarmerResponse
                {
                    UserId = farmer.UserId,
                    FarmerName = farmer.UserName,
                    ProfileImage = farmer.UserProfileImage,
                    IsVerified = farmer.IsVerifiedFarmer,
                    TotalPosts = farmer.TotalPosts,
                    TotalLikes = farmer.TotalLikes,
                    TotalFollowers = farmer.TotalFollowers,
                    DistrictName = farmer.DistrictName
                }).ToList();

                return ApiResponse<List<TopFarmerResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top farmers");
                return ApiResponse<List<TopFarmerResponse>>.ErrorResponse("An error occurred while fetching top farmers",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> SetVerifiedFarmerAsync(int userId, bool isVerified)
        {
            try
            {
                var success = await _farmerProfileRepository.SetVerifiedFarmerAsync(userId, isVerified);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to update farmer verification status or profile not found");
                }

                var message = isVerified ? "Farmer verified successfully" : "Farmer verification removed successfully";
                return ApiResponse<bool>.SuccessResponse(true, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting farmer verification for user {UserId}", userId);
                return ApiResponse<bool>.ErrorResponse("An error occurred while updating farmer verification status",
                    new List<string> { ex.Message });
            }
        }

        #region Private Helper Methods

        private FarmerProfileResponse MapToFarmerProfileResponse(FarmerProfile profile)
        {
            if (profile == null) return null;

            var response = new FarmerProfileResponse
            {
                FarmerProfileId = profile.FarmerProfileId,
                UserId = profile.UserId,
                FarmName = profile.FarmName,
                TotalFarmArea = profile.TotalFarmArea,
                FarmingExperience = profile.FarmingExperience,
                Bio = profile.Bio,
                IsVerifiedFarmer = profile.IsVerifiedFarmer,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt,
                TotalPosts = profile.TotalPosts,
                TotalFollowers = profile.TotalFollowers,
                TotalLikes = profile.TotalLikes,
                HelpfulAnswers = profile.HelpfulAnswers,
                UserName = profile.UserName,
                UserProfileImage = profile.UserProfileImage,
                DistrictName = profile.DistrictName,
                TalukaName = profile.TalukaName
            };

            // Parse JSON fields
            if (!string.IsNullOrEmpty(profile.MainCrops))
            {
                try
                {
                    response.MainCrops = JsonSerializer.Deserialize<List<string>>(profile.MainCrops);
                }
                catch
                {
                    response.MainCrops = new List<string>();
                }
            }

            if (!string.IsNullOrEmpty(profile.SpecialtyAreas))
            {
                try
                {
                    response.SpecialtyAreas = JsonSerializer.Deserialize<List<string>>(profile.SpecialtyAreas);
                }
                catch
                {
                    response.SpecialtyAreas = new List<string>();
                }
            }

            if (!string.IsNullOrEmpty(profile.Achievements))
            {
                try
                {
                    response.Achievements = JsonSerializer.Deserialize<List<string>>(profile.Achievements);
                }
                catch
                {
                    response.Achievements = new List<string>();
                }
            }

            return response;
        }

        private AgriFieldResponse MapToAgriFieldResponse(AgriField agriField)
        {
            if (agriField == null) return null;

            return new AgriFieldResponse
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
        }

        #endregion
    }
}