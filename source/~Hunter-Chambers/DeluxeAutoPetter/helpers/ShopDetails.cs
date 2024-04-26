/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hunter-Chambers/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.GameData.Shops;

namespace DeluxeAutoPetter.helpers
{
    internal static class ShopDetails
    {
        private static ShopItemData? DELUXE_AUTO_PETTER_SHOP_DATA;

        public static void Initialize()
        {
            DELUXE_AUTO_PETTER_SHOP_DATA = new()
            {
                Id = $"(BC){ObjectDetails.GetDeluxeAutoPetterID()}",
                ItemId = $"(BC){ObjectDetails.GetDeluxeAutoPetterID()}",
                Condition = $"PLAYER_HAS_MAIL Current {QuestMail.GetQuestRewardMailID()} Received",
                Price = 75000
            };
        }

        public static void LoadShopItem()
        {
            if (DELUXE_AUTO_PETTER_SHOP_DATA is null) throw new ArgumentNullException($"{nameof(DELUXE_AUTO_PETTER_SHOP_DATA)} has not been initialized! The {nameof(Initialize)} method must be called first!");

            if (!DataLoader.Shops(Game1.content)["AnimalShop"].Items.Any(itemData => itemData.Id.Equals($"(BC){ObjectDetails.GetDeluxeAutoPetterID()}")))
                DataLoader.Shops(Game1.content)["AnimalShop"].Items.Add(DELUXE_AUTO_PETTER_SHOP_DATA);
        }
    }
}
