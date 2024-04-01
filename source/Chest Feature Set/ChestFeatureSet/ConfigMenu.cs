/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zack20136/ChestFeatureSet
**
*************************************************/

using ChestFeatureSet.API;
using StardewModdingAPI;

namespace ChestFeatureSet
{
    public class ConfigMenu
    {
        public static void StartConfigMenu(IModHelper helper, IManifest manifest, ModConfig config)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu == null)
                return;

            // register mod
            configMenu.Register(
                mod: manifest,
                reset: () => config = new ModConfig(),
                save: () => helper.WriteConfig(config)
            );

            // StashToChests
            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => "StashToChests",
                tooltip: () => "Stash to the chests that is been selected."
                );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "StashToChests",
                tooltip: () => "Open the feature of StashToChests. (If changed, please go back to Title and load agian.)",
                getValue: () => config.StashToChests,
                setValue: value => config.StashToChests = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "OnlyStashToExistingStacks",
                tooltip: () => "Only stash to existing stacks.",
                getValue: () => config.OnlyStashToExistingStacks,
                setValue: value => config.OnlyStashToExistingStacks = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => "StashLocationSetting",
                tooltip: () => "Can stash to chests at these {lcations}.\n Default: The location you are.\n FarmArea: All the location inside the farm(include).\n Anywhere: All the location.",
                getValue: () => config.StashLocationSetting,
                setValue: value => config.StashLocationSetting = value,
                allowedValues: new string[] { "Default", "FarmArea", "Anywhere" }
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => "StashRadius (-1 is Unlimited)",
                tooltip: () => "The distance that can be stashed to chests.\nActive when LocationSetting is Default.",
                getValue: () => config.StashRadius,
                setValue: value => config.StashRadius = value
            );
            configMenu.AddKeybind(
                mod: manifest,
                name: () => "StashKey",
                tooltip: () => "StashKey Keybind.",
                getValue: () => config.StashKey,
                setValue: value => config.StashKey = value
            );

            // LockItems
            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => "LockItems",
                tooltip: () => "Locked the items that do not want to stash."
                );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "LockItems (Needed StashToChests)",
                tooltip: () => "Open the feature of LockItems. (If changed, please go back to Title and load agian.)",
                getValue: () => config.LockItems,
                setValue: value => config.LockItems = value
            );
            configMenu.AddKeybind(
                mod: manifest,
                name: () => "LockItemKey",
                tooltip: () => "LockItemKey Keybind.",
                getValue: () => config.LockItemKey,
                setValue: value => config.LockItemKey = value
            );
            configMenu.AddKeybind(
                mod: manifest,
                name: () => "ResetLockItemKey",
                tooltip: () => "ResetLockItemKey Keybind.",
                getValue: () => config.ResetLockItemKey,
                setValue: value => config.ResetLockItemKey = value
            );

            // CraftFromChests
            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => "CraftFromChests",
                tooltip: () => "Crafting form the chests that is been select."
                );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "CraftFromChests",
                tooltip: () => "Open the feature of CraftFromChests. (If changed, please go back to Title and load agian.)",
                getValue: () => config.CraftFromChests,
                setValue: value => config.CraftFromChests = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => "CraftLocationSetting",
                tooltip: () => "Can craft from chests at these {lcations}.\n Default: The location you are.\n FarmArea: All the location inside the farm(include).\n Anywhere: All the location.",
                getValue: () => config.CraftLocationSetting,
                setValue: value => config.CraftLocationSetting = value,
                allowedValues: new string[] { "Default", "FarmArea", "Anywhere" }
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => "CraftRadius (-1 is Unlimited)",
                tooltip: () => "The distance that can be crafted from chests.\nActive when LocationSetting is Default.",
                getValue: () => config.CraftRadius,
                setValue: value => config.CraftRadius = value
            );
            configMenu.AddKeybind(
                mod: manifest,
                name: () => "OpenCraftingPageKey",
                tooltip: () => "OpenCraftingPageKey Keybind.",
                getValue: () => config.OpenCraftingPageKey,
                setValue: value => config.OpenCraftingPageKey = value
            );

            // MoveChests
            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => "MoveChests",
                tooltip: () => "Move the chest with item inside."
                );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "MoveChests",
                tooltip: () => "Open the feature of MoveChests. (If changed, please go back to Title and load agian.)",
                getValue: () => config.MoveChests,
                setValue: value => config.MoveChests = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "MoveChestsDebuff",
                tooltip: () => "While moving chest get a debuff.",
                getValue: () => config.MoveChestsDebuff,
                setValue: value => config.MoveChestsDebuff = value
            );
            configMenu.AddKeybind(
                mod: manifest,
                name: () => "MoveChestKey",
                tooltip: () => "MoveChestKey Keybind.",
                getValue: () => config.MoveChestKey,
                setValue: value => config.MoveChestKey = value
            );
        }
    }
}
