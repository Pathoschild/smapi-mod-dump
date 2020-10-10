/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley;

namespace NpcAdventure.AI.Controller
{
    /// <summary>
    /// Idle behaviors
    /// </summary>
    partial class IdleController
    {
        private class LookAroundBehavior : IdleBehavior
        {
            private int beforeTurnFrames;
            private int minSeconds;
            private int maxSeconds;
            private int direction;

            public LookAroundBehavior(IdleController controller, int minSeconds, int maxSeconds) : base(controller)
            {
                this.minSeconds = minSeconds;
                this.maxSeconds = maxSeconds;
            }

            public override void StartBehavior()
            {
                base.StartBehavior();
                this.ChangeLook();
            }

            public override void Update(UpdateTickedEventArgs e)
            {
                base.Update(e);

                if (--this.beforeTurnFrames <= 0)
                {
                    this.ChangeLook();
                }
            }

            private void ChangeLook()
            {
                this.direction = Game1.random.Next(0, 3);
                this.npc.FacingDirection = this.direction;
                this.npc.Sprite.faceDirectionStandard(this.direction);
                this.beforeTurnFrames = Game1.random.Next(this.minSeconds * 60, this.maxSeconds * 60);
            }
        }
    }
}
