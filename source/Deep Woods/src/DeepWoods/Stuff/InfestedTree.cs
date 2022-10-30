/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.TerrainFeatures;

namespace DeepWoodsMod
{
    public class InfestedTree : FruitTree
    {
        public InfestedTree(int saplingIndex)
           : base(saplingIndex, 4)
        {
            base.struckByLightningCountdown.Value = 4;
            base.fruitsOnTree.Value = 0;
        }

        public void DeInfest()
        {
            base.struckByLightningCountdown.Value = 0;
            base.fruitsOnTree.Value = 3;
            base.daysUntilMature.Value = 0;
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            if (base.struckByLightningCountdown.Value > 0)
            {
                string backupSeason = this.currentLocation.seasonOverride;
                this.currentLocation.seasonOverride = "winter";
                base.draw(spriteBatch, tileLocation);
                this.currentLocation.seasonOverride = backupSeason;
            }
            else
            {
                base.draw(spriteBatch, tileLocation);
            }
        }
    }
}
