/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using Microsoft.Xna.Framework;
using stardew_access.Features;
using stardew_access.Patches;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;
// ReSharper disable UnusedMember.Global

namespace stardew_access
{
    public class API
    {
        // Note to future self, don't make these static, it won't give errors in sv access but it will in other mods if they try to use the stardew access api.
        //Setting Pragma to disable warning CA1822 prompting to make fields static.
        public API()
        {
        }
#pragma warning disable CA1822 // Mark members as static

        #region Screen reader related
        
        public string PrevMenuQueryText
        {
            get => MainClass.ScreenReader.PrevMenuQueryText;
            set => MainClass.ScreenReader.PrevMenuQueryText = value;
        }

        public string MenuPrefixText
        {
            get => MainClass.ScreenReader.MenuPrefixText;
            set => MainClass.ScreenReader.MenuPrefixText = value;
        }

        public string MenuSuffixText
        {
            get => MainClass.ScreenReader.MenuSuffixText;
            set => MainClass.ScreenReader.MenuSuffixText = value;
        }

        public string MenuPrefixNoQueryText
        {
            get => MainClass.ScreenReader.MenuPrefixNoQueryText;
            set => MainClass.ScreenReader.MenuPrefixNoQueryText = value;
        }

        public string MenuSuffixNoQueryText
        {
            get => MainClass.ScreenReader.MenuSuffixNoQueryText;
            set => MainClass.ScreenReader.MenuSuffixNoQueryText = value;
        }

        /// <summary>Speaks the text via the loaded screen reader (if any).</summary>
        /// <param name="text">The text to be narrated.</param>
        /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
        /// <returns>true if the text was spoken otherwise false.</returns>
        public bool Say(String text, Boolean interrupt)
            => MainClass.ScreenReader.Say(text, interrupt);

        /// <summary>Speaks the text via the loaded screen reader (if any).
        /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.</summary>
        /// <param name="text">The text to be narrated.</param>
        /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
        /// <param name="customQuery">If set, uses this instead of <paramref name="text"/> as query to check whether to speak the text or not.</param>
        /// <returns>true if the text was spoken otherwise false.</returns>
        public bool SayWithChecker(String text, Boolean interrupt, String? customQuery = null)
            => MainClass.ScreenReader.SayWithChecker(text, interrupt, customQuery);

        /// <summary>Speaks the text via the loaded screen reader (if any).
        /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.
        /// <br/><br/>Use this when narrating hovered component in menus to avoid interference.</summary>
        /// <param name="text">The text to be narrated.</param>
        /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
        /// <param name="customQuery">If set, uses this instead of <paramref name="text"/> as query to check whether to speak the text or not.</param>
        /// <returns>true if the text was spoken otherwise false.</returns>
        public bool SayWithMenuChecker(String text, Boolean interrupt, String? customQuery = null)
            => MainClass.ScreenReader.SayWithMenuChecker(text, interrupt, customQuery);

        /// <summary>Speaks the text via the loaded screen reader (if any).
        /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.
        /// <br/><br/>Use this when narrating chat messages to avoid interference.</summary>
        /// <param name="text">The text to be narrated.</param>
        /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
        /// <returns>true if the text was spoken otherwise false.</returns>
        public bool SayWithChatChecker(String text, Boolean interrupt)
            => MainClass.ScreenReader.SayWithChatChecker(text, interrupt);

        /// <summary>Speaks the text via the loaded screen reader (if any).
        /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.
        /// <br/><br/>Use this when narrating texts based on tile position to avoid interference.</summary>
        /// <param name="text">The text to be narrated.</param>
        /// <param name="x">The X location of tile.</param>
        /// <param name="y">The Y location of tile.</param>
        /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
        /// <returns>true if the text was spoken otherwise false.</returns>
        public bool SayWithTileQuery(String text, int x, int y, Boolean interrupt)
            => MainClass.ScreenReader.SayWithTileQuery(text, x, y, interrupt);

        #endregion

        #region Tiles related

        /// <summary>
        /// Search the area using Breadth First Search algorithm(BFS).
        /// </summary>
        /// <param name="center">The starting point.</param>
        /// <param name="limit">The limiting factor or simply radius of the search area.</param>
        /// <returns>A dictionary with all the detected tiles along with the name of the object on it and it's category.</returns>
        public Dictionary<Vector2, (string name, string category)> SearchNearbyTiles(
            Vector2 center,
            int limit
        ) => new Radar().SearchNearbyTiles(center, limit, false);

        /// <summary>
        /// Search the entire location using Breadth First Search algorithm(BFS).
        /// </summary>
        /// <returns>A dictionary with all the detected tiles along with the name of the object on it and it's category.</returns>
        public Dictionary<Vector2, (string name, string category)> SearchLocation() => Radar.SearchLocation();

