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
using System.Linq;
using Pathoschild.Stardew.FastAnimations.Framework;
using StardewModdingAPI;
using StardewValley;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the horse flute animation.</summary>
    /// <remarks>See game logic in <see cref="StardewValley.Object.performUseAction"/> (search for <c>(O)911</c>).</remarks>
    internal class HorseFluteHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public HorseFluteHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            if (!Context.IsWorldReady)
                return false;

            List<FarmerSprite.AnimationFrame>? animation = Game1.player.Sprite.CurrentAnimation;
            return
                animation?.Any() == true
                && animation[0].frame == 98;
        }

        /// <inheritdoc />
        public override void Update(int playerAnimationID)
        {
            // speed up animation
            this.SpeedUpPlayer();

            // reduce freeze time & horse summon time
            int reduceTimersBy = (int)(BaseAnimationHandler.MillisecondsPerFrame * this.Multiplier);
            Game1.player.freezePause = Math.Max(0, Game1.player.freezePause - reduceTimersBy);
            foreach (DelayedAction action in Game1.delayedActions)
                action.timeUntilAction = Math.Max(0, action.timeUntilAction - reduceTimersBy);
        }
    }
}
