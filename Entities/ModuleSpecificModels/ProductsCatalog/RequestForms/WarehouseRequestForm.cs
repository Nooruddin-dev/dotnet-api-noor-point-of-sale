using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.ProductsCatalog.RequestForms
{
    public class WarehouseRequestForm
    {
        public int WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public bool? IsActive { get; set; }

        [JsonIgnore]
        [NotMapped]
        public int BusnPartnerId { get; set; }
    }
}
