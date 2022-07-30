/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;

namespace FreeLove
{
    public class FreeLoveAPI
    {
        public void PlaceSpousesInFarmhouse(FarmHouse farmHouse)
        {
            ModEntry.PlaceSpousesInFarmhouse(farmHouse);
        }
        public Dictionary<string, NPC> GetSpouses(Farmer farmer, bool all = true)
        {
            return ModEntry.GetSpouses(farmer, all);
        }
        public Dictionary<string, NPC> GetSpouses(Farmer farmer, int all = -1)
        {
            return ModEntry.GetSpouses(farmer, all != 0);
        }
        public void SetLastPregnantSpouse(string name)
        {
            ModEntry.lastPregnantSpouse = Game1.getCharacterFromName(name);
        }
    }
}