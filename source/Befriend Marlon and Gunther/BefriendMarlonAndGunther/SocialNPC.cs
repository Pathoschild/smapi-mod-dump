/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/yoshimax2/Befriend-Marlon-and-Gunther
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace BefriendMarlonAndGunther
{
    /// <summary>Wraps an NPC to force it to be socialisable.</summary>
    public class SocialNPC : NPC
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The original NPC.</summary>
        public NPC OriginalNpc { get; }

        /// <summary>Where the NPC can socialize.</summary>
        public override bool CanSocialize { get; } = true;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="npc">The original NPC.</param>
        /// <param name="tile">The initial tile position.</param>
        public SocialNPC(NPC npc, Vector2 tile)
            : base(npc.Sprite, new Vector2(tile.X * Game1.tileSize, tile.Y * Game1.tileSize), npc.DefaultMap, npc.FacingDirection, npc.Name, npc.datable.Value, null, npc.Portrait)
        {
            this.OriginalNpc = npc;
        }

        /// <summary>Force the NPC data to reload.</summary>
        public void ForceReload()
        {
            // force reload for current day
            bool newDay = Game1.newDay;
            try
            {
                Game1.newDay = true;
                this.reloadSprite();
            }
            finally
            {
                Game1.newDay = newDay;
            }

            // set schedule
            //this.scheduleTimeToTry = 9999999; // use current time
            this.checkSchedule(600);
        }
    }
}
