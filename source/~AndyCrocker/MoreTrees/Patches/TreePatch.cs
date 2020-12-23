/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Reflection;

using SObject = StardewValley.Object;

namespace MoreTrees.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="Tree"/> class.</summary>
    internal static class TreePatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The prefix for the LoadTexture method.</summary>
        /// <param name="__instance">The <see cref="Tree"/> instance being patched.</param>
        /// <param name="__result">The return value of the LoadTexture method."/></param>
        /// <returns>True if the original method should get ran and false if it shouldn't.</returns>
        internal static bool LoadTexturePrefix(Tree __instance, ref Texture2D __result)
        {
            // if the tree type is a default one, let the original method handle it
            if (__instance.treeType <= 7)
            {
                __result = null;
                return true;
            }

            var tree = ModEntry.Instance.Api.GetTreeByType(__instance.treeType);
            if (tree == null)
            {
                ModEntry.Instance.Monitor.Log($"A tree with the type: {__instance.treeType} couldn't be found.", LogLevel.Error);
                __result = null;
                return false;
            }

            __result = tree.Texture;
            return false;
        }

        /// <summary>The prefix for the Shake method.</summary>
        /// <param name="tileLocation">The location of the tree being shaken.</param>
        /// <param name="doEvenIfStillShaking">Whether the shake action can be started if the tree is still shaking.</param>
        /// <param name="location">The location of the tree.</param>
        /// <param name="__instance">The current <see cref="Tree"/> instance being patched.</param>
        /// <returns>Whether the original method should get ran.</returns>
        internal static bool ShakePrefix(Vector2 tileLocation, bool doEvenIfStillShaking, GameLocation location, Tree __instance)
        {
            if (__instance.treeType < 7) // ensure tree being shaken is a custom one
                return true;

            // get private member
            var maxShake = (float)typeof(Tree).GetField("maxShake", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            // create seed and shake produce debris
            if ((maxShake == 0 || doEvenIfStillShaking) && __instance.growthStage >= 5 && !__instance.stump.Value && !Game1.IsMultiplayer && Game1.player.ForagingLevel >= 1)
            {
                var currentTreeData = ModEntry.Instance.Api.GetTreeDataByLocation(tileLocation);
                var treeData = ModEntry.Instance.Api.GetTreeByType(__instance.treeType);

                // handle dropping seed
                if (__instance.hasSeed)
                    Game1.createObjectDebris(Convert.ToInt32(treeData.Data.Seed), (int)tileLocation.X, (int)tileLocation.Y - 3, location);

                // handle dropping custom shake produce
                for (int i = 0; i < currentTreeData.DaysTillNextShakeProduct.Count; i++)
                {
                    if (currentTreeData.DaysTillNextShakeProduct[i] > 0)
                        continue;

                    // add product to drop list and reset time till next drop
                    Game1.createObjectDebris(Convert.ToInt32(treeData.Data.ShakingProducts[i].Product), (int)tileLocation.X, (int)tileLocation.Y - 3, ((int)tileLocation.Y + 1) * 64, location: location); // no need to try parse as it's already been validated
                    currentTreeData.DaysTillNextShakeProduct[i] = treeData.Data.ShakingProducts[i].DaysBetweenProduce;
                }
            }

            return true;
        }

        /// <summary>The prefix for the PerformToolAction method.</summary>
        /// <param name="t">The tool being used.</param>
        /// <param name="tileLocation">The tile action of the tree.</param>
        /// <param name="location">The location the player is currently in.</param>
        /// <param name="__result">Whether the tree gets cut down.</param>
        /// <returns>True, meaning the original method will get ran.</returns>
        internal static bool PerformToolActionPrefix(Tool t, Vector2 tileLocation, GameLocation location, ref bool __result)
        {
            //if (t is BarkRemover)
            //{
            //    // TODO: ensure tree is grown, alive, and isn't already barkless

            //    location.playSound("axchop", NetAudio.SoundContext.Default);
            //    // TODO: mark tree as barkless
            //}

            return true;
        }

        /// <summary>The prefix for the UpdateTapperProduct method.</summary>
        /// <param name="tapper_instance">The tapper object on the tree.</param>
        /// <returns>True, meaning the original method will get ran.</returns>
        internal static bool UpdateTapperProductPrefix(SObject tapper_instance)
        {
            // TODO: determine how the minutesUntilReady should get calculated

            return true;
        }

        /// <summary>The prefix for the Draw method.</summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> to draw the tree to.</param>
        /// <param name="tileLocation">The current location of the tree.</param>
        /// <param name="__instance">THe current <see cref="Tree"/> instance being patched.</param>
        /// <returns>Whether the original method will get ran.S</returns>
        internal static bool DrawPrefix(SpriteBatch spriteBatch, Vector2 tileLocation, Tree __instance)
        {
            // ensure tree trying to be drawn is custom
            if (__instance.treeType <= 7)
                return true;

            // get private members
            var shakeRotation = (float)typeof(Tree).GetField("shakeRotation", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var falling = (NetBool)typeof(Tree).GetField("falling", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var alpha = (float)typeof(Tree).GetField("alpha", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var shakeTimer = (float)typeof(Tree).GetField("shakeTimer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var leaves = (List<Leaf>)typeof(Tree).GetField("leaves", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            // calculate offsets depending on the season and whether the tree has been debarked
            var stumpXOffset = 0;
            var seasonXOffset = 0;
            var treeTopYOffset = 0;

            // calculate debark offset
            var debarked = ModEntry.Instance.Api.IsTreeDebarked(tileLocation);
            if (debarked)
            {
                stumpXOffset += 16;
                treeTopYOffset += 96;
            }

            // calculate season offset
            switch (Game1.currentSeason)
            {
                case "summer":
                    seasonXOffset += 48;
                    break;
                case "fall":
                    seasonXOffset += 96;
                    break;
                case "winter":
                    seasonXOffset += 144;
                    break;
            }

            // draw tree
            if (__instance.growthStage < 5) // tree is not fully grown
            {
                var sourceRectangle = Rectangle.Empty;

                // get the sourceRectangle for the current growth stage
                switch (__instance.growthStage)
                {
                    case 0:
                        sourceRectangle = new Rectangle(32, 32, 16, 16);
                        break;
                    case 1:
                        sourceRectangle = new Rectangle(16, 32, 16, 16);
                        break;
                    case 2:
                        sourceRectangle = new Rectangle(0, 32, 16, 16);
                        break;
                    case 3:
                        sourceRectangle = new Rectangle(0, 0, 16, 32);
                        break;
                }

                // draw tree
                spriteBatch.Draw(
                    texture: __instance.texture.Value,
                    position: Game1.GlobalToLocal(Game1.viewport, new Vector2((tileLocation.X * 64 + 32), (tileLocation.Y * 64 - (sourceRectangle.Height * 4 - 64) + (__instance.growthStage >= 3 ? 128 : 64)))), 
                    sourceRectangle: sourceRectangle, 
                    color: __instance.fertilized ? Color.HotPink : Color.White,
                    rotation: shakeRotation, 
                    origin: new Vector2(8, __instance.growthStage >= 3 ? 32 : 16), 
                    scale: 4,
                    effects: __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    layerDepth: __instance.growthStage == 0 ? 0.0001f : __instance.getBoundingBox(tileLocation).Bottom / 10000
                );
            }
            else // draw fully grown tree
            {
                if (!__instance.stump || falling) // check if the tree is still alive, if so draw it's shadow
                {
                    // draw tree shadow
                    spriteBatch.Draw(
                        texture: Game1.mouseCursors,
                        position: Game1.GlobalToLocal(Game1.viewport, new Vector2((tileLocation.X * 64 - 51), (tileLocation.Y * 64 - 16))), 
                        sourceRectangle: Tree.shadowSourceRect, 
                        color: Color.White * (1.570796f - Math.Abs(shakeRotation)), // oddly specific number is to emulate the game (game also is this specific)
                        rotation: 0, 
                        origin: Vector2.Zero, 
                        scale: 4,
                        effects: __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 
                        layerDepth: 1E-06f
                    ); 
                    
                    // draw tree top
                    spriteBatch.Draw(
                        texture: __instance.texture.Value,
                        position: Game1.GlobalToLocal(Game1.viewport, new Vector2((tileLocation.X * 64 + 32), (tileLocation.Y * 64 + 64))), 
                        sourceRectangle: new Rectangle(0 + seasonXOffset, 64 + treeTopYOffset, 48, 96), 
                        color: Color.White * alpha, 
                        rotation: shakeRotation, 
                        origin: new Vector2(24, 96), 
                        scale: 4,
                        effects: __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 
                        layerDepth: (float)((__instance.getBoundingBox(tileLocation).Bottom + 2) / 10000.0 - tileLocation.X / 1000000.0)
                    );
                }

                // draw the stump
                if (__instance.health >= 1.0 || !falling && __instance.health > -99.0)
                {
                    spriteBatch.Draw(
                        texture: __instance.texture.Value, 
                        position: Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(tileLocation.X * 64 + (shakeTimer > 0 ? Math.Sin(2 * Math.PI / shakeTimer) * 3 : 0)), (float)((double)tileLocation.Y * 64 - 64))), 
                        sourceRectangle: new Rectangle(16 + seasonXOffset + stumpXOffset, 0, 16, 32), 
                        color: Color.White * alpha, 
                        rotation: 0,
                        origin: Vector2.Zero,
                        scale: 4,
                        effects: __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 
                        layerDepth: __instance.getBoundingBox(tileLocation).Bottom / 10000f
                    );
                }
            }

            // draw falling leaves
            foreach (var leaf in leaves)
            {
                spriteBatch.Draw(
                    texture: __instance.texture.Value, 
                    position: Game1.GlobalToLocal(Game1.viewport, leaf.position), 
                    sourceRectangle: new Rectangle(leaf.type % 2 * 8, 48 + leaf.type / 2 * 8, 8, 8),
                    color: Color.White, 
                    rotation: leaf.rotation, 
                    origin: Vector2.Zero, 
                    scale: 4, 
                    effects: SpriteEffects.None, 
                    layerDepth: (float)(__instance.getBoundingBox(tileLocation).Bottom / 10000.0 + 0.00999999977648258) // oddly specific number is to emulate the game (game also is this specific)
                );
            }

            return false;
        }
    }
}
