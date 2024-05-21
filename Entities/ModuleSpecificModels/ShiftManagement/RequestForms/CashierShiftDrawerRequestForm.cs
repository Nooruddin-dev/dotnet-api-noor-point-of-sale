using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.ShiftManagement.RequestForms
{
    public class CashierShiftDrawerRequestForm
    {
        public int ShiftCashDrawerId { get; set; }
        public int ShiftNameId { get; set; }
        public int? ShiftId { get; set; }
        public DateTime ShiftStartedAt { get; set; }
        public DateTime? ShiftEndedAt { get; set; }
        public decimal StartingCash { get; set; }
        public decimal? EndingCash { get; set; }
        public int ReconciliationStatusId { get; set; }
        public string? ReconciliationComments { get; set; }

        [JsonIgnore]
        [NotMapped]
        public int BusnPartnerId { get; set; }
    }
}
