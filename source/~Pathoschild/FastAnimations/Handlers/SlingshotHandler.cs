/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using StardewValley.Tools;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the slingshot draw animation.</summary>
    internal sealed class SlingshotHandler : BaseAnimationHandler
    {
        /*********
        ** Fields
        *********/
        /// <summary>The <see cref="GameTime.TotalGameTime"/> in milliseconds when the handler started skipping the current animation.</summary>
        private int LastPullStartTime;

        /// <summary>The total number of milliseconds that were skipped for the current animation.</summary>
        private double SkippedMilliseconds;

        /// <summary>The number of milliseconds to elapse for each skip frame.</summary>
        private const double MillisecondsPerSkip = 1000d / 60; // 60 ticks per second


        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public SlingshotHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool TryApply(int playerAnimationId)
        {
            Farmer player = Game1.player;

            if (player.CurrentTool is Slingshot slingshot && this.IsAnimating(player, slingshot))
            {
                // start new animation
                {
                    int startedSkipAt = (int)slingshot.pullStartTime;
                    if (startedSkipAt != this.LastPullStartTime)
                    {
                        this.LastPullStartTime = startedSkipAt;
                        this.SkippedMilliseconds = 0;
                    }
                }

                // apply skips
                this.ApplySkipsWhile(() =>
                {
                    slingshot.pullStartTime -= SlingshotHandler.MillisecondsPerSkip;
                    this.LastPullStartTime = (int)slingshot.pullStartTime;
                    this.SkippedMilliseconds += SlingshotHandler.MillisecondsPerSkip;

                    TimeSpan skippedTime = TimeSpan.FromMilliseconds(this.SkippedMilliseconds);
                    GameTime time = new GameTime(Game1.currentGameTime.TotalGameTime.Add(skippedTime), Game1.currentGameTime.ElapsedGameTime.Add(skippedTime), Game1.currentGameTime.IsRunningSlowly);

                    player.CurrentTool.tickUpdate(time, player);

                    return this.IsAnimating(player, slingshot);
                });

                return true;
            }

            return false;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get whether the target animation is playing.</summary>
        /// <param name="player">The player to check.</param>
        /// <param name="slingshot">The slingshot to check.</param>
        private bool IsAnimating(Farmer player, Slingshot slingshot)
        {
            return
                player.UsingTool
                && slingshot.pullStartTime > SlingshotHandler.MillisecondsPerSkip; // don't decrement past zero, which will disable firing
        }
    }
}
