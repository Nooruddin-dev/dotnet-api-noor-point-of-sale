using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.Users.RequestForms
{
    public class BusnPartnerRequestForm
    {
        public int BusnPartnerId { get; set; }
        public int BusnPartnerTypeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? EmailAddress { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsVerified { get; set; }
        public int? CountryId { get; set; }
        public string? AddressOne { get; set; }
        public string? PhoneNo { get; set; }
        public string? Password { get; set; }
        public int? ProfilePictureId { get; set; }
        public IFormFile? UserProfileImage { get; set; }

        [JsonIgnore]
        [NotMapped]
        public int CreateByUserId { get; set; }
    }
}
