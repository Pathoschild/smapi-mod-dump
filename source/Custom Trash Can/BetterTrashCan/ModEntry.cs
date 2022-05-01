/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ShivaGuy/StardewMods
**
*************************************************/

using System;
using StardewModdingAPI;
using HarmonyLib;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using StardewObject = StardewValley.Object;
using StardewModdingAPI.Events;
using GenericModConfigMenu;

namespace BetterTrashCan
{
    public class ModEntry : Mod
    {
        private static ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            var original = AccessTools.Method(typeof(Utility), nameof(Utility.getTrashReclamationPrice));
            var patched = new HarmonyMethod(typeof(ModEntry), nameof(Patch_getTrashReclamationPrice));

            new Harmony(ModManifest.UniqueID).Patch(original, patched);

            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs evt)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenu == null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Progression Mode",
                getValue: () => Config.progression.ToString(),
                setValue: value => Config.progression = (Progression)Enum.Parse(typeof(Progression), value),
                allowedValues: new string[] { Progression.Linear.ToString(), Progression.Exponential.ToString() }
            );
        }

        internal static bool Patch_getTrashReclamationPrice(Item i, Farmer f, out int __result)
        {
            __result = -1;

            double sellPercentage = f.trashCanLevel / 4.0;

            if (Config.progression == Progression.Exponential)
                sellPercentage = f.trashCanLevel > 0 ? Math.Floor(Math.Pow(3.164, f.trashCanLevel)) / 100 : 0;

            if (i.canBeTrashed() && i is not Wallpaper && i is not Furniture)
            {
                if (i is StardewObject obj && !obj.bigCraftable.Value)
                {
                    __result = (int)(i.Stack * (obj.sellToStorePrice(-1L) * sellPercentage));
                }
                if (i is MeleeWeapon || i is Ring || i is Boots)
                {
                    __result = (int)(i.Stack * ((i.salePrice() / 2.0) * sellPercentage));
                }
            }

            return false;
        }
    }
}
