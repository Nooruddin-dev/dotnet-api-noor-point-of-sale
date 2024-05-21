using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.CommonModels.SearchConfiguration
{ 
    public class SearchParam
    {
        public string? ParamName { get; set; }
        public string? ParamValue { get; set; }
        public int WhereConditionType { get; set; }
    }
}
