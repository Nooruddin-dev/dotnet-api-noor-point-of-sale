using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels
{
    public class ProductPicturesMappingEntity: IPageBasicData
    {
        public int ProductPictureMappingId { get; set; }
        public int ProductId { get; set; }
        public int PictureId { get; set; }
        public int? ColorId { get; set; }
        public string? AttachmentURL { get; set; }
        public string? ProductsImgColorsMappingItemsJson { get; set; }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
