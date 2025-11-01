using GujaratClassified.API.Models.Entity;
using GujaratClassified.API.Models.Request;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface ILocalCardRepository
    {
        // CRUD Operations
        Task<int> CreateLocalCardAsync(LocalCard card);
        Task<bool> UpdateLocalCardAsync(LocalCard card);
        Task<bool> DeleteLocalCardAsync(int cardId, int userId);
        Task<LocalCard?> GetLocalCardByIdAsync(int cardId);

        // Image Operations
        Task<bool> AddCardImagesAsync(int cardId, List<string> imageUrls);
        Task<List<LocalCardImage>> GetCardImagesAsync(int cardId);
        Task<bool> DeleteCardImagesAsync(int cardId);

        // User Cards
        Task<(List<LocalCard> Cards, int TotalCount)> GetUserCardsAsync(int userId, int pageNumber, int pageSize);

        // Browse & Search
        Task<(List<LocalCard> Cards, int TotalCount)> BrowseCardsAsync(LocalCardSearchRequest request);
        Task<(List<LocalCard> Cards, int TotalCount)> SearchCardsAsync(string searchTerm, int pageNumber, int pageSize);

        // Nearby Cards (Location-based)
        Task<(List<LocalCard> Cards, int TotalCount)> GetNearbyCardsAsync(
            decimal latitude,
            decimal longitude,
            decimal radiusKm,
            int? categoryId,
            int? subCategoryId,
            int pageNumber,
            int pageSize);

        // Validation
        Task<bool> IsCardOwnerAsync(int cardId, int userId);
        Task<bool> CardExistsAsync(int cardId);
    }
}