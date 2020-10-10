/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LukeSeewald/PublicStardewValleyMods
**
*************************************************/

using StardewValley;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace WhatAreYouMissing
{
    public interface IItemBagsAPI
    {
        /// <summary>Added in v1.4.2. Returns all <see cref="StardewValley.Object"/> items that are stored inside of any bags found in the given <paramref name="source"/> list.</summary>
        /// <param name="source">The list of items that will be searched for bags.</param>
        /// <param name="includeNestedBags">OmniBags are specialized types of bags that can hold other bags inside of them. If includeNestedBags is true, then items inside of bags that are nested within Omnibags will also be included in the result list.</param>
        IList<SObject> GetObjectsInsideBags(IList<Item> source, bool includeNestedBags);
    }
}
