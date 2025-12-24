using System;

namespace GujaratClassified.API.Models.Entity
{
    public class Sponsor
    {
        public int SponsorId { get; set; }
        public string SponsorName { get; set; }
        public string CompanyName { get; set; }
        public string BannerImagePath { get; set; }
        public string CardImagePath { get; set; }
        public string ClickUrl { get; set; }
        public string WhatsappNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public int TotalViews { get; set; }
        public int TotalClicks { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class SponsorTracking
    {
        public int TrackingId { get; set; }
        public int SponsorId { get; set; }
        public int? UserId { get; set; }
        public string DeviceId { get; set; }
        public string TrackingType { get; set; } // VIEW, CLICK
        public DateTime TrackingDate { get; set; }
        public string IpAddress { get; set; }
    }

    public class SystemParameter
    {
        public int ParameterId { get; set; }
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
