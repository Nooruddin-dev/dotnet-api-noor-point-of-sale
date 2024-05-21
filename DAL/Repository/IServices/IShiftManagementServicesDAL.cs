using Entities.DBModels.ShiftManagement;
using Entities.ModuleSpecificModels.Common;
using Entities.ModuleSpecificModels.ShiftManagement.RequestForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.IServices
{
    public interface IShiftManagementServicesDAL
    {
        Task<List<CashierShiftDrawerInfoEntity>> GetCashierShiftDrawerInfoDAL(CashierShiftDrawerInfoEntity FormData);
        Task<ServicesResponse>? InsertUpdateCashierShiftDrawerDAL(CashierShiftDrawerRequestForm FormData);
        Task<Boolean> CheckIfAnyActiveShiftExistsDAL();
        Task<List<ShiftNamesEntity>> GetShiftNamesListDAL(ShiftNamesEntity FormData);
        Task<ServicesResponse>? InsertUpdateShiftNameDAL(ShiftNamesRequestForm FormData);
        Task<List<ShiftCashTransactionTypesEntity>> GetShiftTransactionTypesDAL(ShiftCashTransactionTypesEntity FormData);
        Task<List<ShiftCashDrawerReconciliationStatusesEntity>> GetShiftCashDrawerReconciliationStatusesDAL(ShiftCashDrawerReconciliationStatusesEntity FormData);
        Task<List<ShiftCashTransactionsEntity>> GetShiftCashTransactionDataDAL(ShiftCashTransactionsEntity FormData);
        Task<ServicesResponse>? InsertUpdateCashDrawerTransactionDAL(ShiftCashTransactionRequestForm FormData);

    }
}
