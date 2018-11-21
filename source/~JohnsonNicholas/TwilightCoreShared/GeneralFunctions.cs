using Microsoft.Xna.Framework;
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

        public static double DegreeToRadians(double deg) => deg * (Math.PI / 180);
        public static double RadiansToDegree(double rad) => (rad * 180) / Math.PI;

        public static string ConvMinToHrMin(double val)
        {
            int hour = (int)Math.Floor(val / 60.0);
            double min = val - (hour * 60);

            return $"{hour}h{min.ToString("00")}m";
        }

        public static Color GetRGBFromTemp(double temp)
        {
            float r,g,b = 0.0f;
            float effTemp = (float)Math.Min(Math.Max(temp, 1000), 40000);
            effTemp = effTemp / 100;

            //calc red
            if (effTemp <= 66)
                r = 255f;
            else
            { 
              float x = effTemp -55f;   
              r = 351.97690566805693f + (0.114206453784165f*x) - (40.25366309332127f * (float)Math.Log(x));
            }

            //clamp red
            r = (float)Math.Min(Math.Max(r, 0), 255);

            //calc green
            if (effTemp <= 66)
            {
                float x = effTemp - 2f;
                g = -155.25485562709179f - (.44596950469579133f * x) + (104.49216199393888f * (float)Math.Log(x));
            }
            else
            {
                float x = effTemp - 50f;
                g = 325.4494125711974f + (.07943456536662342f * x) - (28.0852963507957f * (float)Math.Log(x));
            }

            //clamp green
            g = (float)Math.Min(Math.Max(g,0), 255);

            //calc blue
            if (effTemp <= 19) 
                b = 0;
            else if (effTemp >= 66)
                b = 255;
            else
            {
                float x = effTemp - 10f;
                b = -254.76935184120902f + (.8274096064007395f * x) + (115.67994401066147f * (float)Math.Log(x));
            }


            //clamp blue
            b = (float)Math.Min(Math.Max(b, 0), 255);
            Color ourColor = new Color((uint)r, (uint)g, (uint)b);
            ourColor.R = (byte)r;
            ourColor.G = (byte)g;
            ourColor.B = (byte)b;
            return ourColor;

        }

        private static Color CalculateMaskFromColor(Color target)
        {
           return new Color(255 - target.R, 255 - target.G, 255 - target.B);
        }
    }
}
