using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.UsersManagement
{
    public class CountryEntity: IPageBasicData
    {
        public int CountryId { get; set; }

        public string CountryName { get; set; } = null!;

        public int? FlagUrlid { get; set; }

        public bool? IsActive { get; set; }

        public decimal? DisplaySeqNo { get; set; }

        public string? CountryCode { get; set; }

        public string? MobileCode { get; set; }

        public int? CurrencyId { get; set; }

        public int? WorldRegionId { get; set; }

        public string? MetaTitle { get; set; }

        public string? MetaKeywords { get; set; }

        public string? MetaDescription { get; set; }

        public DateTime CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? ModifiedBy { get; set; }


        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
