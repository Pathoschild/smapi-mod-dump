/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Pong.Framework.Game.States;
using StardewModdingAPI.Events;

namespace Pong.Game.Controllers
{
    internal class LocalPlayerController : StatePaddleController
    {
        public LocalPlayerController(PositionState intendedState) : base(intendedState)
        {
        }

        /// <summary>Raised after the player moves the in-game cursor.</summary>
        /// <param name="e">The event arguments.</param>
        public void OnCursorMoved(CursorMovedEventArgs e)
        {
            this.intendedState.XPosition = (int)e.NewPosition.ScreenPixels.X;
        }
    }
}
