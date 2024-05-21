using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.CommonHelpers.Enums
{
    public enum LanguagesEnum : short
    {
        [Description("en")]
        English = 1,
        [Description("ar")]
        Arabic = 2,
        [Description("spn")]
        Spanish = 3,
        [Description("frn")]
        French = 4,
        [Description("rus")]
        Russian = 5,
        [Description("turk")]
        Turkish = 6,
    }


    public enum SqlDeleteTypes : short
    {

        PlainTableDelete = 1,
        ForeignKeyDelete = 2,

    }


    public enum DataOperationType : short
    {
        Insert = 1,
        Update = 2,
        Delete = 3,
        ViewAll = 5
    }

    public enum BusnPartnerAddressTypeEnum : short
    {
        Home = 1,
        Billing = 2,
        Shipping = 3,
        Mailing = 4
    }

    public enum BusnPartnerTypeEnum: short
    {
        Admin = 1,
        Customer = 2,
        Developer = 3,
        DemoUser = 4
    }

    public enum PaymentMethodsEnum : short
    {
        CashOnDelivery = 1,
        Stripe = 2,
        PayPal = 3,

    }

}
