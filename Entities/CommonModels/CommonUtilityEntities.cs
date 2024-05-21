using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.CommonModels
{
    internal class CommonUtilityEntities
    {
    }

    public class HtmlDropDownRemoteData
    {
        public string? DisplayValue { get; set; }
        public string? DisplayText { get; set; }
    }

    public class ImageFileInfo
    {
        public int AttachmentId { get; set; }
        public string? ImageName { get; set; }
        public string? ImageGuidName { get; set; }
        public string? ImageURL { get; set; }

    }
}
