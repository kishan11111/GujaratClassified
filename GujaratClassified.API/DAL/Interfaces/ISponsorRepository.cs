using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface ISponsorRepository
    {
        Task<Sponsor> GetActiveSponsorForBannerAsync(int? lastSponsorId = null);
        Task<List<Sponsor>> GetActiveSponsorsForCardsAsync(int count = 5);
        Task<bool> TrackSponsorViewAsync(int sponsorId, int? userId, string deviceId, string ipAddress);
        Task<bool> TrackSponsorClickAsync(int sponsorId, int? userId, string deviceId, string ipAddress);
        Task<Sponsor> GetSponsorByIdAsync(int sponsorId);
        Task<List<Sponsor>> GetAllActiveSponsorsAsync();
    }

    public interface ISystemParameterRepository
    {
        Task<SystemParameter> GetParameterByNameAsync(string parameterName);
        Task<List<SystemParameter>> GetAllActiveParametersAsync();
        Task<bool> UpdateParameterValueAsync(string parameterName, string parameterValue);
    }
}
