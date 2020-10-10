/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using StardewValley;

namespace Entoarox.Framework.Interface
{
    internal interface IItemContainer
    {
        /*********
        ** Accessors
        *********/
        Item CurrentItem { get; set; }
        bool IsGhostSlot { get; }

        /*********
        ** Methods
        *********/
        bool AcceptsItem(Item item);
    }
}
