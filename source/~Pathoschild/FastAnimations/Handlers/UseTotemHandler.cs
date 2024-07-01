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
using StardewValley.ItemTypeDefinitions;
using SObject = StardewValley.Object;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the use-totem animation.</summary>
    /// <remarks>See game logic in <see cref="SObject.performUseAction"/>.</remarks>
    internal sealed class UseTotemHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public UseTotemHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool TryApply(int playerAnimationId)
        {
            Farmer player = Game1.player;
            Item? totem = player.CurrentItem;

            return
                totem?.Name?.Contains("Totem") is true
                && this.IsUsingTotem(player)
                && this.ApplySkipsWhile(() =>
                {
                    // player animation
                    player.Update(Game1.currentGameTime, player.currentLocation);

                    // animation of item thrown in the air
                    foreach (TemporaryAnimatedSprite sprite in this.GetTemporarySprites(player, totem).ToArray())
                    {
                        bool done = sprite.update(Game1.currentGameTime);
                        if (done)
                            player.currentLocation.TemporarySprites.Remove(sprite);
                    }

                    // rain totem
                    if (totem.QualifiedItemId == "(O)681")
                        Game1.updatePause(Game1.currentGameTime);

                    return this.IsUsingTotem(player);
                });
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the temporary animated sprites added as part of the use-totem animation.</summary>
        /// <param name="player">The player being animated.</param>
        /// <param name="totem">The totem being animated.</param>
        /// <remarks>Derived from <see cref="SObject.performUseAction"/>.</remarks>
        private IEnumerable<TemporaryAnimatedSprite> GetTemporarySprites(Farmer player, Item totem)
        {
            foreach (TemporaryAnimatedSprite sprite in player.currentLocation.TemporarySprites)
            {
                ParsedItemData data = ItemRegistry.GetDataOrErrorItem(totem.QualifiedItemId);

                // Item sprite
                if (sprite.textureName == data.TextureName && sprite.sourceRect == data.GetSourceRect())
                    yield return sprite;

                // Sprinkles sprite
                if (sprite.textureName == Game1.animationsName && sprite.sourceRect.Intersects(new Rectangle(0, 10 * 64, 64 * 8, 128)))
                    yield return sprite;

                // Rain totem sprite
                if (sprite.initialParentTileIndex == 0)
                    yield return sprite;
            }
        }

        /// <summary>Check whether the current player's animation is the animation of using totem.</summary>
        /// <remarks>Derived from <see cref="SObject.performUseAction"/>.</remarks>
        private bool IsUsingTotem(Farmer player)
        {
            return player.FarmerSprite.currentAnimation is [{ frame: 57, milliseconds: 2000 }, ..];
        }
    }
}
