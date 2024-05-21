using Entities.CommonModels.Pagination;
using Entities.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.Users
{
    public class BusnPartnerTypeEntity : IPageBasicData
    {
        public int BusnPartnerTypeId { get; set; }
        public string? BusnPartnerTypeName { get; set; }
        public Boolean? IsActive { get; set; }
        public string? ImageUrl { get; set; }


        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
