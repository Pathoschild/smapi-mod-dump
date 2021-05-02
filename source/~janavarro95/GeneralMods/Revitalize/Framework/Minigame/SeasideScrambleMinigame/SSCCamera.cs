/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame
{
    public class SSCCamera
    {

        public xTile.Dimensions.Rectangle viewport;

        public Vector2 Position
        {
            get
            {
                return new Vector2(this.viewport.X, this.viewport.Y);
            }
        }

        public SSCCamera()
        {
            this.viewport = new xTile.Dimensions.Rectangle(StardewValley.Game1.viewport);
        }

        public void snapToPosition(Vector2 position)
        {
            this.viewport.Location = new xTile.Dimensions.Location((int)position.X, (int)position.Y);
        }

        public void centerOnPosition(Vector2 position)
        {
            this.viewport.Location = new xTile.Dimensions.Location((int)position.X - (int)(SeasideScramble.self.camera.viewport.Width / 2), (int)position.Y - (int)(SeasideScramble.self.camera.viewport.Height / 2));
        }

        public Rectangle getXNARect()
        {
            return new Rectangle(this.viewport.X, this.viewport.Y, this.viewport.Width, this.viewport.Height);
        }

        public void update(GameTime time)
        {

        }

        /// <summary>
        /// Checks to see if the given position is inside the viewport.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool positionInsideViewport(Vector2 position)
        {
            return this.getXNARect().Contains((int)position.X, (int)position.Y);
        }

        public bool positionInsideViewport(Point position)
        {
            return this.getXNARect().Contains((int)position.X, (int)position.Y);
        }
    }
}
