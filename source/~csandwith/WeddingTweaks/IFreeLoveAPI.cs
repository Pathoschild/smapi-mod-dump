/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;

namespace WeddingTweaks
{
    public interface IFreeLoveAPI
    {
        public void PlaceSpousesInFarmhouse(FarmHouse farmHouse);
        public Dictionary<string, NPC> GetSpouses(Farmer farmer, bool all = true);

    }
}