/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/TransparencySettings
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using System;

namespace TransparencySettings
{
    public partial class ModEntry : Mod
    {
        // <summary>A SMAPI GameLaunched event that enables GMCM support if that mod is available.</summary>
        public void EnableGMCM(object sender, GameLaunchedEventArgs e)
        {
            try
            {
                GenericModConfigMenuAPI api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu"); //attempt to get GMCM's API instance

                if (api == null) //if the API is not available
                    return;

                api.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config)); //register "revert to default" and "write" methods for this mod's config
                api.SetDefaultIngameOptinValue(ModManifest, true); //allow in-game setting changes (rather than just at the main menu)

                //register an option for each of this mod's config settings

                //buildings
                api.RegisterLabel
                (
                    ModManifest,
                    "Building settings",
                    "Transparency settings for player-constructed farm buildings.\nNote: These settings only affect buildings that can be constructed or moved by players."
                );

                api.RegisterSimpleOption
                (
                    ModManifest,
                    "Enable",
                    "Check this box to enable custom settings for building transparency.\nUncheck this box to use Stardew's default transparency.",
                    () => Config.BuildingSettings.Enable,
                    (bool val) => Config.BuildingSettings.Enable = val
                );

                api.RegisterSimpleOption
                (
                    ModManifest,
                    "Below player only",
                    "If this box is checked, buildings will only be transparent if they're vertically lower than your character.",
                    () => Config.BuildingSettings.BelowPlayerOnly,
                    (bool val) => Config.BuildingSettings.BelowPlayerOnly = val
                );

                api.RegisterSimpleOption
                (
                    ModManifest,
                    "Tile distance",
                    "If the number of tiles between your character and a building is less than this setting, the building will be transparent.",
                    () => Config.BuildingSettings.TileDistance,
                    (int val) => Config.BuildingSettings.TileDistance = val
                );

                //bushes
                api.RegisterLabel
                (
                    ModManifest,
                    "Bush settings",
                    "Transparency settings for bushes.\nNote: These settings only affect bushes that players can interact with."
                );

                api.RegisterSimpleOption
                (
                    ModManifest,
                    "Enable",
                    "Check this box to enable custom settings for bush transparency.\nUncheck this box to use Stardew's default transparency.",
                    () => Config.BushSettings.Enable,
                    (bool val) => Config.BushSettings.Enable = val
                );

                api.RegisterSimpleOption
                (
                    ModManifest,
                    "Below player only",
                    "If this box is checked, bushes will only be transparent if they're vertically lower than your character.",
                    () => Config.BushSettings.BelowPlayerOnly,
                    (bool val) => Config.BushSettings.BelowPlayerOnly = val
                );

                api.RegisterSimpleOption
                (
                    ModManifest,
                    "Tile distance",
                    "If the number of tiles between your character and a bush is less than this setting, the bush will be transparent.",
                    () => Config.BushSettings.TileDistance,
                    (int val) => Config.BushSettings.TileDistance = val
                );

                //trees
                api.RegisterLabel
                (
                    ModManifest,
                    "Tree settings",
                    "Transparency settings for trees.\nNote: These settings only affect trees that players can interact with."
                );

                api.RegisterSimpleOption
                (
                    ModManifest,
                    "Enable",
                    "Check this box to enable custom settings for tree transparency.\nUncheck this box to use Stardew's default transparency.",
                    () => Config.TreeSettings.Enable,
                    (bool val) => Config.TreeSettings.Enable = val
                );

                api.RegisterSimpleOption
                (
                    ModManifest,
                    "Below player only",
                    "If this box is checked, trees will only be transparent if they're vertically lower than your character.",
                    () => Config.TreeSettings.BelowPlayerOnly,
                    (bool val) => Config.TreeSettings.BelowPlayerOnly = val
                );

                api.RegisterSimpleOption
                (
                    ModManifest,
                    "Tile distance",
                    "If the number of tiles between your character and a tree is less than this setting, the tree will be transparent.",
                    () => Config.TreeSettings.TileDistance,
                    (int val) => Config.TreeSettings.TileDistance = val
                );

                //keybinds
                api.RegisterLabel
                (
                    ModManifest,
                    "Key bindings",
                    "Keybind settings for this mod's optional toggle controls."
                );

                api.RegisterSimpleOption
                (
                    ModManifest,
                    "Disable transparency",
                    "A list of keybinds that toggle between this mod's settings and normal Stardew transparency.\n\nSee the wiki for info about multi-key bindings: https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings",
                    () => Config.KeyBindings.DisableTransparency,
                    (KeybindList val) => Config.KeyBindings.DisableTransparency = val
                );

                api.RegisterSimpleOption
                (
                    ModManifest,
                    "Full transparency",
                    "A list of keybinds that toggle between this mod's settings and maximum transparency on all objects.\n\nSee the wiki for info about multi-key bindings: https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings",
                    () => Config.KeyBindings.FullTransparency,
                    (KeybindList val) => Config.KeyBindings.FullTransparency = val
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"An error happened while loading this mod's GMCM options menu. Its menu might be missing or fail to work. The auto-generated error message has been added to the log.", LogLevel.Warn);
                Monitor.Log($"----------", LogLevel.Trace);
                Monitor.Log($"{ex.ToString()}", LogLevel.Trace);
            }
        }
    }

    /// <summary>Generic Mod Config Menu's API interface. Used to recognize & interact with the mod's API when available.</summary>
    public interface GenericModConfigMenuAPI
    {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);
        void SetDefaultIngameOptinValue(IManifest mod, bool optedIn);

        void RegisterLabel(IManifest mod, string labelName, string labelDesc);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<KeybindList> optionGet, Action<KeybindList> optionSet);
    }
}
