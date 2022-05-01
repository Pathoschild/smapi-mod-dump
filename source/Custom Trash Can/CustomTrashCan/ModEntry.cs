/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ShivaGuy/StardewMods
**
*************************************************/

using GenericModConfigMenu;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewObject = StardewValley.Object;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;

namespace ShivaGuy.Stardew.CustomTrashCan
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

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Level 0 Multiplier",
                tooltip: () => "Trash Can (Not Upgraded)",
                min: 0.0f,
                max: 1.0f,
                interval: 0.01f,
                formatValue: value => value.ToString("0.##"),
                getValue: () => Config.Level0Multiplier,
                setValue: value => Config.Level0Multiplier = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Level 1 Multiplier",
                tooltip: () => "Trash Can (Copper)",
                min: 0.0f,
                max: 1.0f,
                interval: 0.01f,
                formatValue: value => value.ToString("0.##"),
                getValue: () => Config.Level1Multiplier,
                setValue: value => Config.Level1Multiplier = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Level 2 Multiplier",
                tooltip: () => "Trash Can (Iron)",
                min: 0.0f,
                max: 1.0f,
                interval: 0.01f,
                formatValue: value => value.ToString("0.##"),
                getValue: () => Config.Level2Multiplier,
                setValue: value => Config.Level2Multiplier = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Level 3 Multiplier",
                tooltip: () => "Trash Can (Gold)",
                min: 0.0f,
                max: 1.0f,
                interval: 0.01f,
                formatValue: value => value.ToString("0.##"),
                getValue: () => Config.Level3Multiplier,
                setValue: value => Config.Level3Multiplier = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Level 4 Multiplier",
                tooltip: () => "Trash Can (Iridium)",
                min: 0.0f,
                max: 1.0f,
                interval: 0.01f,
                formatValue: value => value.ToString("0.##"),
                getValue: () => Config.Level4Multiplier,
                setValue: value => Config.Level4Multiplier = value
            );
        }

        internal static bool Patch_getTrashReclamationPrice(Item i, Farmer f, out int __result)
        {
            __result = -1;

            double sellPercentage = 0.15 * f.trashCanLevel;

            if (f.trashCanLevel == 0)
                sellPercentage = Config.Level0Multiplier;
            else if (f.trashCanLevel == 1)
                sellPercentage = Config.Level1Multiplier;
            else if (f.trashCanLevel == 2)
                sellPercentage = Config.Level2Multiplier;
            else if (f.trashCanLevel == 3)
                sellPercentage = Config.Level3Multiplier;
            else if (f.trashCanLevel == 4)
                sellPercentage = Config.Level4Multiplier;


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
