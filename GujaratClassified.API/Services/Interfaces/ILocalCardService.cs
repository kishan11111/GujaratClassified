using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface ILocalCardService
    {
        // CRUD Operations
        Task<ApiResponse<LocalCardResponse>> CreateLocalCardAsync(int userId, CreateLocalCardRequest request);
        Task<ApiResponse<LocalCardResponse>> UpdateLocalCardAsync(int cardId, int userId, UpdateLocalCardRequest request);
        Task<ApiResponse<bool>> DeleteLocalCardAsync(int cardId, int userId);
        Task<ApiResponse<LocalCardResponse>> GetLocalCardByIdAsync(int cardId);

        // User Cards
        Task<ApiResponse<PagedResponse<LocalCardResponse>>> GetMyCardsAsync(int userId, int pageNumber, int pageSize);

        // Browse & Search
        Task<ApiResponse<PagedResponse<LocalCardResponse>>> BrowseCardsAsync(LocalCardSearchRequest request);
        Task<ApiResponse<PagedResponse<LocalCardResponse>>> SearchCardsAsync(string searchTerm, int pageNumber, int pageSize);

        // Nearby Cards
        Task<ApiResponse<PagedResponse<LocalCardResponse>>> GetNearbyCardsAsync(NearbyCardsRequest request);
    }
}