using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.UsersManagement
{
    public class BusnPartnerPhoneAssociationEntity
    {
        public int PhoneId { get; set; }
        public int BusnPartnerId { get; set; }
        public int PhoneTypeId { get; set; }
        public string? PhoneTypeName { get; set; }
        public string? PhoneNo { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
