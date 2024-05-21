using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.ProductsCatalog.RequestForms
{
    public class ProductTagRequestForm
    {
        public int TagId { get; set; }
        public string? TagName { get; set; }
        public bool? IsActive { get; set; } = true;

        [NotMapped]
        public int BusnPartnerId { get; set; }

    }
}
