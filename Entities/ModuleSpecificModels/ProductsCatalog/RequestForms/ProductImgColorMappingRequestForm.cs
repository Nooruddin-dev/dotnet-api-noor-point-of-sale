using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.ProductsCatalog.RequestForms
{
    public class ProductImgColorMappingRequestForm
    {
        public int ProductId { get; set; }
        public string productMappedColorsJson { get; set; }

        [NotMapped]
        public int BusnPartnerId { get; set; }
    }
}
