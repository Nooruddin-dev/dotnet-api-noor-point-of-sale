using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DBContext
{
    public class DapperConnectionHelper : IDapperConnectionHelper
    {

        private readonly IConfiguration _configuration;
       

        //dapper setting
        public string ConnectionString { get; set; }
        public string ProviderName { get; }

        //--Constructor of the class
        public DapperConnectionHelper(IConfiguration configuration)
        {
            _configuration = configuration;
           
            //--Dapper setting
            ConnectionString = _configuration.GetConnectionString("DBConnection");
            ProviderName = "System.Data.SqlClient";
        }

        //--Dapper Connection String
        public IDbConnection GetDapperContextHelper()
        {
            return new SqlConnection(ConnectionString);
        }

    }

    public interface IDapperConnectionHelper
    {
        IDbConnection GetDapperContextHelper();
    }
}
