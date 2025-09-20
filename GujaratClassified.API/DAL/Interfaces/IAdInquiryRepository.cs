using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IAdInquiryRepository
    {
        Task<int> CreateAdInquiryAsync(AdInquiry inquiry);
        Task<AdInquiry?> GetAdInquiryByIdAsync(int inquiryId);
        Task<(List<AdInquiry> Inquiries, int TotalCount)> GetAdInquiriesAsync(int? adId = null, string? status = null,
            int pageSize = 20, int pageNumber = 1);
        Task<bool> UpdateInquiryStatusAsync(int inquiryId, string status);
        Task<List<AdInquiry>> GetUserInquiriesAsync(int userId, int pageSize = 20, int pageNumber = 1);
    }
}