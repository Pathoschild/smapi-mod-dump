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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.GameData.WildTrees;
using StardewValley.TerrainFeatures;

namespace Pathoschild.Stardew.LookupAnything.Framework.Lookups.TerrainFeatures
{
    /// <summary>Positional metadata about a wild tree.</summary>
    internal class TreeTarget : GenericTarget<Tree>
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="gameHelper">Provides utility methods for interacting with the game code.</param>
        /// <param name="value">The underlying in-game entity.</param>
        /// <param name="tilePosition">The object's tile position in the current location (if applicable).</param>
        /// <param name="getSubject">Get the subject info about the target.</param>
        public TreeTarget(GameHelper gameHelper, Tree value, Vector2 tilePosition, Func<ISubject> getSubject)
            : base(gameHelper, SubjectType.WildTree, value, tilePosition, getSubject) { }

        /// <summary>Get the sprite's source rectangle within its texture.</summary>
        public override Rectangle GetSpritesheetArea()
        {
            Tree tree = this.Value;

            // stump
            if (tree.stump.Value)
                return Tree.stumpSourceRect;

            // growing tree
            if (tree.growthStage.Value < 5)
            {
                return (WildTreeGrowthStage)tree.growthStage.Value switch
                {
                    WildTreeGrowthStage.Seed => new Rectangle(32, 128, 16, 16),
                    WildTreeGrowthStage.Sprout => new Rectangle(0, 128, 16, 16),
                    WildTreeGrowthStage.Sapling => new Rectangle(16, 128, 16, 16),
                    _ => new Rectangle(0, 96, 16, 32)
                };
            }

            // grown tree
            return Tree.treeTopSourceRect;
        }

        /// <summary>Get a rectangle which roughly bounds the visible sprite relative the viewport.</summary>
        /// <remarks>Reverse-engineered from <see cref="Tree.draw"/>.</remarks>
        public override Rectangle GetWorldArea()
        {
            return this.GetSpriteArea(this.Value.getBoundingBox(), this.GetSpritesheetArea());
        }

        /// <summary>Get whether the visible sprite intersects the specified coordinate. This can be an expensive test.</summary>
        /// <param name="tile">The tile to search.</param>
        /// <param name="position">The viewport-relative coordinates to search.</param>
        /// <param name="spriteArea">The approximate sprite area calculated by <see cref="GetWorldArea"/>.</param>
        /// <remarks>Reverse engineered from <see cref="Tree.draw"/>.</remarks>
        public override bool SpriteIntersectsPixel(Vector2 tile, Vector2 position, Rectangle spriteArea)
        {
            // get tree
            Tree tree = this.Value;
            WildTreeGrowthStage growth = (WildTreeGrowthStage)tree.growthStage.Value;

            // get sprite data
            Texture2D spriteSheet = tree.texture.Value;
            SpriteEffects spriteEffects = tree.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // check tree sprite
            if (this.SpriteIntersectsPixel(tile, position, spriteArea, spriteSheet, this.GetSpritesheetArea(), spriteEffects))
                return true;

            // check stump attached to bottom of grown tree
            if (growth == WildTreeGrowthStage.Tree)
            {
                Rectangle stumpSpriteArea = new Rectangle(spriteArea.Center.X - (Tree.stumpSourceRect.Width / 2 * Game1.pixelZoom), spriteArea.Y + spriteArea.Height - Tree.stumpSourceRect.Height * Game1.pixelZoom, Tree.stumpSourceRect.Width * Game1.pixelZoom, Tree.stumpSourceRect.Height * Game1.pixelZoom);
                if (stumpSpriteArea.Contains((int)position.X, (int)position.Y) && this.SpriteIntersectsPixel(tile, position, stumpSpriteArea, spriteSheet, Tree.stumpSourceRect, spriteEffects))
                    return true;
            }

            return false;
        }
    }
}
