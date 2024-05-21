using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.CashierMain
{
    public class CustomerInfoPos
    {
        public int BusnPartnerId { get; set; } 
        public string? FirstName { get; set; } 
        public string? MiddleName { get; set; } 
        public string? LastName { get; set; } 
        public string? EmailAddress { get; set; } 
        public string? ContactNo { get; set; } 

    }
}
