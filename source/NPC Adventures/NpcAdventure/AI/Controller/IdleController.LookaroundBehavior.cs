using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using NpcAdventure.Internal;
using NpcAdventure.Loader;
using NpcAdventure.Utils;
using StardewModdingAPI;
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
