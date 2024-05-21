using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels
{
    public class AttachmentEntity: IPageBasicData
    {
        public int AttachmentId { get; set; }
        public string AttachmentName { get; set; } = null!;
        public int AttachmentTypeId { get; set; }
        public string? AttachmentUrl { get; set; }
        public byte[]? ByteArrayAttachment { get; set; }
        public bool IsByteArray { get; set; }
        public string? SeoName { get; set; }
        public string? AltAttribute { get; set; }
        public string? TitleAttribute { get; set; }
        public string? MimeType { get; set; }
        public string? Guid { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsCommonImageUpload { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CreatedBy { get; set; }

      
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
