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
using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the read-book animation.</summary>
    /// <remarks>See game logic in <see cref="SObject.readBook"/>.</remarks>
    internal sealed class ReadBookHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public ReadBookHandler(float multiplier)
            : base(multiplier) { }

        public override bool TryApply(int playerAnimationId)
        {
            Farmer player = Game1.player;

            if (this.IsAnimating(player))
            {
                this.SpeedUpPlayer(until: () => !this.IsAnimating(player));

                // reduce freeze time
                int reduceTimersBy = (int)(BaseAnimationHandler.MillisecondsPerFrame * this.Multiplier);
                Game1.player.freezePause = Math.Max(0, Game1.player.freezePause - reduceTimersBy);

                return true;
            }

            return false;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get whether the target animation is playing.</summary>
        /// <param name="player">The player to check.</param>
        private bool IsAnimating(Farmer player)
        {
            return player.FarmerSprite.currentAnimation is [{ frame: 57, milliseconds: 1000 }, ..];
        }
    }
}
