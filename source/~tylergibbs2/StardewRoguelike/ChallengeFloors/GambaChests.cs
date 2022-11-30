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
using StardewValley.Locations;
using System.Collections.Generic;

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

        public GambaChests() : base() { }

        public override List<string> MapPaths => new() { "custom-chest" };

        public override bool ShouldSpawnLadder(MineShaft mine)
        {
            return false;
        }

        public override void Initialize(MineShaft mine)
        {
            base.Initialize(mine);
        }
    }
}
