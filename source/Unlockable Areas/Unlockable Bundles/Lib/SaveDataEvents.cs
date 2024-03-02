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

namespace Unlockable_Bundles.Lib
{
    public class SaveDataEvents
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        const string SaveKey = "main";

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;

            Helper.Events.GameLoop.DayEnding += DayEnding;
        }

        public static void LoadModData()
        {
            ModData.Instance = Helper.Data.ReadSaveData<ModData>(SaveKey) ?? new ModData();
            ModData.checkLegacySaveData();
        }

        //Solid Foundations has [EventPriority(EventPriority.High + 1)], but we want to run before it
        [EventPriority(EventPriority.High + 2)]
        private static void DayEnding(object sender, DayEndingEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            clearUnlockableShops();

            Helper.Data.WriteSaveData(SaveKey, ModData.Instance);
        }

        //Unlockable ShopObjects will be re-added every DayStart.
        //We delete them during Saving to keep the savefiles clean from custom objects
        private static void clearUnlockableShops()
        {
            foreach (var loc in Game1.locations)
                foreach (var building in loc.buildings.Where(el => el.isUnderConstruction() && el.indoors.Value is not null))
                    if(!ShopPlacement.ModifiedLocations.Contains(building.indoors.Value))
                        ShopPlacement.ModifiedLocations.Add(building.indoors.Value);


            foreach (var loc in ShopPlacement.ModifiedLocations)
                foreach (var obj in loc.Objects.Values.Where(el => el is ShopObject))
                    loc.removeObject(obj.TileLocation, false);

        }
    }
}
