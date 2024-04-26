/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using ItemBags.Bags;
using ItemBags.Persistence;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace ItemBags
{
    /// <summary>The API which lets other mods add a config UI through Generic Mod Config Menu.</summary>
    public interface IGenericModConfigMenuApi
    {
        /*********
        ** Methods
        *********/
        /****
        ** Must be called first
        ****/
        /// <summary>Register a mod whose config can be edited through the UI.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="reset">Reset the mod's config to its default values.</param>
        /// <param name="save">Save the mod's current config to the <c>config.json</c> file.</param>
        /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
        /// <remarks>Each mod can only be registered once, unless it's deleted via <see cref="Unregister"/> before calling this again.</remarks>
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);


        /****
        ** Basic options
        ****/
        /// <summary>Add a section title at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="text">The title text shown in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the title, or <c>null</c> to disable the tooltip.</param>
        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);

        /// <summary>Add a paragraph of text at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="text">The paragraph text to display.</param>
        void AddParagraph(IManifest mod, Func<string> text);

        /// <summary>Add a boolean option at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to show in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
        /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        /// <summary>Add an integer option at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to show in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
        /// <param name="min">The minimum allowed value, or <c>null</c> to allow any.</param>
        /// <param name="max">The maximum allowed value, or <c>null</c> to allow any.</param>
        /// <param name="interval">The interval of values that can be selected.</param>
        /// <param name="formatValue">Get the display text to show for a value, or <c>null</c> to show the number as-is.</param>
        /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);

        /// <summary>Add a float option at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to show in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
        /// <param name="min">The minimum allowed value, or <c>null</c> to allow any.</param>
        /// <param name="max">The maximum allowed value, or <c>null</c> to allow any.</param>
        /// <param name="interval">The interval of values that can be selected.</param>
        /// <param name="formatValue">Get the display text to show for a value, or <c>null</c> to show the number as-is.</param>
        /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
        void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);

        /// <summary>Add a string option at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to show in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
        /// <param name="allowedValues">The values that can be selected, or <c>null</c> to allow any.</param>
        /// <param name="formatAllowedValue">Get the display text to show for a value from <paramref name="allowedValues"/>, or <c>null</c> to show the values as-is.</param>
        /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);

        /****
        ** Multi-page management
        ****/
        /// <summary>Start a new page in the mod's config UI, or switch to that page if it already exists. All options registered after this will be part of that page.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="pageId">The unique page ID.</param>
        /// <param name="pageTitle">The page title shown in its UI, or <c>null</c> to show the <paramref name="pageId"/> value.</param>
        /// <remarks>You must also call <see cref="AddPageLink"/> to make the page accessible. This is only needed to set up a multi-page config UI. If you don't call this method, all options will be part of the mod's main config UI instead.</remarks>
        void AddPage(IManifest mod, string pageId, Func<string> pageTitle = null);

        /// <summary>Add a link to a page added via <see cref="AddPage"/> at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="pageId">The unique ID of the page to open when the link is clicked.</param>
        /// <param name="text">The link text shown in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the link, or <c>null</c> to disable the tooltip.</param>
        void AddPageLink(IManifest mod, string pageId, Func<string> text, Func<string> tooltip = null);


        /****
        ** Advanced
        ****/
        /// <summary>Add an option at the current position in the form using custom rendering logic.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="name">The label text to show in the form.</param>
        /// <param name="draw">Draw the option in the config UI. This is called with the sprite batch being rendered and the pixel position at which to start drawing.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
        /// <param name="beforeMenuOpened">A callback raised just before the menu containing this option is opened.</param>
        /// <param name="beforeSave">A callback raised before the form's current values are saved to the config (i.e. before the <c>save</c> callback passed to <see cref="Register"/>).</param>
        /// <param name="afterSave">A callback raised after the form's current values are saved to the config (i.e. after the <c>save</c> callback passed to <see cref="Register"/>).</param>
        /// <param name="beforeReset">A callback raised before the form is reset to its default values (i.e. before the <c>reset</c> callback passed to <see cref="Register"/>).</param>
        /// <param name="afterReset">A callback raised after the form is reset to its default values (i.e. after the <c>reset</c> callback passed to <see cref="Register"/>).</param>
        /// <param name="beforeMenuClosed">A callback raised just before the menu containing this option is closed.</param>
        /// <param name="height">The pixel height to allocate for the option in the form, or <c>null</c> for a standard input-sized option. This is called and cached each time the form is opened.</param>
        /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
        /// <remarks>The custom logic represented by the callback parameters is responsible for managing its own state if needed. For example, you can store state in a static field or use closures to use a state variable.</remarks>
        void AddComplexOption(IManifest mod, Func<string> name, Action<SpriteBatch, Vector2> draw, Func<string> tooltip = null, Action beforeMenuOpened = null, Action beforeSave = null, Action afterSave = null, Action beforeReset = null, Action afterReset = null, Action beforeMenuClosed = null, Func<int> height = null, string fieldId = null);

        /// <summary>Register a method to notify when any option registered by this mod is edited through the config UI.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="onChange">The method to call with the option's unique field ID and new value.</param>
        /// <remarks>Options use a randomized ID by default; you'll likely want to specify the <c>fieldId</c> argument when adding options if you use this.</remarks>
        void OnFieldChanged(IManifest mod, Action<string, object> onChange);

        /// <summary>Get the currently-displayed mod config menu, if any.</summary>
        /// <param name="mod">The manifest of the mod whose config menu is being shown, or <c>null</c> if not applicable.</param>
        /// <param name="page">The page ID being shown for the current config menu, or <c>null</c> if not applicable. This may be <c>null</c> even if a mod config menu is shown (e.g. because the mod doesn't have pages).</param>
        /// <returns>Returns whether a mod config menu is being shown.</returns>
        bool TryGetCurrentMenu(out IManifest mod, out string page);
    }

    public static class GMCM
    {
        private static UserConfig Cfg
        {
            get => ItemBagsMod.UserConfig;
            set => ItemBagsMod.UserConfig = value;
        }

        private static IManifest ModManifest => ItemBagsMod.ModInstance.ModManifest;

        internal static void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += (sender, e) =>
            {
                try { TryRegisterGMCM(helper); }
                catch (Exception ex) { ItemBagsMod.Logger.Log($"Unhandled error while registering to Generic Mod Config Menu: You will not be able to modify ItemBags settings through GMCM.\n{ex}", LogLevel.Error); }
            };
        }

        public static event EventHandler<EventArgs> OnConfigChanged;

        private static void TryRegisterGMCM(IModHelper Helper)
        {
            const string GMCMUniqueId = "spacechase0.GenericModConfigMenu";
            if (!Helper.ModRegistry.IsLoaded(GMCMUniqueId))
                return;
            IGenericModConfigMenuApi API = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>(GMCMUniqueId);
            if (API == null)
                return;

            API.Register(ModManifest,
                () => { Cfg = new UserConfig(); OnConfigChanged?.Invoke(null, EventArgs.Empty); },
                () => { Helper.Data.WriteJsonFile(ItemBagsMod.UserConfigFilename, Cfg); OnConfigChanged?.Invoke(null, EventArgs.Empty); });

            //  Add the page links to the main page:
            API.AddPageLink(ModManifest, $"{ItemBagsMod.ModUniqueId}:PriceSettings", () => "Price settings", () => "These settings affect how much the bags costs at shops.");
            API.AddPageLink(ModManifest, $"{ItemBagsMod.ModUniqueId}:CapacitySettings", () => "Capacity settings", () => "These settings affect the maximum stack size of an item that the bag can store.");
            API.AddPageLink(ModManifest, $"{ItemBagsMod.ModUniqueId}:ShopSettings", () => "Shop settings");
            API.AddPageLink(ModManifest, $"{ItemBagsMod.ModUniqueId}:RucksackSettings", () => "Rucksack settings");
            API.AddPageLink(ModManifest, $"{ItemBagsMod.ModUniqueId}:LootSettings", () => "Drop Rates");
            API.AddPageLink(ModManifest, $"{ItemBagsMod.ModUniqueId}:MiscSettings", () => "Misc settings");

            #region Price Modifiers
            API.AddPage(ModManifest, $"{ItemBagsMod.ModUniqueId}:PriceSettings", () => "Prices");

            API.AddSectionTitle(ModManifest, () => "Price Modifiers:", () => "These settings affect how much the bags costs at shops.");

            API.AddNumberOption(ModManifest,
                () => (float)Cfg.GlobalPriceModifier,
                (float val) => Cfg.GlobalPriceModifier = val,
                () => "Main Price Modifier:",
                () => "A multiplier that affects the cost of all bags.\n\n" +
                        "This value is multiplied with the other relevant modifier when computing a bag's price.\n" +
                        "EX: The price modifier of a Small Rucksack is computed by multiplying the Small Rucksack Price Modifier with this value.\n\n" +
                        "Recommended value: 1.0.",
                0.05f, 3f, 0.05f
            );

            API.AddParagraph(ModManifest, () => "These settings affect the price of standard bags, such as Fish Bags, Mining Bags, or Crop Bags. They do not affect the price of rucksacks, bundle bags, or omni bags.");
            foreach (ContainerSize Size in Enum.GetValues(typeof(ContainerSize)).Cast<ContainerSize>())
            {
                API.AddNumberOption(ModManifest,
                    () => (float)Cfg.StandardBagSettings.First(x => x.Size == Size).PriceModifier,
                    (float val) => Cfg.StandardBagSettings.First(x => x.Size == Size).PriceModifier = val,
                    () => $"{Size} Price Modifier:",
                    () => $"A multiplier that affects the cost of {Size} bags.\n\nRecommended value: 1.0.",
                    0.05f, 3f, 0.05f
                );
            }

            API.AddParagraph(ModManifest, () => "These settings affect the price of rucksacks.");
            foreach (ContainerSize Size in Enum.GetValues(typeof(ContainerSize)).Cast<ContainerSize>())
            {
                API.AddNumberOption(ModManifest,
                    () => (float)Cfg.RucksackSettings.First(x => x.Size == Size).PriceModifier,
                    (float val) => Cfg.RucksackSettings.First(x => x.Size == Size).PriceModifier = val,
                    () => $"{Size} Price Modifier:",
                    () => $"A multiplier that affects the cost of {Size} rucksacks.\n\nRecommended value: 1.0.",
                    0.05f, 3f, 0.05f
                );
            }

            API.AddParagraph(ModManifest, () => "These settings affect the price of omni-bags.");
            foreach (ContainerSize Size in Enum.GetValues(typeof(ContainerSize)).Cast<ContainerSize>())
            {
                API.AddNumberOption(ModManifest,
                    () => (float)Cfg.OmniBagSettings.First(x => x.Size == Size).PriceModifier,
                    (float val) => Cfg.OmniBagSettings.First(x => x.Size == Size).PriceModifier = val,
                    () => $"{Size} Price Modifier:",
                    () => $"A multiplier that affects the cost of {Size} omni-bags.\n\nRecommended value: 1.0.",
                    0.05f, 3f, 0.05f
                );
            }

            API.AddParagraph(ModManifest, () => "These settings affect the price of bundle bags.");
            foreach (ContainerSize Size in BundleBag.ValidSizes.Cast<ContainerSize>())
            {
                API.AddNumberOption(ModManifest,
                    () => (float)Cfg.BundleBagSettings.First(x => x.Size == Size).PriceModifier,
                    (float val) => Cfg.BundleBagSettings.First(x => x.Size == Size).PriceModifier = val,
                    () => $"{Size} Price Modifier:",
                    () => $"A multiplier that affects the cost of {Size} bundle bags.\n\nRecommended value: 1.0.",
                    0.05f, 3f, 0.05f
                );
            }
            #endregion Price Modifiers

            #region Capacity Modifiers
            API.AddPage(ModManifest, $"{ItemBagsMod.ModUniqueId}:CapacitySettings", () => "Capacities");

            API.AddSectionTitle(ModManifest, () => "Capacity Modifiers:", () => "These settings affect the maximum stack size of an item that the bag can store.");

            API.AddNumberOption(ModManifest,
                () => (float)Cfg.GlobalCapacityModifier,
                (float val) => Cfg.GlobalCapacityModifier = val,
                () => "Main Capacity Modifier:",
                () => "A multiplier that affects the capacity of all bags.\n\n" +
                        "This value is multiplied with the other relevant modifier when computing a bag's capacity.\n" +
                        "EX: The capacity modifier of a Small Rucksack is computed by multiplying the Small Rucksack Capacity Modifier with this value.\n\n" +
                        "Recommended value: 1.0.",
                0.1f, 5f, 0.1f
            );

            API.AddParagraph(ModManifest, () => "These settings affect the capacity of standard bags, such as Fish Bags, Mining Bags, or Crop Bags. They do not affect the capacity of rucksacks, bundle bags, or omni bags.");
            foreach (ContainerSize Size in Enum.GetValues(typeof(ContainerSize)).Cast<ContainerSize>())
            {
                API.AddNumberOption(ModManifest,
                    () => (float)Cfg.StandardBagSettings.First(x => x.Size == Size).CapacityModifier,
                    (float val) => Cfg.StandardBagSettings.First(x => x.Size == Size).CapacityModifier = val,
                    () => $"{Size} Capacity Modifier:",
                    () => $"A multiplier that affects the capacity of {Size} bags.\n\nRecommended value: 1.0.",
                    0.1f, 5f, 0.1f
                );
            }

            API.AddParagraph(ModManifest, () => "These settings affect the capacity of rucksacks.");
            foreach (ContainerSize Size in Enum.GetValues(typeof(ContainerSize)).Cast<ContainerSize>())
            {
                API.AddNumberOption(ModManifest,
                    () => (float)Cfg.RucksackSettings.First(x => x.Size == Size).CapacityModifier,
                    (float val) => Cfg.RucksackSettings.First(x => x.Size == Size).CapacityModifier = val,
                    () => $"{Size} Capacity Modifier:",
                    () => $"A multiplier that affects the capacity of {Size} rucksacks.\n\nRecommended value: 1.0.",
                    0.1f, 5f, 0.1f
                );
            }
            #endregion Capacity Modifiers

            #region Shops
            API.AddPage(ModManifest, $"{ItemBagsMod.ModUniqueId}:ShopSettings", () => "Shops");

            Dictionary<ContainerSize, Func<bool>> ShopSettingGetters = new()
            {
                { ContainerSize.Small, () => Cfg.HideSmallBagsFromShops },
                { ContainerSize.Medium, () => Cfg.HideMediumBagsFromShops },
                { ContainerSize.Large, () => Cfg.HideLargeBagsFromShops },
                { ContainerSize.Giant, () => Cfg.HideGiantBagsFromShops },
                { ContainerSize.Massive, () => Cfg.HideMassiveBagsFromShops }
            };

            Dictionary<ContainerSize, Action<bool>> ShopSettingSetters = new()
            {
                { ContainerSize.Small, (bool val) => Cfg.HideSmallBagsFromShops = val },
                { ContainerSize.Medium,(bool val) => Cfg.HideMediumBagsFromShops = val },
                { ContainerSize.Large, (bool val) => Cfg.HideLargeBagsFromShops = val },
                { ContainerSize.Giant, (bool val) => Cfg.HideGiantBagsFromShops = val },
                { ContainerSize.Massive, (bool val) => Cfg.HideMassiveBagsFromShops = val }
            };

            foreach (ContainerSize Size in Enum.GetValues(typeof(ContainerSize)).Cast<ContainerSize>())
            {
                API.AddBoolOption(ModManifest,
                    ShopSettingGetters[Size],
                    ShopSettingSetters[Size],
                    () => $"Hide {Size} bags in shops:",
                    () => $"If true, {Size} standard bags will not be added to shop menus and won't be purchaseable.\n\nRecommended value: false."
                );
            }

            API.AddBoolOption(ModManifest,
                () => Cfg.HideObsoleteBagsFromShops,
                (bool val) => Cfg.HideObsoleteBagsFromShops = val,
                () => "Hide obsolete bags in shops:",
                () => "If true, bags that you already own a larger size of will be hidden in shop menus.\n\nRecommended value: true."
            );
            #endregion Shops

            #region Rucksacks
            API.AddPage(ModManifest, $"{ItemBagsMod.ModUniqueId}:RucksackSettings", () => "Rucksacks");

            Dictionary<ContainerSize, int> MaxSlots = new()
            {
                { ContainerSize.Small, 24 },
                { ContainerSize.Medium, 48 },
                { ContainerSize.Large, 96 },
                { ContainerSize.Giant, 144 },
                { ContainerSize.Massive, 288 }
            };

            foreach (ContainerSize Size in Enum.GetValues(typeof(ContainerSize)).Cast<ContainerSize>())
            {
                API.AddNumberOption(ModManifest,
                    () => Cfg.RucksackSettings.First(x => x.Size == Size).Slots,
                    (int val) => 
                    Cfg.RucksackSettings.First(x => x.Size == Size).Slots = val,
                    () => $"{Size} rucksacks slots:",
                    () => $"Determines how many inventory slots {Size} rucksacks have.\n\nRecommended value: {MaxSlots[Size] / 4}.",
                    1, MaxSlots[Size], 1
                );

                API.AddNumberOption(ModManifest,
                    () => Cfg.RucksackSettings.First(x => x.Size == Size).MenuColumns,
                    (int val) => Cfg.RucksackSettings.First(x => x.Size == Size).MenuColumns = val,
                    () => $"{Size} rucksacks num-columns:",
                    () => $"Determines how many columns the menu for {Size} rucksacks has.\nYou may wish to increase this if you set the slots to a large number, so that the menu fits vertically on your screen.\nSetting it too high could result in the menu being too wide for your screen.\n\nRecommended value: 12.",
                    6, 36, 1
                );

                API.AddTextOption(ModManifest,
                    () => Cfg.RucksackSettings.First(x => x.Size == Size).MenuSlotSize.ToString(),
                    (string val) => Cfg.RucksackSettings.First(x => x.Size == Size).MenuSlotSize = int.Parse(val),
                    () => $"{Size} rucksack slot size:",
                    () => $"Determines how many pixels each slot should be in the menu for {Size} rucksacks.\n" +
                        $"You may need to use a small value if you set the slots count to a large number, so that the menu fits on your screen.\n(Note: The dropdown is scrollable. Scroll the dropdown up to choose smaller values)\n\nRecommended value: 64.",
                    new string[] { "32", "40", "48", "64", "72", "84", "96", "128" });
            }
            #endregion Rucksacks

            #region Loot
            API.AddPage(ModManifest, $"{ItemBagsMod.ModUniqueId}:LootSettings", () => "Drop Rates");

            API.AddBoolOption(ModManifest,
                () => Cfg.MonsterLootSettings.CanReceiveBagsAsDrops,
                (bool val) => Cfg.MonsterLootSettings.CanReceiveBagsAsDrops = val,
                () => "Can receive bags as drops:",
                () => "If true, you will have a small chance of receiving a bag after killing a monster."
            );

            API.AddNumberOption(ModManifest,
                () => (float)Cfg.MonsterLootSettings.BaseDropChance,
                (float val) => Cfg.MonsterLootSettings.BaseDropChance = val,
                () => "Base drop chance:",
                () => "Determines the rarity of bags from monster drops.\nThe actual rarity is also affected by several other hidden properties," +
                    "\nsuch as how much combat xp the monster awarded, how much HP it had, how deep in the mines it was etc.\n\nRecommended value: 0.0066.",
                0, 0.05f
            );

            API.AddNumberOption(ModManifest,
                () => (float)Cfg.MonsterLootSettings.ForceNewBagTypeChance,
                (float val) => Cfg.MonsterLootSettings.ForceNewBagTypeChance = val,
                () => "Force new bag chance:",
                () => "Whenever you receive a bag as a monster drop, this value determines\nhow likely you are to be guaranteed to receive an upgrade.\n" +
                    "If you pass this roll, and if you are missing any bags, it will continually\ngenerate bags until finding a non-obsolete/non-duplicate bag to drop.\n\n" +
                    "Setting this to 1.0 will guarantee that you never receive a bag of a\ntype that you already own, or of a smaller size than you already own.\n\n" +
                    "Note: The bags don't need to be in your inventory - it will check your\nentire game world (such as inside chests on your farm) when checking for duplicates.\n\n" +
                    "EX: If you already own a Large Fish Bag and you are about to receive a\nMedium Fish Bag, that drop will be deemed obsolete and it will choose a different drop.\n\n" +
                    "Recommended value: 0.35.",
                0.0f, 1.0f, 0.05f
            );

            //TODO: Other drop-rate settings: Type weights and size weights for each bag type
            #endregion Loot

            #region Misc
            API.AddPage(ModManifest, $"{ItemBagsMod.ModUniqueId}:MiscSettings", () => "Misc");

            API.AddBoolOption(ModManifest,
                () => Cfg.BundleBagSettings.First().AllowDowngradeItemQuality,
                (bool val) =>
                    {
                        foreach (BundleBagSizeConfig Settings in Cfg.BundleBagSettings)
                            Settings.AllowDowngradeItemQuality = val;
                    },
                () => "Allow downgrade item quality:",
                () => "If true, bundle bags will automatically downgrade the quality of items inserted\ninto them to be of the minimum-required quality for that bundle task.\n\n" +
                    "If false, you will need to use the exact quality level when putting items into the bundle bag.\n" +
                    "For example, you wouldn't be able to use gold-quality parsnips for a bundle task that required regular-quality or higher parsnips.\n\nRecommended value: true."
            );

            API.AddBoolOption(ModManifest,
                () => Cfg.AllowAutofillInsideChest,
                (bool val) => Cfg.AllowAutofillInsideChest = val,
                () => "Allow autofill inside chest:",
                () => "If true, bags inside chests with their 'autofill' feature enabled will be able to be autofilled by items that are inserted into the chest.\n" +
                    "If false, only bags inside your inventory will be able to be autofilled.\n\nRecommended value: true.");

            API.AddBoolOption(ModManifest,
                () => Cfg.ShowAutofillMessage,
                (bool val) => Cfg.ShowAutofillMessage = val,
                () => "Show autofill HUD message:",
                () => "If true, a HUD message will appear in the bottom-left corner of your screen when items are autofilled into one of your bags.\n\nRecommended value: true.");
            #endregion Misc
        }
    }
}
