using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.Discounts
{
    public class DiscountTypeEntity: IPageBasicData
    {
        public int DiscountTypeId { get; set; }
        public string DiscountTypeName { get; set; } = null!;
        public bool? IsActive { get; set; }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
