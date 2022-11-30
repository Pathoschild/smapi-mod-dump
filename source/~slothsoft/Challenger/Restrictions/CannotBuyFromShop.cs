/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System;
using System.Linq;
using Slothsoft.Challenger.Api;
using Slothsoft.Challenger.Common;
using StardewModdingAPI.Events;
using StardewValley.Menus;

namespace Slothsoft.Challenger.Restrictions;

public class CannotBuyFromShop : IRestriction {
    private readonly string[] _bannedShopKeepers;
    private readonly IModHelper _modHelper;

    private EventHandler<MenuChangedEventArgs>? _menuChangedHandler;

    public CannotBuyFromShop(IModHelper modHelper, params string[] bannedShopKeepers) {
        _modHelper = modHelper;
        _bannedShopKeepers = bannedShopKeepers;
    }

    public string GetDisplayText() {
        return CommonHelpers.ToListString(_bannedShopKeepers.Select(k => _modHelper.Translation.Get("CannotBuyFromShop.DisplayText",
            new { shopKeeper = _modHelper.Translation.GetShopDisplayName(k) }).ToString()));
    }

    public void Apply() {
        _menuChangedHandler ??= MenuChanged;
        _modHelper.Events.Display.MenuChanged += _menuChangedHandler;
    }

    private void MenuChanged(object? sender, MenuChangedEventArgs e) {
        if (e.NewMenu is ShopMenu newMenu) {
            // if the shop has a tool for sale, it's not a shop, but Clint's upgrade function
            if (_bannedShopKeepers.Contains(newMenu.storeContext) && newMenu.forSale.Any(s => s is not Tool)) {
                newMenu.exitThisMenuNoSound();
                var shopKeeper = newMenu.portraitPerson?.Name ?? newMenu.storeContext;
                Game1.addHUDMessage(new HUDMessage(
                    _modHelper.Translation.Get("CannotBuyFromShop.Message", new { shopKeeper }),
                    HUDMessage.error_type
                ));
            }
        }
    }

    public void Remove() {
        _modHelper.Events.Display.MenuChanged -= _menuChangedHandler;
    }
}