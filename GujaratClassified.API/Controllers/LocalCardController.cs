// Controllers/LocalCardController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.Models.Request;
using System.Security.Claims;

namespace GujaratClassified.API.Controllers
{
    [ApiController]
    [Route("api/localcard")]
    [Produces("application/json")]
    public class LocalCardController : ControllerBase
    {
        private readonly ILocalCardService _localCardService;

        public LocalCardController(ILocalCardService localCardService)
        {
            _localCardService = localCardService;
        }

        /// <summary>
        /// નવું સ્થાનિક કાર્ડ બનાવો (Create new local/business card)
        /// </summary>
        /// <param name="request">Card creation data</param>
        /// <returns>Created card details</returns>
        [HttpPost("create")]
        //[Authorize]
        public async Task<IActionResult> CreateLocalCard([FromBody] CreateLocalCardRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var result = await _localCardService.CreateLocalCardAsync(userId, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// પોતાનું કાર્ડ અપડેટ કરો (Update your card)
        /// </summary>
        /// <param name="cardId">Card ID</param>
        /// <param name="request">Updated card data</param>
        /// <returns>Updated card details</returns>
        [HttpPut("{cardId}/update")]
        [Authorize]
        public async Task<IActionResult> UpdateLocalCard(int cardId, [FromBody] UpdateLocalCardRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var result = await _localCardService.UpdateLocalCardAsync(cardId, userId, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// પોતાનું કાર્ડ ડિલીટ કરો (Delete your card)
        /// </summary>
        /// <param name="cardId">Card ID</param>
        /// <returns>Deletion confirmation</returns>
        [HttpDelete("{cardId}/delete")]
        [Authorize]
        public async Task<IActionResult> DeleteLocalCard(int cardId)
        {
            var userId = GetCurrentUserId();
            var result = await _localCardService.DeleteLocalCardAsync(cardId, userId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// કાર્ડની વિગતો જુઓ (Get card details by ID)
        /// </summary>
        /// <param name="cardId">Card ID</param>
        /// <returns>Card details</returns>
        [HttpGet("{cardId}")]
        public async Task<IActionResult> GetLocalCard(int cardId)
        {
            var result = await _localCardService.GetLocalCardByIdAsync(cardId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// માફી બધા કાર્ડ્સ જુઓ (Get my cards)
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>User's cards with pagination</returns>
        [HttpGet("my-cards")]
        //[Authorize]
        public async Task<IActionResult> GetMyCards(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            if (pageSize > 100)
            {
                return BadRequest(new { success = false, message = "Page size cannot exceed 100" });
            }

            var userId = GetCurrentUserId();
            var result = await _localCardService.GetMyCardsAsync(userId, pageNumber, pageSize);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// બધા કાર્ડ્સ browse કરો + filters (Browse all cards with filters)
        /// </summary>
        /// <param name="categoryId">Filter by category</param>
        /// <param name="subCategoryId">Filter by subcategory</param>
        /// <param name="districtId">Filter by district</param>
        /// <param name="talukaId">Filter by taluka</param>
        /// <param name="villageId">Filter by village</param>
        /// <param name="searchTerm">Search keyword</param>
        /// <param name="isVerified">Filter verified cards only</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of cards</returns>
        [HttpGet("browse")]
        public async Task<IActionResult> BrowseCards(
            [FromQuery] int? categoryId = null,
            [FromQuery] int? subCategoryId = null,
            [FromQuery] int? districtId = null,
            [FromQuery] int? talukaId = null,
            [FromQuery] int? villageId = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isVerified = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            if (pageSize > 100)
            {
                return BadRequest(new { success = false, message = "Page size cannot exceed 100" });
            }

            var request = new LocalCardSearchRequest
            {
                CategoryId = categoryId,
                SubCategoryId = subCategoryId,
                DistrictId = districtId,
                TalukaId = talukaId,
                VillageId = villageId,
                SearchTerm = searchTerm,
                IsVerified = isVerified,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _localCardService.BrowseCardsAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// નજીકના કાર્ડ્સ શોધો (Get nearby cards based on location)
        /// </summary>
        /// <param name="latitude">Current latitude</param>
        /// <param name="longitude">Current longitude</param>
        /// <param name="categoryId">Filter by category</param>
        /// <param name="subCategoryId">Filter by subcategory</param>
        /// <param name="radiusKm">Search radius in kilometers (default 10km)</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Nearby cards sorted by distance</returns>
        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearbyCards(
            [FromQuery] decimal latitude,
            [FromQuery] decimal longitude,
            [FromQuery] int? categoryId = null,
            [FromQuery] int? subCategoryId = null,
            [FromQuery] decimal radiusKm = 10,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            if (pageSize > 100)
            {
                return BadRequest(new { success = false, message = "Page size cannot exceed 100" });
            }

            var request = new NearbyCardsRequest
            {
                Latitude = latitude,
                Longitude = longitude,
                CategoryId = categoryId,
                SubCategoryId = subCategoryId,
                RadiusKm = radiusKm,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _localCardService.GetNearbyCardsAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// સર્ચ કાર્ડ્સ by keyword (Search cards)
        /// </summary>
        /// <param name="searchTerm">Search keyword</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Matching cards</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchCards(
            [FromQuery] string searchTerm,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new { success = false, message = "Search term is required" });
            }

            if (pageSize > 100)
            {
                return BadRequest(new { success = false, message = "Page size cannot exceed 100" });
            }

            var result = await _localCardService.SearchCardsAsync(searchTerm, pageNumber, pageSize);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // Helper method to get current user ID
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}