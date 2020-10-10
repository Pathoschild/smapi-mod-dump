/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace Pong.Game.Controllers
{
    internal class ComputerController : IntentionalPaddleController
    {
        private readonly Ball ball;

        public ComputerController(Ball ball)
        {
            this.ball = ball;
        }

        public override void Update()
        {
            Rectangle boundingBox = this.ball.Bounds;
            this.IntendedPosition = boundingBox.X + boundingBox.Width / 2;
        }
    }
}
