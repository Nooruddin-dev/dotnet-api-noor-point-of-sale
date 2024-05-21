using DAL.DBContext;
using DAL.Repository.IServices;
using Entities.DBModels;
using Entities.DBModels.SalesManagement;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.Services
{
    public class SalesManagementServicesDAL: ISalesManagementServicesDAL
    {
        private readonly IConfiguration _configuration;
        private readonly IDataContextHelper _contextHelper;
        private readonly IDapperConnectionHelper _dapperConnectionHelper;

        //--Constructor of the class
        public SalesManagementServicesDAL(IConfiguration configuration, IDataContextHelper contextHelper, IDapperConnectionHelper dapperConnectionHelper)
        {
            _configuration = configuration;
            _contextHelper = contextHelper;
            _dapperConnectionHelper = dapperConnectionHelper;
        }


        public async Task<List<OrderStatusEntity>> GetOrderStatusTypesDAL(OrderStatusEntity FormData)
        {


            try
            {
                List<OrderStatusEntity> result = new List<OrderStatusEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");



                    if (FormData.StatusId > 0)
                    {
                        SearchParameters.Append("AND MTBL.StatusId =  @0 ", FormData.StatusId);
                    }

                    if (!String.IsNullOrEmpty(FormData.StatusName))
                    {
                        SearchParameters.Append("AND MTBL.StatusName LIKE  @0", "%" + FormData.StatusName + "%");
                    }



                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords,MTBL.*")
                      .From(" OrderStatuses MTBL")
                      .Where("MTBL.StatusId is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.StatusId ASC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<OrderStatusEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;


                }
            }
            catch (Exception)
            {

                throw;
            }



        }


    }
}
