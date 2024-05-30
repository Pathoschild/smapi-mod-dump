/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Util
{
    public class TerrainFeatureHitResult : HitResult
    {
        private TilePos tilePos;
        private bool insideTile;
        private bool missed;

        private TerrainFeatureHitResult(bool missed, Vector2 location, float direction, TilePos tilePos, bool insideTile) : base(location, direction)
        {
            this.missed = missed;
            this.insideTile = insideTile;
            this.tilePos = tilePos;        
        }

        public TerrainFeatureHitResult(Vector2 location, float direction, TilePos tilePos, bool insideTile) : base(location, direction)
        {
            this.tilePos = tilePos;
            this.insideTile = insideTile;
        }

        public TilePos GetTilePos()
        {
            return tilePos;
        }

        public bool IsInside()
        {
            return insideTile;
        }

        public static TerrainFeatureHitResult Miss​(Vector2 location, float direction, TilePos tilePos)
        {
            return new TerrainFeatureHitResult(true, location, direction, tilePos, false);
        }

        //public TerrainFeatureHitResult WithDirection(float direction)
        //{
        //    return new TerrainFeatureHitResult(this.missed, this.location, direction, this.tilePos, this.insideTile);
        //}

        //public TerrainFeatureHitResult WithPosition​(TilePos tilePos)
        //{
        //    //TilePos newTilePos = new TilePos(this.tilePos.GetTilePosX() + tilePos.GetTilePosX(), this.tilePos.GetTilePosY() + tilePos.GetTilePosY());
        //    return new TerrainFeatureHitResult(this.missed, this.location, this.GetDirection() + this.GetDirection(), tilePos, this.insideTile);
        //}

        public override HitResultType GetHitResultType()
        {
            return HitResultType.TERRAIN_FEATURE;
        }
    }
}
