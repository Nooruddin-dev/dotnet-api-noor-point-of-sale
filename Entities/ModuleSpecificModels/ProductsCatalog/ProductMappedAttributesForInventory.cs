using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.ProductsCatalog
{
    public class ProductMappedAttributesForInventory
    {
        public int ProductAttributeMappingID { get; set; }
        public int ProductAttributeID { get; set; }
        public string? AttributeName { get; set; }
        public string? AttributeDisplayName { get; set; }
        public string? AttributeValue { get; set; }
        public string? AttributeValueDisplayText { get; set; }
        public int? PriceAdjustmentType { get; set; }
        public decimal? PriceAdjustment { get; set; }
        public decimal? AdditionalPrice { get; set; }

        public int? InventoryItemId { get; set; }
        public int? InventoryId { get; set; }
        public int? ProductId { get; set; }
        public DateTime? SellStartDatetimeUTC { get; set; }
        public DateTime? SellEndDatetimeUTC { get; set; }
        public int? WarehouseId { get; set; }
        public int? InventoryMethodId { get; set; }
        public int? StockQuantity { get; set; }
        public bool? IsBoundToStockQuantity { get; set; }
        public bool? DisplayStockQuantity { get; set; }
        public int? OrderMinimumQuantity { get; set; }
        public int? OrderMaximumQuantity { get; set; }

        [JsonIgnore]
        [NotMapped]
        public int BusnPartnerId { get; set; }

    }
}
