/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Revitalize.Framework.Objects;
using StardewValley;

namespace Revitalize.Framework.Player.Managers
{
    public class SittingInfo
    {

        /// <summary>How long a Farmer has sat (in milliseconds)</summary>
        private int elapsedTime;

        /// <summary>Gets how long the farmer has sat (in milliseconds).</summary>
        public int ElapsedTime => this.elapsedTime;

        /// <summary>Keeps trck of time elapsed.</summary>
        GameTime timer;
        
        /// <summary>How long a player has to sit to recover energy/health;</summary>
        public int SittingSpan { get; }

        StardewValley.Object sittingObject;

        public StardewValley.Object SittingObject
        {
            get
            {
                return this.sittingObject;
            }
        }

        /// <summary>Construct an instance.</summary>
        public SittingInfo()
        {
            this.timer = Game1.currentGameTime;
            this.SittingSpan = 10000;
        }

        /// <summary>Update the sitting info.</summary>
        public void update()
        {
            if (Game1.activeClickableMenu != null) return;

            if (Game1.player.isMoving())
            {
               
                this.elapsedTime = 0;
            }
            if (Game1.player.IsSitting() && Game1.player.CanMove)
            {
                if (this.timer == null) this.timer = Game1.currentGameTime;
                this.elapsedTime += this.timer.ElapsedGameTime.Milliseconds;
            }

            if (this.elapsedTime >= this.SittingSpan)
            {
                this.elapsedTime %= this.SittingSpan;
                Game1.player.health++;
                Game1.player.Stamina++;
            }

        }

    }
}
