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
    internal class ShearingHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="multiplier">The animation speed multiplier to apply.</param>
        public ShearingHandler(float multiplier)
            : base(multiplier) { }

        /// <summary>Get whether the animation is currently active.</summary>
        /// <param name="playerAnimationID">The player's current animation ID.</param>
        public override bool IsEnabled(int playerAnimationID)
        {
            return
                Context.IsWorldReady
                && Game1.player.Sprite.CurrentAnimation != null
                && playerAnimationID is FarmerSprite.shearDown or FarmerSprite.shearLeft or FarmerSprite.shearRight or FarmerSprite.shearUp;
        }

        /// <summary>Perform any logic needed on update while the animation is active.</summary>
        /// <param name="playerAnimationID">The player's current animation ID.</param>
        public override void Update(int playerAnimationID)
        {
            this.SpeedUpPlayer();
        }
    }
}
