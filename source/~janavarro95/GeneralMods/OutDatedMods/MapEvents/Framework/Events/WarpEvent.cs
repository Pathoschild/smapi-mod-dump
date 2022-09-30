/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using EventSystem.Framework.FunctionEvents;
using EventSystem.Framework.Information;
using Microsoft.Xna.Framework;
using StardewValley;

namespace EventSystem.Framework.Events
{
    /// <summary>Used to handle warp events on the map.</summary>
    public class WarpEvent : MapEvent
    {
        private readonly WarpInformation warpInfo;

        /// <summary>Constructor for handling warp events.</summary>
        /// <param name="Name">The name of the event.</param>
        /// <param name="Location">The game location that this event is located at.</param>
        /// <param name="Position">The x,y tile position of the event.</param>
        /// <param name="playerEvents">The events to occur when the player enters the warp tile before the warp.</param>
        /// <param name="WarpInfo">The information for warping the farmer.</param>
        public WarpEvent(string Name, GameLocation Location, Vector2 Position, PlayerEvents playerEvents, WarpInformation WarpInfo)
            : base(Name, Location, Position, playerEvents)
        {
            this.name = Name;
            this.location = Location;
            this.tilePosition = Position;
            this.playerEvents = playerEvents;
            this.warpInfo = WarpInfo;

            this.doesInteractionNeedToRun = true;
        }

        /// <summary>Occurs when the player enters the warp tile event position.</summary>
        public override bool OnPlayerEnter()
        {
            if (!base.OnPlayerEnter())
                return false;

            Game1.warpFarmer(this.warpInfo.targetMapName, this.warpInfo.targetX, this.warpInfo.targetY, this.warpInfo.facingDirection, this.warpInfo.isStructure);
            return true;
        }

        /// <summary>Runs when the player is not on the tile and resets player interaction.</summary>
        public override bool OnPlayerLeave()
        {
            if (!base.OnPlayerLeave()) return false;
            return true;
        }

        /// <summary>Used to update the event and check for interaction.</summary>
        public override void update()
        {
            this.OnPlayerEnter();
            this.OnPlayerLeave();
        }
    }
}
