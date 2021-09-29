/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Rafseazz/Ridgeside-Village-Mod
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley;

namespace RidgesideVillage
{
    //Corrects the location name in the "X has begun in Y" message
    internal static class HarmonyPatch_UntimedSO
    {
        const string FIXMINECART = "RSV.SpecialOrder.FixMinecart";

        private static IMonitor Monitor { get; set; }
        private static IModHelper Helper { get; set; }

        public static void ApplyPatch(Harmony harmony, IModHelper helper)
        {
            Helper = helper;

            Helper.Events.GameLoop.DayEnding += OnDayEnd;
        }

        private static void OnDayEnd(object sender, DayEndingEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            foreach(SpecialOrder o in Game1.player.team.specialOrders)
            {
                if (o.questKey.Value.Contains(FIXMINECART))
                {
                    o.dueDate.Value = Game1.Date.TotalDays + 100;
                }
            }
        }

    }
}
