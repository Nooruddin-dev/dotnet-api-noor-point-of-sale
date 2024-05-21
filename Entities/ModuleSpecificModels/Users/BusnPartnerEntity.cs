using Entities.CommonModels.Pagination;
using Entities.DBModels;
using Entities.DBModels.UsersManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.Users
{
    public class BusnPartnerEntity :  IPageBasicData
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? EmailAddress { get; set; }
        public int BusnPartnerTypeId { get; set; }
        public string? BusnPartnerTypeName { get; set; }
        public string? Password { get; set; }
        public string? PhoneNo { get; set; }
        public Boolean? IsActive { get; set; }
        public Boolean? IsVerified { get; set; }

        public string? VerificationCode { get; set; }
        public int? CountryId { get; set; }

        public DateTime CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }


        public int? ProfilePictureId { get; set; }
        public string? ProfilePicturePath { get; set; }


        public string? TestWordHooP { get; set; }//--For sending password in unknown type name key

        public virtual ICollection<BusnPartnerAddressAssociation> BusnPartnerAddressAssociationBusnPartners { get; set; } = new List<BusnPartnerAddressAssociation>();
        public virtual ICollection<BusnPartnerPhoneAssociationEntity> BusnPartnerPhoneAssociation { get; set; } = new List<BusnPartnerPhoneAssociationEntity>();


        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }




    }

   

}
