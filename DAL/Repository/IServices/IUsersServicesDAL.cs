using Entities.DBModels.UsersManagement;
using Entities.ModuleSpecificModels.Common;
using Entities.ModuleSpecificModels.Users;
using Entities.ModuleSpecificModels.Users.RequestForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.IServices
{
    public interface IUsersServicesDAL
    {
        Task<List<BusnPartnerEntity>> GetBusinessPartnersDAL(BusnPartnerEntity FormData);
        Task<BusnPartnerEntity?> GetBusinessPartnerByIdDAL(int BusnPartnerId);
        Task<BusnPartnerEntity?> GetBusinessPartnerByEmailPasswordDAL(string Email, string Password);
        Task<List<BusnPartnerTypeEntity>> GetBusinessPartnerTypesDAL(BusnPartnerTypeEntity FormData);
        Task<ServicesResponse>? AddUpdateBusinessPartnerDAL(BusnPartnerEntity FormData);
        Task<List<CountryEntity>> GetAllCountriesDAL(CountryEntity FormData);
        Task<ServicesResponse>? InserUpdateBusnPartnerDAL(BusnPartnerRequestForm FormData);
        Task<List<BusnPartnerAddressAssociation>> GetBusinessPartnerAddressAssociationDAL(int BusnPartnerId);
        Task<List<BusnPartnerPhoneAssociationEntity>> GetBusinessPartnerPhonesAssociationDAL(int BusnPartnerId);
        Task<BusnPartnerEntity?> GetBusinessPartnerByEmailDAL(string EmailAddress);
    }
}
