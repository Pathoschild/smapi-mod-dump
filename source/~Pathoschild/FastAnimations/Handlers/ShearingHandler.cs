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
    /// <summary>Handles the wool shearing animation.</summary>
    /// <remarks>See game logic in <see cref="StardewValley.Tools.Shears.beginUsing"/>.</remarks>
    internal sealed class ShearingHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public ShearingHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool TryApply(int playerAnimationId)
        {
            return
                Context.IsWorldReady
                && Game1.player.Sprite.CurrentAnimation != null
                && playerAnimationId is FarmerSprite.shearDown or FarmerSprite.shearLeft or FarmerSprite.shearRight or FarmerSprite.shearUp
                && this.SpeedUpPlayer();
        }
    }
}
