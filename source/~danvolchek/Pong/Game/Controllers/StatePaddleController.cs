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

namespace Pong.Game.Controllers
{
    internal class StatePaddleController : IntentionalPaddleController
    {
        protected readonly PositionState intendedState;

        public StatePaddleController(PositionState intendedState)
        {
            this.intendedState = intendedState;
        }

        public override void Update()
        {
            this.IntendedPosition = this.intendedState.XPosition;
        }
    }
}
