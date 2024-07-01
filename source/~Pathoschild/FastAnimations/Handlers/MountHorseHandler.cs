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
using StardewModdingAPI;
using StardewValley;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the horse mount/dismount animation.</summary>
    internal sealed class MountHorseHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public MountHorseHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool TryApply(int playerAnimationId)
        {
            Farmer player = Game1.player;

            return
                this.IsMountingOrDismounting(player)
                && this.ApplySkipsWhile(() =>
                {
                    player.update(Game1.currentGameTime, player.currentLocation);
                    player.mount?.update(Game1.currentGameTime, player.currentLocation);

                    return this.IsMountingOrDismounting(player);
                });
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get whether the player is currently mounting or dismounting a horse.</summary>
        /// <param name="player">The player to check.</param>
        private bool IsMountingOrDismounting(Farmer player)
        {
            return
                Context.IsWorldReady
                && (
                    player.isAnimatingMount
                    || player.mount?.dismounting.Value is true
                );
        }
    }
}
