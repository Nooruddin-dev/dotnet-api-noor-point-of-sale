using DAL.DBContext;
using DAL.Repository.IServices;
using Entities.DBModels;
using Entities.DBModels.Common;
using Entities.ModuleSpecificModels.Common;
using Entities.ModuleSpecificModels.Common.RequestForms;
using Entities.ModuleSpecificModels.Setting.RequestForms;
using Entities.ModuleSpecificModels.Users;
using Microsoft.Extensions.Configuration;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.Services
{
    public class CommonServicesDAL : ICommonServicesDAL
    {

        private readonly IConfiguration _configuration;
        private readonly IDataContextHelper _contextHelper;
        private readonly IDapperConnectionHelper _dapperConnectionHelper;

        //--Constructor of the class
        public CommonServicesDAL(IConfiguration configuration, IDataContextHelper contextHelper, IDapperConnectionHelper dapperConnectionHelper)
        {
            _configuration = configuration;
            _contextHelper = contextHelper;
            _dapperConnectionHelper = dapperConnectionHelper;
        }



        public virtual async Task<string> LogRunTimeExceptionDAL(string ExceptionMessage, string? StackTrace, string? Source)
        {
            string result = "";

            using (var context = _contextHelper.GetDataContextHelper())
            {
                try
                {
                    context.EnableAutoSelect = false;

                    var QueryResponse = context.Execute(@";EXEC [dbo].[SP_LogRunTimeException] @ExceptionMessage,@StackTrace , @Source",
                        new { ExceptionMessage = ExceptionMessage, StackTrace = StackTrace, Source = Source });

                    result = "Saved Successfully";
                    await Task.FromResult(result);
                    return result;


                }
                catch (Exception)
                {

                    throw;
                }

            }
        }

        public async Task<UserLoginResponse?> GetUserLoginDAL(string UserName, string Password)
        {
            UserLoginResponse? result = new UserLoginResponse();

            using (var context = _contextHelper.GetDataContextHelper())
            {
                try
                {

                    context.EnableAutoSelect = false;
                    result = context.Fetch<UserLoginResponse>(@";EXEC [dbo].[SP_GetUserLogin] @UserName , @Password",
                        new { UserName = UserName, Password = Password }).FirstOrDefault();


                    await Task.FromResult(result);
                    return result;

                }
                catch (Exception)
                {
                    throw;
                }


            }


        }

        public async Task<bool> DeleteAnyRecordDAL(int primarykeyValue, string primaryKeyColumn, string tableName, int SqlDeleteType = 1)
        {

            try
            {
                bool result = false;

                using (var context = _contextHelper.GetDataContextHelper())
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
                        context.Execute(@";EXEC [dbo].[SpDeleteAnyRecord] @tableName, @primaryKeyColumn, @primarykeyValue",
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
            }
            catch (Exception)
            {

                throw;
            }


        }

        public async Task<int> SaveUpdateAttachmentDAL(AttachmentEntity FormData)
        {
            int result = 0;

            using (var context = _contextHelper.GetDataContextHelper())
            {
                try
                {



                    if (FormData.AttachmentId < 1)
                    {
                        result = context.ExecuteScalar<int>(@"INSERT INTO Attachments(AttachmentName, AttachmentTypeID, AttachmentURL, IsByteArray,IsActive, IsCommonImageUpload , CreatedOn, CreatedBy)
                        VALUES(@AttachmentName, 1, @AttachmentUrl, 0,1,  @IsCommonImageUpload  , GETDATE(), @UserId)
                        SELECT @@@IDENTITY AS AttachmentId;",
                        new
                        {
                            AttachmentName = FormData.AttachmentName,
                            AttachmentUrl = FormData.AttachmentUrl,
                            IsCommonImageUpload = FormData.IsCommonImageUpload ?? false,
                            IsActive = FormData.IsActive,
                            UserId = FormData.BusnPartnerId,

                        });

                    }
                    else if (FormData.AttachmentId > 0)
                    {
                        context.Execute(@"UPDATE top(1) Attachments SET
                        AttachmentName=@AttachmentName, AttachmentTypeID=1, AttachmentURL=@AttachmentUrl, IsActive=1 where AttachmentID=@AttachmentId",
                       new
                       {
                           AttachmentId = FormData.AttachmentId,
                           AttachmentName = FormData.AttachmentName,
                           AttachmentUrl = FormData.AttachmentUrl,
                           IsActive = FormData.IsActive,
                           UserId = FormData.BusnPartnerId,

                       });
                        result = FormData.AttachmentId;
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

        public async Task<List<SiteGeneralNotificationsEntity>> GetSiteGeneralNotificationsDAL(SiteGeneralNotificationsEntity FormData)
        {

            List<SiteGeneralNotificationsEntity> result = new List<SiteGeneralNotificationsEntity>();

            using (var context = _contextHelper.GetDataContextHelper())
            {
                try
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");



                    if (FormData.NotificationId > 0)
                    {
                        SearchParameters.Append("AND MTBL.NotificationId =  @0 ", FormData.NotificationId);
                    }


                    if (FormData.IsReadNullProperty != null)
                    {
                        SearchParameters.Append("AND MTBL.IsRead =  @0 ", FormData.IsReadNullProperty);
                    }


                    if (!String.IsNullOrEmpty(FormData.Title))
                    {
                        SearchParameters.Append("AND MTBL.Title LIKE  @0", "%" + FormData.Title + "%");
                    }



                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords,MTBL.* , NT.NotificationTypeName , USR.FirstName as ReadByFirstName")
                      .From(" SiteGeneralNotifications MTBL")
                      .InnerJoin("NotificationTypes NT").On("NT.NotificationTypeID = MTBL.NotificationTypeID")
                      .LeftJoin("BusnPartner USR").On("USR.BusnPartnerId = MTBL.ReadBy")
                      .Where("MTBL.NotificationID is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.NotificationID DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<SiteGeneralNotificationsEntity>(ppSql);

                    if (result != null && result.Count() > 0)
                    {
                        //--Read total unread
                        var ppSqlCountUnread = PetaPoco.Sql.Builder.Select(@" COUNT(*) AS HeaderUnreadNotificationCount")
                         .From(" SiteGeneralNotifications MTBL")
                         .Where("MTBL.IsRead = 0");

                        int HeaderUnreadNotificationCount = context.ExecuteScalar<int>(ppSqlCountUnread);
                        foreach (var item in result)
                        {
                            item.HeaderUnreadNotificationCount = HeaderUnreadNotificationCount;
                        }

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

        public async Task<ServicesResponse>? MarkNotificationAsReadDAL(NotificationReadRequestForm FormData)
        {

            ServicesResponse? result = new ServicesResponse();

            try
            {

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    if (FormData.NotificationId > 0)
                    {
                        var updatedRecords = context.Update("SiteGeneralNotifications", "NotificationId",
                        new
                        {
                           // NotificationId = FormData.NotificationId,
                            IsRead = true,
                            ReadBy = FormData.BusnPartnerId,
                        
                        },
                        FormData.NotificationId);

                        if (updatedRecords > 0)
                        {

                            result.PrimaryKeyValue = FormData.NotificationId;
                            result.Success = true;
                            result.ResponseMessage = "Updated Successfully!";
                        }
                        else
                        {
                            result.Success = false;
                            result.ResponseMessage = "Not updated successfully!";
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

        public async Task<List<PaymentMethodEntity>> GetPaymentMethodsListDAL(PaymentMethodEntity FormData)
        {

            List<PaymentMethodEntity> result = new List<PaymentMethodEntity>();

            using (var context = _contextHelper.GetDataContextHelper())
            {
                try
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");



                    if (FormData.PaymentMethodId > 0)
                    {
                        SearchParameters.Append("AND MTBL.PaymentMethodId =  @0 ", FormData.PaymentMethodId);
                    }


                    if (!String.IsNullOrEmpty(FormData.PaymentMethodName))
                    {
                        SearchParameters.Append("AND MTBL.PaymentMethodName LIKE  @0", "%" + FormData.PaymentMethodName + "%");
                    }

                    //if (!String.IsNullOrEmpty(FormData.FromDate))
                    //{
                    //    SearchParameters.Append("AND Cast(MTBL.CreatedOn AS Date)>=@0", FormData.FromDate);
                    //}

                    //if (!String.IsNullOrEmpty(FormData.ToDate))
                    //{
                    //    SearchParameters.Append("AND Cast(MTBL.CreatedOn AS Date)<=@0", FormData.ToDate);
                    //}

                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords,MTBL.*")
                      .From(" PaymentMethods MTBL")
                      .Where("MTBL.PaymentMethodId is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.PaymentMethodId DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<PaymentMethodEntity>(ppSql);

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
