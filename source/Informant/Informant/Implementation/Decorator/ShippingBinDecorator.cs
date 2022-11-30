/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Slothsoft.Informant.Api;

namespace Slothsoft.Informant.Implementation.Decorator;

internal class ShippingBinDecorator : IDecorator<Item> {
    
    private static readonly int[] Ship15Items =
        {24, 188, 190, 192, 248, 250, 252, 254, 256, 258, 260, 262, 264, 266, 268, 270, 272, 274, 276, 278, 280, 282, 284, 300, 304, 398, 400, 433};
    
    private static Texture2D? _shippingBin;
    
    private readonly IModHelper _modHelper;
    
    public ShippingBinDecorator(IModHelper modHelper) {
        _modHelper = modHelper;
        _shippingBin ??= modHelper.ModContent.Load<Texture2D>("assets/shipping_bin.png");
    }

    public string Id => "shipping";
    public string DisplayName => _modHelper.Translation.Get("ShippingBinDecorator");
    public string Description => _modHelper.Translation.Get("ShippingBinDecorator.Description");

    public bool HasDecoration(Item input) {
        if (_shippingBin != null && input is SObject obj && !obj.bigCraftable.Value) {
            if (!obj.countsForShippedCollection()) {
                // we do not need to ship this item
                return false;
            }
            var alreadyShipped = Game1.player.basicShipped.ContainsKey(input.ParentSheetIndex) ? Game1.player.basicShipped[input.ParentSheetIndex] : 0;
            
            if (!Ship15Items.Contains(input.ParentSheetIndex)) {
                // we only need to ship this item once
                return alreadyShipped == 0;
            }
            const int needToBeShipped = 15; 
            // we need to ship this item 15 times
            return alreadyShipped < needToBeShipped;
        }
        return false;
    }

    public Decoration Decorate(Item input) {
        return new Decoration(_shippingBin!) {
            Counter = CalculateStillNeeded(input),
        };
    }

    public int? CalculateStillNeeded(Item input) {
        if (!Ship15Items.Contains(input.ParentSheetIndex)) {
            // we don't need to show any number because we don't need to ship 15
            return null;
        }
        var alreadyShipped = Game1.player.basicShipped.ContainsKey(input.ParentSheetIndex) ? Game1.player.basicShipped[input.ParentSheetIndex] : 0;
        const int needToBeShipped = 15;
        if (alreadyShipped >= needToBeShipped) {
            // we don't need to ship anything? why even show the decorator?
            return null;
        }
        return needToBeShipped - alreadyShipped;
    }
}