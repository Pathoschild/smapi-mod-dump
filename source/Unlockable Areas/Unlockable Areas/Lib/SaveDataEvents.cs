/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Unlockable_Areas.Lib
{
    public class SaveDataEvents
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;
        public static ModData Data = new ModData();

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;

            Helper.Events.GameLoop.Saving += Saving;
        }
        //The Moddata is being loaded in DayStartedEvents -> ShopPlacement daystarted
        private static void Saving(object sender, SavingEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            clearUnlockableShops();

            Helper.Data.WriteSaveData("main", Data);
        }

        //Unlockable ShopObjects will be re-added every DayStart.
        //We delete them during Saving to keep the savefiles clean from custom objects
        private static void clearUnlockableShops()
        {
            var unlockables = Unlockable.convertModelDicToEntity(Helper.GameContent.Load<Dictionary<string, UnlockableModel>>("UnlockableAreas/Unlockables"));
            foreach (var unlockable in unlockables) {
                var location = Game1.getLocationFromName(unlockable.Value.Location);
                var obj = location.getObjectAtTile((int)unlockable.Value.vShopPosition.X, (int)unlockable.Value.vShopPosition.Y);
                if (obj != null && obj.GetType() == typeof(ShopObject))
                    location.removeObject(unlockable.Value.vShopPosition, false);
            }

        }
    }
}
