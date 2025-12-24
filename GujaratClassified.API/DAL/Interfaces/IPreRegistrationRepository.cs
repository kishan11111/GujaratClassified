using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IPreRegistrationRepository
    {
        Task<bool> CreateAsync(PreRegistration preRegistration);
        Task<bool> ExistsByMobileAsync(string mobile);
        Task<IEnumerable<PreRegistration>> GetAllAsync();
    }
}
