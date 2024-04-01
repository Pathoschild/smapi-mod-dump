/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/personal-indoor-farm
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalIndoorFarm.Lib
{
    internal class SpaceTimeSynchronizer
    {
        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public const string ItemId = "DLX.PIF_Synchronizer";
        public const string QualifiedItemId = "(O)" + ItemId;
        public const string BuffId = "DLX.PIF_SpaceTimeBuff";
        public const string BuffTriggerAction = BuffId + "Trigger";
        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;
            TriggerActionManager.RegisterAction(BuffTriggerAction, ProcessAction);
        }

        private static bool ProcessAction(string[] args, TriggerActionContext context, out string error)
        {
            error = null;

            var location = Game1.currentLocation;
            if (location.NameOrUniqueName?.StartsWith(PersonalFarm.BaseLocationKey) == false)
                return true;

            if (!location.modData.ContainsKey(PersonalFarm.OwnerKey) || location.modData[PersonalFarm.OwnerKey] != Game1.player.UniqueMultiplayerID.ToString())
                return true;

            PersonalFarm.setInitialDayAndSeason(location);
            return true;
        }
    }
}
