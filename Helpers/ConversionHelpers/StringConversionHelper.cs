using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helpers.ConversionHelpers
{
    public static class StringConversionHelper
    {

        public static string TruncateAnyStringValue(string? Value, int MaxLength, bool AllowTruncationSuffix )
        {
            string truncationSuffix = "…";

            if (string.IsNullOrEmpty(Value)) return (Value ?? String.Empty);

            Value = Value.Length <= MaxLength ? Value : (AllowTruncationSuffix == true ?  Value.Substring(0, MaxLength) + truncationSuffix : Value.Substring(0, MaxLength));
            return Value;
        }

        public static string GetValueFromJsonPairByKey(string JsonPair, string key)
        {
            string ReturnableValue = "";
            if (!String.IsNullOrEmpty(JsonPair))
            {
                JObject JsonFields = JObject.Parse(JsonPair);
                ReturnableValue = JsonFields != null && JsonFields.Property(key) != null ? JsonFields.Property(key).Value.Value<string>().ToString() : string.Empty;
            }
            return ReturnableValue;
        }

        public static string ReplaceSpacesInString(string input, char replacement = '_')
        {
            return input.Replace(" ", replacement.ToString());
        }

        public static string MakeFileNameValid(string input)
        {
            string pattern = @"[^a-zA-Z0-9_.-]";
            string replacement = "";
            string output = Regex.Replace(input, pattern, replacement);
          
            return output;
        }

    }
}
