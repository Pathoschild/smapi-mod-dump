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
using Pathoschild.Stardew.Common;
using Pathoschild.Stardew.FastAnimations.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the falling-tree animation.</summary>
    /// <remarks>See game logic in <see cref="Tree.tickUpdate"/>.</remarks>
    internal class TreeFallingHandler : BaseAnimationHandler
    {
        /*********
        ** Fields
        *********/
        /// <summary>The trees in the current location.</summary>
        private Dictionary<Vector2, TerrainFeature> Trees = new();


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="multiplier">The animation speed multiplier to apply.</param>
        public TreeFallingHandler(float multiplier)
            : base(multiplier) { }

        /// <summary>Perform any updates needed when the player enters a new location.</summary>
        /// <param name="location">The new location.</param>
        public override void OnNewLocation(GameLocation location)
        {
            this.Trees =
                (
                    from pair in location.terrainFeatures.FieldDict
                    let tree = pair.Value.Value as Tree
                    let fruitTree = pair.Value.Value as FruitTree
                    where
                        (
                            tree != null
                            && !tree.stump.Value
                            && tree.growthStage.Value > Tree.bushStage
                        )
                        || (
                            fruitTree != null
                            && !fruitTree.stump.Value
                            && fruitTree.growthStage.Value > FruitTree.bushStage
                        )
                    select pair
                )
                .ToDictionary(p => p.Key, p => p.Value.Value);
        }

        /// <summary>Get whether the animation is currently active.</summary>
        /// <param name="playerAnimationID">The player's current animation ID.</param>
        public override bool IsEnabled(int playerAnimationID)
        {
            return
                Context.IsWorldReady
                && this.GetFallingTrees().Any();
        }

        /// <summary>Perform any logic needed on update while the animation is active.</summary>
        /// <param name="playerAnimationID">The player's current animation ID.</param>
        public override void Update(int playerAnimationID)
        {
            GameTime gameTime = Game1.currentGameTime;

            int skips = this.GetSkipsThisTick();
            foreach (TerrainFeature tree in this.GetFallingTrees())
                this.ApplySkips(skips, () => tree.tickUpdate(gameTime));
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get all trees in the current location which are currently falling.</summary>
        private IEnumerable<TerrainFeature> GetFallingTrees()
        {
            Rectangle visibleTiles = TileHelper.GetVisibleArea();
            foreach (KeyValuePair<Vector2, TerrainFeature> pair in this.Trees)
            {
                if (visibleTiles.Contains((int)pair.Key.X, (int)pair.Key.Y))
                {
                    bool isFalling = pair.Value switch
                    {
                        Tree tree => tree.falling.Value,
                        FruitTree tree => tree.falling.Value,
                        _ => false
                    };

                    if (isFalling)
                        yield return pair.Value;
                }
            }
        }
    }
}
