using Entities.DBModels.SalesManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.IServices
{
    public interface ISalesManagementServicesDAL
    {
        Task<List<OrderStatusEntity>> GetOrderStatusTypesDAL(OrderStatusEntity FormData);
    }
}
