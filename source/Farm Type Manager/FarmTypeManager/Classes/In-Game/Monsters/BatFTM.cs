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
    /// <summary>A subclass of Stardew's Bat class, adjusted for use by this mod.</summary>
    public class BatFTM : Bat
    {
        /// <summary>Creates an instance of Stardew's Bat class, but with adjustments made for this mod.</summary>
        public BatFTM()
            : base()
        {

        }

        /// <summary>Creates an instance of Stardew's Bat class, but with adjustments made for this mod.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        /// <param name="mineLevel">A number that affects the type and/or stats of this monster. This normally represents which floor of the mines the monster spawned on (121+ for skull cavern).</param>
        public BatFTM(Vector2 position, int mineLevel)
            : base(position, mineLevel)
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
