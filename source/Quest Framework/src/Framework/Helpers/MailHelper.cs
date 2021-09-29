/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Framework.Helpers
{
    public static class MailHelper
    {
        public static void AddMail(string mailDescription)
        {
            string[] letters = mailDescription.Split(',');

            foreach (string letter in letters)
            {
                string[] letterData = letter.Trim().Split(' ');

                if (letterData.Length < 1)
                    continue;

                string letterName = letterData[0];
                bool noLetter = letterData.Length > 1 && letterData.Contains("noletter");
                bool tomorrow = letterData.Length > 1 && letterData.Contains("tomorrow");
                bool sendToEveryone = letterData.Length > 1 && letterData.Contains("everyone");

                if (tomorrow)
                    Game1.addMailForTomorrow(letterName, noLetter, sendToEveryone);
                else
                    Game1.addMail(letterName, noLetter, sendToEveryone);
            }
        }

        public static void RemoveMail(string mailDescription)
        {
            string[] letters = mailDescription.Split(',');

            foreach (string letter in letters)
            {
                string[] letterData = letter.Trim().Split(' ');

                if (letterData.Length < 1)
                    continue;

                string letterName = letterData[0];
                bool boradcasted = letterData.Length > 1 && letterData.Contains("everyone");

                Game1.player.RemoveMail(letterName, boradcasted);
            }
        }
    }
}
