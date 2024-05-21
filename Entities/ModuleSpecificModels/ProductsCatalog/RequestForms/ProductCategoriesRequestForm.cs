using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.ProductsCatalog.RequestForms
{
    public class ProductCategoriesRequestForm
    {
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public int? ParentCategoryID { get; set; }

        public bool? IsActive { get; set; } = true;
        public int? AttachmentId { get; set; }

        public IFormFile? CategoryImage { get; set; }

        public int dataOperationType { get; set; } = 1;

        public int BusnPartnerId { get; set; } = 1;
    }
}