        /// <summary>
        /// Check the tile for any object
        /// </summary>
        /// <param name="tile">The tile where we want to check the name and category of object if any</param>
        /// <returns>Name of the object as the first item (name) and category as the second item (category). Returns null if no object found.</returns>
        public (string? name, string? category) GetNameWithCategoryNameAtTile(Vector2 tile)
        {
            return TileInfo.GetNameWithCategoryNameAtTile(tile, null);
        }

        /// <summary>
        /// Check the tile for any object
        /// </summary>
        /// <param name="tile">The tile where we want to check the name and category of object if any</param>
        /// <returns>Name of the object. Returns null if no object found.</returns>
        public string? GetNameAtTile(Vector2 tile) => TileInfo.GetNameAtTile(tile, null);
        
        #endregion

        #region Inventory and Item related

        /// <summary>
        /// (Legacy! Should not be used from v1.6)
        /// Speaks the hovered inventory slot from the provided <see cref="InventoryMenu"/>.
        /// In case there is nothing in a slot, then it will speak "Empty Slot".
        /// Also plays a sound if the slot is grayed out, like tools in <see cref="GeodeMenu">geode menu</see>.
        /// </summary>
        /// <param name="inventoryMenu">The object of <see cref="InventoryMenu"/> whose inventory is to be spoken.</param>
        /// <param name="giveExtraDetails">(Optional) Whether to speak extra details about the item in slot or not. Default to null in which case it uses <see cref="ModConfig.DisableInventoryVerbosity"/> to get whether to speak extra details or not.</param>
        /// <param name="hoverPrice">(Optional) The price of the hovered item, generally used in <see cref="ShopMenu"/>.</param>
        /// <param name="extraItemToShowIndex">(Optional) The index (probably parentSheetIndex) of the extra item which is generally a requirement for the hovered item in certain menus.</param>
        /// <param name="extraItemToShowAmount">(Optional) The amount or quantity of the extra item which is generally a requirement for the hovered item in certain menus.</param>
        /// <param name="highlightedItemPrefix">(Optional) The prefix to add to the spoken hovered item's details if it is highlighted i.e., not grayed out.</param>
        /// <param name="highlightedItemSuffix">(Optional) The suffix to add to the spoken hovered item's details if it is highlighted i.e., not grayed out.</param>
        /// <param name="hoverX">(Optional) The X position on screen to check. Default to null, in which case it uses the mouse's X position.</param>
        /// <param name="hoverY">(Optional) The Y position on screen to check. Default to null, in which case it uses the mouse's Y position.</param>
        /// <returns>true if any inventory slot was hovered or found at the <paramref name="hoverX"/> and <paramref name="hoverY"/>.</returns>
        public bool SpeakHoveredInventorySlot(InventoryMenu? inventoryMenu,
            bool? giveExtraDetails = null,
            int hoverPrice = -1,
            int extraItemToShowIndex = -1,
            int extraItemToShowAmount = -1,
            string highlightedItemPrefix = "",
            string highlightedItemSuffix = "",
            int? hoverX = null,
            int? hoverY = null) =>
            InventoryUtils.NarrateHoveredSlot(inventoryMenu,
                giveExtraDetails,
                hoverPrice,
                extraItemToShowIndex == -1 ? null : extraItemToShowIndex.ToString(),
                extraItemToShowAmount,
                highlightedItemPrefix,
                highlightedItemSuffix,
                hoverX,
                hoverY);

        /// <summary>
        /// Speaks the hovered inventory slot from the provided <see cref="InventoryMenu"/>.
        /// In case there is nothing in a slot, then it will speak "Empty Slot".
        /// Also plays a sound if the slot is grayed out, like tools in <see cref="GeodeMenu">geode menu</see>.
        /// </summary>
        /// <param name="inventoryMenu">The object of <see cref="InventoryMenu"/> whose inventory is to be spoken.</param>
        /// <param name="giveExtraDetails">(Optional) Whether to speak extra details about the item in slot or not. Default to null in which case it uses <see cref="ModConfig.DisableInventoryVerbosity"/> to get whether to speak extra details or not.</param>
        /// <param name="hoverPrice">(Optional) The price of the hovered item, generally used in <see cref="ShopMenu"/>.</param>
        /// <param name="extraItemToShowIndex">(Optional) The index (probably parentSheetIndex) of the extra item which is generally a requirement for the hovered item in certain menus.</param>
        /// <param name="extraItemToShowAmount">(Optional) The amount or quantity of the extra item which is generally a requirement for the hovered item in certain menus.</param>
        /// <param name="highlightedItemPrefix">(Optional) The prefix to add to the spoken hovered item's details if it is highlighted i.e., not grayed out.</param>
        /// <param name="highlightedItemSuffix">(Optional) The suffix to add to the spoken hovered item's details if it is highlighted i.e., not grayed out.</param>
        /// <param name="hoverX">(Optional) The X position on screen to check. Default to null, in which case it uses the mouse's X position.</param>
        /// <param name="hoverY">(Optional) The Y position on screen to check. Default to null, in which case it uses the mouse's Y position.</param>
        /// <returns>true if any inventory slot was hovered or found at the <paramref name="hoverX"/> and <paramref name="hoverY"/>.</returns>
        public bool SpeakHoveredInventorySlot(InventoryMenu? inventoryMenu,
            bool? giveExtraDetails = null,
            int hoverPrice = -1,
            string? extraItemToShowIndex = null,
            int extraItemToShowAmount = -1,
            string highlightedItemPrefix = "",
            string highlightedItemSuffix = "",
            int? hoverX = null,
            int? hoverY = null) =>
            InventoryUtils.NarrateHoveredSlot(inventoryMenu,
                giveExtraDetails,
                hoverPrice,
                extraItemToShowIndex,
                extraItemToShowAmount,
                highlightedItemPrefix,
                highlightedItemSuffix,
                hoverX,
                hoverY);

