using DAL.DBContext;
using DAL.Repository.IServices;
using Dapper;
using Entities.CommonModels.SearchConfiguration;
using Entities.DBModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DAL.Repository.Services
{
    public class ApiOperationServicesDAL : IApiOperationServicesDAL
    {

        private readonly IConfiguration _configuration;
        private readonly IDataContextHelper _contextHelper;
        private readonly IDapperConnectionHelper _dapperConnectionHelper;

        //--Constructor of the class
        public ApiOperationServicesDAL(IConfiguration configuration, IDataContextHelper contextHelper, IDapperConnectionHelper dapperConnectionHelper)
        {
            _configuration = configuration;
            _contextHelper = contextHelper;
            _dapperConnectionHelper = dapperConnectionHelper;
        }

        public async Task<ApiConfiguration?> GetAPIConfiguration(string UrlName)
        {
            ApiConfiguration? result = new ApiConfiguration();

            using (var repo = _contextHelper.GetDataContextHelper())
            {
                try
                {

                    var ppSql = PetaPoco.Sql.Builder.Select(@"TOP 1 *")

                        .From("APIConfigurations")
                        .Where("UrlName=@0", UrlName);


                    result = repo.Query<ApiConfiguration>(ppSql).FirstOrDefault();

                    await Task.FromResult(result);
                    return result;

                }
                catch (Exception)
                {

                    throw;
                }

            }
        }


        public async Task<string?> GetApiDataDynamicDAL(Dictionary<string, object>? requestParameters, ApiConfiguration? apiConfiguration)
        {
            string result = "";

            try
            {

                if (String.IsNullOrWhiteSpace(apiConfiguration.Ormtype) || apiConfiguration.Ormtype == "PetaPoco")
                {
                    using (var repo = _contextHelper.GetDataContextHelper())
                    {
                        result = repo.Fetch<string>(apiConfiguration.SqlQuery, requestParameters).FirstOrDefault();
                        await Task.FromResult(result);
                        return result;
                    }
                }
                else if (apiConfiguration.Ormtype == "Dapper")
                {
                    using (IDbConnection dbConnection = _dapperConnectionHelper.GetDapperContextHelper())
                    {
                        dbConnection.Open();

                        result = dbConnection.Query<string>(apiConfiguration.SqlQuery, requestParameters , commandType: CommandType.Text).FirstOrDefault();

                        dbConnection.Close();

                        await Task.FromResult(result);
                        return result;
                    }
                }
                else
                {
                    using (var repo = _contextHelper.GetDataContextHelper())
                    {
                        result = repo.Fetch<string>(apiConfiguration.SqlQuery, requestParameters).FirstOrDefault();
                        await Task.FromResult(result);
                        return result;
                    }
                }
                

            }
            catch (Exception)
            {

                throw;
            }

        }


        public async Task<Tuple<dynamic?, PaginationCommonData>> GetDynamicDataFromSingleEntityDAL(string TableName, int PageNo, int PageSize, List<SearchParam> SearchParams)
        {

            try
            {

                string resultQuery = $"SELECT * FROM {TableName}";
                string countQuery = $"SELECT COUNT(*) AS TotalRecords FROM {TableName}";

                string whereClause = "";

                if (SearchParams != null && SearchParams.Count() > 0)
                {
                    foreach (var param in SearchParams.Where(x=>!String.IsNullOrWhiteSpace(x.ParamName) && !String.IsNullOrWhiteSpace(x.ParamValue)).ToList())
                    {
                        string condition;
                        if(param.WhereConditionType ==  0 && !String.IsNullOrWhiteSpace(param.ParamName) && !String.IsNullOrWhiteSpace(param.ParamValue))
                            condition = $"{param.ParamName} = '{param.ParamValue}'";
                        else if (param.WhereConditionType == 1 && !String.IsNullOrWhiteSpace(param.ParamName) && !String.IsNullOrWhiteSpace(param.ParamValue))
                            condition = $"{param.ParamName} = {param.ParamValue}";
                        else if (param.WhereConditionType == 2 && !String.IsNullOrWhiteSpace(param.ParamName) && !String.IsNullOrWhiteSpace(param.ParamValue))
                            condition = $"{param.ParamName} LIKE '%{param.ParamValue}%'";
                        else if (param.WhereConditionType == 3 && !String.IsNullOrWhiteSpace(param.ParamName) && !String.IsNullOrWhiteSpace(param.ParamValue))
                            condition = $"{param.ParamName} is not null and {param.ParamName} > '{param.ParamValue}'";
                        else
                            throw new ArgumentException($"Invalid WhereConditionType for parameter: {param.ParamName}");

                        if (string.IsNullOrEmpty(whereClause))
                            whereClause += condition;
                        else
                            whereClause += $" AND {condition}";
                    }
                    if (!String.IsNullOrWhiteSpace(whereClause))
                    {
                        resultQuery += $" WHERE {whereClause}";
                        countQuery += $" WHERE {whereClause}";
                    }

                }

               

                using (IDbConnection dbConnection = _dapperConnectionHelper.GetDapperContextHelper())
                {
                    dbConnection.Open();

                    var countData = await dbConnection.QueryAsync<PaginationCommonData>(countQuery, commandType: CommandType.Text);

                    if (countData != null && countData.ToList()?.FirstOrDefault()?.TotalRecords > 0)
                    {
                        var countDataDefaultRow = countData.FirstOrDefault();
                        countDataDefaultRow.PageNo = PageNo;
                        countDataDefaultRow.PageSize = PageSize;

                        string query = @"
                        SELECT c.name AS PrimaryKeyName
                        FROM sys.indexes AS i
                        JOIN sys.index_columns AS ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                        JOIN sys.columns AS c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                        WHERE i.object_id = OBJECT_ID(@TableNameForPrimaryKey)
                            AND i.is_primary_key = 1;"
                        ;

                        string primaryKeyColumnName = dbConnection.QuerySingleOrDefault<string>(query, new { TableNameForPrimaryKey = TableName });
                        if (!String.IsNullOrWhiteSpace(primaryKeyColumnName))
                        {
                            PageNo = (PageNo - 1) * PageSize;
                            resultQuery += $" ORDER BY {primaryKeyColumnName} DESC OFFSET {PageNo} ROWS FETCH NEXT {PageSize} ROWS ONLY";
                        }
                        else
                        {
                            PageNo = (PageNo - 1) * PageSize;
                            resultQuery += $" ORDER BY 1 DESC OFFSET {PageNo} ROWS FETCH NEXT {PageSize} ROWS ONLY";
                        }

                        var dynamicResult = await dbConnection.QueryAsync<dynamic>(resultQuery, commandType: CommandType.Text);
                        await Task.FromResult(dynamicResult);
                       // return dynamicResult;
                        return new Tuple<dynamic?, PaginationCommonData>(dynamicResult, countDataDefaultRow);

                    }
                    dbConnection.Close();

                   
                }


            }
            catch (Exception)
            {

                throw;
            }

            return null;
        }

        public async Task<bool> DeleteAnyRecordDAL(int primarykeyValue, string? primaryKeyColumn, string? tableName, int SqlDeleteType = 1)
        {
            bool result = false;

            using (var context = _contextHelper.GetDataContextHelper())
            {
                try
                {

                    if (SqlDeleteType == 1)
                    {
                        string deleteQuery = String.Format("DELETE TOP(1) FROM {0} WHERE {1}='{2}'", tableName, primaryKeyColumn, primarykeyValue);
                        context.Execute(deleteQuery);
                        result = true;
                    }
                    else
                    {
                        context.EnableAutoSelect = false;
                        context.Execute(@";EXEC [dbo].[SP_AdmPanel_DeleteAnyRecord] @tableName, @primaryKeyColumn, @primarykeyValue",
                            new
                            {
                                tableName = tableName,
                                primaryKeyColumn = primaryKeyColumn,
                                primarykeyValue = primarykeyValue,
                            });

                        result = true;
                    }


                    await Task.FromResult(result);
                    return result;

                }
                catch (Exception)
                {
                    throw;
                }

            }
        }


    }
}
