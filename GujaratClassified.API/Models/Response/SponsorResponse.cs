using System;

namespace GujaratClassified.API.Models.Response
{
    public class SponsorBannerResponse
    {
        public int SponsorId { get; set; }
        public string SponsorName { get; set; }
        public string CompanyName { get; set; }
        public string BannerImagePath { get; set; }
        public string ClickUrl { get; set; }
        public string WhatsappNumber { get; set; }
        public string DisplayType { get; set; } // BANNER, CARD
    }

    public class SponsorCardResponse
    {
        public int SponsorId { get; set; }
        public string SponsorName { get; set; }
        public string CompanyName { get; set; }
        public string CardImagePath { get; set; }
        public string ClickUrl { get; set; }
        public string WhatsappNumber { get; set; }
    }

    public class SystemParameterResponse
    {
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public bool IsActive { get; set; }
    }
}
