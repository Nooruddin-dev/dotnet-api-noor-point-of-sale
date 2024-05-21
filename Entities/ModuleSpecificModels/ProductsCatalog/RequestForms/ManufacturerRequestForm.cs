using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.ProductsCatalog.RequestForms
{
    public class ManufacturerRequestForm
    {
       public int ManufacturerID { get; set; }
       public string? Name { get; set; }
       public bool? IsActive { get; set; }

       [NotMapped]
       public int BusnPartnerId { get; set; }
    }
}
