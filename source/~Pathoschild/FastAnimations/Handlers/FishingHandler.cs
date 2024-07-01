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
using StardewValley.Tools;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the fishing animation.</summary>
    /// <remarks>See game logic in <see cref="StardewValley.Tools.FishingRod.beginUsing"/>.</remarks>
    internal sealed class FishingHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public FishingHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool TryApply(int playerAnimationId)
        {
            return
                Context.IsWorldReady
                && Game1.player.UsingTool
                && Game1.player.CurrentTool is FishingRod { isTimingCast: false, isFishing: false }
                && this.SpeedUpPlayer();
        }
    }
}
