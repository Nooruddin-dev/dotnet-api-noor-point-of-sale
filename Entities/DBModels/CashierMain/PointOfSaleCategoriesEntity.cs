using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.CashierMain
{
    public class PointOfSaleCategoriesEntity: IPageBasicData
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryImagePath { get; set; }
        public int? TotalProducts { get; set; }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
