/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;

namespace UndergroundSecrets
{
    public interface ITreasureChestsExpandedApi 
    {
        List<Item> GetChestItems(int level);
        int GetChestCoins(int level);
        Chest MakeChest(List<Item> chestItems, int coins, Vector2 chestSpot);
        Chest MakeChest(int mult, Vector2 chestSpot);
    }
}