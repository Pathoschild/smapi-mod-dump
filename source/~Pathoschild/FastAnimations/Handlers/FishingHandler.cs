/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

#nullable disable

using Pathoschild.Stardew.FastAnimations.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the fishing animation.</summary>
    /// <remarks>See game logic in <see cref="StardewValley.Tools.FishingRod.beginUsing"/>.</remarks>
    internal class FishingHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="multiplier">The animation speed multiplier to apply.</param>
        public FishingHandler(float multiplier)
            : base(multiplier) { }

        /// <summary>Get whether the animation is currently active.</summary>
        /// <param name="playerAnimationID">The player's current animation ID.</param>
        public override bool IsEnabled(int playerAnimationID)
        {
            return
                Context.IsWorldReady
                && Game1.player.UsingTool
                && Game1.player.CurrentTool is FishingRod { isTimingCast: false, isFishing: false };
        }

        /// <summary>Perform any logic needed on update while the animation is active.</summary>
        /// <param name="playerAnimationID">The player's current animation ID.</param>
        public override void Update(int playerAnimationID)
        {
            this.SpeedUpPlayer();
        }
    }
}
