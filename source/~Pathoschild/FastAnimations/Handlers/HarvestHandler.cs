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
using Microsoft.Xna.Framework.Graphics;
using Pathoschild.Stardew.FastAnimations.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the crop harvesting animation.</summary>
    /// <remarks>See game logic in <see cref="GameLocation.checkAction(xTile.Dimensions.Location, xTile.Dimensions.Rectangle, Farmer)"/> (look for <c>who.animateOnce(279</c>), <see cref="FarmerSprite"/>'s private <c>animateOnce(GameTime)</c> method (runs animation + some logic), and <see cref="Farmer.showItemIntake"/>.</remarks>
    internal class HarvestHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public HarvestHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            return
                Context.IsWorldReady
                && Game1.player.Sprite.CurrentAnimation != null
                && playerAnimationID is FarmerSprite.harvestItemDown or FarmerSprite.harvestItemLeft or FarmerSprite.harvestItemRight or FarmerSprite.harvestItemUp
                && !this.IsRidingTractor();
        }

        /// <inheritdoc />
        public override void Update(int playerAnimationID)
        {
            var player = Game1.player;
            var location = player.currentLocation;

            this.ApplySkips(
                run: () =>
                {
                    // player animation
                    player.Update(Game1.currentGameTime, location);

                    // animation of item thrown in the air
                    foreach (var sprite in this.GetTemporarySprites(player))
                    {
                        bool done = sprite.update(Game1.currentGameTime);
                        if (done)
                            location.TemporarySprites.Remove(sprite);
                    }
                }
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the temporary animated sprites added as part of the harvest animation.</summary>
        /// <param name="player">The player being animated.</param>
        /// <remarks>Derived from <see cref="Farmer.showItemIntake"/>.</remarks>
        private IEnumerable<TemporaryAnimatedSprite> GetTemporarySprites(Farmer player)
        {
            // get harvested item
            SObject harvestedObj = player.mostRecentlyGrabbedItem as SObject ?? player.ActiveObject;
            if (harvestedObj == null)
                return Array.Empty<TemporaryAnimatedSprite>();

            // get source rectangles
            var data = ItemRegistry.GetDataOrErrorItem(harvestedObj.QualifiedItemId);
            Texture2D texture = data.GetTexture();
            Rectangle mainSourceRect = data.GetSourceRect();
            Rectangle? coloredSourceRect = null;
            if (harvestedObj is ColoredObject)
                coloredSourceRect = data.GetSourceRect(offset: 1);

            // get temporary sprites
            return player.currentLocation
                .TemporarySprites
                .Where(sprite =>
                    sprite.Texture == texture
                    && (
                        sprite.sourceRect == mainSourceRect
                        || sprite.sourceRect == coloredSourceRect
                    )
                )
                .ToArray();
        }
    }
}
