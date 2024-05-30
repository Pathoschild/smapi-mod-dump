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
    internal class MountHorseHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public MountHorseHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            return this.IsMountingOrDismounting(Game1.player);
        }

        /// <inheritdoc />
        public override void Update(int playerAnimationID)
        {
            this.ApplySkips(
                run: () =>
                {
                    Game1.player.update(Game1.currentGameTime, Game1.player.currentLocation);
                    Game1.player.mount?.update(Game1.currentGameTime, Game1.player.currentLocation);
                },
                until: () => !this.IsMountingOrDismounting(Game1.player)
            );
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
                    || player.mount?.dismounting.Value == true
                );
        }
    }
}
