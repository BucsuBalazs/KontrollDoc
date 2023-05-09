using System;

namespace FIT_Common
{
    public static class StringExtensions
    {
        public static string Left(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            maxLength = Math.Abs(maxLength);

            return (value.Length <= maxLength
                   ? value
                   : value.Substring(0, maxLength)
                   );
        }

        public static string SHA512UnicodeHash(this string value)
        {
            System.Security.Cryptography.SHA512 alg = System.Security.Cryptography.SHA512Managed.Create();
            byte[] hashValue;
            hashValue = alg.ComputeHash(System.Text.Encoding.Unicode.GetBytes(value));
            return BitConverter.ToString(hashValue).Replace("-", "").ToUpper();
        }

        public static object GetAsByteParamValue(this string value)
        {
            object ret;
            byte bValue;
            if (value.Length == 0)
                ret = 0;
            else if (byte.TryParse(value, out bValue))
                ret = bValue;
            else
                ret = DBNull.Value;
            return ret;
        }

        public static object GetAsByteParamValue(this string value, object defaultValue)
        {
            object ret;
            byte bValue;
            if (value.Length == 0)
                ret = defaultValue;
            else if (byte.TryParse(value, out bValue))
                ret = bValue;
            else
                ret = defaultValue;
            return ret;
        }

        public static object GetAsDecimalParamValue(this string value)
        {
            object ret;
            decimal dValue;
            if (value.Length == 0)
                ret = 0;
            else if (decimal.TryParse(value, out dValue))
                ret = dValue;
            else
                ret = DBNull.Value;
            return ret;
        }

        public static object GetAsDecimalParamValue(this string value,object defaultValue)
        {
            object ret;
            decimal dValue;
            if (value.Length == 0)
                ret = defaultValue;
            else if (decimal.TryParse(value, out dValue))
                ret = dValue;
            else
                ret = defaultValue;
            return ret;
        }
    }
}
