using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.CommonHelpers
{
    public static class GlobalUrlHelper
    {
        public static int GetBusnPartnerIdFromApiHeader(HttpContext httpContext)
        {

            try
            {
                if (httpContext == null)
                    return 0;

                string busnPartnerId = httpContext?.Request?.Headers["busnPartnerId"];
                if (!string.IsNullOrEmpty(busnPartnerId))
                {
                    return Convert.ToInt32(busnPartnerId);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {

                return 0;
            }

           
        }
    }
}
