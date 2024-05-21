using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.Discounts
{
    public class DiscountCategoryMappingEntity: IPageBasicData
    {
        public int DiscountCategoryMappingId { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int DiscountID { get; set; }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
