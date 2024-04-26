/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using StardewValley;
using SObject = StardewValley.Object;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using PyTK.Extensions;

namespace PyTK.Types
{
    public class ItemSelector<T> where T : Item
    {
        public Func<Item, bool> predicate = o => o is T;

        public ItemSelector(Func<T, bool> predicate = null)
        {
            if (predicate != null)
                this.predicate = o => (o is T) ? predicate.Invoke((T) o) : false;
        }
    }

}
