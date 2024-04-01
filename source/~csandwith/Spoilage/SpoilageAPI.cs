/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Netcode;
using StardewValley;
using System.Collections.Generic;
using System.Globalization;

namespace Spoilage
{
    public interface ISpoilageAPI
    {
        public void SpoilItems(IList<Item> items, float mult = 1);
        public void SpoilFridgeItems(IList<Item> items);
        public void SpoilPlayerItems(IList<Item> items);
        public string SpoiledItem(Item item);
        public float ItemAge(Item item);
    }
    public class SpoilageAPI : ISpoilageAPI
    {
        public void SpoilFridgeItems(IList<Item> items)
        {
            ModEntry.SpoilItems(items, ModEntry.Config.FridgeMult);
        }
        public void SpoilPlayerItems(IList<Item> items)
        {
            ModEntry.SpoilItems(items, ModEntry.Config.PlayerMult);
        }
        public void SpoilItems(IList<Item> items, float mult = 1)
        {
            ModEntry.SpoilItems(items, mult);
        }
        public string SpoiledItem(Item item)
        {
            return !item.modData.TryGetValue(ModEntry.spoiledKey, out string name) ? null : name;
        }
        public float ItemAge(Item item)
        {
            return !item.modData.TryGetValue(ModEntry.ageKey, out string ageString) ? -1 : float.Parse(ageString, NumberStyles.Any, CultureInfo.InvariantCulture);
        }
    }
}