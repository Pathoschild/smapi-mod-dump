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
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the hold-up-item animation.</summary>
    /// <remarks>See game logic in <see cref="Farmer.holdUpItemThenMessage"/>.</remarks>
    internal class HoldUpItemHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public HoldUpItemHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            Farmer player = Game1.player;
            if (player.mostRecentlyGrabbedItem is null)
                return false;

            List<FarmerSprite.AnimationFrame>? animation = player.FarmerSprite?.currentAnimation;
            return
                animation?.Count == 3
                && animation[0] is { frame: 57, milliseconds: 0 }
                && animation[1] is { frame: 57, milliseconds: 2500 }
                && animation[2] is { milliseconds: 500 };
        }

        /// <inheritdoc />
        public override void Update(int playerAnimationID)
        {
            Farmer player = Game1.player;
            GameLocation location = Game1.currentLocation;

            this.ApplySkips(
                run: () =>
                {
                    // player animation
                    player.Update(Game1.currentGameTime, location);

                    // animation of item thrown in the air
                    foreach (TemporaryAnimatedSprite sprite in this.GetTemporarySprites(player).ToArray())
                    {
                        bool done = sprite.update(Game1.currentGameTime);
                        if (done)
                            location.TemporarySprites.Remove(sprite);
                    }
                },
                until: () => !this.IsEnabled(playerAnimationID)
            );

            // reduce freeze time
            int reduceTimersBy = (int)(BaseAnimationHandler.MillisecondsPerFrame * this.Multiplier);
            player.freezePause = Math.Max(0, player.freezePause - reduceTimersBy);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the temporary animated sprites added as part of the item-hold-up animation.</summary>
        /// <param name="player">The player being animated.</param>
        /// <remarks>Derived from <see cref="Farmer.showHoldingItem"/>.</remarks>
        private IEnumerable<TemporaryAnimatedSprite> GetTemporarySprites(Farmer player)
        {
            // get hold up item
            Item? holdingItem = player.mostRecentlyGrabbedItem;

            foreach (TemporaryAnimatedSprite sprite in player.currentLocation.TemporarySprites)
            {
                switch (holdingItem)
                {
                    case null:
                        if (sprite.textureName == "LooseSprites\\Cursors" && sprite.sourceRect == new Rectangle(420, 489, 25, 18))
                            yield return sprite;
                        break;

                    case SpecialItem specialItem:
                        if (
                            sprite.textureName == "LooseSprites\\Cursors"
                            && (
                                sprite.sourceRect == new Rectangle(Game1.player.MaxItems == Farmer.maxInventorySpace ? 268 : 257, 1436, Game1.player.MaxItems == Farmer.maxInventorySpace ? 11 : 9, 13)
                                || sprite.sourceRect == new Rectangle(129 + 16 * specialItem.which.Value, 320, 16, 16)
                            )
                        )
                            yield return sprite;
                        break;

                    default:
                        {
                            ParsedItemData data = ItemRegistry.GetDataOrErrorItem(holdingItem.QualifiedItemId);
                            if (sprite.textureName == data.TextureName && sprite.sourceRect == data.GetSourceRect())
                                yield return sprite;
                            break;
                        }
                }
            }
        }
    }
}
