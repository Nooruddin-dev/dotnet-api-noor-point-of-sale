using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.Common
{
    public class ServicesResponse
    {
        public string? ResponseMessage { get; set; }
        public Boolean Success { get; set; }
        public int? PrimaryKeyValue { get; set; }
        public object? ValidationMessages { get; set; }
        public object? ResponseObject { get; set; }
    }
}
