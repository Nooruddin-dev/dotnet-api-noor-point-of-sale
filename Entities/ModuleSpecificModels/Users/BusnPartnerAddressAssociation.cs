using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.Users
{
    public class BusnPartnerAddressAssociation : IPageBasicData
    {

        public int AddressAsocId { get; set; }

        public int BusnPartnerId { get; set; }

        public int AddressTypeId { get; set; }
        public string? AddressTypeName { get; set; }

        public string AddressOne { get; set; } = null!;

        public string? AddressTwo { get; set; }

        public int CountryId { get; set; }

        public int? StateId { get; set; }

        public int? CityId { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
     
    }
}
