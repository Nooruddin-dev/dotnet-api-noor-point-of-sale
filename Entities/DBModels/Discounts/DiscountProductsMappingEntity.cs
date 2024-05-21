using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.Discounts
{
    public class DiscountProductsMappingEntity: IPageBasicData
    {
        public int DiscountProductMappingId { get; set; }
        public int DiscountId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
