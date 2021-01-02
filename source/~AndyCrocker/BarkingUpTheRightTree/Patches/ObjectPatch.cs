/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace BarkingUpTheRightTree.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="StardewValley.Object"/> class.</summary>
    internal static class ObjectPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The post fix for the <see cref="StardewValley.Object.isWildTreeSeed(int)"/> method.</summary>
        /// <param name="index">The object id to check if it's a wild tree seed.</param>
        /// <param name="__result">The return value of the method being patched.</param>
        /// <remarks>This is used to add the custom tree seeds as valid tree seeds.</remarks>
        internal static void IsWildTreeSeedPostFix(int index, ref bool __result)
        {
            // set result to true if there are any custom trees with the passed object as its seed
            if (ModEntry.Instance.Api.GetAllTrees().Any(customTree => customTree.Seed == index))
                __result = true;
        }

        /// <summary>The transpiler for the <see cref="StardewValley.Object.placementAction(StardewValley.GameLocation, int, int, StardewValley.Farmer)"/> method.</summary>
        /// <param name="instructions">The IL instructions.</param>
        /// <returns>The new IL instructions.</returns>
        /// <remarks>This is used to stop the stop tappers being able to be placed on trees.<br/>This is to make it determine if tappers can be placed on tree in <see cref="BarkingUpTheRightTree.Patches.ObjectPatch.PlacementActionPrefix(GameLocation, int, int, Object, ref bool)"/> patch, this was done as a <see cref="StardewValley.Object"/> instance can't be retrieved in a transpile but can in a prefix (and as such can't get the tree whether the tree can be tapped).</remarks>
        internal static IEnumerable<CodeInstruction> PlacementActionTranspile(IEnumerable<CodeInstruction> instructions)
        {
            for (int i = 0; i < instructions.Count(); i++)
            {
                var instruction = instructions.ElementAt(i);

                // if the instruction is one of the last three, skip checking them for groups
                if (i >= instructions.Count() - 3)
                {
                    yield return instruction;
                    continue;
                }

                // check if the instruction being added is the start of the group of instructions to patch
                var nextNextInstruction = instructions.ElementAt(i + 2);
                if (instruction.opcode == OpCodes.Ldfld && instruction.operand == typeof(Tree).GetField("growthStage", BindingFlags.Public | BindingFlags.Instance)
                    && nextNextInstruction.opcode == OpCodes.Ldc_I4_5)
                {
                    // this will change the code: tree.growthStage >= 5
                    // to be                    : tree.growthStage >= 8
                    // this will always return false so that it can be reimplemented in the prefix patch

                    nextNextInstruction.opcode = OpCodes.Ldc_I4_8;

                    yield return instruction;
                    yield return instructions.ElementAt(i + 1);
                    yield return nextNextInstruction;

                    i += 2; // increment as the next instructions have been handled
                    continue;
                }

                yield return instruction;
            }
        }

        /// <summary>The prefix for the <see cref="StardewValley.Object.placementAction(StardewValley.GameLocation, int, int, StardewValley.Farmer)"/> method.</summary>
        /// <param name="location">The current game location.</param>
        /// <param name="x">The X position of the cursor (screen space).</param>
        /// <param name="y">The Y position of the cursor (screen space).</param>
        /// <param name="__instance">The <see cref="StardewValley.Object"/> instance being patched.</param>
        /// <param name="__result">The return value of the method being patched (in this case whether a tree was successfully planted).</param>
        /// <returns><see langword="true"/> if the original method should get ran; otherwise, <see langword="false"/> (this depends on if the tree is custom).</returns>
        /// <remarks>This is used to allow custom tree seeds to be planted.</remarks>
        internal static bool PlacementActionPrefix(GameLocation location, int x, int y, StardewValley.Object __instance, ref bool __result)
        {
            var placementTile = new Vector2(x / 64, y / 64); // 64 = 16 (tile size) * 4 (tile scale)

            // ensure object trying to be placed isn't a big craftable or furniture (the original method should handle those)
            if (__instance.bigCraftable || __instance is Furniture)
            {
                //// handle placement logic for if object being paces is a tapper
                if (__instance.ParentSheetIndex != 105 && __instance.ParentSheetIndex != 264)
                    return true;
                
                // ensure a tree exists at the placement location
                if (!location.terrainFeatures.ContainsKey(placementTile) || !(location.terrainFeatures[placementTile] is Tree tree))
                    return false;
                
                // ensure tree is valid to receive a tapper
                if (tree.growthStage < 5 || tree.stump || location.Objects.ContainsKey(placementTile))
                    return false;
                
                var isTreeCustom = ModEntry.Instance.Api.GetTreeById(tree.treeType, out _, out _, out var tappedProduct, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _);
                if (isTreeCustom && tappedProduct.Product == -1) // ensure there is a valid tapped product if the tree is custom
                    return false;
                
                // place tapper
                var tapper = (Object)__instance.getOne();
                tapper.heldObject.Value = null;
                tapper.tileLocation.Value = placementTile;
                location.Objects.Add(placementTile, tapper);
                tree.tapped.Value = true;
                tree.UpdateTapperProduct(tapper);
                location.playSound("axe");
                
                __result = true;
                return false;
            }

            // try to get a tree whose seed is the object trying to be planted
            if (!ModEntry.Instance.Api.GetAllTrees().Any(tree => tree.Seed == __instance.ParentSheetIndex)) // object being placed either isn't a tree or is a base game tree
                return true;
            var customTree = ModEntry.Instance.Api.GetAllTrees().FirstOrDefault(tree => tree.Seed == __instance.ParentSheetIndex);

            // ensure tree can be planted (checks tile data for NoSpawn spots etc)
            var canPlaceWildTreeSeed = typeof(StardewValley.Object).GetMethod("canPlaceWildTreeSeed", BindingFlags.NonPublic | BindingFlags.Instance);
            if (!(bool)canPlaceWildTreeSeed.Invoke(__instance, new object[] { location, placementTile }))
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13021")); // "Invalid Position"
                __result = false;
                return false;
            }

            // plant tree
            var newTree = new Tree(customTree.Id);
            newTree.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/daysTillBarkHarvest"] = "0";
            newTree.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/daysTillNextShakeProducts"] = JsonConvert.SerializeObject(new int[customTree.ShakingProducts.Count]);

            location.terrainFeatures.Remove(placementTile);
            location.terrainFeatures.Add(placementTile, newTree);
            location.playSound("dirtyHit");
            __result = true;
            return false;
        }

        /// <summary>The post fix for the <see cref="StardewValley.Object.isPlaceable()"/> method.</summary>
        /// <param name="__instance">The <see cref="StardewValley.Object"/> instance being patched.</param>
        /// <param name="__result">The return value of the method being patched.</param>
        /// <remarks>This is used so the game knows the custom tree seeds are placeable objects.</remarks>
        internal static void IsPlaceablePostFix(StardewValley.Object __instance, ref bool __result)
        {
            if (ModEntry.Instance.Api.GetAllTrees().Any(tree => tree.Seed == __instance.ParentSheetIndex))
                __result = true;
        }

        /// <summary>The post fix for the <see cref="StardewValley.Object.isPassable()"/> method.</summary>
        /// <param name="__instance">The <see cref="StardewValley.Object"/> instance being patched.</param>
        /// <param name="__result">The return value of the method being patched.</param>
        /// <remarks>This is used so trees can be planted under the feet of the farmer.</remarks>
        internal static void IsPassablePostFix(StardewValley.Object __instance, ref bool __result)
        {
            if (ModEntry.Instance.Api.GetAllTrees().Any(tree => tree.Seed == __instance.ParentSheetIndex))
                __result = true;
        }

        /// <summary>The prefix for the <see cref="StardewValley.Object.drawPlacementBounds(Microsoft.Xna.Framework.Graphics.SpriteBatch, StardewValley.GameLocation)"/> method.</summary>
        /// <param name="spriteBatch">The <see cref="Microsoft.Xna.Framework.Graphics.SpriteBatch"/> to draw the placement bounds to.</param>
        /// <param name="location">The current game location.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This reimplements the original method so the object doesn't get drawn over the placement bounds when the object being placed is a custom tree's seed.<br/>The reason the object gets drawn over the placement bounds is because the custom tree seeds aren't categorised as seeds (as custom trees can use anything for their seed).</remarks>
        internal static bool DrawPlacementBoundsPrefix(SpriteBatch spriteBatch, GameLocation location, StardewValley.Object __instance)
        {
            // ensure object is placeable and not a wallpaper
            if (!__instance.isPlaceable() || __instance is Wallpaper)
                return false;

            // get screen space coordinates of the placement tile (when using a mouse)
            var xScreen = (int)Game1.GetPlacementGrabTile().X * 64;
            var yScreen = (int)Game1.GetPlacementGrabTile().Y * 64;

            // get screen space cooridnates if the user is not using a mouse
            Game1.isCheckingNonMousePlacement = !Game1.IsPerformingMousePlacement();
            if (Game1.isCheckingNonMousePlacement)
            {
                var nearbyValidPlacementPosition = Utility.GetNearbyValidPlacementPosition(Game1.player, location, __instance, xScreen, yScreen);
                xScreen = (int)nearbyValidPlacementPosition.X;
                yScreen = (int)nearbyValidPlacementPosition.Y;
            }
            if (Utility.isThereAnObjectHereWhichAcceptsThisItem(location, __instance, xScreen, yScreen))
                return false;
            Game1.isCheckingNonMousePlacement = false;

            // check if the object is placeable (either in the environment of a nearby object)
            var canPlaceHere = Utility.playerCanPlaceItemHere(location, __instance, xScreen, yScreen, Game1.player)
                || (Utility.isThereAnObjectHereWhichAcceptsThisItem(location, __instance, xScreen, yScreen) && Utility.withinRadiusOfPlayer(xScreen, yScreen, 1, Game1.player));

            // get the number of placement bounds to draw (if furniture is being drawn)
            var width = 1;
            var height = 1;
            if (__instance is Furniture furniture)
            {
                width = furniture.getTilesWide();
                height = furniture.getTilesHigh();
            }

            // draw placement boxs (the red / green box)
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    spriteBatch.Draw(
                        texture: Game1.mouseCursors,
                        position: new Vector2((xScreen / 64 + x) * 64 - Game1.viewport.X, (yScreen / 64 + y) * 64 - Game1.viewport.Y),
                        sourceRectangle: new Rectangle(canPlaceHere ? 194 : 210, 388, 16, 16),
                        color: Color.White,
                        rotation: 0f,
                        origin: Vector2.Zero,
                        scale: 4f,
                        effects: SpriteEffects.None,
                        layerDepth: 0.01f
                    );

            // draw the object over the placement bounds
            {
                // ensure object is either a big craftable or piece of furniture
                if (!__instance.bigCraftable && !(__instance is Furniture))
                    return false;

                // ensure object isn't a seed or fertilizer (by category)
                if (__instance.category == StardewValley.Object.SeedsCategory || __instance.category == StardewValley.Object.fertilizerCategory)
                    return false;

                // ensure object isn't a custom tree seed
                if (ModEntry.Instance.Api.GetAllTrees().Any(customTree => customTree.Seed == __instance.ParentSheetIndex))
                    return false;

                __instance.draw(spriteBatch, xScreen / 64, yScreen / 64, .5f);
                return false;
            }
        }
    }
}
