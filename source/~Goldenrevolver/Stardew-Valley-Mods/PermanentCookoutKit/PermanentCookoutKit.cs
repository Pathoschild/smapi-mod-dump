/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace PermanentCookoutKit
{
    using StardewModdingAPI;
    using StardewValley;
    using System;
    using System.Collections.Generic;
    using StardewObject = StardewValley.Object;

    public static class ExtensionMethods
    {
        public static bool IsCookoutKit(this StardewObject o)
        {
            return o != null && o.ParentSheetIndex == 278;
        }
    }

    public class PermanentCookoutKit : Mod, IAssetEditor
    {
        public CookoutKitConfig Config { get; set; }

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<CookoutKitConfig>();

            CookoutKitConfig.VerifyConfigValues(Config, this);

            Helper.Events.GameLoop.GameLaunched += delegate { CookoutKitConfig.SetUpModConfigMenu(Config, this); };

            Helper.Events.GameLoop.DayEnding += delegate { SaveCookingKits(); };

            Patcher.PatchAll(this);
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data/CraftingRecipes");
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/CraftingRecipes"))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                data["Cookout Kit"] = "390 10 388 10 771 10 382 3 335 1/Field/926/false/Foraging 6/Cookout Kit";
            }
        }

        public void DebugLog(object o)
        {
            Monitor.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
        }

        public void ErrorLog(object o, Exception e = null)
        {
            string baseMessage = o == null ? "null" : o.ToString();

            string errorMessage = e == null ? string.Empty : $"\n{e.Message}\n{e.StackTrace}";

            Monitor.Log(baseMessage + errorMessage, LogLevel.Error);
        }

        private static void SaveCookingKits()
        {
            // check locations list first, then specifically the repeatedly buildable farm buildings
            // this differentiation is also done a lot in the base game code, e.g. in Game1.getCharacterFromName in Game1.cs

            // all locations except repeatedly buildable farm buildings
            foreach (var location in Game1.locations)
            {
                foreach (var item in location.Objects.Values)
                {
                    SaveSingleKit(item, location);
                }
            }

            // repeatedly buildable farm buildings
            if (Game1.getFarm() != null)
            {
                foreach (var building in Game1.getFarm().buildings)
                {
                    var interior = building.indoors.Value;

                    if (interior != null)
                    {
                        foreach (var item in interior.Objects.Values)
                        {
                            SaveSingleKit(item, interior);
                        }
                    }
                }
            }
        }

        private static void SaveSingleKit(StardewObject item, GameLocation location)
        {
            if (item.IsCookoutKit())
            {
                // extinguishes the fire, does not truly remove the object
                item.performRemoveAction(item.TileLocation, location);

                item.destroyOvernight = false;
            }
        }
    }
}