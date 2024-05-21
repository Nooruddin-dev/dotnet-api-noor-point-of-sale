using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.ProductsCatalog.RequestForms
{
    public class InventoryMainRequestForm
    {
        public int InventoryId { get; set; }
        public int? InventoryMethodId { get; set; }
        public int? WarehouseId { get; set; }
        public int? StockQuantity { get; set; }
        public int? OrderMinimumQuantity { get; set; }
        public int? OrderMaximumQuantity { get; set; }
        public DateTime? SellStartDatetimeUTC { get; set; }
        public DateTime? SellEndDatetimeUTC { get; set; }
        public bool? IsBoundToStockQuantity { get; set; }
        public bool? DisplayStockQuantity { get; set; }

        [JsonIgnore]
        [NotMapped]
        public int BusnPartnerId { get; set; }
    }

    public class InventoryItemsRequestForm
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public string? attributeBasedInventoryDataJson { get; set; }


    }
}
