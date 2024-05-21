using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DBContext
{
    public class DataContextHelper : IDataContextHelper
    {
        private readonly IConfiguration _configuration;


        public DataContextHelper(IConfiguration configuration)
        {
            _configuration = configuration;

            ConnetionString = _configuration.GetConnectionString("DBConnection");
            providerName = "System.Data.SqlClient";
        }
        public string ConnetionString { get; }
        public string providerName { get; }

        public NoorPortalConnDB GetDataContextHelper(bool enableAutoSelect = true)
        {

            return (GetNewDataContext(ConnetionString, providerName, enableAutoSelect));
        }

        private static NoorPortalConnDB GetNewDataContext(string ConnetionString, string providerName, bool enableAutoSelect)
        {
            NoorPortalConnDB repository = new NoorPortalConnDB(ConnetionString, providerName);
            repository.EnableAutoSelect = enableAutoSelect;
            //repository.ELHelperInstance = elHelperInstance;



            return (repository);
        }
    }
}
