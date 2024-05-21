using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.ShiftManagement
{
    public class ShiftCashDrawerReconciliationStatusesEntity : IPageBasicData
    {
        public int ReconciliationStatusId { get; set; }
        public string? ReconciliationStatusName { get; set; }
        public string? ReconciliationStatusDesc { get; set; }
        public bool IsActive { get; set; }




        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
