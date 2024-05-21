using DAL.DBContext;
using DAL.Repository.IServices;
using Dapper;
using Entities.DBModels.ShiftManagement;
using Entities.ModuleSpecificModels.Common;
using Entities.ModuleSpecificModels.ShiftManagement.RequestForms;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.Services
{
    public class ShiftManagementServicesDAL: IShiftManagementServicesDAL
    {
        private readonly IConfiguration _configuration;
        private readonly IDataContextHelper _contextHelper;
        private readonly IDapperConnectionHelper _dapperConnectionHelper;
      
        //--Constructor of the class
        public ShiftManagementServicesDAL(IConfiguration configuration, IDataContextHelper contextHelper, IDapperConnectionHelper dapperConnectionHelper)
        {
            _configuration = configuration;
            _contextHelper = contextHelper;
            _dapperConnectionHelper = dapperConnectionHelper;

        }

        public async Task<List<CashierShiftDrawerInfoEntity>> GetCashierShiftDrawerInfoDAL(CashierShiftDrawerInfoEntity FormData)
        {


            try
            {
                List<CashierShiftDrawerInfoEntity> result = new List<CashierShiftDrawerInfoEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {
                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");

                    if (FormData.ShiftId > 0)
                    {
                        SearchParameters.Append("AND SCD.ShiftId =  @0 ", FormData.ShiftId);
                    }

                    if (!String.IsNullOrEmpty(FormData.CashierNameOnlyForSearchPurpose))
                    {
                        SearchParameters.Append("AND (USR.FirstName LIKE  @0 OR USR.LastName LIKE  @0)", "%" + FormData.CashierNameOnlyForSearchPurpose + "%");
                    }

                    if (!String.IsNullOrEmpty(FormData.FromDate))
                    {
                        SearchParameters.Append("AND Cast(SCD.CreatedOn AS Date)>=@0", FormData.FromDate);
                    }

                    if (!String.IsNullOrEmpty(FormData.ToDate))
                    {
                        SearchParameters.Append("AND Cast(SCD.CreatedOn AS Date)<=@0", FormData.ToDate);
                    }

                    if (FormData.ShiftStatusId != null)
                    {
                        if (FormData.ShiftStatusId == 1) //-- 1 is for active shifts
                        {
                            SearchParameters.Append("AND SM.ShiftEndedAt IS NULL");
                        }
                        else if (FormData.ShiftStatusId == 0) //-- 0 is for ended shifts
                        {
                            SearchParameters.Append("AND SM.ShiftEndedAt IS NOT NULL ");
                        }

                    }

                    var ppSql = PetaPoco.Sql.Builder.Select(@"COUNT(*) OVER () as TotalRecords,   SCD.*, SM.ShiftStartedAt, SM.ShiftEndedAt, SN.ShiftNameId,  SN.ShiftName, USR.FirstName AS StartedByFirstName, USR.LastName AS StartedByLastName, SCWS.ReconciliationStatusName")
                      .From(" ShiftCashDrawer SCD")
                      .InnerJoin("ShiftMaster SM").On("SM.ShiftId =  SCD.ShiftId")
                      .InnerJoin("ShiftNames SN").On("SN.ShiftNameId = SM.ShiftNameId")
                      .LeftJoin("BusnPartner USR").On("USR.BusnPartnerId = SM.StartedByUserId")
                      .InnerJoin("ShiftCashDrawerReconciliationStatuses SCWS").On("SCWS.ReconciliationStatusId = SCD.ReconciliationStatusId")
                      .Where("SCD.ShiftCashDrawerId IS NOT NULL")
                      .Append(SearchParameters)
                     .OrderBy("SCD.ShiftCashDrawerId DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<CashierShiftDrawerInfoEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;



                }
            }
            catch (Exception)
            {

                throw;
            }



        }


        public async Task<ServicesResponse>? InsertUpdateCashierShiftDrawerDAL(CashierShiftDrawerRequestForm FormData)
        {

            ServicesResponse? result = new ServicesResponse();

            try
            {

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    if (FormData.ShiftCashDrawerId > 0 && FormData.ShiftId != null && FormData.ShiftId > 0)
                    {
                        var updatedRecords = context.Update("ShiftMaster", "ShiftId",
                        new
                        {
                            //ShiftId = FormData.ShiftId,
                            ShiftStartedAt = FormData.ShiftStartedAt,
                            ShiftEndedAt = FormData.ShiftEndedAt,

                        },
                        FormData.ShiftId);

                        var updatedRecordsShiftCashDrawer = context.Update("ShiftCashDrawer", "ShiftCashDrawerId",
                           new
                           {
                               ShiftId = FormData.ShiftId,
                               StartingCash = FormData.StartingCash,
                               EndingCash = FormData.EndingCash,
                               ReconciliationStatusId = FormData.ReconciliationStatusId,
                               ReconciliationComments = FormData.ReconciliationComments,
                               ModifiedOn = DateTime.Now,
                               ModifiedBy = FormData.BusnPartnerId,

                           },
                           FormData.ShiftCashDrawerId);

                        if (updatedRecordsShiftCashDrawer > 0)
                        {

                            result.PrimaryKeyValue = FormData.ShiftCashDrawerId;
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
                        var ShiftId = context.Insert("ShiftMaster", "ShiftId", true,
                           new
                           {

                               ShiftNameId = FormData.ShiftNameId,
                               ShiftStartedAt = FormData.ShiftStartedAt,
                               StartedByUserId = FormData.BusnPartnerId,
                           }
                            );

                        if (ShiftId != null)
                        {
                            var ShiftCashDrawerId = context.Insert("ShiftCashDrawer", "ShiftCashDrawerId", true,
                            new
                            {

                                ShiftId = ShiftId,
                                StartingCash = FormData.StartingCash,
                                ReconciliationStatusId = FormData.ReconciliationStatusId,
                                ReconciliationComments = FormData.ReconciliationComments,
                                CreatedOn = DateTime.Now,
                                CreatedBy = FormData.BusnPartnerId,
                            }
                            );

                            if (ShiftCashDrawerId != null)
                            {
                                result.PrimaryKeyValue = Convert.ToInt32(ShiftCashDrawerId);
                                result.Success = true;
                                result.ResponseMessage = "Saved Successfully!";
                            }
                            else
                            {
                                result.Success = false;
                                result.ResponseMessage = "Not saved successfully!";
                            }



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

        public async Task<Boolean> CheckIfAnyActiveShiftExistsDAL()
        {


            try
            {
                Boolean result = false;

                using (var context = _dapperConnectionHelper.GetDapperContextHelper())
                {
                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");

                    const string query = "SELECT COUNT(*) FROM ShiftMaster WHERE ShiftEndedAt IS NULL";
                    int TotalActiveShifts = await context.QuerySingleAsync<int>(query);
                    if (TotalActiveShifts > 0)
                    {
                        result = true;
                        return result;
                    }
                    else
                    {
                        result = false;
                        return result;
                    }


                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<List<ShiftNamesEntity>> GetShiftNamesListDAL(ShiftNamesEntity FormData)
        {


            try
            {
                List<ShiftNamesEntity> result = new List<ShiftNamesEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");


                    if (FormData.ShiftNameId > 0)
                    {
                        SearchParameters.Append("AND MTBL.ShiftNameId =  @0 ", FormData.ShiftNameId);
                    }

                    if (!String.IsNullOrEmpty(FormData.ShiftName))
                    {
                        SearchParameters.Append("AND MTBL.ShiftName LIKE  @0", "%" + FormData.ShiftName + "%");
                    }



                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords,MTBL.*")
                      .From(" ShiftNames MTBL")
                      .Where("MTBL.ShiftNameId is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.ShiftNameId DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<ShiftNamesEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;

                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<ServicesResponse>? InsertUpdateShiftNameDAL(ShiftNamesRequestForm FormData)
        {

            ServicesResponse? result = new ServicesResponse();

            try
            {

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    if (FormData.ShiftNameId > 0)
                    {
                        var updatedRecords = context.Update("ShiftNames", "ShiftNameId",
                        new
                        {
                            ShiftNameId = FormData.ShiftNameId,
                            ShiftName = FormData.ShiftName,
                            DefaultStartTime = FormData.DefaultStartTime,
                            DefaultEndTime = FormData.DefaultEndTime,
                            IsActive = FormData.IsActive,
                            ModifiedOn = DateTime.Now,
                            ModifiedBy = FormData.BusnPartnerId,
                        },
                        FormData.ShiftNameId);

                        if (updatedRecords > 0)
                        {

                            result.PrimaryKeyValue = FormData.ShiftNameId;
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
                        var ShiftNameId = context.Insert("ShiftNames", "ShiftNameId", true,
                           new
                           {

                               ShiftName = FormData.ShiftName,
                               DefaultStartTime = FormData.DefaultStartTime,
                               DefaultEndTime = FormData.DefaultEndTime,
                               IsActive = FormData.IsActive,
                               CreatedOn = DateTime.Now,
                               CreatedBy = FormData.BusnPartnerId,
                           }
                            );

                        if (ShiftNameId != null)
                        {


                            result.PrimaryKeyValue = Convert.ToInt32(ShiftNameId);
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

        public async Task<List<ShiftCashTransactionTypesEntity>> GetShiftTransactionTypesDAL(ShiftCashTransactionTypesEntity FormData)
        {


            try
            {
                List<ShiftCashTransactionTypesEntity> result = new List<ShiftCashTransactionTypesEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");


                    if (FormData.CashTransactionTypeId > 0)
                    {
                        SearchParameters.Append("AND MTBL.CashTransactionTypeId =  @0 ", FormData.CashTransactionTypeId);
                    }

                    if (!String.IsNullOrEmpty(FormData.CashTransactionTypeName))
                    {
                        SearchParameters.Append("AND MTBL.CashTransactionTypeName LIKE  @0", "%" + FormData.CashTransactionTypeName + "%");
                    }



                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords,MTBL.*")
                      .From(" ShiftCashTransactionTypes MTBL")
                      .Where("MTBL.CashTransactionTypeId is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.CashTransactionTypeId DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<ShiftCashTransactionTypesEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;

                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<List<ShiftCashDrawerReconciliationStatusesEntity>> GetShiftCashDrawerReconciliationStatusesDAL(ShiftCashDrawerReconciliationStatusesEntity FormData)
        {


            try
            {
                List<ShiftCashDrawerReconciliationStatusesEntity> result = new List<ShiftCashDrawerReconciliationStatusesEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");


                    if (FormData.ReconciliationStatusId > 0)
                    {
                        SearchParameters.Append("AND MTBL.ReconciliationStatusId =  @0 ", FormData.ReconciliationStatusId);
                    }

                    if (!String.IsNullOrEmpty(FormData.ReconciliationStatusName))
                    {
                        SearchParameters.Append("AND MTBL.ReconciliationStatusName LIKE  @0", "%" + FormData.ReconciliationStatusName + "%");
                    }



                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords,MTBL.*")
                      .From(" ShiftCashDrawerReconciliationStatuses MTBL")
                      .Where("MTBL.ReconciliationStatusId is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.ReconciliationStatusId DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<ShiftCashDrawerReconciliationStatusesEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;

                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<List<ShiftCashTransactionsEntity>> GetShiftCashTransactionDataDAL(ShiftCashTransactionsEntity FormData)
        {


            try
            {
                List<ShiftCashTransactionsEntity> result = new List<ShiftCashTransactionsEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");


                    if (FormData.TransactionId > 0)
                    {
                        SearchParameters.Append("AND MTBL.TransactionId =  @0 ", FormData.TransactionId);
                    }


                    if (FormData.CashDrawerId > 0)
                    {
                        SearchParameters.Append("AND MTBL.CashDrawerId =  @0 ", FormData.CashDrawerId);
                    }

                    if (FormData.CashTransactionTypeId > 0)
                    {
                        SearchParameters.Append("AND MTBL.CashTransactionTypeId =  @0 ", FormData.CashTransactionTypeId);
                    }  
                    
                    if (FormData.ShiftId > 0)
                    {
                        SearchParameters.Append("AND SCDW.ShiftId =  @0 ", FormData.ShiftId);
                    }

                    if (!String.IsNullOrEmpty(FormData.FromDate))
                    {
                        SearchParameters.Append("AND Cast(MTBL.TransactionDate AS Date)>=@0", FormData.FromDate);
                    }

                    if (!String.IsNullOrEmpty(FormData.ToDate))
                    {
                        SearchParameters.Append("AND Cast(MTBL.TransactionDate AS Date)<=@0", FormData.ToDate);
                    }

                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords, MTBL.*, SCDW.ShiftId, SCTT.CashTransactionTypeName")
                      .From(" ShiftCashTransactions MTBL")
                      .InnerJoin("ShiftCashDrawer SCDW").On("MTBL.CashDrawerId =  SCDW.ShiftCashDrawerId")
                      .InnerJoin(" ShiftCashTransactionTypes SCTT").On("SCTT.CashTransactionTypeId = MTBL.CashTransactionTypeId")
                      .Where("MTBL.TransactionId is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.TransactionId DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<ShiftCashTransactionsEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;

                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<ServicesResponse>? InsertUpdateCashDrawerTransactionDAL(ShiftCashTransactionRequestForm FormData)
        {

            ServicesResponse? result = new ServicesResponse();

            try
            {
                
                using (var context = _contextHelper.GetDataContextHelper())
                {

                    if (FormData.TransactionId > 0)
                    {
                        var updatedRecords = context.Update("ShiftCashTransactions", "TransactionId",
                        new
                        {

                            CashDrawerId = FormData.CashDrawerId,
                            CashTransactionTypeId = FormData.CashTransactionTypeId,
                            Amount = FormData.Amount,
                            Description = FormData.Description,
                            TransactionDate = FormData.TransactionDate,
                            OrderId = FormData.OrderId,
                            ModifiedOn = DateTime.Now,
                            ModifiedBy = FormData.BusnPartnerId,
                        },
                        FormData.TransactionId);

                        if (updatedRecords > 0)
                        {

                            result.PrimaryKeyValue = FormData.TransactionId;
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
                        var TransactionId = context.Insert("ShiftCashTransactions", "TransactionId", true,
                           new
                           {

                               CashDrawerId = FormData.CashDrawerId,
                               CashTransactionTypeId = FormData.CashTransactionTypeId,
                               Amount = FormData.Amount,
                               Description = FormData.Description,
                               TransactionDate = FormData.TransactionDate,
                               OrderId = FormData.OrderId,
                               CreatedOn = DateTime.Now,
                               CreatedBy = FormData.BusnPartnerId,
                           }
                            );

                        if (TransactionId != null)
                        {


                            result.PrimaryKeyValue = Convert.ToInt32(TransactionId);
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
