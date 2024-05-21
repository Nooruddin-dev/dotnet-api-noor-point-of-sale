using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.CommonModels.Pagination
{
    public interface IPageBasicData
    {
        public int PageNo { get; set; }
        public int PageSize  { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
