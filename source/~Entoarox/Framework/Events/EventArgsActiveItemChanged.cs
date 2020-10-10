/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System;
using StardewValley;

namespace Entoarox.Framework.Events
{
    public class EventArgsActiveItemChanged : EventArgs
    {
        public readonly Item OldItem;
        public readonly Item NewItem;

        public EventArgsActiveItemChanged(Item oldItem, Item newItem)
        {
            this.OldItem = oldItem;
            this.NewItem = newItem;
        }
    }
}
