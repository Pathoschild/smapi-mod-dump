/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Monsters;

namespace FarmTypeManager.Monsters
{
    /// <summary>A subclass of Stardew's Fly class, adjusted for use by this mod.</summary>
    public class FlyFTM : Fly
    {
        /// <summary>Creates an instance of Stardew's Fly class, but with adjustments made for this mod.</summary>
        public FlyFTM()
            : base()
        {

        }

        /// <summary>Creates an instance of Stardew's Fly class, but with adjustments made for this mod.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        /// <param name="hard">If true, this fly will be the Mutant Fly subtype.</param>
        public FlyFTM(Vector2 position, bool hard)
            : base(position, hard)
        {

        }

        //this override forces any instance of GameLocation to call drawAboveAllLayers, fixing a bug where flying monsters are invisible on some maps
        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {
            base.drawAboveAlwaysFrontLayer(b); //call the base version of this, if one exists
            base.drawAboveAllLayers(b); //call the extra draw method used by flying monsters
        }
    }
}
