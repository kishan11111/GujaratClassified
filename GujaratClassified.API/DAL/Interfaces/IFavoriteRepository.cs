namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IFavoriteRepository
    {
        Task<string> ToggleFavoriteAsync(int userId, int postId);
        Task<bool> IsFavoriteAsync(int userId, int postId);
    }
}
