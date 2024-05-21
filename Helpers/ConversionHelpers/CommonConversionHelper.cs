using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Helpers.CommonHelpers;
using System.Data;
using DocumentFormat.OpenXml.Bibliography;
using Helpers.CommonHelpers.Enums;
using System.Globalization;


namespace Helpers.ConversionHelpers
{
    public static class CommonConversionHelper
    {
     
        private const string KEY = "ARG@PLIS";
        private const string IV = "HUN@IDIS";

        public static string Encrypt(string text)
        {
            if (text.Trim().Length == 0)
                return string.Empty;
            byte[] bKey, bIV, bInput;

            bKey = System.Text.Encoding.UTF8.GetBytes(KEY);
            bIV = System.Text.Encoding.UTF8.GetBytes(IV);
            bInput = System.Text.Encoding.UTF8.GetBytes(text);

            MemoryStream memStream = new MemoryStream();

            DES des = new DESCryptoServiceProvider();
            CryptoStream encStream = new CryptoStream(memStream, des.CreateEncryptor(bKey, bIV), CryptoStreamMode.Write);

            encStream.Write(bInput, 0, bInput.Length);
            encStream.FlushFinalBlock();
            string st = Convert.ToBase64String(memStream.ToArray());

            return st;


        }

        public static string Decrypt(string encText)
        {
            if (encText.Trim().Length == 0)
                return string.Empty;
            byte[] bKey, bIV, bInput;

            bKey = System.Text.Encoding.UTF8.GetBytes(KEY);
            bIV = System.Text.Encoding.UTF8.GetBytes(IV);
            bInput = Convert.FromBase64String(encText);

            MemoryStream memStream = new MemoryStream();

            DES des = new DESCryptoServiceProvider();
            CryptoStream encStream = new CryptoStream(memStream, des.CreateDecryptor(bKey, bIV), CryptoStreamMode.Write);

            encStream.Write(bInput, 0, bInput.Length);
            encStream.FlushFinalBlock();

            string MemoryStreamUTF8 = System.Text.Encoding.UTF8.GetString(memStream.ToArray());
            return MemoryStreamUTF8;
        }

        public static int GenerateRandomNumber()
        {
            Random r = new Random();
            int randNum = r.Next(1000000);
            return randNum;
        }

        public static string GetDefaultCurrencySymbol()
        {
            string DefaultCurrencySymbol = "$";  //--USD is consider as default if there is no setting in appsetting.json file
            try
            {
                DefaultCurrencySymbol = StaticMethodsDependencyInjctHelper.config?.GetSection("AppSetting")?.GetSection("DefaultCurrencySymbol")?.Value ?? "$";
                return DefaultCurrencySymbol;
            }
            catch
            {
                throw;
            }

        }

        public static string GetDefaultCurrencyCode()
        {
            string DefaultCurrencyCode = "USD";  //--USD is consider as default if there is no setting in appsetting.json file
            try
            {
                DefaultCurrencyCode = StaticMethodsDependencyInjctHelper.config?.GetSection("AppSetting")?.GetSection("DefaultCurrencyCode")?.Value ?? "USD";
                return DefaultCurrencyCode;
            }
            catch
            {
                throw;
            }

        }

        public static List<T> ConvertDataTableToListType<T>(DataTable dt)
        {

            try
            {
                var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
                var properties = typeof(T).GetProperties();
                return dt.AsEnumerable().Select(row => {
                    var objT = Activator.CreateInstance<T>();
                    foreach (var pro in properties)
                    {
                        if (columnNames.Contains(pro.Name.ToLower()))
                        {
                            try
                            {
                               // pro.SetValue(objT, row[pro.Name]);
                                pro.SetValue(objT, Convert.ChangeType(row[pro.Name], pro.PropertyType), null);
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    }
                    return objT;
                }).ToList();
            }
            catch
            {
                throw;
            }

           
        }

        public static int GetLanguageIdbyLanguageCode(string LangCode)
        {
            int LanguageID = 1; //--1 is default for eng

          
            switch (LangCode)
            {
                case "en":
                    LanguageID = (short)LanguagesEnum.English;
                    break;
                case "ar":
                    LanguageID = (short)LanguagesEnum.Arabic;
                    break;
                case "es":
                    LanguageID = (short)LanguagesEnum.Spanish;
                    break;
                case "fr":
                    LanguageID = (short)LanguagesEnum.French;
                    break;
                case "ru":
                    LanguageID = (short)LanguagesEnum.Russian;
                    break;
                case "tr":
                    LanguageID = (short)LanguagesEnum.Turkish;
                    break;
                default:
                    LanguageID = (short)LanguagesEnum.English;
                    break;
            }

            return LanguageID;
        }

        public static string GetMonthName(int monthId)
        {
            // Validate the input monthId
            if (monthId < 1 || monthId > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(monthId), "Month ID must be between 1 and 12.");
            }

            // Return the month name using DateTime and CultureInfo
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthId);
        }
    }
}
