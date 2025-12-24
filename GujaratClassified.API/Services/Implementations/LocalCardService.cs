// Services/Implementations/LocalCardService.cs
using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;
using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.Services.Implementations
{
    public class LocalCardService : ILocalCardService
    {
        private readonly ILocalCardRepository _localCardRepository;
        private readonly ILocalCardCategoryRepository _categoryRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly ILogger<LocalCardService> _logger;

        public LocalCardService(
            ILocalCardRepository localCardRepository,
            ILocalCardCategoryRepository categoryRepository,
            ILocationRepository locationRepository,
            ILogger<LocalCardService> logger)
        {
            _localCardRepository = localCardRepository;
            _categoryRepository = categoryRepository;
            _locationRepository = locationRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<LocalCardResponse>> CreateLocalCardAsync(int userId, CreateLocalCardRequest request)
        {
            try
            {
                // Validate category exists
                var category = await _categoryRepository.GetCategoryByIdAsync(request.CategoryId);
                if (category == null)
                {
                    return ApiResponse<LocalCardResponse>.ErrorResponse("Invalid category");
                }

                // Validate subcategory if provided
                if (request.SubCategoryId.HasValue)
                {
                    var subCategory = await _categoryRepository.GetSubCategoryByIdAsync(request.SubCategoryId.Value);
                    if (subCategory == null || subCategory.CategoryId != request.CategoryId)
                    {
                        return ApiResponse<LocalCardResponse>.ErrorResponse("Invalid subcategory for the selected category");
                    }
                }

                // Validate location
                var district = await _locationRepository.GetDistrictByIdAsync(request.DistrictId);
                if (district == null)
                {
                    return ApiResponse<LocalCardResponse>.ErrorResponse("Invalid district");
                }

                var taluka = await _locationRepository.GetTalukaByIdAsync(request.TalukaId);
                if (taluka == null || taluka.DistrictId != request.DistrictId)
                {
                    return ApiResponse<LocalCardResponse>.ErrorResponse("Invalid taluka for the selected district");
                }

                var village = await _locationRepository.GetVillageByIdAsync(request.VillageId);
                if (village == null || village.TalukaId != request.TalukaId)
                {
                    return ApiResponse<LocalCardResponse>.ErrorResponse("Invalid village for the selected taluka");
                }

                var secondary = string.IsNullOrWhiteSpace(request.SecondaryPhone) ? null : request.SecondaryPhone;
                var whatsapp = string.IsNullOrWhiteSpace(request.WhatsAppNumber) ? null : request.WhatsAppNumber;
                var email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email;

                // Create card entity
                var card = new LocalCard
                {
                    UserId = userId,
                    BusinessName = request.BusinessName,
                    BusinessNameGujarati = request.BusinessNameGujarati,
                    BusinessDescription = request.BusinessDescription,
                    BusinessDescriptionGujarati = request.BusinessDescriptionGujarati,
                    CategoryId = request.CategoryId,
                    SubCategoryId = request.SubCategoryId,
                    ContactPersonName = request.ContactPersonName,
                    PrimaryPhone = request.PrimaryPhone,
                    SecondaryPhone = secondary,
                    WhatsAppNumber = whatsapp,
                    Email = email,
                    DistrictId = request.DistrictId,
                    TalukaId = request.TalukaId,
                    VillageId = request.VillageId,
                    FullAddress = request.FullAddress,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    WorkingHours = request.WorkingHours,
                    WorkingDays = request.WorkingDays,
                    IsOpen24Hours = request.IsOpen24Hours,
                    ProfileImage = request.ProfileImage,
                    CoverImage = request.CoverImage
                };

                // Insert card
                var cardId = await _localCardRepository.CreateLocalCardAsync(card);

                if (cardId <= 0)
                {
                    return ApiResponse<LocalCardResponse>.ErrorResponse("Failed to create local card");
                }

                // Add additional images if provided
                if (request.AdditionalImages != null && request.AdditionalImages.Any())
                {
                    await _localCardRepository.AddCardImagesAsync(cardId, request.AdditionalImages);
                }

                // Get created card with full details
                var createdCard = await _localCardRepository.GetLocalCardByIdAsync(cardId);
                var response = MapToLocalCardResponse(createdCard);

                return ApiResponse<LocalCardResponse>.SuccessResponse(response, "Local card created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating local card for user {UserId}", userId);
                return ApiResponse<LocalCardResponse>.ErrorResponse("An error occurred while creating the local card",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<LocalCardResponse>> UpdateLocalCardAsync(int cardId, int userId, UpdateLocalCardRequest request)
        {
            try
            {
                // Check if card exists and user is owner
                var existingCard = await _localCardRepository.GetLocalCardByIdAsync(cardId);
                if (existingCard == null)
                {
                    return ApiResponse<LocalCardResponse>.ErrorResponse("Local card not found");
                }

                var isOwner = await _localCardRepository.IsCardOwnerAsync(cardId, userId);
                if (!isOwner)
                {
                    return ApiResponse<LocalCardResponse>.ErrorResponse("You are not authorized to update this card");
                }

                // Validate category if provided
                if (request.CategoryId.HasValue)
                {
                    var category = await _categoryRepository.GetCategoryByIdAsync(request.CategoryId.Value);
                    if (category == null)
                    {
                        return ApiResponse<LocalCardResponse>.ErrorResponse("Invalid category");
                    }
                    existingCard.CategoryId = request.CategoryId.Value;
                }

                // Validate subcategory if provided
                if (request.SubCategoryId.HasValue)
                {
                    var subCategory = await _categoryRepository.GetSubCategoryByIdAsync(request.SubCategoryId.Value);
                    if (subCategory == null || subCategory.CategoryId != existingCard.CategoryId)
                    {
                        return ApiResponse<LocalCardResponse>.ErrorResponse("Invalid subcategory for the selected category");
                    }
                    existingCard.SubCategoryId = request.SubCategoryId.Value;
                }

                // Validate location if provided
                if (request.DistrictId.HasValue)
                {
                    var district = await _locationRepository.GetDistrictByIdAsync(request.DistrictId.Value);
                    if (district == null)
                    {
                        return ApiResponse<LocalCardResponse>.ErrorResponse("Invalid district");
                    }
                    existingCard.DistrictId = request.DistrictId.Value;
                }

                if (request.TalukaId.HasValue)
                {
                    var taluka = await _locationRepository.GetTalukaByIdAsync(request.TalukaId.Value);
                    if (taluka == null || taluka.DistrictId != existingCard.DistrictId)
                    {
                        return ApiResponse<LocalCardResponse>.ErrorResponse("Invalid taluka for the selected district");
                    }
                    existingCard.TalukaId = request.TalukaId.Value;
                }

                if (request.VillageId.HasValue)
                {
                    var village = await _locationRepository.GetVillageByIdAsync(request.VillageId.Value);
                    if (village == null || village.TalukaId != existingCard.TalukaId)
                    {
                        return ApiResponse<LocalCardResponse>.ErrorResponse("Invalid village for the selected taluka");
                    }
                    existingCard.VillageId = request.VillageId.Value;
                }

                // Update fields
                existingCard.BusinessName = request.BusinessName ?? existingCard.BusinessName;
                existingCard.BusinessNameGujarati = request.BusinessNameGujarati ?? existingCard.BusinessNameGujarati;
                existingCard.BusinessDescription = request.BusinessDescription ?? existingCard.BusinessDescription;
                existingCard.BusinessDescriptionGujarati = request.BusinessDescriptionGujarati ?? existingCard.BusinessDescriptionGujarati;
                existingCard.ContactPersonName = request.ContactPersonName ?? existingCard.ContactPersonName;
                existingCard.PrimaryPhone = request.PrimaryPhone ?? existingCard.PrimaryPhone;
                existingCard.SecondaryPhone = request.SecondaryPhone ?? existingCard.SecondaryPhone;
                existingCard.WhatsAppNumber = request.WhatsAppNumber ?? existingCard.WhatsAppNumber;
                existingCard.Email = request.Email ?? existingCard.Email;
                existingCard.FullAddress = request.FullAddress ?? existingCard.FullAddress;
                existingCard.Latitude = request.Latitude ?? existingCard.Latitude;
                existingCard.Longitude = request.Longitude ?? existingCard.Longitude;
                existingCard.WorkingHours = request.WorkingHours ?? existingCard.WorkingHours;
                existingCard.WorkingDays = request.WorkingDays ?? existingCard.WorkingDays;
                existingCard.IsOpen24Hours = request.IsOpen24Hours ?? existingCard.IsOpen24Hours;
                existingCard.ProfileImage = request.ProfileImage ?? existingCard.ProfileImage;
                existingCard.CoverImage = request.CoverImage ?? existingCard.CoverImage;
                existingCard.CardId = cardId;
                existingCard.UserId = userId;

                // Update card
                var success = await _localCardRepository.UpdateLocalCardAsync(existingCard);

                if (!success)
                {
                    return ApiResponse<LocalCardResponse>.ErrorResponse("Failed to update local card");
                }

                // Update additional images if provided
                if (request.AdditionalImages != null)
                {
                    await _localCardRepository.DeleteCardImagesAsync(cardId);
                    if (request.AdditionalImages.Any())
                    {
                        await _localCardRepository.AddCardImagesAsync(cardId, request.AdditionalImages);
                    }
                }

                // Get updated card with full details
                var updatedCard = await _localCardRepository.GetLocalCardByIdAsync(cardId);
                var response = MapToLocalCardResponse(updatedCard);

                return ApiResponse<LocalCardResponse>.SuccessResponse(response, "Local card updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating local card {CardId} for user {UserId}", cardId, userId);
                return ApiResponse<LocalCardResponse>.ErrorResponse("An error occurred while updating the local card",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> DeleteLocalCardAsync(int cardId, int userId)
        {
            try
            {
                var isOwner = await _localCardRepository.IsCardOwnerAsync(cardId, userId);
                if (!isOwner)
                {
                    return ApiResponse<bool>.ErrorResponse("You are not authorized to delete this card");
                }

                var success = await _localCardRepository.DeleteLocalCardAsync(cardId, userId);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to delete local card or card not found");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Local card deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting local card {CardId} for user {UserId}", cardId, userId);
                return ApiResponse<bool>.ErrorResponse("An error occurred while deleting the local card",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<LocalCardResponse>> GetLocalCardByIdAsync(int cardId)
        {
            try
            {
                var card = await _localCardRepository.GetLocalCardByIdAsync(cardId);

                if (card == null)
                {
                    return ApiResponse<LocalCardResponse>.ErrorResponse("Local card not found");
                }

                var response = MapToLocalCardResponse(card);

                return ApiResponse<LocalCardResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting local card {CardId}", cardId);
                return ApiResponse<LocalCardResponse>.ErrorResponse("An error occurred while fetching the local card",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<PagedResponse<LocalCardResponse>>> GetMyCardsAsync(int userId, int pageNumber, int pageSize)
        {
            try
            {
                var (cards, totalCount) = await _localCardRepository.GetUserCardsAsync(userId, pageNumber, pageSize);

                var responses = cards.Select(MapToLocalCardResponse).ToList();

                var pagedResponse = new PagedResponse<LocalCardResponse>
                {
                    Data = responses,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return ApiResponse<PagedResponse<LocalCardResponse>>.SuccessResponse(pagedResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cards for user {UserId}", userId);
                return ApiResponse<PagedResponse<LocalCardResponse>>.ErrorResponse("An error occurred while fetching your cards",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<PagedResponse<LocalCardResponse>>> BrowseCardsAsync(LocalCardSearchRequest request)
        {
            try
            {
                var (cards, totalCount) = await _localCardRepository.BrowseCardsAsync(request);

                var responses = cards.Select(MapToLocalCardResponse).ToList();

                var pagedResponse = new PagedResponse<LocalCardResponse>
                {
                    Data = responses,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
                };

                return ApiResponse<PagedResponse<LocalCardResponse>>.SuccessResponse(pagedResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error browsing local cards");
                return ApiResponse<PagedResponse<LocalCardResponse>>.ErrorResponse("An error occurred while browsing cards",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<PagedResponse<LocalCardResponse>>> SearchCardsAsync(string searchTerm, int pageNumber, int pageSize)
        {
            try
            {
                var (cards, totalCount) = await _localCardRepository.SearchCardsAsync(searchTerm, pageNumber, pageSize);

                var responses = cards.Select(MapToLocalCardResponse).ToList();

                var pagedResponse = new PagedResponse<LocalCardResponse>
                {
                    Data = responses,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return ApiResponse<PagedResponse<LocalCardResponse>>.SuccessResponse(pagedResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching local cards with term {SearchTerm}", searchTerm);
                return ApiResponse<PagedResponse<LocalCardResponse>>.ErrorResponse("An error occurred while searching cards",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<PagedResponse<LocalCardResponse>>> GetNearbyCardsAsync(NearbyCardsRequest request)
        {
            try
            {
                var (cards, totalCount) = await _localCardRepository.GetNearbyCardsAsync(
                    request.Latitude,
                    request.Longitude,
                    request.RadiusKm,
                    request.CategoryId,
                    request.SubCategoryId,
                    request.PageNumber,
                    request.PageSize);

                var responses = cards.Select(MapToLocalCardResponse).ToList();

                var pagedResponse = new PagedResponse<LocalCardResponse>
                {
                    Data = responses,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
                };

                return ApiResponse<PagedResponse<LocalCardResponse>>.SuccessResponse(pagedResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting nearby local cards");
                return ApiResponse<PagedResponse<LocalCardResponse>>.ErrorResponse("An error occurred while fetching nearby cards",
                    new List<string> { ex.Message });
            }
        }

        // Helper method to map entity to response
        private LocalCardResponse MapToLocalCardResponse(LocalCard card)
        {
            return new LocalCardResponse
            {
                CardId = card.CardId,
                UserId = card.UserId,
                UserName = card.UserName,
                UserMobile = card.UserMobile,
                BusinessName = card.BusinessName,
                BusinessNameGujarati = card.BusinessNameGujarati,
                BusinessDescription = card.BusinessDescription,
                BusinessDescriptionGujarati = card.BusinessDescriptionGujarati,
                CategoryId = card.CategoryId,
                CategoryNameGujarati = card.CategoryNameGujarati,
                CategoryNameEnglish = card.CategoryNameEnglish,
                SubCategoryId = card.SubCategoryId,
                SubCategoryNameGujarati = card.SubCategoryNameGujarati,
                SubCategoryNameEnglish = card.SubCategoryNameEnglish,
                ContactPersonName = card.ContactPersonName,
                PrimaryPhone = card.PrimaryPhone,
                SecondaryPhone = card.SecondaryPhone,
                WhatsAppNumber = card.WhatsAppNumber,
                Email = card.Email,
                DistrictId = card.DistrictId,
                DistrictNameGujarati = card.DistrictNameGujarati,
                DistrictNameEnglish = card.DistrictNameEnglish,
                TalukaId = card.TalukaId,
                TalukaNameGujarati = card.TalukaNameGujarati,
                TalukaNameEnglish = card.TalukaNameEnglish,
                VillageId = card.VillageId,
                VillageNameGujarati = card.VillageNameGujarati,
                VillageNameEnglish = card.VillageNameEnglish,
                FullAddress = card.FullAddress,
                Latitude = card.Latitude,
                Longitude = card.Longitude,
                DistanceKm = card.DistanceKm,
                WorkingHours = card.WorkingHours,
                WorkingDays = card.WorkingDays,
                IsOpen24Hours = card.IsOpen24Hours,
                ProfileImage = card.ProfileImage,
                CoverImage = card.CoverImage,
                Images = card.Images?.Select(img => new LocalCardImageResponse
                {
                    ImageId = img.ImageId,
                    ImageUrl = img.ImageUrl,
                    Caption = img.Caption,
                    SortOrder = img.SortOrder
                }).ToList(),
                IsActive = card.IsActive,
                IsVerified = card.IsVerified,
                CreatedAt = card.CreatedAt,
                UpdatedAt = card.UpdatedAt
            };
        }
    }
}