/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/idermailer/RandomStartDay
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using xTile;
using xTile.Dimensions;

namespace RandomStartDay
{
    internal class HarmonyMethodPatches
    {
        private static IModHelper Helper;
        private static IMonitor Monitor;
        private static string resultString = "";

        public static void Initialize(IModHelper helper, IMonitor monitor)
        {
            Helper = helper;
            Monitor = monitor;
        }
        public static void changeTodaysTip(ref string __result)
        {
            try
            {
                Dictionary<string, string> dic = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\TV\\TipChannel");
                var date = SDate.Now();
                int year = (date.Year + 1) % 2;
                int season = date.SeasonIndex;
                int day = date.Day;
                int todayNumber = 112 * year + (28 * season) + day;
                if (dic.ContainsKey(todayNumber.ToString())) 
                {
                    resultString = dic[todayNumber.ToString()];
                }
                else
                {
                    resultString = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13148");

                }
                __result = resultString;
                return;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(changeTodaysTip)}:\n{ex}", LogLevel.Error);
                resultString = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13148");
                __result = resultString;
                return;
            }
        }
    }

}
