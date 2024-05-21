using DAL.DBContext;
using DAL.Repository.IServices;
using Entities.DBModels;
using Entities.DBModels.Setting;
using Entities.DBModels.ShiftManagement;
using Entities.ModuleSpecificModels.Common;
using Entities.ModuleSpecificModels.ProductsCatalog.RequestForms;
using Entities.ModuleSpecificModels.Setting.RequestForms;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.Services
{
    public class SettingServicesDAL: ISettingServicesDAL
    {
        private readonly IConfiguration _configuration;
        private readonly IDataContextHelper _contextHelper;
        private readonly IDapperConnectionHelper _dapperConnectionHelper;

        //--Constructor of the class
        public SettingServicesDAL(IConfiguration configuration, IDataContextHelper contextHelper, IDapperConnectionHelper dapperConnectionHelper)
        {
            _configuration = configuration;
            _contextHelper = contextHelper;
            _dapperConnectionHelper = dapperConnectionHelper;
        }

        public async Task<List<TaxCategoriesEntity>> GetTaxCategoriesDAL(TaxCategoriesEntity FormData)
        {


            try
            {
                List<TaxCategoriesEntity> result = new List<TaxCategoriesEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");



                    if (FormData.TaxCategoryId > 0)
                    {
                        SearchParameters.Append("AND MTBL.TaxCategoryId =  @0 ", FormData.TaxCategoryId);
                    }

                    if (!String.IsNullOrEmpty(FormData.CategoryName))
                    {
                        SearchParameters.Append("AND MTBL.CategoryName LIKE  @0", "%" + FormData.CategoryName + "%");
                    }



                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords,MTBL.*")
                      .From(" TaxCategories MTBL")
                      .Where("MTBL.TaxCategoryId is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.TaxCategoryId DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<TaxCategoriesEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;


                }
            }
            catch (Exception)
            {

                throw;
            }



        }
        public async Task<List<TaxRulesEntity>> GetTaxRulesDAL(TaxRulesEntity FormData)
        {


            try
            {
                List<TaxRulesEntity> result = new List<TaxRulesEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");



                    if (FormData.TaxRuleId > 0)
                    {
                        SearchParameters.Append("AND MTBL.TaxRuleId =  @0 ", FormData.TaxRuleId);
                    } 
                    if (FormData.TaxCategoryId > 0)
                    {
                        SearchParameters.Append("AND MTBL.TaxCategoryId =  @0 ", FormData.TaxCategoryId);
                    }  
                    
                    if (FormData.CountryId != null && FormData.CountryId > 0)
                    {
                        SearchParameters.Append("AND MTBL.CountryId =  @0 ", FormData.CountryId);
                    }

                    if (!String.IsNullOrEmpty(FormData.TaxRuleType))
                    {
                        SearchParameters.Append("AND MTBL.TaxRuleType LIKE  @0", "%" + FormData.TaxRuleType + "%");
                    } 
                    if (!String.IsNullOrEmpty(FormData.CategoryName))
                    {
                        SearchParameters.Append("AND TCTG.CategoryName LIKE  @0", "%" + FormData.CategoryName + "%");
                    }



                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords,MTBL.*, TCTG.CategoryName, CNTR.CountryName")
                      .From(" TaxRules MTBL")
                      .InnerJoin("TaxCategories TCTG").On("TCTG.TaxCategoryId = MTBL.TaxCategoryId")
                      .InnerJoin("Countries CNTR").On("CNTR.CountryID = MTBL.CountryID")
                      .Where("MTBL.TaxRuleId is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.TaxRuleId DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<TaxRulesEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;


                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<ServicesResponse>? InsertUpdateTaxRuleDAL(TaxRuleRequestForm FormData)
        {

            ServicesResponse? result = new ServicesResponse();

            try
            {

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    if (FormData.TaxRuleId > 0)
                    {
                        var updatedRecords = context.Update("TaxRules", "TaxRuleId",
                        new
                        {
                            TaxRuleId = FormData.TaxRuleId,
                            TaxCategoryId = FormData.TaxCategoryId,
                            CountryId = FormData.CountryId,
                            StateId = FormData.StateId,
                            TaxRate = FormData.TaxRate,
                            TaxRuleType = FormData.TaxRuleType,
                            ModifiedOn = DateTime.Now,
                            ModifiedBy = FormData.BusnPartnerId,
                        },
                        FormData.TaxRuleId);

                        if (updatedRecords > 0)
                        {

                            result.PrimaryKeyValue = FormData.TaxRuleId;
                            result.Success = true;
                            result.ResponseMessage = "Updated Successfully!";
                        }
                        else
                        {
                            result.Success = false;
                            result.ResponseMessage = "Not updated successfully!";
                        }

                    }
                    else
                    {
                        //-- Here return primary key
                        var TaxRuleId = context.Insert("TaxRules", "TaxRuleId", true,
                           new
                           {

                               TaxCategoryId = FormData.TaxCategoryId,
                               CountryId = FormData.CountryId,
                               StateId = FormData.StateId,
                               TaxRate = FormData.TaxRate,
                               TaxRuleType = FormData.TaxRuleType,
                               CreatedOn = DateTime.Now,
                               CreatedBy = FormData.BusnPartnerId,
                           }
                            );

                        if (TaxRuleId != null)
                        {


                            result.PrimaryKeyValue = Convert.ToInt32(TaxRuleId);
                            result.Success = true;
                            result.ResponseMessage = "Saved Successfully!";

                        }
                        else
                        {
                            result.Success = false;
                            result.ResponseMessage = "Not saved successfully!";

                        }

                    }

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
