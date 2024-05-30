/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using StardewValley.Tools;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the fishing-treasure-open animation.</summary>
    /// <remarks>See game logic in <see cref="FishingRod.openChestEndFunction"/>.</remarks>
    internal class FishingTreasureHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public FishingTreasureHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            Farmer player = Game1.player;

            return
                playerAnimationID == 84
                && player.CurrentTool is FishingRod
                && this.GetTemporarySprites(player).Any();
        }

        /// <inheritdoc />
        public override void Update(int playerAnimationID)
        {
            Farmer? player = Game1.player;

            this.ApplySkips(
                run: () =>
                {
                    foreach (TemporaryAnimatedSprite sprite in this.GetTemporarySprites(player).ToArray())
                    {
                        bool done = sprite.update(Game1.currentGameTime);
                        if (done)
                            (player.CurrentTool as FishingRod)?.animations.Remove(sprite);
                    }
                },
                until: () => !this.IsEnabled(playerAnimationID)
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the temporary animated sprites added as part of the fishing treasure open animation.</summary>
        /// <param name="player">The player being animated.</param>
        private IEnumerable<TemporaryAnimatedSprite> GetTemporarySprites(Farmer player)
        {
            if (player.CurrentTool is not FishingRod fishingRod)
                yield break;

            foreach (TemporaryAnimatedSprite sprite in fishingRod.animations)
            {
                if (sprite.textureName == Game1.mouseCursors1_6Name && sprite.sourceRect.Intersects(new Rectangle(256, 75, 32 * 4, 32)))
                    yield return sprite;

                if (sprite.textureName == Game1.mouseCursorsName && sprite.sourceRect.Intersects(new Rectangle(64, 1920, 32 * 4, 32)))
                    yield return sprite;
            }
        }
    }
}
