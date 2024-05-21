using Entities.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.Users
{
    public class UserLoginForm
    {
        
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }

    public class UserLoginResponse
    {
        public int BusnPartnerId { get; set; }
        public int BusnPartnerTypeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? EmailAddress { get; set; }
        public string? UserName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Password { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean? IsWalkThroughCustomer { get; set; }
    }



  
    public class BusnPartnerSignUpResponse
    {
        public string? message { get; set; }
        public Boolean isSuccess { get; set; }
    }

    public class SubscribeUserEmailRequestForm
    {
      
        public int SubscriptionEmailID { get; set; }
        public string? EmailAddress { get; set; }
        public Boolean IsActive { get; set; }
        public int? busnPartnerId { get; set; }
        public int dataOperationType { get; set; }
      
    }

    public class ContactUsFormRequest
    {
        public string? FullName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Message { get; set; }
        public int BusnPartnerId { get; set; }

     
    }
}
