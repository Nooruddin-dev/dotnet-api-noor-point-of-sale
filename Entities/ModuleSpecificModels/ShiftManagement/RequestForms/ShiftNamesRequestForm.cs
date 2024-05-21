using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.ShiftManagement.RequestForms
{
    public class ShiftNamesRequestForm
    {
        public int ShiftNameId { get; set; }
        public string? ShiftName { get; set; }
        public string? DefaultStartTime { get; set; }
        public string? DefaultEndTime { get; set; }
        public bool? IsActive { get; set; }


        [JsonIgnore]
        [NotMapped]
        public int BusnPartnerId { get; set; }
    }
}
