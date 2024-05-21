using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DBContext
{
    public interface IDataContextHelper
    {
        public string ConnetionString { get; }
        public string providerName { get; }

        public NoorPortalConnDB GetDataContextHelper(bool enableAutoSelect = true);
    }
}
