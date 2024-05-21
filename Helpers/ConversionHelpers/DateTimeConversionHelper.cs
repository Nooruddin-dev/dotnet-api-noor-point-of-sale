using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.ConversionHelpers
{
    public static class DateTimeConversionHelper
    {

        public static bool CheckValueIsDateTime(string txtDate)
        {
            DateTime tempDate;
            return DateTime.TryParse(txtDate, out tempDate);
        }
    }
}
