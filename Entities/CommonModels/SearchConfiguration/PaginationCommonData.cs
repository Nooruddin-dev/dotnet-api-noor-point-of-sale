using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.CommonModels.SearchConfiguration
{
    public class PaginationCommonData
    {
        public int TotalRecords { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
}
