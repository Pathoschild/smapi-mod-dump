/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-bundles
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

            Helper.Events.GameLoop.Saving += Saving;
        }

        public static void LoadModData()
        {
            ModData.Instance = Helper.Data.ReadSaveData<ModData>(SaveKey) ?? new ModData();
            ModData.checkLegacySaveData();
        }
        private static void Saving(object sender, SavingEventArgs e)
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
            var farm = Game1.getFarm();
            if (farm.isThereABuildingUnderConstruction()
                && farm.getBuildingUnderConstruction().indoors.Value != null
                && !ShopPlacement.modifiedLocations.Contains(farm.getBuildingUnderConstruction().indoors.Value))
                ShopPlacement.modifiedLocations.Add(farm.getBuildingUnderConstruction().indoors.Value);

            foreach (var loc in ShopPlacement.modifiedLocations)
                foreach (var obj in loc.Objects.Values.Where(el => el is ShopObject))
                    loc.removeObject(obj.TileLocation, false);
        }
    }
}
