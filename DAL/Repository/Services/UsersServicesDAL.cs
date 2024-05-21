using DAL.DBContext;
using DAL.Repository.IServices;
using Dapper;
using Entities.DBModels;
using Entities.DBModels.UsersManagement;
using Entities.ModuleSpecificModels.Common;
using Entities.ModuleSpecificModels.ProductsCatalog.RequestForms;
using Entities.ModuleSpecificModels.Users;
using Entities.ModuleSpecificModels.Users.RequestForms;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DAL.Repository.Services
{
    public class UsersServicesDAL : IUsersServicesDAL
    {
        private readonly IConfiguration _configuration;
        private readonly IDataContextHelper _contextHelper;
        private readonly IDapperConnectionHelper _dapperConnectionHelper;

        //--Constructor of the class
        public UsersServicesDAL(IConfiguration configuration, IDataContextHelper contextHelper, IDapperConnectionHelper dapperConnectionHelper)
        {
            _configuration = configuration;
            _contextHelper = contextHelper;
            _dapperConnectionHelper = dapperConnectionHelper;
        }

        public async Task<List<BusnPartnerEntity>> GetBusinessPartnersDAL(BusnPartnerEntity FormData)
        {

            List<BusnPartnerEntity> result = new List<BusnPartnerEntity>();

            using (var context = _contextHelper.GetDataContextHelper())
            {
                try
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");

                    if (FormData.BusnPartnerTypeId > 0)
                    {
                        SearchParameters.Append("AND MTBL.BusnPartnerTypeId =  @0 ", FormData.BusnPartnerTypeId);
                    }

                    if (FormData.BusnPartnerId > 0)
                    {
                        SearchParameters.Append("AND MTBL.BusnPartnerId =  @0 ", FormData.BusnPartnerId);
                    }

                    if (FormData.IsActive != null)
                    {
                        SearchParameters.Append("AND MTBL.IsActive =  @0 ", (FormData.IsActive == true ? 1 : 0));
                    }

                    if (!String.IsNullOrEmpty(FormData.FirstName))
                    {
                        SearchParameters.Append("AND MTBL.FirstName LIKE  @0", "%" + FormData.FirstName + "%");
                    }

                    if (!String.IsNullOrEmpty(FormData.EmailAddress))
                    {
                        SearchParameters.Append("AND MTBL.EmailAddress LIKE  @0", "%" + FormData.EmailAddress + "%");
                    }


                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords, MTBL.*, BPA.PhoneNo, ATC.AttachmentURL AS ProfilePicturePath, BTYPE.BusnPartnerTypeName")
                      .From(" BusnPartner MTBL")
                      .LeftJoin("Attachments ATC").On("ATC.AttachmentID = MTBL.ProfilePictureId")
                      .InnerJoin("BusnPartnerType BTYPE").On("BTYPE.BusnPartnerTypeId = MTBL.BusnPartnerTypeId")
                      .Append(@"OUTER APPLY
                        (
                         SELECT TOP 1 BPA.PhoneNo FROM BusnPartnerPhoneAssociation BPA WHERE BPA.BusnPartnerId = MTBL.BusnPartnerId
                        )BPA")
                      .Where("MTBL.BusnPartnerId is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.BusnPartnerId DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<BusnPartnerEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;


                }
                catch (Exception)
                {

                    throw;
                }

            }

        }

        public async Task<BusnPartnerEntity?> GetBusinessPartnerByIdDAL(int BusnPartnerId)
        {

            BusnPartnerEntity? result = new BusnPartnerEntity();

            using (var context = _contextHelper.GetDataContextHelper())
            {
                try
                {


                    var ppSql = PetaPoco.Sql.Builder.Select(@" MTBL.*")
                      .From(" BusnPartner MTBL")
                      .Where("MTBL.BusnPartnerId = @0", BusnPartnerId);
                    result = context.Fetch<BusnPartnerEntity>(ppSql).FirstOrDefault();

                    await Task.FromResult(result);
                    return result;


                }
                catch (Exception)
                {

                    throw;
                }

            }

        }

        public async Task<BusnPartnerEntity?> GetBusinessPartnerByEmailPasswordDAL(string Email, string Password)
        {

            BusnPartnerEntity? result = new BusnPartnerEntity();

            using (var context = _contextHelper.GetDataContextHelper())
            {
                try
                {


                    var ppSql = PetaPoco.Sql.Builder.Select(@" MTBL.*")
                      .From(" BusnPartner MTBL")
                      .Where("MTBL.EmailAddress = @0 AND MTBL.Password = @1", Email, Password);
                    result = context.Fetch<BusnPartnerEntity>(ppSql).FirstOrDefault();

                    await Task.FromResult(result);
                    return result;


                }
                catch (Exception)
                {

                    throw;
                }

            }

        }

        public async Task<BusnPartnerEntity?> GetBusinessPartnerByEmailDAL(string EmailAddress)
        {

            BusnPartnerEntity? result = new BusnPartnerEntity();

            using (var context = _contextHelper.GetDataContextHelper())
            {
                try
                {


                    var ppSql = PetaPoco.Sql.Builder.Select(@" MTBL.*")
                      .From(" BusnPartner MTBL")
                      .Where("MTBL.EmailAddress = @0", EmailAddress);
                    result = context.Fetch<BusnPartnerEntity>(ppSql).FirstOrDefault();

                    await Task.FromResult(result);
                    return result;


                }
                catch (Exception)
                {

                    throw;
                }

            }

        }


        public async Task<List<BusnPartnerTypeEntity>> GetBusinessPartnerTypesDAL(BusnPartnerTypeEntity FormData)
        {

            List<BusnPartnerTypeEntity> result = new List<BusnPartnerTypeEntity>();

            using (var context = _contextHelper.GetDataContextHelper())
            {
                try
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");



                    if (FormData.BusnPartnerTypeId > 0)
                    {
                        SearchParameters.Append("AND MTBL.BusnPartnerTypeId =  @0 ", FormData.BusnPartnerTypeId);
                    }


                    var ppSql = PetaPoco.Sql.Builder.Select(@" MTBL.*")
                      .From(" BusnPartnerType MTBL")
                      .Where("MTBL.BusnPartnerTypeId is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.BusnPartnerTypeId DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<BusnPartnerTypeEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;


                }
                catch (Exception)
                {

                    throw;
                }

            }

        }

        public async Task<ServicesResponse>? AddUpdateBusinessPartnerDAL(BusnPartnerEntity FormData)
        {
         
            ServicesResponse? result = new ServicesResponse();

            try
            {



                using (var context = _contextHelper.GetDataContextHelper())
                {

                    if (FormData.BusnPartnerId > 0)
                    {
                        var updatedRecords = context.Update("BusnPartner", "BusnPartnerId",
                        new
                        {
                            BusnPartnerId = FormData.BusnPartnerId,
                            BusnPartnerTypeId = FormData.BusnPartnerTypeId,
                            FirstName = FormData.FirstName,
                            LastName = FormData.LastName,
                            EmailAddress = FormData.EmailAddress,
                            Password = FormData.Password,
                            IsActive = FormData.IsActive,
                            CreatedOn = DateTime.Now,
                        },
                        FormData.BusnPartnerId);

                        if (updatedRecords > 0)
                        {
                            if (FormData.BusnPartnerAddressAssociationBusnPartners?.Count() > 0)
                            {
                                foreach (var item in FormData.BusnPartnerAddressAssociationBusnPartners)
                                {
                                    if (item.AddressAsocId > 0)
                                    {



                                        var updatedRecordsAddressAssociation = context.Update("BusnPartnerAddressAssociation", "AddressAsocId",
                                           new
                                           {
                                               AddressAsocId = item.AddressAsocId,
                                               BusnPartnerId = FormData.BusnPartnerId,
                                               AddressTypeId = item.AddressTypeId,
                                               AddressOne = item.AddressOne,
                                               AddressTwo = item.AddressTwo,
                                               CountryId = item.CountryId,
                                               StateId = item.StateId,
                                               CityId = item.CityId,
                                               IsActive = item.IsActive,
                                               UpdatedOn = DateTime.Now,
                                           },
                                           item.AddressAsocId);
                                    }
                                    else
                                    {
                                        var AddressAsocId = context.Insert("BusnPartnerAddressAssociation", "AddressAsocId", true,
                                       new
                                       {

                                           BusnPartnerId = FormData.BusnPartnerId,
                                           AddressTypeId = item.AddressTypeId,
                                           AddressOne = item.AddressOne,
                                           AddressTwo = item.AddressTwo,
                                           CountryId = item.CountryId,
                                           StateId = item.StateId,
                                           CityId = item.CityId,
                                           IsActive = item.IsActive,
                                           CreatedOn = DateTime.Now,
                                       }
                                     );
                                    }

                                }
                            }

                            result.PrimaryKeyValue =  FormData.BusnPartnerId;
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
                        var BusnPartnerId = context.Insert("BusnPartner", "BusnPartnerId", true,
                           new
                           {

                               BusnPartnerTypeId = FormData.BusnPartnerTypeId,
                               FirstName = FormData.FirstName,
                               UserName = FormData.FirstName,
                               LastName = FormData.LastName,
                               EmailAddress = FormData.EmailAddress,
                               Password = FormData.Password,
                               IsActive = FormData.IsActive,
                               IsVerified = FormData.IsVerified,
                               CreatedOn = DateTime.Now,

                           }
                            );

                        if (BusnPartnerId != null)
                        {
                            //--Update user name
                            var updatedRecordsUserName = context.Update("BusnPartner", "BusnPartnerId",
                             new
                             {
                                 //BusnPartnerId = Convert.ToInt32(BusnPartnerId),
                                 UserName = FormData.FirstName + "_" + Convert.ToInt32(BusnPartnerId),
                             },
                             Convert.ToInt32(BusnPartnerId));


                            if (FormData.BusnPartnerAddressAssociationBusnPartners?.Count() > 0)
                            {


                                foreach (var item in FormData.BusnPartnerAddressAssociationBusnPartners)
                                {
                                    var AddressAsocId = context.Insert("BusnPartnerAddressAssociation", "AddressAsocId", true,
                                    new
                                    {

                                        BusnPartnerId = BusnPartnerId,
                                        AddressTypeId = item.AddressTypeId,
                                        AddressOne = item.AddressOne,
                                        AddressTwo = item.AddressTwo,
                                        CountryId = item.CountryId,
                                        StateId = item.StateId,
                                        CityId = item.CityId,
                                        IsActive = item.IsActive,
                                        CreatedOn = DateTime.Now,
                                    }
                                     );
                                }
                            }

                            result.PrimaryKeyValue = Convert.ToInt32(BusnPartnerId);
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

        public async Task<List<CountryEntity>> GetAllCountriesDAL(CountryEntity FormData)
        {

            List<CountryEntity> result = new List<CountryEntity>();

            using (var context = _contextHelper.GetDataContextHelper())
            {
                try
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");



                    if (FormData.CountryId > 0)
                    {
                        SearchParameters.Append("AND MTBL.CountryId =  @0 ", FormData.CountryId);
                    }

                    if (!String.IsNullOrEmpty(FormData.CountryName))
                    {
                        SearchParameters.Append("AND MTBL.CountryName LIKE  @0", "%" + FormData.CountryName + "%");
                    }

                    var ppSql = PetaPoco.Sql.Builder.Select(@" MTBL.*")
                      .From(" Countries MTBL")
                      .Where("MTBL.CountryId is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.CountryId DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<CountryEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;


                }
                catch (Exception)
                {

                    throw;
                }

            }

        }

        public async Task<ServicesResponse>? InserUpdateBusnPartnerDAL(BusnPartnerRequestForm FormData)
        {

            ServicesResponse? result = new ServicesResponse();

            try
            {

                using (IDbConnection dbConnection = _dapperConnectionHelper.GetDapperContextHelper())
                {
                    var updatedRecords = 0;
                    dbConnection.Open();

                 
                    int affectedRowId =  dbConnection.QuerySingle<int>("SP_CreateUpdateBusnPartner",
                        new
                        {
                            BusnPartnerId = FormData?.BusnPartnerId,
                            BusnPartnerTypeId = FormData?.BusnPartnerTypeId,
                            FirstName = FormData?.FirstName,
                            LastName = FormData?.LastName,
                            EmailAddress = FormData?.EmailAddress,
                            IsActive = FormData?.IsActive,
                            IsVerified = FormData?.IsVerified,
                            CountryId = FormData?.CountryId,
                            AddressOne = FormData?.AddressOne,
                            PhoneNo = FormData?.PhoneNo,
                            Password = FormData?.Password,
                            ProfilePictureId = FormData?.ProfilePictureId,
                            CreateByUserId = FormData?.CreateByUserId,
                         
                        }
                        , commandType: CommandType.StoredProcedure);
                    dbConnection.Close();


                    result.PrimaryKeyValue = FormData?.BusnPartnerId > 0 ? FormData?.BusnPartnerId : affectedRowId;
                    result.Success = true;
                    result.ResponseMessage = "Saved Successfully!";



                    await Task.FromResult(result);
                    return result;


                }


            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<List<BusnPartnerAddressAssociation>> GetBusinessPartnerAddressAssociationDAL(int BusnPartnerId)
        {
            List<BusnPartnerAddressAssociation> result = new List<BusnPartnerAddressAssociation>();


            using (var context = _contextHelper.GetDataContextHelper())
            {
                try
                {


                    var ppSql = PetaPoco.Sql.Builder.Select(@" MTBL.*, BPAT.AddressTypeName")
                      .From(" BusnPartnerAddressAssociation MTBL")
                      .LeftJoin("BusnPartnerAddressType BPAT").On("BPAT.AddressTypeId = MTBL.AddressTypeId")
                      .Where("MTBL.BusnPartnerId = @0", BusnPartnerId);
                    result = context.Fetch<BusnPartnerAddressAssociation>(ppSql);

                    await Task.FromResult(result);
                    return result;


                }
                catch (Exception)
                {

                    throw;
                }

            }

        }

        public async Task<List<BusnPartnerPhoneAssociationEntity>> GetBusinessPartnerPhonesAssociationDAL(int BusnPartnerId)
        {
            List<BusnPartnerPhoneAssociationEntity> result = new List<BusnPartnerPhoneAssociationEntity>();


            using (var context = _contextHelper.GetDataContextHelper())
            {
                try
                {


                    var ppSql = PetaPoco.Sql.Builder.Select(@" MTBL.*, BPPT.PhoneTypeName")
                      .From(" BusnPartnerPhoneAssociation MTBL")
                      .LeftJoin("BusnPartnerPhoneType BPPT").On("BPPT.PhoneTypeId = MTBL.PhoneTypeId")
                      .Where("MTBL.BusnPartnerId = @0", BusnPartnerId);
                    result = context.Fetch<BusnPartnerPhoneAssociationEntity>(ppSql);

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
