using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels
{
    public class ProductAttributeMappingEntity : IPageBasicData
    {
       

        public int ProductAttributeMappingID { get; set; }
        public int ProductAttributeID { get; set; }
        public string? AttributeName { get; set; }
        public string? DisplayName { get; set; }
        public string? AttributeSqlTableName { get; set; }
        public int ProductID { get; set; }
        public string? AttributeValue { get; set; }
        public string? AttributeDisplayText { get; set; }
        public string? PriceAdjustmentType { get; set; }
        public decimal PriceAdjustment { get; set; }



        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
