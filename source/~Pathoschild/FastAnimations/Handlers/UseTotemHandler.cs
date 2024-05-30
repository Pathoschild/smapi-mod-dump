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
    internal class UseTotemHandler : BaseAnimationHandler
    {
        /*********
        ** Fields
        *********/
        /// <summary>The last totem used.</summary>
        private Item? LastTotem;


        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public UseTotemHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            Farmer player = Game1.player;

            if (player.CurrentItem?.Name?.Contains("Totem") == true)
                this.LastTotem = player.CurrentItem;

            return this.IsUsingTotem(player);
        }

        /// <inheritdoc />
        public override void Update(int playerAnimationID)
        {
            Farmer player = Game1.player;
            GameLocation location = player.currentLocation;

            this.ApplySkips(
                () =>
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

                    // rain totem
                    if (this.LastTotem?.QualifiedItemId == "(O)681")
                        Game1.updatePause(Game1.currentGameTime);
                },
                () => !this.IsUsingTotem(player)
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the temporary animated sprites added as part of the use-totem animation.</summary>
        /// <param name="player">The player being animated.</param>
        /// <remarks>Derived from <see cref="SObject.performUseAction"/>.</remarks>
        private IEnumerable<TemporaryAnimatedSprite> GetTemporarySprites(Farmer player)
        {
            if (this.LastTotem is null)
                yield break;

            foreach (TemporaryAnimatedSprite sprite in player.currentLocation.TemporarySprites)
            {
                ParsedItemData data = ItemRegistry.GetDataOrErrorItem(this.LastTotem.QualifiedItemId);

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
            List<FarmerSprite.AnimationFrame>? animation = player.FarmerSprite.currentAnimation;

            return
                animation?.Count > 0
                && animation[0] is { frame: 57, milliseconds: 2000 };
        }
    }
}
