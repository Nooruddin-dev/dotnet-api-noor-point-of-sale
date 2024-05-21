using Entities.CommonModels.SearchConfiguration;
using Entities.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.IServices
{
    public interface IApiOperationServicesDAL
    {
        Task<ApiConfiguration?> GetAPIConfiguration(string UrlName);
        Task<string?> GetApiDataDynamicDAL(Dictionary<string, object>? requestParameters, ApiConfiguration? apiConfiguration);
        Task<Tuple<dynamic?, PaginationCommonData>> GetDynamicDataFromSingleEntityDAL(string TableName, int PageNo, int PageSize, List<SearchParam> SearchParams);
        Task<bool> DeleteAnyRecordDAL(int primarykeyValue, string? primaryKeyColumn, string? tableName, int SqlDeleteType = 1);
    }
}
