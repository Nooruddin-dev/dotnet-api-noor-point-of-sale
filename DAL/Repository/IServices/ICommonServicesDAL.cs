using Entities.DBModels;
using Entities.DBModels.Common;
using Entities.ModuleSpecificModels.Common;
using Entities.ModuleSpecificModels.Common.RequestForms;
using Entities.ModuleSpecificModels.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.IServices
{
    public interface ICommonServicesDAL
    {
        Task<string> LogRunTimeExceptionDAL(string ExceptionMessage, string? StackTrace, string? Source);
        Task<UserLoginResponse?> GetUserLoginDAL(string UserName, string Password);
        Task<bool> DeleteAnyRecordDAL(int primarykeyValue, string primaryKeyColumn, string tableName, int SqlDeleteType = 1);
        Task<int> SaveUpdateAttachmentDAL(AttachmentEntity FormData);
        Task<List<SiteGeneralNotificationsEntity>> GetSiteGeneralNotificationsDAL(SiteGeneralNotificationsEntity FormData);
        Task<ServicesResponse>? MarkNotificationAsReadDAL(NotificationReadRequestForm FormData);
        Task<List<PaymentMethodEntity>> GetPaymentMethodsListDAL(PaymentMethodEntity FormData);
    }
}
