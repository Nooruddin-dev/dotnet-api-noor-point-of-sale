using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels
{
    public class InventoryMainEntity: IPageBasicData
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDefaultImgPath { get; set; }
        public DateTime? SellStartDatetimeUTC { get; set; }
        public DateTime? SellEndDatetimeUTC { get; set; }
        public int WarehouseId { get; set; }
        public int InventoryMethodId { get; set; }
        public int StockQuantity { get; set; }
        public bool? IsBoundToStockQuantity { get; set; }
        public bool? DisplayStockQuantity { get; set; }
        public int OrderMinimumQuantity { get; set; }
        public int OrderMaximumQuantity { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
