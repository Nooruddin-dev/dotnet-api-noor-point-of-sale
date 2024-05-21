using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels
{
    public class ProductCategoriesEntity: IPageBasicData
    {

        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public string? ParentCategoryName { get; set; }
        public string? Description { get; set; }
        public int? ParentCategoryID { get; set; }
        public bool? IsActive { get; set; }
        public int DisplaySeqNo { get; set; }
        public int? AttachmentID { get; set; }
        public string? CategoryImagePath { get; set; }
        public string? MetaKeywords { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
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
