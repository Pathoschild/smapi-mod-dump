/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewRoguelike.ChallengeFloors;
using StardewRoguelike.UI;
using StardewRoguelike.VirtualProperties;
using StardewValley;
using StardewValley.Locations;
using System.Linq;
using System.Reflection;

namespace StardewRoguelike.Patches
{
    internal class OpenShopPatch : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(GameLocation), "openShopMenu");

        public static bool OnPurchase(ISalable item, Farmer who, int numberToBuy)
        {
            if (item is Object boughtItem && boughtItem.ParentSheetIndex == 803)
                Roguelike.MilksBought += numberToBuy;

            return false;
        }

        public static bool Prefix(GameLocation __instance, ref bool __result, string which)
        {
            if (__instance is not MineShaft mine)
                return true;

            if (which.Equals("Roguelike"))
            {
                if (Merchant.CurrentShop is not RefreshableShopMenu && Perks.HasPerk(Perks.PerkType.Indecisive))
                    Merchant.CurrentShop = new RefreshableShopMenu(Merchant.CurrentShop.itemPriceAndStock, false, context: "Blacksmith", on_purchase: OnPurchase);

                foreach (ISalable buyback_item in Merchant.CurrentShop.buyBackItems)
                    Merchant.CurrentShop.itemPriceAndStock.Remove(buyback_item);

                Merchant.CurrentShop.forSale = Merchant.CurrentShop.itemPriceAndStock.Keys.ToList();

                Merchant.CurrentShop.currentItemIndex = 0;
                MethodInfo? scroll = Merchant.CurrentShop.GetType().GetMethod("setScrollBarToCurrentIndex", BindingFlags.Instance | BindingFlags.NonPublic);
                scroll?.Invoke(Merchant.CurrentShop, null);

                Merchant.CurrentShop.buyBackItems.Clear();
                Merchant.CurrentShop.buyBackItemsToResellTomorrow.Clear();
                Game1.activeClickableMenu = Merchant.CurrentShop;

                __result = true;
                return false;
            }
            else if (which.Equals("RoguelikeDiscounted") && ChallengeFloor.IsChallengeFloor(mine))
            {
                PickAPath pickAPath = (PickAPath)mine.get_MineShaftChallengeFloor().Value;
                Game1.activeClickableMenu = pickAPath.CurrentShop;

                __result = true;
                return false;
            }
            else if (which.Equals("Perks"))
            {
                Perks.CurrentMenu.isActive = true;
                Perks.CurrentMenu.informationUp = true;
                Game1.activeClickableMenu = Perks.CurrentMenu;

                __result = true;
                return false;
            }
            else if (which.Equals("Seeds"))
            {
                Game1.activeClickableMenu = Merchant.CurrentSeedShop;

                __result = true;
                return false;
            }

            return true;
        }
    }
}
