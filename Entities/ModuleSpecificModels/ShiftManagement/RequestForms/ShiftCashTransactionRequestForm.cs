using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.ShiftManagement.RequestForms
{
    public class ShiftCashTransactionRequestForm
    {
        public int TransactionId {  get; set; }
        public int CashDrawerId {  get; set; }
        public int CashTransactionTypeId {  get; set; }
        public decimal Amount {  get; set; }
        public string? Description {  get; set; }
        public DateTime? TransactionDate {  get; set; }
        public int? OrderId {  get; set; }

        [JsonIgnore]
        [NotMapped]
        public int BusnPartnerId { get; set; }
    }
}

