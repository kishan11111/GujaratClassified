using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using Microsoft.Extensions.Logging;

namespace GujaratClassified.API.DAL.Repositories
{
    public class SponsorRepository : ISponsorRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<SponsorRepository> _logger;

        public SponsorRepository(IDbConnectionFactory connectionFactory, ILogger<SponsorRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<Sponsor> GetActiveSponsorForBannerAsync(int? lastSponsorId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                
                var sql = @"
                    WITH ActiveSponsors AS (
                        SELECT 
                            SponsorId,
                            SponsorName,
                            CompanyName,
                            BannerImagePath,
                            CardImagePath,
                            ClickUrl,
                            WhatsappNumber,
                            StartDate,
                            EndDate,
                            DisplayOrder,
                            IsActive,
                            TotalViews,
                            TotalClicks,
                            ROW_NUMBER() OVER (ORDER BY DisplayOrder, SponsorId) AS RowNum
                        FROM Sponsors
                        WHERE IsActive = 1 
                            AND GETDATE() BETWEEN StartDate AND EndDate
                            AND BannerImagePath IS NOT NULL
                    )
                    SELECT TOP 1 * FROM ActiveSponsors
                    WHERE (@LastSponsorId IS NULL OR SponsorId != @LastSponsorId)
                    ORDER BY 
                        CASE WHEN @LastSponsorId IS NULL THEN 0 
                             ELSE (SELECT RowNum FROM ActiveSponsors WHERE SponsorId = @LastSponsorId) 
                        END,
                        RowNum";

                var sponsor = await connection.QueryFirstOrDefaultAsync<Sponsor>(sql, new { LastSponsorId = lastSponsorId });
                
                // If no different sponsor found, get the first one
                if (sponsor == null && lastSponsorId.HasValue)
                {
                    sql = @"
                        SELECT TOP 1 
                            SponsorId,
                            SponsorName,
                            CompanyName,
                            BannerImagePath,
                            CardImagePath,
                            ClickUrl,
                            WhatsappNumber,
                            StartDate,
                            EndDate,
                            DisplayOrder,
                            IsActive,
                            TotalViews,
                            TotalClicks
                        FROM Sponsors
                        WHERE IsActive = 1 
                            AND GETDATE() BETWEEN StartDate AND EndDate
                            AND BannerImagePath IS NOT NULL
                        ORDER BY DisplayOrder, SponsorId";
                    
                    sponsor = await connection.QueryFirstOrDefaultAsync<Sponsor>(sql);
                }

                return sponsor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active sponsor for banner");
                return null;
            }
        }

        public async Task<List<Sponsor>> GetActiveSponsorsForCardsAsync(int count = 5)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                
                var sql = @"
                    SELECT TOP (@Count)
                        SponsorId,
                        SponsorName,
                        CompanyName,
                        BannerImagePath,
                        CardImagePath,
                        ClickUrl,
                        WhatsappNumber,
                        StartDate,
                        EndDate,
                        DisplayOrder,
                        IsActive
                    FROM Sponsors
                    WHERE IsActive = 1 
                        AND GETDATE() BETWEEN StartDate AND EndDate
                        AND CardImagePath IS NOT NULL
                    ORDER BY DisplayOrder, NEWID()"; // Random order for fairness

                var sponsors = await connection.QueryAsync<Sponsor>(sql, new { Count = count });
                return sponsors.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active sponsors for cards");
                return new List<Sponsor>();
            }
        }

        public async Task<bool> TrackSponsorViewAsync(int sponsorId, int? userId, string deviceId, string ipAddress)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                
                var sql = @"
                    INSERT INTO SponsorTracking (SponsorId, UserId, DeviceId, TrackingType, TrackingDate, IpAddress)
                    VALUES (@SponsorId, @UserId, @DeviceId, 'VIEW', GETDATE(), @IpAddress);
                    
                    UPDATE Sponsors 
                    SET TotalViews = TotalViews + 1
                    WHERE SponsorId = @SponsorId;";

                await connection.ExecuteAsync(sql, new 
                { 
                    SponsorId = sponsorId,
                    UserId = userId,
                    DeviceId = deviceId,
                    IpAddress = ipAddress
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking sponsor view for SponsorId: {SponsorId}", sponsorId);
                return false;
            }
        }

        public async Task<bool> TrackSponsorClickAsync(int sponsorId, int? userId, string deviceId, string ipAddress)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                
                var sql = @"
                    INSERT INTO SponsorTracking (SponsorId, UserId, DeviceId, TrackingType, TrackingDate, IpAddress)
                    VALUES (@SponsorId, @UserId, @DeviceId, 'CLICK', GETDATE(), @IpAddress);
                    
                    UPDATE Sponsors 
                    SET TotalClicks = TotalClicks + 1
                    WHERE SponsorId = @SponsorId;";

                await connection.ExecuteAsync(sql, new 
                { 
                    SponsorId = sponsorId,
                    UserId = userId,
                    DeviceId = deviceId,
                    IpAddress = ipAddress
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking sponsor click for SponsorId: {SponsorId}", sponsorId);
                return false;
            }
        }

        public async Task<Sponsor> GetSponsorByIdAsync(int sponsorId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                
                var sql = @"
                    SELECT * FROM Sponsors
                    WHERE SponsorId = @SponsorId";

                return await connection.QueryFirstOrDefaultAsync<Sponsor>(sql, new { SponsorId = sponsorId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sponsor by id: {SponsorId}", sponsorId);
                return null;
            }
        }

        public async Task<List<Sponsor>> GetAllActiveSponsorsAsync()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                
                var sql = @"
                    SELECT * FROM Sponsors
                    WHERE IsActive = 1 
                        AND GETDATE() BETWEEN StartDate AND EndDate
                    ORDER BY DisplayOrder, SponsorId";

                var sponsors = await connection.QueryAsync<Sponsor>(sql);
                return sponsors.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all active sponsors");
                return new List<Sponsor>();
            }
        }
    }

    public class SystemParameterRepository : ISystemParameterRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<SystemParameterRepository> _logger;

        public SystemParameterRepository(IDbConnectionFactory connectionFactory, ILogger<SystemParameterRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<SystemParameter> GetParameterByNameAsync(string parameterName)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                
                var sql = @"
                    SELECT * FROM SystemParameters
                    WHERE ParameterName = @ParameterName
                        AND IsActive = 1";

                return await connection.QueryFirstOrDefaultAsync<SystemParameter>(sql, new { ParameterName = parameterName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting parameter: {ParameterName}", parameterName);
                return null;
            }
        }

        public async Task<List<SystemParameter>> GetAllActiveParametersAsync()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                
                var sql = @"
                    SELECT * FROM SystemParameters
                    WHERE IsActive = 1
                    ORDER BY ParameterName";

                var parameters = await connection.QueryAsync<SystemParameter>(sql);
                return parameters.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all active parameters");
                return new List<SystemParameter>();
            }
        }

        public async Task<bool> UpdateParameterValueAsync(string parameterName, string parameterValue)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                
                var sql = @"
                    UPDATE SystemParameters
                    SET ParameterValue = @ParameterValue,
                        ModifiedDate = GETDATE()
                    WHERE ParameterName = @ParameterName";

                var rowsAffected = await connection.ExecuteAsync(sql, new 
                { 
                    ParameterName = parameterName,
                    ParameterValue = parameterValue
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating parameter: {ParameterName}", parameterName);
                return false;
            }
        }
    }
}
