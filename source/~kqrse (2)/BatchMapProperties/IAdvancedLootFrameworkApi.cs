/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;

namespace DynamicMapTiles
{
    public interface IAdvancedLootFrameworkApi
    {
        List<object> LoadPossibleTreasures(string[] itemTypeList, int minItemValue, int maxItemValue);
        List<Item> GetChestItems(List<object> treasures, Dictionary<string, int> itemChances, int maxItems, int minItemValue, int maxItemValue, int mult, float increaseRate, int baseValue);
        int GetChestCoins(int mult, float increaseRate, int baseMin, int baseMax);
        Chest MakeChest(List<Item> chestItems, int coins, Vector2 chestSpot);
        Chest MakeChest(List<object> treasures, Dictionary<string, int> itemChances, int maxItems, int minItemValue, int maxItemValue, int mult, float increaseRate, int itemBaseValue, int coinBaseMin, int coinBaseMax, Vector2 chestSpot);
    }
}