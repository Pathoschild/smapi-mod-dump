/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using FarmAnimalVarietyRedux.Menus;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace FarmAnimalVarietyRedux.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="PurchaseAnimalsMenu"/> class.</summary>
    internal class PurchaseAnimalsMenuPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The prefix for the <see cref="PurchaseAnimalsMenu"/>() constructor.</summary>
        /// <param name="stock">The animals that were passed to the menu.</param>
        /// <remarks>This is used to keep track of the animals that should be displayed in the new <see cref="CustomPurchaseAnimalsMenu"/>.</remarks>
        internal static void ConstructorPrefix(List<Object> stock) => ModEntry.Instance.AnimalsInPurchaseMenu = stock;

        /// <summary>The prefix for the <see cref="PurchaseAnimalsMenu.draw(SpriteBatch)"/> method.</summary>
        /// <returns><see langword="false"/>, meaning the original method won't get ran.</returns>
        /// <remarks>This is used to stop the menu from rendering, this is because there is a single frame where the old menu gets rendered before the new menu does.</remarks>
        internal static bool DrawPrefix() => false;
    }
}
