using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.Common
{
    public class SiteGeneralNotificationsEntity: IPageBasicData
    {

        public int NotificationId { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public int NotificationTypeId { get; set; }
        public bool IsRead { get; set; }
        public int? ReadBy { get; set; }
        public DateTime? ReadByDate { get; set; }
        public string? ClickUrl { get; set; }
        public DateTime CreatedOn { get; set; }

        public string? ReadByFirstName { get; set; }
        public bool? IsReadNullProperty { get; set; }
        public string? NotificationTypeName { get; set; }
        public string? SelectedNotificationsIdsForReadJson { get; set; }
        public int HeaderUnreadNotificationCount { get; set; }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
