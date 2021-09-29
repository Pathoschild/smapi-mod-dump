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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using StardewValley.Menus;

namespace RidgesideVillage
{
    internal static class SpecialOrders
    {
        const string FIXMINECART = "RSV.SpecialOrder.FixMinecart";

        static IModHelper Helper;
        static IMonitor Monitor;
        internal static void Initialize(IMod ModInstance)
        {
            Helper = ModInstance.Helper;
            Monitor = ModInstance.Monitor;

            //Helper.Events.GameLoop.DayStarted += OnDayStarted;
            Helper.Events.GameLoop.OneSecondUpdateTicked += OnUpdate;
        }

        private static void OnUpdate(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (Game1.player.eventsSeen.Contains(75160190) && !Game1.player.team.SpecialOrderActive(FIXMINECART) && !Game1.player.team.completedSpecialOrders.ContainsKey(FIXMINECART))
            {
                Game1.player.team.specialOrders.Add(SpecialOrder.GetSpecialOrder(FIXMINECART, null));
            }
        }
    }
}
