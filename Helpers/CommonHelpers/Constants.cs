using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.CommonHelpers
{
    public class Constants : IConstants
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contx;
        public Constants(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this._configuration = configuration;
            this._contx = httpContextAccessor;
        }



        public int ITEMS_PER_PAGE()
        {
            int pageSize = 0;
            try
            {

                if (_contx != null && _contx.HttpContext != null && _contx.HttpContext.Session != null && _contx.HttpContext.Session.GetInt32("ITEMS_PER_PAGE") != null && _contx.HttpContext.Session.GetInt32("ITEMS_PER_PAGE")>0) //--Session ITEMS_PER_PAGE is setting in Dynamic controller
                {
                    pageSize = Convert.ToInt32(_contx.HttpContext.Session.GetInt32("ITEMS_PER_PAGE"));
                }
                else
                {
                    string itemValue = _configuration.GetSection("AppSetting").GetSection("ListingItemsPerPage").Value;
                    pageSize = !String.IsNullOrEmpty(itemValue) ? Convert.ToInt32(itemValue) : 10;

                }


                if (pageSize == 0 || pageSize < 1)
                {
                    return 10;
                }
                else
                {
                    return pageSize;
                }
            }
            catch (Exception)
            {

                return 10;
            }
        }

        public string GetAppSettingKeyValue(string MainSectionName, string SubSectionName)
        {
            try
            {
                string keyValue = _configuration?.GetSection(MainSectionName)?.GetSection(SubSectionName)?.Value ?? "";
                return keyValue;
            }
            catch (Exception)
            {
                throw;
            }
           
        }

        public int SiteMainLoaderDuration()
        {
            return (1500);

        }

    }

    public interface IConstants
    {

        public int ITEMS_PER_PAGE();
        public string GetAppSettingKeyValue(string MainSectionName, string SubSectionName);
        int SiteMainLoaderDuration();
    }
}
