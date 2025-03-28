﻿using System.ComponentModel.DataAnnotations;

namespace Clubmates.Web.Models
{
    public class Club
    {
        [Key]
        public int ClubId { get; set; }
        [Required]
        public string? ClubName { get; set; }
        public string? ClubDescription { get; set; }
        public ClubCategory ClubCategory { get; set; }
        public ClubType ClubType { get; set; }
        public string? ClubRules { get; set; }
        public string? ClubContactNumber { get; set; }
        public ClubmatesUser? ClubManager { get; set; }
        public string? ClubEmail { get; set; }
        public byte[]? ClubLogo { get; set; }
        public byte[]? ClubBanner { get; set; }
        public byte[]? ClubBackground { get; set; }


    }
    public enum ClubCategory
    {
        Sports,
        Leisure,
        Entertainment,
        Resarch,
        travel,
        Official,
        Government,
        Social,
        Music,
        Religious,
        Dance,
        Political,
        Others

    }
    public enum ClubType
    {
        Public,
        Private
    }
}
