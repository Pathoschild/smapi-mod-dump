/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using GenericModConfigMenu;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace ConvenientInventory.Compatibility
{
    public class ModInitializer
    {
        private readonly IManifest modManifest;
        private readonly IModHelper helper;

        public ModInitializer(IManifest modManifest, IModHelper helper)
        {
            this.modManifest = modManifest;
            this.helper = helper;
        }

        public void Initialize(IGenericModConfigMenuApi api, ModConfig config)
        {
            api.RegisterModConfig(
                mod: modManifest,
                revertToDefault: () =>
                {
                    config = new ModConfig();
                    ModEntry.Config = config;
                },
                saveToFile: () => helper.WriteConfig(config)
            );

            api.SetDefaultIngameOptinValue(modManifest, true);

            api.RegisterLabel(
                mod: modManifest,
                labelName: helper.Translation.Get("ModConfigMenu.Label.QuickStackToNearbyChests"),
                labelDesc: null
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: helper.Translation.Get("ModConfigMenu.IsEnableQuickStack.Name"),
                optionDesc: helper.Translation.Get("ModConfigMenu.IsEnableQuickStack.Desc"),
                optionGet: () => config.IsEnableQuickStack,
                optionSet: value => config.IsEnableQuickStack = value
            );

            api.RegisterClampedOption(
                mod: modManifest,
                optionName: helper.Translation.Get("ModConfigMenu.QuickStackRange.Name"),
                optionDesc: helper.Translation.Get("ModConfigMenu.QuickStackRange.Desc"),
                optionGet: () => config.QuickStackRange,
                optionSet: value => config.QuickStackRange = value,
                min: 0,
                max: 10,
                interval: 1
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: helper.Translation.Get("ModConfigMenu.IsEnableQuickStackHotkey.Name"),
                optionDesc: helper.Translation.Get("ModConfigMenu.IsEnableQuickStackHotkey.Desc"),
                optionGet: () => config.IsEnableQuickStackHotkey,
                optionSet: value => config.IsEnableQuickStackHotkey = value
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: helper.Translation.Get("ModConfigMenu.QuickStackKeyboardHotkey.Name"),
                optionDesc: helper.Translation.Get("ModConfigMenu.QuickStackKeyboardHotkey.Desc"),
                optionGet: () => config.QuickStackKeyboardHotkey,
                optionSet: value => config.QuickStackKeyboardHotkey = value
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: helper.Translation.Get("ModConfigMenu.QuickStackControllerHotkey.Name"),
                optionDesc: helper.Translation.Get("ModConfigMenu.QuickStackControllerHotkey.Desc"),
                optionGet: () => config.QuickStackControllerHotkey,
                optionSet: value => config.QuickStackControllerHotkey = value
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: helper.Translation.Get("ModConfigMenu.IsQuickStackIntoBuildingsWithInventories.Name"),
                optionDesc: helper.Translation.Get("ModConfigMenu.IsQuickStackIntoBuildingsWithInventories.Desc"),
                optionGet: () => config.IsQuickStackIntoBuildingsWithInventories,
                optionSet: value => config.IsQuickStackIntoBuildingsWithInventories = value
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: helper.Translation.Get("ModConfigMenu.IsQuickStackOverflowItems.Name"),
                optionDesc: helper.Translation.Get("ModConfigMenu.IsQuickStackOverflowItems.Desc"),
                optionGet: () => config.IsQuickStackOverflowItems,
                optionSet: value => config.IsQuickStackOverflowItems = value
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: helper.Translation.Get("ModConfigMenu.IsQuickStackIgnoreItemQuality.Name"),
                optionDesc: $"(Requires \"{helper.Translation.Get("ModConfigMenu.IsQuickStackOverflowItems.Name")}\" to be enabled.) " +
                    helper.Translation.Get("ModConfigMenu.IsQuickStackIgnoreItemQuality.Desc"),
                optionGet: () => config.IsQuickStackIgnoreItemQuality,
                optionSet: value => config.IsQuickStackIgnoreItemQuality = value
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: helper.Translation.Get("ModConfigMenu.IsQuickStackTooltipDrawNearbyChests.Name"),
                optionDesc: helper.Translation.Get("ModConfigMenu.IsQuickStackTooltipDrawNearbyChests.Desc"),
                optionGet: () => config.IsQuickStackTooltipDrawNearbyChests,
                optionSet: value => config.IsQuickStackTooltipDrawNearbyChests = value
            );

            api.RegisterLabel(
                mod: modManifest,
                labelName: helper.Translation.Get("ModConfigMenu.Label.FavoriteItems"),
                labelDesc: null
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: helper.Translation.Get("ModConfigMenu.IsEnableFavoriteItems.Name"),
                optionDesc: helper.Translation.Get("ModConfigMenu.IsEnableFavoriteItems.Desc"),
                optionGet: () => config.IsEnableFavoriteItems,
                optionSet: value => config.IsEnableFavoriteItems = value
            );

            string[] highlightStyleDescriptions =
                {
                    $"0: {helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Desc-0")}",
                    $"1: {helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Desc-1")}",
                    $"2: {helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Desc-2")}",
                    $"3: {helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Desc-3")}",
                    $"4: {helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Desc-4")}",
                    $"5: {helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Desc-5")}",
                    $"6: {helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Desc-6")}",
                };
            api.RegisterChoiceOption(
                mod: modManifest,
                optionName: helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Name"),
                optionDesc: helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Desc"),
                optionGet: () => highlightStyleDescriptions[config.FavoriteItemsHighlightTextureChoice],
                optionSet: value =>
                    {
                        config.FavoriteItemsHighlightTextureChoice = int.Parse(value.Substring(0, 1));
                        ConvenientInventory.FavoriteItemsHighlightTexture = helper.Content.Load<Texture2D>($@"assets\favoriteHighlight_{value[0]}.png");
                    },
                choices: highlightStyleDescriptions
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: helper.Translation.Get("ModConfigMenu.FavoriteItemsKeyboardHotkey.Name"),
                optionDesc: helper.Translation.Get("ModConfigMenu.FavoriteItemsKeyboardHotkey.Desc"),
                optionGet: () => config.FavoriteItemsKeyboardHotkey,
                optionSet: value => config.FavoriteItemsKeyboardHotkey = value
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: helper.Translation.Get("ModConfigMenu.FavoriteItemsControllerHotkey.Name"),
                optionDesc: helper.Translation.Get("ModConfigMenu.FavoriteItemsControllerHotkey.Desc"),
                optionGet: () => config.FavoriteItemsControllerHotkey,
                optionSet: value => config.FavoriteItemsControllerHotkey = value
            );

            api.RegisterLabel(
                mod: modManifest,
                labelName: helper.Translation.Get("ModConfigMenu.Label.Miscellaneous"),
                labelDesc: null
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: helper.Translation.Get("ModConfigMenu.IsEnableInventoryPageSideWarp.Name"),
                optionDesc: helper.Translation.Get("ModConfigMenu.IsEnableInventoryPageSideWarp.Desc"),
                optionGet: () => config.IsEnableInventoryPageSideWarp,
                optionSet: value => config.IsEnableInventoryPageSideWarp = value
            );
        }
    }
}
