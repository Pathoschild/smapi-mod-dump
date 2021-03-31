/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aRooooooba/SortingChests
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewModdingAPI.Utilities;
using StardewModdingAPI.Events;

namespace SortingChests
{
    class AddedRemoved
    {
        public IEnumerable<Item> Added { get; }
        public IEnumerable<Item> Removed { get; }

        // remove duplicate in two ienumerables
        public AddedRemoved(IEnumerable<Item> added, IEnumerable<Item> removed)
        {
            HashSet<Item> addedSet = new HashSet<Item>(added);
            HashSet<Item> removedSet = new HashSet<Item>(removed);
            addedSet.ExceptWith(removed);
            removedSet.ExceptWith(added);
            Added = addedSet;
            Removed = removedSet;
        }
    }
}
