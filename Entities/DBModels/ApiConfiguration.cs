using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels
{
    public class ApiConfiguration
    {
        public int ApiId { get; set; }

        public bool? IsAuthorizationNeeded { get; set; }
        public string? Ormtype { get; set; }
        public string? SqlQuery { get; set; }
    }
}
