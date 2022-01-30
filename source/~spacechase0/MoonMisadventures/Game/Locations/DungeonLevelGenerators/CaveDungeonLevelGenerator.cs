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
using StardewValley;
using xTile;

namespace MoonMisadventures.Game.Locations.DungeonLevelGenerators
{
    public class CaveDungeonLevelGenerator : BaseDungeonLevelGenerator
    {
        public override void Generate( AsteroidsDungeon location, ref Vector2 warpFromPrev, ref Vector2 warpFromNext )
        {
            Random rand = new Random( location.genSeed.Value );
            location.isIndoorLevel = true;

            var caveMap = Game1.game1.xTileContent.Load<Map>( Mod.instance.Helper.Content.GetActualAssetKey( "assets/maps/MoonDungeonCave.tmx" ) );

            int x = ( location.Map.Layers[ 0 ].LayerWidth - caveMap.Layers[ 0 ].LayerWidth ) / 2;
            int y = ( location.Map.Layers[ 0 ].LayerHeight - caveMap.Layers[ 0 ].LayerHeight ) / 2;

            location.ApplyMapOverride( caveMap, "actual_map", null, new Rectangle( x, y, caveMap.Layers[ 0 ].LayerWidth, caveMap.Layers[ 0 ].LayerHeight ) );

            var mp = Mod.instance.Helper.Reflection.GetField< Multiplayer >( typeof( Game1 ), "multiplayer" ).GetValue();
            long id = mp.getNewID();
            LunarAnimalType type = rand.Next( 2 ) == 0 ? LunarAnimalType.Cow : LunarAnimalType.Chicken;
            location.Animals.Add( id, new LunarAnimal( type, new Vector2( x + caveMap.Layers[ 0 ].LayerWidth / 2, y + caveMap.Layers[ 0 ].LayerHeight / 2 ) * Game1.tileSize, id ) );

            warpFromPrev = new Vector2( x + 6, y + 10 );
            location.warps.Add( new Warp( x + 6, y + 11, "Custom_MM_MoonAsteroidsDungeon" + location.level.Value / 100, 1, location.level.Value % 100, false ) );
        }
    }
}