        /// <summary>
        /// Get the details (name, description, quality, etc.) of an <see cref="Item"/>.
        /// </summary>
        /// <param name="item">The <see cref="Item"/>'s object that we want to get details of.</param>
        /// <param name="giveExtraDetails">(Optional) Whether to also return extra details or not. These include: description, health, stamina and other buffs.</param>
        /// <param name="price">(Optional) Generally the selling price of the item.</param>
        /// <param name="extraItemToShowIndex">(Optional) The index of the extra item which is generally the required item for the given item.</param>
        /// <param name="extraItemToShowAmount">(Optional) The amount or quantity of the extra item.</param>
        /// <returns>The details of the given <paramref name="item"/>.</returns>
        public string GetDetailsOfItem(Item item,
            bool giveExtraDetails = false,
            int price = -1,
            string? extraItemToShowIndex = null,
            int extraItemToShowAmount = -1)
            => InventoryUtils.GetItemDetails(item,
                giveExtraDetails: giveExtraDetails,
                hoverPrice: price,
                extraItemToShowIndex: extraItemToShowIndex,
                extraItemToShowAmount: extraItemToShowAmount);

        /// <summary>
        /// (Legacy! Should not be used from v1.6)
        /// Get the details (name, description, quality, etc.) of an <see cref="Item"/>.
        /// </summary>
        /// <param name="item">The <see cref="Item"/>'s object that we want to get details of.</param>
        /// <param name="giveExtraDetails">(Optional) Whether to also return extra details or not. These include: description, health, stamina and other buffs.</param>
        /// <param name="price">(Optional) Generally the selling price of the item.</param>
        /// <param name="extraItemToShowIndex">(Optional) The index of the extra item which is generally the required item for the given item.</param>
        /// <param name="extraItemToShowAmount">(Optional) The amount or quantity of the extra item.</param>
        /// <returns>The details of the given <paramref name="item"/>.</returns>
        public string GetDetailsOfItem(Item item,
            bool giveExtraDetails = false,
            int price = -1,
            int extraItemToShowIndex = -1,
            int extraItemToShowAmount = -1)
            => InventoryUtils.GetItemDetails(item,
                giveExtraDetails: giveExtraDetails,
                hoverPrice: price,
                extraItemToShowIndex: extraItemToShowIndex == -1 ? null : extraItemToShowIndex.ToString(),
                extraItemToShowAmount: extraItemToShowAmount);

        #endregion

        /// <summary>
        /// Necessary to be called once if you have manually made a custom menu of your mod accessible.
        /// This will skip stardew access' patch that speaks the hover info in that menu.
        /// </summary>
        /// <param name="fullNameOfClass">The full name of the menu's class.
        /// <example>typeof(MyCustomMenu).FullName</example>
        /// </param>
        public void RegisterCustomMenuAsAccessible(string? fullNameOfClass)
        {
            if (string.IsNullOrWhiteSpace(fullNameOfClass))
            {
                Log.Error("fullNameOfClass cannot be null or empty!");
                return;
            }
            
            IClickableMenuPatch.ManuallyPatchedCustomMenus.Add(fullNameOfClass);
        }

        /// <summary>
        /// Registers a language helper to be used for a specific locale.
        /// </summary>
        /// <param name="locale">The locale for which the helper should be used (e.g., "en", "fr", "es-es").</param>
        /// <param name="helper">An instance of the language helper class implementing <see cref="ILanguageHelper"/>.</param>
        /// <remarks>
        /// The provided helper class should ideally derive from <see cref="LanguageHelperBase"/> for optimal compatibility, though this is not strictly required as long as it implements <see cref="ILanguageHelper"/>.
        /// </remarks>
        public void RegisterLanguageHelper(string locale, Type helperType)
        {
#if DEBUG
            Log.Trace($"Registered language helper for locale '{locale}': Type: {helperType.Name}");
#endif
            CustomFluentFunctions.RegisterLanguageHelper(locale, helperType);
        }
#pragma warning restore CA1822 // Mark members as static
    }
}