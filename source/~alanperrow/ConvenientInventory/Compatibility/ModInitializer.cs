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
            api.Register(
                mod: modManifest,
                reset: () =>
                {
                    config = new ModConfig();
                    ModEntry.Config = config;
                },
                save: () => helper.WriteConfig(config)
            );

            api.AddSectionTitle(
                mod: modManifest,
                text: () => helper.Translation.Get("ModConfigMenu.Label.QuickStackToNearbyChests")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsEnableQuickStack,
                setValue: value => config.IsEnableQuickStack = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsEnableQuickStack.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsEnableQuickStack.Desc")
            );

            api.AddNumberOption(
                mod: modManifest,
                getValue: () => config.QuickStackRange,
                setValue: value => config.QuickStackRange = value,
                name: () => helper.Translation.Get("ModConfigMenu.QuickStackRange.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.QuickStackRange.Desc"),
                min: 0,
                max: 10,
                interval: 1
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsEnableQuickStackHotkey,
                setValue: value => config.IsEnableQuickStackHotkey = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsEnableQuickStackHotkey.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsEnableQuickStackHotkey.Desc")
            );

            api.AddKeybind(
                mod: modManifest,
                getValue: () => config.QuickStackKeyboardHotkey,
                setValue: value => config.QuickStackKeyboardHotkey = value,
                name: () => helper.Translation.Get("ModConfigMenu.QuickStackKeyboardHotkey.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.QuickStackKeyboardHotkey.Desc")
            );

            api.AddKeybind(
                mod: modManifest,
                getValue: () => config.QuickStackControllerHotkey,
                setValue: value => config.QuickStackControllerHotkey = value,
                name: () => helper.Translation.Get("ModConfigMenu.QuickStackControllerHotkey.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.QuickStackControllerHotkey.Desc")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsQuickStackIntoBuildingsWithInventories,
                setValue: value => config.IsQuickStackIntoBuildingsWithInventories = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIntoBuildingsWithInventories.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIntoBuildingsWithInventories.Desc")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsQuickStackOverflowItems,
                setValue: value => config.IsQuickStackOverflowItems = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsQuickStackOverflowItems.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsQuickStackOverflowItems.Desc")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsQuickStackIgnoreItemQuality,
                setValue: value => config.IsQuickStackIgnoreItemQuality = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIgnoreItemQuality.Name"),
                tooltip: () => $"(Requires \"{helper.Translation.Get("ModConfigMenu.IsQuickStackOverflowItems.Name")}\" to be enabled.) " +
                    helper.Translation.Get("ModConfigMenu.IsQuickStackIgnoreItemQuality.Desc")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsQuickStackTooltipDrawNearbyChests,
                setValue: value => config.IsQuickStackTooltipDrawNearbyChests = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsQuickStackTooltipDrawNearbyChests.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsQuickStackTooltipDrawNearbyChests.Desc")
            );

            api.AddSectionTitle(
                mod: modManifest,
                text: () => helper.Translation.Get("ModConfigMenu.Label.FavoriteItems")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsEnableFavoriteItems,
                setValue: value => config.IsEnableFavoriteItems = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsEnableFavoriteItems.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsEnableFavoriteItems.Desc")
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
            api.AddTextOption(
                mod: modManifest,
                getValue: () => highlightStyleDescriptions[config.FavoriteItemsHighlightTextureChoice],
                setValue: value =>
                {
                    config.FavoriteItemsHighlightTextureChoice = int.Parse(value[..1]);
                    ConvenientInventory.FavoriteItemsHighlightTexture = helper.ModContent.Load<Texture2D>($@"assets\favoriteHighlight_{value[0]}.png");
                },
                name: () => helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Desc"),
                allowedValues: highlightStyleDescriptions
            );

            api.AddKeybind(
                mod: modManifest,
                getValue: () => config.FavoriteItemsKeyboardHotkey,
                setValue: value => config.FavoriteItemsKeyboardHotkey = value,
                name: () => helper.Translation.Get("ModConfigMenu.FavoriteItemsKeyboardHotkey.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.FavoriteItemsKeyboardHotkey.Desc")
            );

            api.AddKeybind(
                mod: modManifest,
                getValue: () => config.FavoriteItemsControllerHotkey,
                setValue: value => config.FavoriteItemsControllerHotkey = value,
                name: () => helper.Translation.Get("ModConfigMenu.FavoriteItemsControllerHotkey.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.FavoriteItemsControllerHotkey.Desc")
            );

            api.AddSectionTitle(
                mod: modManifest,
                text: () => helper.Translation.Get("ModConfigMenu.Label.Miscellaneous")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsEnableInventoryPageSideWarp,
                setValue: value => config.IsEnableInventoryPageSideWarp = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsEnableInventoryPageSideWarp.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsEnableInventoryPageSideWarp.Desc")
            );
        }
    }
}
