using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.Setting
{
    public class TaxCategoriesEntity: IPageBasicData
    {

        public int TaxCategoryId { get; set; }
        public string? CategoryName { get; set; }
        public decimal DefaultRate { get; set; }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
