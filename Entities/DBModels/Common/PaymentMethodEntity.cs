using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.Common
{
    public class PaymentMethodEntity: IPageBasicData
    {
        public int PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; } = null!;
        public bool? IsActive { get; set; }
        public decimal? DisplaySeqNo { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
