using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.CashierMain
{
    public class OrderVariantDetail: IPageBasicData
    {
        public int OrderAttributeMappingID { get; set; }
        public int ProductAttributeID { get; set; }
        public int OrderItemID { get; set; }
        public int PrimaryKeyValue { get; set; }
        public string? PrimaryKeyDisplayText { get; set; }
        public string? AttributeDisplayName { get; set; }
        public string? AttributeSqlTableName { get; set; }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
