using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.ProductsCatalog.RequestForms
{
    public class ColorRequestForm
    {
        public int ColorId { get; set; }
        public string? ColorName { get; set; }
        public string? HexCode { get; set; }
        public bool? IsActive { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
