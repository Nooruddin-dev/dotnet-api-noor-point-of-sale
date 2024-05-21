using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.Setting.RequestForms
{
    public class TaxRuleRequestForm
    {
        public int TaxRuleId { get; set; }
        public int TaxCategoryId { get; set; }
        public int CountryId { get; set; }
        public int? StateId { get; set; }
        public decimal TaxRate { get; set; }
        public string? TaxRuleType { get; set; }


        [JsonIgnore]
        [NotMapped]
        public int BusnPartnerId { get; set; }
    }
}
