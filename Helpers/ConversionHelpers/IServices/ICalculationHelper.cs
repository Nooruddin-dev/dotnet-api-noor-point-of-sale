using Entities.DBModels.CashierMain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.ConversionHelpers.IServices
{
    public interface ICalculationHelper
    {
        Task<List<ProductPointOfSaleEntity>?> CalculateDiscountsForProducts(List<ProductPointOfSaleEntity>? data);
       
    }
}
