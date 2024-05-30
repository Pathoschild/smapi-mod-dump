/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/TransparencySettings
**
*************************************************/

using GenericModConfigMenu;
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
                var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu"); //attempt to get GMCM's API instance

                if (api == null) //if the API is not available
                    return;

                api.Register(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config)); //register the mod's menu, define its reset/save actions, and allow in-game changes

                //register an option for each of this mod's config settings

                //buildings

                api.AddSectionTitle
                (
                    mod: ModManifest,
                    text: () => Helper.Translation.Get("Building.Title.Name"),
                    tooltip: () => Helper.Translation.Get("Building.Title.Desc")
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Building.Enable.Name"),
                    tooltip: () => Helper.Translation.Get("Building.Enable.Desc"),
                    getValue: () => Config.BuildingSettings.Enable,
                    setValue: (bool val) => Config.BuildingSettings.Enable = val
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Building.Below.Name"),
                    tooltip: () => Helper.Translation.Get("Building.Below.Desc"),
                    getValue: () => Config.BuildingSettings.BelowPlayerOnly,
                    setValue: (bool val) => Config.BuildingSettings.BelowPlayerOnly = val
                );

                api.AddNumberOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Building.Distance.Name"),
                    tooltip: () => Helper.Translation.Get("Building.Distance.Desc"),
                    getValue: () => Config.BuildingSettings.TileDistance,
                    setValue: (int val) => Config.BuildingSettings.TileDistance = val
                );

                api.AddNumberOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Building.MinimumOpacity.Name"),
                    tooltip: () => Helper.Translation.Get("Building.MinimumOpacity.Desc"),
                    getValue: () => Config.BuildingSettings.MinimumOpacity,
                    setValue: (float val) => Config.BuildingSettings.MinimumOpacity = val,
                    min: 0f,
                    max: 1f,
                    interval: 0.01f
                );

                //bushes

                api.AddSectionTitle
                (
                    mod: ModManifest,
                    text: () => Helper.Translation.Get("Bush.Title.Name"),
                    tooltip: () => Helper.Translation.Get("Bush.Title.Desc")
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Bush.Enable.Name"),
                    tooltip: () => Helper.Translation.Get("Bush.Enable.Desc"),
                    getValue: () => Config.BushSettings.Enable,
                    setValue: (bool val) => Config.BushSettings.Enable = val
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Bush.Below.Name"),
                    tooltip: () => Helper.Translation.Get("Bush.Below.Desc"),
                    getValue: () => Config.BushSettings.BelowPlayerOnly,
                    setValue: (bool val) => Config.BushSettings.BelowPlayerOnly = val
                );

                api.AddNumberOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Bush.Distance.Name"),
                    tooltip: () => Helper.Translation.Get("Bush.Distance.Desc"),
                    getValue: () => Config.BushSettings.TileDistance,
                    setValue: (int val) => Config.BushSettings.TileDistance = val
                );

                api.AddNumberOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Bush.MinimumOpacity.Name"),
                    tooltip: () => Helper.Translation.Get("Bush.MinimumOpacity.Desc"),
                    getValue: () => Config.BushSettings.MinimumOpacity,
                    setValue: (float val) => Config.BushSettings.MinimumOpacity = val,
                    min: 0f,
                    max: 1f,
                    interval: 0.01f
                );

                //trees

                api.AddSectionTitle
                (
                    mod: ModManifest,
                    text: () => Helper.Translation.Get("Tree.Title.Name"),
                    tooltip: () => Helper.Translation.Get("Tree.Title.Desc")
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Tree.Enable.Name"),
                    tooltip: () => Helper.Translation.Get("Tree.Enable.Desc"),
                    getValue: () => Config.TreeSettings.Enable,
                    setValue: (bool val) => Config.TreeSettings.Enable = val
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Tree.Below.Name"),
                    tooltip: () => Helper.Translation.Get("Tree.Below.Desc"),
                    getValue: () => Config.TreeSettings.BelowPlayerOnly,
                    setValue: (bool val) => Config.TreeSettings.BelowPlayerOnly = val
                );

                api.AddNumberOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Tree.Distance.Name"),
                    tooltip: () => Helper.Translation.Get("Tree.Distance.Desc"),
                    getValue: () => Config.TreeSettings.TileDistance,
                    setValue: (int val) => Config.TreeSettings.TileDistance = val
                );

                api.AddNumberOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Tree.MinimumOpacity.Name"),
                    tooltip: () => Helper.Translation.Get("Tree.MinimumOpacity.Desc"),
                    getValue: () => Config.TreeSettings.MinimumOpacity,
                    setValue: (float val) => Config.TreeSettings.MinimumOpacity = val,
                    min: 0f,
                    max: 1f,
                    interval: 0.01f
                );

                //grass

                api.AddSectionTitle
                (
                    mod: ModManifest,
                    text: () => Helper.Translation.Get("Grass.Title.Name"),
                    tooltip: () => Helper.Translation.Get("Grass.Title.Desc")
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Grass.Enable.Name"),
                    tooltip: () => Helper.Translation.Get("Grass.Enable.Desc"),
                    getValue: () => Config.GrassSettings.Enable,
                    setValue: (bool val) => Config.GrassSettings.Enable = val
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Grass.Below.Name"),
                    tooltip: () => Helper.Translation.Get("Grass.Below.Desc"),
                    getValue: () => Config.GrassSettings.BelowPlayerOnly,
                    setValue: (bool val) => Config.GrassSettings.BelowPlayerOnly = val
                );

                api.AddNumberOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Grass.Distance.Name"),
                    tooltip: () => Helper.Translation.Get("Grass.Distance.Desc"),
                    getValue: () => Config.GrassSettings.TileDistance,
                    setValue: (int val) => Config.GrassSettings.TileDistance = val
                );

                api.AddNumberOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Grass.MinimumOpacity.Name"),
                    tooltip: () => Helper.Translation.Get("Grass.MinimumOpacity.Desc"),
                    getValue: () => Config.GrassSettings.MinimumOpacity,
                    setValue: (float val) => Config.GrassSettings.MinimumOpacity = val,
                    min: 0f,
                    max: 1f,
                    interval: 0.01f
                );

                //objects

                api.AddSectionTitle
                (
                    mod: ModManifest,
                    text: () => Helper.Translation.Get("Object.Title.Name"),
                    tooltip: () => Helper.Translation.Get("Object.Title.Desc")
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Object.Enable.Name"),
                    tooltip: () => Helper.Translation.Get("Object.Enable.Desc"),
                    getValue: () => Config.ObjectSettings.Enable,
                    setValue: (bool val) => Config.ObjectSettings.Enable = val
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Object.Below.Name"),
                    tooltip: () => Helper.Translation.Get("Object.Below.Desc"),
                    getValue: () => Config.ObjectSettings.BelowPlayerOnly,
                    setValue: (bool val) => Config.ObjectSettings.BelowPlayerOnly = val
                );

                api.AddNumberOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Object.Distance.Name"),
                    tooltip: () => Helper.Translation.Get("Object.Distance.Desc"),
                    getValue: () => Config.ObjectSettings.TileDistance,
                    setValue: (int val) => Config.ObjectSettings.TileDistance = val
                );

                api.AddNumberOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Object.MinimumOpacity.Name"),
                    tooltip: () => Helper.Translation.Get("Object.MinimumOpacity.Desc"),
                    getValue: () => Config.ObjectSettings.MinimumOpacity,
                    setValue: (float val) => Config.ObjectSettings.MinimumOpacity = val,
                    min: 0f,
                    max: 1f,
                    interval: 0.01f
                );

                //craftables

                api.AddSectionTitle
                (
                    mod: ModManifest,
                    text: () => Helper.Translation.Get("Craftable.Title.Name"),
                    tooltip: () => Helper.Translation.Get("Craftable.Title.Desc")
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Craftable.Enable.Name"),
                    tooltip: () => Helper.Translation.Get("Craftable.Enable.Desc"),
                    getValue: () => Config.CraftableSettings.Enable,
                    setValue: (bool val) => Config.CraftableSettings.Enable = val
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Craftable.Below.Name"),
                    tooltip: () => Helper.Translation.Get("Craftable.Below.Desc"),
                    getValue: () => Config.CraftableSettings.BelowPlayerOnly,
                    setValue: (bool val) => Config.CraftableSettings.BelowPlayerOnly = val
                );

                api.AddNumberOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Craftable.Distance.Name"),
                    tooltip: () => Helper.Translation.Get("Craftable.Distance.Desc"),
                    getValue: () => Config.CraftableSettings.TileDistance,
                    setValue: (int val) => Config.CraftableSettings.TileDistance = val
                );

                api.AddNumberOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("Craftable.MinimumOpacity.Name"),
                    tooltip: () => Helper.Translation.Get("Craftable.MinimumOpacity.Desc"),
                    getValue: () => Config.CraftableSettings.MinimumOpacity,
                    setValue: (float val) => Config.CraftableSettings.MinimumOpacity = val,
                    min: 0f,
                    max: 1f,
                    interval: 0.01f
                );

                //keybinds

                api.AddSectionTitle
                (
                    mod: ModManifest,
                    text: () => Helper.Translation.Get("KeyBindings.Title.Name"),
                    tooltip: () => Helper.Translation.Get("KeyBindings.Title.Desc")
                );

                api.AddKeybindList
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("KeyBindings.Disable.Name"),
                    tooltip: () => Helper.Translation.Get("KeyBindings.Disable.Desc"),
                    getValue: () => Config.KeyBindings.DisableTransparency,
                    setValue: (KeybindList val) => Config.KeyBindings.DisableTransparency = val
                );

                api.AddKeybindList
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("KeyBindings.Full.Name"),
                    tooltip: () => Helper.Translation.Get("KeyBindings.Full.Desc"),
                    getValue: () => Config.KeyBindings.FullTransparency,
                    setValue: (KeybindList val) => Config.KeyBindings.FullTransparency = val
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
}
