using Entities.DBModels.Setting;
using Entities.DBModels.ShiftManagement;
using Entities.ModuleSpecificModels.Common;
using Entities.ModuleSpecificModels.Setting.RequestForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.IServices
{
    public interface ISettingServicesDAL
    {
        Task<List<TaxCategoriesEntity>> GetTaxCategoriesDAL(TaxCategoriesEntity FormData);
        Task<List<TaxRulesEntity>> GetTaxRulesDAL(TaxRulesEntity FormData);
        Task<ServicesResponse>? InsertUpdateTaxRuleDAL(TaxRuleRequestForm FormData);
      

    }
}
