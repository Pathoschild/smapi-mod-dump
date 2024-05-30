/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace ActiveMenuAnywhere.Framework.Options;

internal class ShippingBinOption : BaseOption
{
    private readonly IModHelper helper;

    public ShippingBinOption(Rectangle sourceRect, IModHelper helper) :
        base(I18n.Option_ShippingBin(), sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        var itemGrabMenu = new ItemGrabMenu(null, true, false, Utility.highlightShippableObjects,
            ShipItem, "", null, true, true, false,
            true, false, 0, null, -1, this);
        itemGrabMenu.initializeUpperRightCloseButton();
        itemGrabMenu.setBackgroundTransparency(false);
        itemGrabMenu.setDestroyItemOnClick(true);
        itemGrabMenu.initializeShippingBin();
        Game1.activeClickableMenu = itemGrabMenu;
        Game1.player.showCarrying();
    }

    private void ShipItem(Item? i, Farmer who)
    {
        var farm = Game1.RequireLocation<Farm>("farm");
        if (i != null)
        {
            who.removeItemFromInventory(i);
            farm?.getShippingBin(who).Add(i);
            if (i is Object obj && farm != null)
                helper.Reflection.GetMethod(new ShippingBin(), "showShipment").Invoke(obj, false);
            if (farm != null)
                farm.lastItemShipped = i;
            if (Game1.player.ActiveObject == null)
            {
                Game1.player.showNotCarrying();
                Game1.player.Halt();
            }
        }
    }
}