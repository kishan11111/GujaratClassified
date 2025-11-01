// DAL/Repositories/LocalCardRepository.cs
using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using GujaratClassified.API.Models.Request;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class LocalCardRepository : ILocalCardRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public LocalCardRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        //public async Task<int> CreateLocalCardAsync(LocalCard card)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var sql = @"
        //        INSERT INTO LocalCard (
        //            UserId, BusinessName, BusinessNameGujarati, BusinessDescription, BusinessDescriptionGujarati,
        //            CategoryId, SubCategoryId, ContactPersonName, PrimaryPhone, SecondaryPhone, 
        //            WhatsAppNumber, Email, DistrictId, TalukaId, VillageId, FullAddress,
        //            Latitude, Longitude, WorkingHours, WorkingDays, IsOpen24Hours,
        //            ProfileImage, CoverImage, IsActive, CreatedAt, UpdatedAt
        //        )
        //        VALUES (
        //            @UserId, @BusinessName, @BusinessNameGujarati, @BusinessDescription, @BusinessDescriptionGujarati,
        //            @CategoryId, @SubCategoryId, @ContactPersonName, @PrimaryPhone, @SecondaryPhone,
        //            @WhatsAppNumber, @Email, @DistrictId, @TalukaId, @VillageId, @FullAddress,
        //            @Latitude, @Longitude, @WorkingHours, @WorkingDays, @IsOpen24Hours,
        //            @ProfileImage, @CoverImage, 1, GETUTCDATE(), GETUTCDATE()
        //        );
        //        SELECT CAST(SCOPE_IDENTITY() AS INT);";

        //    return await connection.ExecuteScalarAsync<int>(sql, card);
        //}

        //public async Task<bool> UpdateLocalCardAsync(LocalCard card)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var sql = @"
        //        UPDATE LocalCard 
        //        SET 
        //            BusinessName = @BusinessName,
        //            BusinessNameGujarati = @BusinessNameGujarati,
        //            BusinessDescription = @BusinessDescription,
        //            BusinessDescriptionGujarati = @BusinessDescriptionGujarati,
        //            CategoryId = @CategoryId,
        //            SubCategoryId = @SubCategoryId,
        //            ContactPersonName = @ContactPersonName,
        //            PrimaryPhone = @PrimaryPhone,
        //            SecondaryPhone = @SecondaryPhone,
        //            WhatsAppNumber = @WhatsAppNumber,
        //            Email = @Email,
        //            DistrictId = @DistrictId,
        //            TalukaId = @TalukaId,
        //            VillageId = @VillageId,
        //            FullAddress = @FullAddress,
        //            Latitude = @Latitude,
        //            Longitude = @Longitude,
        //            WorkingHours = @WorkingHours,
        //            WorkingDays = @WorkingDays,
        //            IsOpen24Hours = @IsOpen24Hours,
        //            ProfileImage = @ProfileImage,
        //            CoverImage = @CoverImage,
        //            UpdatedAt = GETUTCDATE()
        //        WHERE CardId = @CardId AND UserId = @UserId";

        //    var rowsAffected = await connection.ExecuteAsync(sql, card);
        //    return rowsAffected > 0;
        //}

        //public async Task<bool> DeleteLocalCardAsync(int cardId, int userId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var sql = "UPDATE LocalCard SET IsActive = 0, UpdatedAt = GETUTCDATE() WHERE CardId = @CardId AND UserId = @UserId";

        //    var rowsAffected = await connection.ExecuteAsync(sql, new { CardId = cardId, UserId = userId });
        //    return rowsAffected > 0;
        //}

        public async Task<int> CreateLocalCardAsync(LocalCard card)
        {
            using var connection = _connectionFactory.CreateConnection();

            var cardId = await connection.ExecuteScalarAsync<int>(
                "SP_LocalCard_Create",
                card,
                commandType: CommandType.StoredProcedure
            );

            return cardId;
        }

        public async Task<bool> UpdateLocalCardAsync(LocalCard card)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.ExecuteAsync(
                "SP_LocalCard_Update",
                card,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

        public async Task<bool> DeleteLocalCardAsync(int cardId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@CardId", cardId);
            parameters.Add("@UserId", userId);

            var result = await connection.ExecuteAsync(
                "SP_LocalCard_Delete",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }




        //public async Task<LocalCard?> GetLocalCardByIdAsync(int cardId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var sql = @"
        //        SELECT 
        //            lc.*,
        //            u.FirstName + ' ' + u.LastName AS UserName,
        //            u.Mobile AS UserMobile,
        //            cat.CategoryNameGujarati,
        //            cat.CategoryNameEnglish,
        //            sub.SubCategoryNameGujarati,
        //            sub.SubCategoryNameEnglish,
        //            d.DistrictNameGujarati,
        //            d.DistrictNameEnglish,
        //            t.TalukaNameGujarati,
        //            t.TalukaNameEnglish,
        //            v.VillageNameGujarati,
        //            v.VillageNameEnglish
        //        FROM LocalCard lc
        //        INNER JOIN [Users] u ON lc.UserId = u.UserId
        //        INNER JOIN LocalCardCategory cat ON lc.CategoryId = cat.CategoryId
        //        LEFT JOIN LocalCardSubCategory sub ON lc.SubCategoryId = sub.SubCategoryId
        //        INNER JOIN Districts d ON lc.DistrictId = d.DistrictId
        //        INNER JOIN Talukas t ON lc.TalukaId = t.TalukaId
        //        INNER JOIN Villages v ON lc.VillageId = v.VillageId
        //        WHERE lc.CardId = @CardId AND lc.IsActive = 1";

        //    var card = await connection.QueryFirstOrDefaultAsync<LocalCard>(sql, new { CardId = cardId });

        //    if (card != null)
        //    {
        //        card.Images = (await GetCardImagesAsync(cardId)).ToList();
        //    }

        //    return card;
        //}

        public async Task<LocalCard?> GetLocalCardByIdAsync(int cardId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var card = await connection.QueryFirstOrDefaultAsync<LocalCard>(
                "SP_LocalCard_GetById",
                new { CardId = cardId },
                commandType: CommandType.StoredProcedure
            );

            if (card != null)
            {
                card.Images = (await GetCardImagesAsync(cardId)).ToList();
            }

            return card;
        }


        //public async Task<bool> AddCardImagesAsync(int cardId, List<string> imageUrls)
        //{
        //    if (imageUrls == null || !imageUrls.Any())
        //        return true;

        //    using var connection = _connectionFactory.CreateConnection();

        //    var sql = @"
        //        INSERT INTO LocalCardImage (CardId, ImageUrl, SortOrder, CreatedAt)
        //        VALUES (@CardId, @ImageUrl, @SortOrder, GETUTCDATE())";

        //    var imageData = imageUrls.Select((url, index) => new
        //    {
        //        CardId = cardId,
        //        ImageUrl = url,
        //        SortOrder = index + 1
        //    });

        //    var rowsAffected = await connection.ExecuteAsync(sql, imageData);
        //    return rowsAffected > 0;
        //}
        public async Task<bool> AddCardImagesAsync(int cardId, List<string> imageUrls)
        {
            if (imageUrls == null || !imageUrls.Any())
                return true;

            using var connection = _connectionFactory.CreateConnection();

            var imageData = imageUrls.Select((url, index) => new
            {
                CardId = cardId,
                ImageUrl = url,
                SortOrder = index + 1
            });

            var rowsAffected = await connection.ExecuteAsync(
                "SP_LocalCardImage_Add",
                imageData,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }


        //public async Task<List<LocalCardImage>> GetCardImagesAsync(int cardId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var sql = @"
        //        SELECT ImageId, CardId, ImageUrl, Caption, SortOrder, CreatedAt
        //        FROM LocalCardImage
        //        WHERE CardId = @CardId
        //        ORDER BY SortOrder";

        //    var images = await connection.QueryAsync<LocalCardImage>(sql, new { CardId = cardId });
        //    return images.ToList();
        //}

        public async Task<List<LocalCardImage>> GetCardImagesAsync(int cardId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var images = await connection.QueryAsync<LocalCardImage>(
                "SP_LocalCardImage_GetByCardId",
                new { CardId = cardId },
                commandType: CommandType.StoredProcedure
            );

            return images.ToList();
        }

        //public async Task<bool> DeleteCardImagesAsync(int cardId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var sql = "DELETE FROM LocalCardImage WHERE CardId = @CardId";

        //    await connection.ExecuteAsync(sql, new { CardId = cardId });
        //    return true;
        //}
        public async Task<bool> DeleteCardImagesAsync(int cardId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync(
                "SP_LocalCardImage_DeleteByCardId",
                new { CardId = cardId },
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }


        //public async Task<(List<LocalCard> Cards, int TotalCount)> GetUserCardsAsync(int userId, int pageNumber, int pageSize)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var countSql = "SELECT COUNT(*) FROM LocalCard WHERE UserId = @UserId AND IsActive = 1";
        //    var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { UserId = userId });

        //    var sql = @"
        //        SELECT 
        //            lc.*,
        //            cat.CategoryNameGujarati,
        //            cat.CategoryNameEnglish,
        //            sub.SubCategoryNameGujarati,
        //            sub.SubCategoryNameEnglish,
        //            d.DistrictNameGujarati,
        //            t.TalukaNameGujarati,
        //            v.VillageNameGujarati
        //        FROM LocalCard lc
        //        INNER JOIN LocalCardCategory cat ON lc.CategoryId = cat.CategoryId
        //        LEFT JOIN LocalCardSubCategory sub ON lc.SubCategoryId = sub.SubCategoryId
        //        INNER JOIN Districts d ON lc.DistrictId = d.DistrictId
        //        INNER JOIN Talukas t ON lc.TalukaId = t.TalukaId
        //        INNER JOIN Villages v ON lc.VillageId = v.VillageId
        //        WHERE lc.UserId = @UserId AND lc.IsActive = 1
        //        ORDER BY lc.CreatedAt DESC
        //        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        //    var offset = (pageNumber - 1) * pageSize;
        //    var cards = await connection.QueryAsync<LocalCard>(sql, new { UserId = userId, Offset = offset, PageSize = pageSize });

        //    return (cards.ToList(), totalCount);
        //}

        public async Task<(List<LocalCard> Cards, int TotalCount)> GetUserCardsAsync(int userId, int pageNumber, int pageSize)
        {
            using var connection = _connectionFactory.CreateConnection();

            // Step 1: Get total count
            var totalCount = await connection.ExecuteScalarAsync<int>(
                "SP_LocalCard_GetUserCardsCount",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure
            );

            // Step 2: Get paged records
            var offset = (pageNumber - 1) * pageSize;

            var cards = await connection.QueryAsync<LocalCard>(
                "SP_LocalCard_GetUserCardsPaged",
                new { UserId = userId, Offset = offset, PageSize = pageSize },
                commandType: CommandType.StoredProcedure
            );

            return (cards.ToList(), totalCount);
        }


        public async Task<(List<LocalCard> Cards, int TotalCount)> BrowseCardsAsync(LocalCardSearchRequest request)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var whereConditions = new List<string> { "lc.IsActive = 1" };
                var parameters = new DynamicParameters();

                if (request.CategoryId.HasValue)
                {
                    whereConditions.Add("lc.CategoryId = @CategoryId");
                    parameters.Add("CategoryId", request.CategoryId.Value);
                }

                if (request.SubCategoryId.HasValue)
                {
                    whereConditions.Add("lc.SubCategoryId = @SubCategoryId");
                    parameters.Add("SubCategoryId", request.SubCategoryId.Value);
                }

                if (request.DistrictId.HasValue)
                {
                    whereConditions.Add("lc.DistrictId = @DistrictId");
                    parameters.Add("DistrictId", request.DistrictId.Value);
                }

                if (request.TalukaId.HasValue)
                {
                    whereConditions.Add("lc.TalukaId = @TalukaId");
                    parameters.Add("TalukaId", request.TalukaId.Value);
                }

                if (request.VillageId.HasValue)
                {
                    whereConditions.Add("lc.VillageId = @VillageId");
                    parameters.Add("VillageId", request.VillageId.Value);
                }

                if (request.IsVerified.HasValue)
                {
                    whereConditions.Add("lc.IsVerified = @IsVerified");
                    parameters.Add("IsVerified", request.IsVerified.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    whereConditions.Add(@"(
                    lc.BusinessName LIKE @SearchTerm OR 
                    lc.BusinessNameGujarati LIKE @SearchTerm OR
                    lc.ContactPersonName LIKE @SearchTerm OR
                    cat.CategoryNameGujarati LIKE @SearchTerm OR
                    cat.CategoryNameEnglish LIKE @SearchTerm OR
                    sub.SubCategoryNameGujarati LIKE @SearchTerm OR
                    sub.SubCategoryNameEnglish LIKE @SearchTerm
                )");
                    parameters.Add("SearchTerm", $"%{request.SearchTerm}%");
                }

                var whereClause = string.Join(" AND ", whereConditions);

                var countSql = $@"
                SELECT COUNT(*) 
                FROM LocalCard lc
                INNER JOIN LocalCardCategory cat ON lc.CategoryId = cat.CategoryId
                LEFT JOIN LocalCardSubCategory sub ON lc.SubCategoryId = sub.SubCategoryId
                WHERE {whereClause}";

                var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

                var sql = $@"
                SELECT 
                    lc.*,
                    u.FirstName + ' ' + u.LastName AS UserName,
                    cat.CategoryNameGujarati,
                    cat.CategoryNameEnglish,
                    sub.SubCategoryNameGujarati,
                    sub.SubCategoryNameEnglish,
                    d.DistrictNameGujarati,
                    d.DistrictNameEnglish,
                    t.TalukaNameGujarati,
                    t.TalukaNameEnglish,
                    v.VillageNameGujarati,
                    v.VillageNameEnglish
                FROM LocalCard lc
                INNER JOIN [users] u ON lc.UserId = u.UserId
                INNER JOIN LocalCardCategory cat ON lc.CategoryId = cat.CategoryId
                LEFT JOIN LocalCardSubCategory sub ON lc.SubCategoryId = sub.SubCategoryId
                INNER JOIN Districts d ON lc.DistrictId = d.DistrictId
                INNER JOIN talukas t ON lc.TalukaId = t.TalukaId
                INNER JOIN Villages v ON lc.VillageId = v.VillageId
                WHERE {whereClause}
                ORDER BY lc.IsVerified DESC, lc.CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var offset = (request.PageNumber - 1) * request.PageSize;
                parameters.Add("Offset", offset);
                parameters.Add("PageSize", request.PageSize);

                var cards = await connection.QueryAsync<LocalCard>(sql, parameters);

                return (cards.ToList(), totalCount);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
           
        }

        public async Task<(List<LocalCard> Cards, int TotalCount)> SearchCardsAsync(string searchTerm, int pageNumber, int pageSize)
        {
            var request = new LocalCardSearchRequest
            {
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return await BrowseCardsAsync(request);
        }

        public async Task<(List<LocalCard> Cards, int TotalCount)> GetNearbyCardsAsync(
            decimal latitude,
            decimal longitude,
            decimal radiusKm,
            int? categoryId,
            int? subCategoryId,
            int pageNumber,
            int pageSize)
        {
            using var connection = _connectionFactory.CreateConnection();

            var whereConditions = new List<string>
            {
                "lc.IsActive = 1",
                "lc.Latitude IS NOT NULL",
                "lc.Longitude IS NOT NULL"
            };

            var parameters = new DynamicParameters();
            parameters.Add("Latitude", latitude);
            parameters.Add("Longitude", longitude);
            parameters.Add("RadiusKm", radiusKm);

            if (categoryId.HasValue)
            {
                whereConditions.Add("lc.CategoryId = @CategoryId");
                parameters.Add("CategoryId", categoryId.Value);
            }

            if (subCategoryId.HasValue)
            {
                whereConditions.Add("lc.SubCategoryId = @SubCategoryId");
                parameters.Add("SubCategoryId", subCategoryId.Value);
            }

            var whereClause = string.Join(" AND ", whereConditions);

            var sql = $@"
                WITH DistanceCalc AS (
                    SELECT 
                        lc.*,
                        u.FirstName + ' ' + u.LastName AS UserName,
                        cat.CategoryNameGujarati,
                        cat.CategoryNameEnglish,
                        sub.SubCategoryNameGujarati,
                        sub.SubCategoryNameEnglish,
                        d.DistrictNameGujarati,
                        d.DistrictNameEnglish,
                        t.TalukaNameGujarati,
                        t.TalukaNameEnglish,
                        v.VillageNameGujarati,
                        v.VillageNameEnglish,
                        dbo.CalculateDistance(@Latitude, @Longitude, lc.Latitude, lc.Longitude) AS DistanceKm
                    FROM LocalCard lc
                    INNER JOIN [Users] u ON lc.UserId = u.UserId
                    INNER JOIN LocalCardCategory cat ON lc.CategoryId = cat.CategoryId
                    LEFT JOIN LocalCardSubCategory sub ON lc.SubCategoryId = sub.SubCategoryId
                    INNER JOIN Districts d ON lc.DistrictId = d.DistrictId
                    INNER JOIN Talukas t ON lc.TalukaId = t.TalukaId
                    INNER JOIN Villages v ON lc.VillageId = v.VillageId
                    WHERE {whereClause}
                )
                SELECT * FROM DistanceCalc
                WHERE DistanceKm <= @RadiusKm
                ORDER BY DistanceKm, IsVerified DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var countSql = $@"
                SELECT COUNT(*) FROM (
                    SELECT CardId
                    FROM LocalCard lc
                    WHERE {whereClause}
                    AND dbo.CalculateDistance(@Latitude, @Longitude, lc.Latitude, lc.Longitude) <= @RadiusKm
                ) AS CountQuery";

            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

            var offset = (pageNumber - 1) * pageSize;
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);

            var cards = await connection.QueryAsync<LocalCard>(sql, parameters);

            return (cards.ToList(), totalCount);
        }

        //public async Task<bool> IsCardOwnerAsync(int cardId, int userId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var sql = "SELECT COUNT(*) FROM LocalCard WHERE CardId = @CardId AND UserId = @UserId";
        //    var count = await connection.ExecuteScalarAsync<int>(sql, new { CardId = cardId, UserId = userId });

        //    return count > 0;
        //}
        public async Task<bool> IsCardOwnerAsync(int cardId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new { CardId = cardId, UserId = userId };

            var count = await connection.ExecuteScalarAsync<int>(
                "SP_LocalCard_IsOwner",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return count > 0;
        }

        //public async Task<bool> CardExistsAsync(int cardId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var sql = "SELECT COUNT(*) FROM LocalCard WHERE CardId = @CardId AND IsActive = 1";
        //    var count = await connection.ExecuteScalarAsync<int>(sql, new { CardId = cardId });

        //    return count > 0;
        //}
        public async Task<bool> CardExistsAsync(int cardId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new { CardId = cardId };

            var count = await connection.ExecuteScalarAsync<int>(
                "SP_LocalCard_Exists",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return count > 0;
        }

    }
}