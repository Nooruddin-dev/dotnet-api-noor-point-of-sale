using Entities.DBModels.CashierMain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.CashierMain
{
    public class OrderDetailMainModel
    {
        public OrderEntity? OrderMainDetail { get; set; }
        public OrderShippingMasterEntity? OrderShippingMaster { get; set; }
        public List<OrderShippingDetailEntity>? OrderShippingDetails { get; set; }
        public List<OrderItemEntity>? OrderItems { get; set; }
        public List<OrdersPaymentEntity>? OrderPaymentDetails { get; set; }
        public List<OrderNotesEntity>? orderNotesEntities { get; set; }
        

    }
}
