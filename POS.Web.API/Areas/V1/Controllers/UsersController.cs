using DAL.Repository.IServices;
using Entities.ModuleSpecificModels.Users;
using Helpers.AuthorizationHelpers.JwtTokenHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Plugins;
using Newtonsoft.Json;
using Helpers.ConversionHelpers;
using DAL.Repository.Services;
using Entities.DBModels;
using Helpers.CommonHelpers;
using Helpers.CommonHelpers.Enums;
using Entities.DBModels.UsersManagement;
using Entities.ModuleSpecificModels.Common;
using Entities.ModuleSpecificModels.ProductsCatalog.RequestForms;
using Entities.ModuleSpecificModels.Users.RequestForms;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Net.Mail;
using DocumentFormat.OpenXml.Office.CustomUI;


namespace POS.Web.API.Areas.V1.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    [Area("V1")]
    public class UsersController : ControllerBase
    {
        private readonly ICommonServicesDAL _commonServicesDAL;
        private readonly IUsersServicesDAL _usersServicesDAL;
        private readonly IFilesHelpers _filesHelpers;

        public UsersController(ICommonServicesDAL commonServicesDAL, IUsersServicesDAL usersServicesDAL, IFilesHelpers filesHelpers)
        {
            _commonServicesDAL = commonServicesDAL;
            _usersServicesDAL = usersServicesDAL;
            _filesHelpers = filesHelpers;
        }

        [Route("get-users-types")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetUserTypes(int pageNo = 1, int pageSize = 10)
        {
            try
            {
                BusnPartnerTypeEntity FormData = new BusnPartnerTypeEntity
                {
                    PageNo = pageNo,
                    PageSize = pageSize,
                };
                var result = await _usersServicesDAL.GetBusinessPartnerTypesDAL(FormData);

                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }


        }

        [Route("user-login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginForm model)
        {

            if (model == null || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Incorrect username or password");
            }


            var user = await _commonServicesDAL.GetUserLoginDAL(model.UserName, CommonConversionHelper.Encrypt(model.Password));

            var token = JwtManager.GetJwtToken(JsonConvert.SerializeObject(user) ?? "{}");

            // Return the token to the client
            return Ok(new { Token = token, User = user });
        }

        [Route("get-all-business-partners")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(int? busnPartnerId, int? BusnPartnerTypeId, string? firstName, bool? isActive, int pageNo = 1, int pageSize = 10)
        {
            try
            {

                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);


                BusnPartnerEntity FormData = new BusnPartnerEntity
                {
                    PageNo = pageNo,
                    PageSize = pageSize,
                    BusnPartnerId = busnPartnerId ?? 0,
                    BusnPartnerTypeId = BusnPartnerTypeId ?? 0,
                    IsActive = isActive,
                    FirstName = firstName ?? "",

                };
                var result = await _usersServicesDAL.GetBusinessPartnersDAL(FormData);
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        var resultAddressAssociation = await _usersServicesDAL.GetBusinessPartnerAddressAssociationDAL(item.BusnPartnerId);
                        item.BusnPartnerAddressAssociationBusnPartners = resultAddressAssociation;

                        var resultPhoneAssociation = await _usersServicesDAL.GetBusinessPartnerPhonesAssociationDAL(item.BusnPartnerId);
                        item.BusnPartnerPhoneAssociation = resultPhoneAssociation;

                        if (!String.IsNullOrWhiteSpace(item.Password))
                        {
                            try
                            {
                                string DecryptedPassword = CommonConversionHelper.Decrypt(item.Password);
                                item.Password = ""; //--Make user password empty in api response.
                                item.TestWordHooP = DecryptedPassword;
                            }
                            catch (Exception ex)
                            {

                                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                                
                            }
                          
                        }
                      
                    }

                   
                }

                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }


        }

        [Route("get-countries-list")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetAllCountries(int? CountryId, string? CountryName = "", int PageNo = 1, int PageSize = 10)
        {
            try
            {
                CountryEntity FormData = new CountryEntity
                {
                    CountryId = CountryId ?? 0,
                    CountryName = CountryName ?? "",
                    PageNo = PageNo,
                    PageSize = PageSize,
                };
                var result = await _usersServicesDAL.GetAllCountriesDAL(FormData);

                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }


        }


        [Route("insert-update-business-partner")]
        [HttpPost]
        public async Task<IActionResult> InserUpdateBusinessPartner([FromForm] BusnPartnerRequestForm model)
        {
            ServicesResponse? response = new ServicesResponse();
           

            try
            {
                if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName) || string.IsNullOrWhiteSpace(model.EmailAddress)
                || model.CountryId == null || model.CountryId < 1 || string.IsNullOrWhiteSpace(model.Password) || model.IsActive == null)
                {
                    response.Success = false;
                    response.ResponseMessage = "Please fill all required fields";
                    response.PrimaryKeyValue = null;
                    return Ok(new { Response = response });
                }

                if (model.BusnPartnerTypeId < 1)
                {
                    response.Success = false;
                    response.ResponseMessage = "User type is required!";
                    response.PrimaryKeyValue = null;
                    return Ok(new { Response = response });
               
                }

                //--check if email already exists
                var BusnPartnerByEmail = await _usersServicesDAL.GetBusinessPartnerByEmailDAL(model.EmailAddress);
                if (model.BusnPartnerId > 0 && BusnPartnerByEmail != null)
                {
                    //--If different users but same email address
                    if (BusnPartnerByEmail.BusnPartnerId != model.BusnPartnerId &&  BusnPartnerByEmail.EmailAddress == model.EmailAddress)
                    {
                        response.Success = false;
                        response.ResponseMessage = "Email address already exists!";
                        response.PrimaryKeyValue = null;
                        return Ok(new { Response = response });
                    }
                }
                else
                {
                    if (BusnPartnerByEmail != null && BusnPartnerByEmail.BusnPartnerId > 0)
                    {
                        response.Success = false;
                        response.ResponseMessage = "Email address already exists!";
                        response.PrimaryKeyValue = null;
                        return Ok(new { Response = response });
                    }
                }
               

                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                //--login user id
                model.CreateByUserId = busnPartnerIdHeader;
                #region image checking
                if (model.UserProfileImage != null)
                {
                    string url = await _filesHelpers.SaveFileToDirectory(model.UserProfileImage, null);
                    if (!String.IsNullOrWhiteSpace(url))
                    {
                        AttachmentEntity atch = new AttachmentEntity();
                        atch.AttachmentUrl = url;
                        atch.AttachmentName = model.UserProfileImage.FileName;
                        atch.BusnPartnerId = busnPartnerIdHeader;

                        if (model != null && model.ProfilePictureId > 0) //--update case
                        {
                            atch.AttachmentId = Convert.ToInt32(model.ProfilePictureId);


                            //--save attachment url in database
                            int AttachmentIdAttachmentTable = await this._commonServicesDAL.SaveUpdateAttachmentDAL(atch);
                        }
                        else
                        {
                            //--save attachment url in database
                            int AttachmentIdAttachmentTable = await this._commonServicesDAL.SaveUpdateAttachmentDAL(atch);

                            atch.AttachmentId = AttachmentIdAttachmentTable;

                            model.ProfilePictureId = AttachmentIdAttachmentTable;
                        }



                    }



                }
                else
                {
                    model.ProfilePictureId = model.BusnPartnerId > 0 ? model.ProfilePictureId : null;
                }
                #endregion

            
                model.Password = CommonConversionHelper.Encrypt(model.Password);

                response = await _usersServicesDAL.InserUpdateBusnPartnerDAL(model);

                return Ok(new { Response = response });
            }
            catch (Exception ex)
            {
                await this._commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.StackTrace);

                return BadRequest(ex.Message);
            }


        }


    }
}
