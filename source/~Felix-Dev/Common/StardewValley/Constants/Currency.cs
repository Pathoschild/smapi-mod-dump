using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Text;

namespace FelixDev.StardewMods.Common.StardewValley
{
    /// <summary>
    /// Defines constants that specify the currency of an in-game monetary value.
    /// </summary>
    public enum Currency
    { 
        /// <summary>The common in-game money currency.</summary>
        Money = IClickableMenu.currency_g,
        /// <summary>The currency used to buy items at the Stardew Valley Fair festival.</summary>
        StarTokens = IClickableMenu.currency_starTokens,
        /// <summary>The currency used to gamble and buy items in Mr. Qi's casino.</summary>
        QiCoins = IClickableMenu.currency_qiCoins
    }
}
