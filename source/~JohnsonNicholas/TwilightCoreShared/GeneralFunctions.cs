using System;

namespace TwilightShards.Common
{
    class GeneralFunctions
    {
        public static string FirstLetterToUpper(string str)
        {
            if (String.IsNullOrEmpty(str))
                throw new ArgumentException("ARGH!");

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        public static double ConvCtF(double temp) => ((temp * 1.8) + 32);
        public static double ConvFtC(double temp) => ((temp - 32) / 1.8);
        public static double ConvCtK(double temp) => (temp + 273.15);
        public static double ConvFtK(double temp) => ((temp + 459.67) * (5.0 / 9.0));
        public static double ConvKtC(double temp) => (temp - 273.15);
        public static double ConvKtF(double temp) => ((temp * 1.8) - 459.67);

        public static bool ContainsOnlyMatchingFlags(Enum enumType, int FlagVal)
        {
            int enumVal = Convert.ToInt32(enumType);
            if (enumVal == FlagVal)
            {
                return true;
            }

            return false;
        }
    }
}
