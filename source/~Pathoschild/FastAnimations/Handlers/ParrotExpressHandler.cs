/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the parrot-express animation.</summary>
    /// <remarks>See game logic in <see cref="ParrotPlatform.Update"/>.</remarks>
    internal sealed class ParrotExpressHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public ParrotExpressHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool TryApply(int playerAnimationId)
        {
            ParrotPlatform? platform = ParrotPlatform.activePlatform;

            return
                platform is not null
                && this.IsAnimating(platform)
                && this.ApplySkipsWhile(() =>
                {
                    platform.Update(Game1.currentGameTime);

                    return this.IsAnimating(platform);
                });
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get whether the target animation is playing.</summary>
        /// <param name="platform">The parrot platform to check.</param>
        private bool IsAnimating(ParrotPlatform platform)
        {
            return platform.takeoffState > ParrotPlatform.TakeoffState.Idle;
        }
    }
}
