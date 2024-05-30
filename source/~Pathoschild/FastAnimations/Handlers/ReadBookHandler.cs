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
using System.Collections.Generic;
using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the read-book animation.</summary>
    /// <remarks>See game logic in <see cref="SObject.readBook"/>.</remarks>
    internal class ReadBookHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public ReadBookHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            List<FarmerSprite.AnimationFrame>? animation = Game1.player.FarmerSprite.currentAnimation;

            return
                animation?.Count > 0
                && animation[0] is { frame: 57, milliseconds: 1000 };
        }

        /// <inheritdoc />
        public override void Update(int playerAnimationID)
        {
            this.SpeedUpPlayer(() => !this.IsEnabled(playerAnimationID));

            // reduce freeze time
            int reduceTimersBy = (int)(BaseAnimationHandler.MillisecondsPerFrame * this.Multiplier);
            Game1.player.freezePause = Math.Max(0, Game1.player.freezePause - reduceTimersBy);
        }
    }
}
