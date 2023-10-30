/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewRoguelike.Extensions;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;

namespace StardewRoguelike.ChallengeFloors
{
    internal class GambaChests : ChallengeBase
    {
        private static readonly List<Vector2> ChestTiles = new()
        {
            new(10, 11),
            new(14, 11),
            new(18, 11),
            new(10, 15),
            new(14, 15),
            new(18, 15),
            new(10, 19),
            new(14, 19),
            new(18, 19)
        };

        private bool RemovedChests { get; set; } = false;

        public GambaChests() : base() { }

        public override List<string> MapPaths => new() { "custom-chest" };

        public override bool ShouldSpawnLadder(MineShaft mine)
        {
            return false;
        }

        public void RemoveLocalChests(MineShaft mine)
        {
            List<Vector2> toRemove = mine.overlayObjects
                .Where(kvp => kvp.Value is Chest && (kvp.Value as Chest).frameCounter.Value != 2)
                .Select(kvp => kvp.Key).ToList();

            foreach (Vector2 v in toRemove)
            {
                Chest chest = mine.overlayObjects[v] as Chest;
                Item chestItem = chest.items[0];
                Sign sign = new(v, 39);
                sign.displayItem.Value = chestItem;
                sign.displayType.Value = chestItem is Ring ? 4 : 1;
                mine.overlayObjects[v] = sign;
            }

            RemovedChests = true;
        }

        public override void Update(MineShaft mine, GameTime time)
        {
            if (RemovedChests)
                return;

            int chestCount = mine.overlayObjects.Values.OfType<Chest>().Count();
            if (chestCount != ChestTiles.Count)
                RemoveLocalChests(mine);
        }

        public override void Initialize(MineShaft mine)
        {
            foreach (var tile in ChestTiles)
                mine.SpawnLocalChest(tile);
        }
    }
}
