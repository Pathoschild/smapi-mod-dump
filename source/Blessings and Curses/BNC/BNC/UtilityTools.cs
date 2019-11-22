using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;

namespace BNC
{
    public class UtilityTools
    {

        public static FileInfo getSaveDirectory()
        {
            string str = Game1.player.Name;
            foreach (char c in str)
            {
                if (!char.IsLetterOrDigit(c))
                    str = str.Replace(c.ToString() ?? "", "");
            }
            string path2 = str + "_" + (object)Game1.uniqueIDForThisGame;
            FileInfo fileInfo1 = new FileInfo(Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley"), "Saves"), path2));

            return fileInfo1;
        }

    public static int[] getNextDate(int days)
        {
            int day = Game1.dayOfMonth + 3;
            int month = StardewValley.Utility.getSeasonNumber(Game1.currentSeason);
            int year = Game1.year;

            if (day > 28)
            {
                day -= 28;
                month += 1;

                if (month > 4)
                {
                    month = 1;
                    year += 1;
                }
            }

            return new int[] { month, day, year };
        }

        public static  List<int> ParseDate(String time)
        {
            List<int> formated = new List<int>();
            string[] split = time.Split('-');
            foreach (string output in split)
                formated.Add(Int32.Parse(output));
            return formated;
        }

        public static String GetCurrentDate()
        {
            return $"{StardewValley.Utility.getSeasonNumber(Game1.currentSeason)}-{Game1.dayOfMonth}-{Game1.year}";
        }

    }
}
