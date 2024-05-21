using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels
{
    public class ProductAttributeEntity: IPageBasicData
    {
        public int ProductAttributeId { get; set; }
        public string AttributeName { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string? AttributeSqlTableName { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }


        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
