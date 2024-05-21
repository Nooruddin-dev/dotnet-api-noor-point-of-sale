using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.ShiftManagement
{
    public class ShiftCashTransactionsEntity : IPageBasicData
    {

        public int TransactionId { get; set; }
        public int CashDrawerId { get; set; }
        public int? CashTransactionTypeId { get; set; }
        public decimal? Amount { get; set; }
        public string? Description { get; set; }
        public DateTime? TransactionDate { get; set; }
        public int? OrderId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int ShiftId { get; set; }
        public string? CashTransactionTypeName { get; set; }

        public string? FromDate { get; set; }
        public string? ToDate { get; set; }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
