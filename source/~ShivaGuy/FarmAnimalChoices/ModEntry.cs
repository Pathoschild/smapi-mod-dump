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
using ShivaGuy.Stardew.FarmAnimalChoices.Patch;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ShivaGuy.Stardew.FarmAnimalChoices
{
    public class ModEntry : Mod
    {
        public static ModConfig Config { get; private set; }
        public static Mod Context { get; private set; }

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            Context = this;

            var harmony = new Harmony(ModManifest.UniqueID);

            FarmAnimalPatch.ApplyPatch(harmony);
            PurchaseAnimalsMenuPatch.ApplyPatch(harmony);

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

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Progression Mode",
                tooltip: () => "Recommended to keep it enabled if you want to follow the progression set by the game.",
                getValue: () => Config.ProgressionMode,
                setValue: value => Config.ProgressionMode = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Void Chicken Cost",
                tooltip: () => "Cost of a Void Chicken in Marnie's shop",
                getValue: () => Config.VoidChicken,
                setValue: value => Config.VoidChicken = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Golden Chicken Cost",
                tooltip: () => "Cost of a Golden Chicken in Marnie's shop",
                getValue: () => Config.GoldenChicken,
                setValue: value => Config.GoldenChicken = value
            );
        }
    }
}
