using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.ShiftManagement
{
    public class ShiftNamesEntity : IPageBasicData
    {
        public int ShiftNameId { get; set; }
        public string? ShiftName { get; set; }
        public TimeSpan? DefaultStartTime { get; set; }
        public TimeSpan? DefaultEndTime { get; set; }
        public bool? IsTwelveHoursShift { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
