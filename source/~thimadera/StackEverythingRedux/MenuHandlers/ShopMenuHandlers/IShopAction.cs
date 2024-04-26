/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thimadera/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace StackEverythingRedux.MenuHandlers.ShopMenuHandlers
{
    public interface IShopAction
    {
        /// <summary>Gets the size of the stack the action is acting on.</summary>
        int StackAmount { get; }

        /// <summary>Verifies the conditions to perform the action.</summary>
        bool CanPerformAction();

        /// <summary>Does the action.</summary>
        /// <param name="amount">Number of items.</param>
        /// <param name="clickLocation">Where the player clicked.</param>
        void PerformAction(int amount, Point clickLocation);
    }
}
