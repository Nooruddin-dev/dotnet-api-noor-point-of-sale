using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.ShiftManagement
{
    public class CashierShiftDrawerInfoEntity : IPageBasicData
    {

        public int ShiftCashDrawerId { get; set; }
        public int ShiftId { get; set; }
        public decimal? StartingCash { get; set; }
        public decimal? EndingCash { get; set; }
        public int? ReconciliationStatusId { get; set; }
        public string? ReconciliationComments { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ShiftStartedAt { get; set; }
        public DateTime? ShiftEndedAt { get; set; }
        public int? ShiftNameId { get; set; }
        public string? ShiftName { get; set; }
        public string? StartedByFirstName { get; set; }
        public string? StartedByLastName { get; set; }
        public string? ReconciliationStatusName { get; set; }


        public int? ShiftStatusId { get; set; } //--For search purpose only
        public string? CashierNameOnlyForSearchPurpose { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
