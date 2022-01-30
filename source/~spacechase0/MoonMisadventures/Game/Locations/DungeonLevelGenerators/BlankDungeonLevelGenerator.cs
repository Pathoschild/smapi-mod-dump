/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MoonMisadventures.Game.Locations.DungeonLevelGenerators
{
    public class BlankDungeonLevelGenerator : BaseDungeonLevelGenerator
    {
        public override void Generate( AsteroidsDungeon location, ref Vector2 warpFromPrev, ref Vector2 warpFromNext )
        {
            // spooky, no tiles to stand on
            warpFromPrev = new Vector2( 5, 5 );
            warpFromNext = new Vector2( 5, 5 );

            PlacePreviousWarp( location, 5, 4 );
            PlaceNextWarp( location, 5, 6 );
        }
    }
}
