/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using BarkingUpTheRightTree.Tools;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using xTile.Dimensions;

using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace BarkingUpTheRightTree.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="Tree"/> class.</summary>
    internal static class TreePatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The prefix for the <see cref="Tree.loadTexture()"/> method.</summary>
        /// <param name="__instance">The <see cref="Tree"/> instance being patched.</param>
        /// <param name="__result">The return value of the method being patched.</param>
        /// <returns><see langword="true"/> if the original method should get ran; otherwise, <see langword="false"/> (this depends on if the tree is custom).</returns>
        /// <remarks>This is used to intercept the tree texture loading to add the custom textures.</remarks>
        internal static bool LoadTexturePrefix(Tree __instance, ref Texture2D __result)
        {
            // if the tree type is a default one, let the original method handle it
            if (__instance.treeType < 20)
            {
                __result = null;
                return true;
            }

            if (!ModEntry.Instance.Api.GetTreeById(__instance.treeType, out _, out var texture, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _))
            {
                ModEntry.Instance.Monitor.Log($"A tree with the id: {__instance.treeType} couldn't be found.", LogLevel.Error);
                __result = null;
                return false;
            }

            __result = texture;
            return false;
        }

        /// <summary>The prefix for the <see cref="Tree.dayUpdate(GameLocation, Vector2)"/> method.</summary>
        /// <param name="environment">The location of the tree being patched.</param>
        /// <param name="tileLocation">The tile location of the tree being patched.</param>
        /// <param name="__instance">The <see cref="Tree"/> instance being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This is used to </remarks>
        internal static bool DayUpdatePrefix(GameLocation environment, Vector2 tileLocation, Tree __instance)
        {
            if (__instance.health <= -100)
            {
                var destroy = typeof(Tree).GetField("destroy", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                typeof(NetBool).GetProperty("Value", BindingFlags.Public | BindingFlags.Instance).SetValue(destroy, true);
            }

            // ensure tree's tapper state is correct
            if (__instance.tapped.Value)
            {
                var tileObject = environment.getObjectAtTile((int)tileLocation.X, (int)tileLocation.Y);
                if (tileObject == null || !tileObject.bigCraftable || tileObject.ParentSheetIndex != 105)
                    __instance.tapped.Value = false;
            }

            // try to grow the tree if it's fertilised, or a palm tree, or it's not in winter, or a tree can be planted there
            if (__instance.fertilized.Value || __instance.treeType == Tree.palmTree || __instance.treeType == Tree.palmTree2 || Game1.GetSeasonForLocation(__instance.currentLocation) != "winter" || environment.CanPlantTreesHere(-1, (int)tileLocation.X, (int)tileLocation.Y))
            {
                // ensure there are no NoSpawn tile properties
                var noSpawn = environment.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "NoSpawn", "Back");
                if (noSpawn != null || (noSpawn == "Tree" || noSpawn == "All" || noSpawn == "All"))
                    return false;

                // ensure there is enough space for the tree to fully grow
                if (__instance.growthStage == 4)
                {
                    // create a rectangle that encompasses the surrounding tiles, this will be used to determine if any fully grown trees surround this tree (as no two fully grown trees can be next to each other)
                    var surroundingRectangle = new Rectangle((int)(tileLocation.X - 1) * 64, (int)(tileLocation.Y - 2) * 64, 192, 192); // 192 = 16 (tile size) * 3 (number of tiles) * 4 (pixel scale)
                    foreach (var terrainFeaturePair in environment.terrainFeatures.Pairs)
                        if (terrainFeaturePair.Value is Tree tree && terrainFeaturePair.Value != __instance && tree.growthStage >= 5 && tree.getBoundingBox(terrainFeaturePair.Key).Intersects(surroundingRectangle))
                            return false; // this tree has a fully grown tree next to it, as such it can't be allowed to grow
                }
                else if (__instance.growthStage == 0 && environment.Objects.ContainsKey(tileLocation)) // don't let a seed grow if there's an object on it
                    return false;

                // grow tree
                var unfertilisedGrowthChance = (__instance.treeType == Tree.mahoganyTree) ? .15f : .2f;
                var fertilisedGrowthChance = (__instance.treeType == Tree.mahoganyTree) ? .6f : 1;
                if (ModEntry.Instance.Api.GetTreeById(__instance.treeType, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _, out var customUnfertilisedGrowthChance, out var customFertilisedGrowthChance))
                {
                    unfertilisedGrowthChance = customUnfertilisedGrowthChance;
                    fertilisedGrowthChance = customFertilisedGrowthChance;
                }

                if (Game1.random.NextDouble() < unfertilisedGrowthChance || (__instance.fertilized && Game1.random.NextDouble() < fertilisedGrowthChance))
                    __instance.growthStage.Value++;
            }

            // turn mushroom tree into stump in the winter and back into a regular tree in spring
            if (__instance.treeType == Tree.mushroomTree)
                if (Game1.GetSeasonForLocation(__instance.currentLocation) == "winter")
                    __instance.stump.Value = true;
                else if (Game1.dayOfMonth == 1 && Game1.currentSeason == "spring")
                    __instance.stump.Value = false;

            // spread tree seed
            if (__instance.growthStage >= 5 && environment is Farm && Game1.random.NextDouble() < .15f)
            {
                var xTile = Game1.random.Next(-3, 4) + (int)tileLocation.X;
                var yTile = Game1.random.Next(-3, 4) + (int)tileLocation.Y;
                var newLocation = new Vector2(xTile, yTile);

                var noSpawn = environment.doesTileHaveProperty(xTile, yTile, "NoSpawn", "Back");
                if (noSpawn == null || (noSpawn != "Tree" && noSpawn != "All" && noSpawn != "All")) // ensure there are no NoSpawn tile properties
                    if (environment.isTileLocationOpen(new Location(xTile, yTile)) && !environment.isTileOccupied(newLocation) && environment.doesTileHaveProperty(xTile, yTile, "Water", "Back") == null && environment.isTileOnMap(newLocation)) // ensure location is valid on the tile map
                        environment.terrainFeatures.Add(newLocation, new Tree(__instance.treeType, 0));
            }

            // recalculate whether the tree has a seed (for some reason the game also resets this everyday, this is just the replicate the game functionality)
            __instance.hasSeed.Value = false;
            var seedChance = .05f;
            if (__instance.treeType == Tree.palmTree2)
                seedChance *= 3;
            if (__instance.growthStage >= 5 && Game1.random.NextDouble() < seedChance)
                __instance.hasSeed.Value = true;

            return false;
        }

        /// <summary>The prefix for the <see cref="Tree.shake(Vector3, bool, GameLocation)"/> method.</summary>
        /// <param name="tileLocation">The tile location of the tree being patched.</param>
        /// <param name="doEvenIfStillShaking">Whether the shake action can be started if the tree is still shaking.</param>
        /// <param name="location">The location of the tree being patched.</param>
        /// <param name="__instance">The current <see cref="Tree"/> instance being patched.</param>
        /// <returns><see langword="true"/>, meaning the original method will get ran.</returns>
        /// <remarks>This is used to drop custom debris when a custom tree is shaken.</remarks>
        internal static bool ShakePrefix(Vector2 tileLocation, bool doEvenIfStillShaking, GameLocation location, Tree __instance)
        {
            // ensure tree being shaken is a custom one
            if (__instance.treeType < 20)
                return true;

            // get private member
            var maxShake = (float)typeof(Tree).GetField("maxShake", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            // ensure tree is valid for dropping shake products
            if ((maxShake == 0 || doEvenIfStillShaking) && __instance.growthStage >= 5 && !__instance.stump.Value && (Game1.IsMultiplayer || Game1.player.ForagingLevel >= 1))
            {
                // get seed and shake produce debris
                if (!ModEntry.Instance.Api.GetTreeById(__instance.treeType, out _, out _, out _, out _, out _, out var seed, out _, out var shakingProducts, out _, out _, out _, out _, out _))
                    return false;

                // handle dropping seed
                if (__instance.hasSeed)
                    Game1.createObjectDebris(seed, (int)tileLocation.X, (int)tileLocation.Y - 3, location);

                // handle dropping custom shake produce
                if (__instance.modData.ContainsKey($"{ModEntry.Instance.ModManifest.UniqueID}/daysTillNextShakeProducts"))
                {
                    var daysTillNextShakeProducts = JsonConvert.DeserializeObject<int[]>(__instance.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/daysTillNextShakeProducts"]);
                    for (int i = 0; i < daysTillNextShakeProducts.Length; i++)
                    {
                        if (daysTillNextShakeProducts[i] > 0)
                            continue;

                        var seasons = shakingProducts[i].Seasons.Select(season => season?.ToLower()).ToArray();
                        if (!shakingProducts[i].Seasons.Contains(Game1.currentSeason.ToLower()))
                            continue;

                        Game1.createObjectDebris(shakingProducts[i].Product, (int)tileLocation.X, (int)tileLocation.Y - 3, ((int)tileLocation.Y + 1) * 64, location: location);
                        daysTillNextShakeProducts[i] = shakingProducts[i].DaysBetweenProduce;
                    }
                    __instance.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/daysTillNextShakeProducts"] = JsonConvert.SerializeObject(daysTillNextShakeProducts);
                }
            }

            return true;
        }

        /// <summary>The prefix for the <see cref="Tree.performToolAction(Tool, int, Vector2, GameLocation)"/> method.</summary>
        /// <param name="t">The tool being used.</param>
        /// <param name="explosion">The explosion damage the tree is receiving.</param>
        /// <param name="tileLocation">The tile location of the tree being patched.</param>
        /// <param name="location">The location of the tree being patched.</param>
        /// <param name="__instance">The <see cref="Tree"/> instance being patched.</param>
        /// <param name="__result">The return value of the method being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This reimplements the original method to add the <see cref="BarkRemover"/> functionality, to drop the custom wood when using an axe with the <see cref="ShavingEnchantment"/>, and to make sure trees can only be cut down when the are allowed (based on map tile data and tool level requirements).</remarks>
        internal static bool PerformToolActionPrefix(Tool t, int explosion, Vector2 tileLocation, GameLocation location, Tree __instance, ref bool __result)
        {
            // validate
            location ??= Game1.currentLocation;
            if (explosion > 0) // any explosion damage knocks off tappers
                __instance.tapped.Value = false;
            if (__instance.tapped) // trees can't be destroyed with a tapper
            {
                __result = false;
                return false;
            }
            if (__instance.health <= -99)
            {
                __result = false;
                return false;
            }

            var treeFound = ModEntry.Instance.Api.GetTreeById(__instance.treeType, out _, out _, out _, out var customWoodId, out _, out _, out var requiredToolLevel, out _, out _, out _, out var barkProduct, out _, out _);
            var woodId = (treeFound) ? customWoodId : 388;

            // handle bark remover functionality first
            if (t is BarkRemover)
            {
                location.playSound("axchop", NetAudio.SoundContext.Default);
                typeof(Tree).GetMethod("shake", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { tileLocation, true, location });

                // ensure tree is custom
                if (__instance.treeType < 20)
                    return false;

                // ensure tree is grown, alive (not a strump), and has bark
                if (__instance.growthStage < 5 || __instance.stump || !ModEntry.Instance.Api.GetBarkState(location.Name, tileLocation))
                    return false;

                // make sure tree has a bark product (meaning it can be debarked in the first place)
                if (!treeFound || barkProduct.Product == -1)
                    return false;

                // mark tree as barkless
                ModEntry.Instance.Api.SetBarkState(location.Name, tileLocation, false);

                // drop bark objects
                var debris = new Debris(barkProduct.Product, barkProduct.Amount, t.getLastFarmerToUse().GetToolLocation() + new Vector2(16f, 0.0f), t.getLastFarmerToUse().Position);
                location.debris.Add(debris);
                __result = false;
                return false;
            }

            var isToolAxe = t != null && t is Axe;
            // get the damage of the axe (this is used when chopping down the tree and calculating the chance for dropping wood with the shaving enchant)
            float damage;
            switch (t.UpgradeLevel)
            {
                case 0: damage = 1; break;
                case 1: damage = 1.25f; break;
                case 2: damage = 1.67f; break;
                case 3: damage = 2.5f; break;
                case 4: damage = 5; break;
                default: damage = t.UpgradeLevel + 1; break;
            }

            // handle tool action on full grown trees
            if (__instance.growthStage >= 5)
            {
                if (isToolAxe)
                {
                    var lastPlayerToHit = typeof(Tree).GetField("lastPlayerToHit", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                    typeof(NetLong).GetProperty("Value", BindingFlags.Public | BindingFlags.Instance).SetValue(lastPlayerToHit, t.getLastFarmerToUse().UniqueMultiplayerID);
                    location.playSound("axchop");

                    // try to spawn secret note
                    if (!__instance.stump && location.HasUnlockedAreaSecretNotes(t.getLastFarmerToUse()) && Game1.random.NextDouble() < .005)
                    {
                        var secretNote = location.tryToCreateUnseenSecretNote(t.getLastFarmerToUse());
                        if (secretNote != null)
                            Game1.createItemDebris(secretNote, new Vector2(tileLocation.X, tileLocation.Y - 3) * 64, -1, location, Game1.player.getStandingY() - 32);
                    }
                }
                else if (explosion <= 0) // exit early if the fully grown tree didn't get hit by and axe or explosion
                {
                    __result = false;
                    return false;
                }

                // shake the tree
                typeof(Tree).GetMethod("shake", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { tileLocation, true, location });

                // if an explosion is happening, use that as the damage
                if (explosion > 0)
                    damage = explosion;

                // determine if wood should be dropping (when using shaving enchanct on axe)
                if (isToolAxe && t.hasEnchantmentOfType<ShavingEnchantment>() && Game1.random.NextDouble() <= damage / 5)
                {
                    var woodDebris = new Debris(woodId, new Vector2(tileLocation.X * 64 + 32, (tileLocation.Y - .5f) * 64 + 32), new Vector2(Game1.player.getStandingX(), Game1.player.getStandingY()));
                    woodDebris.Chunks[0].xVelocity.Value += Game1.random.Next(-10, 11) / 10f;
                    woodDebris.chunkFinalYLevel = (int)(tileLocation.Y * 64 + 64);
                    location.debris.Add(woodDebris);
                }

                // ensure the tree can be damaged (based on map tile and tool level)
                var cutDownTree = true;
                if (__instance.modData.ContainsKey($"{ModEntry.Instance.ModManifest.UniqueID}/nonChoppable")) // no need to check the value as the presence of this key is enough
                    cutDownTree = false;
                if (treeFound && requiredToolLevel > (t?.UpgradeLevel ?? 0))
                {
                    // show message to say current tool is too weak
                    if (__instance.stump)
                        Game1.drawObjectDialogue("Your axe isn't strong enough to break this stump.");
                    else
                        Game1.drawObjectDialogue("Your axe isn't strong enough to break this tree.");
                    cutDownTree = false;
                }
                if (!cutDownTree)
                {
                    __result = false;
                    return false;
                }

                // cut down the tree
                __instance.health.Value -= damage;
                if (__instance.health <= 0)
                    __result = (bool)typeof(Tree).GetMethod("performTreeFall", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { t, explosion, tileLocation, location }); ;

                return false;
            }

            // handle tool action on bushes (partially grown trees)
            else if (__instance.growthStage >= 3)
            {
                if (isToolAxe)
                {
                    location.playSound("axchop");
                    if (__instance.treeType != Tree.mushroomTree)
                        location.playSound("leafrustle");
                }
                else if (explosion <= 0) // exit early if the tree didn't get hit by and axe or explosion
                {
                    __result = false;
                    return false;
                }

                // shake the tree
                typeof(Tree).GetMethod("shake", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { tileLocation, true, location });

                // if an explosion is happening, use that as the damage
                if (explosion > 0)
                    damage = explosion;

                // ensure the tree can be damaged (based on map tile and tool level)
                var cutDownTree = true;
                if (__instance.modData.ContainsKey($"{ModEntry.Instance.ModManifest.UniqueID}/nonChoppable")) // no need to check the value as the presence of this key is enough
                    cutDownTree = false;
                if (treeFound && requiredToolLevel > (t?.UpgradeLevel ?? 0))
                {
                    // show message to say current tool is too weak
                    Game1.drawObjectDialogue("Your axe isn't strong enough to break this tree.");
                    cutDownTree = false;
                }
                if (!cutDownTree)
                {
                    __result = false;
                    return false;
                }

                // cut down the tree
                __instance.health.Value -= damage;
                if (__instance.health <= 0)
                {
                    typeof(Tree).GetMethod("performBushDestroy", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { tileLocation, location });
                    __result = true;
                }
                return false;
            }

            // handle tool action on sprouts (partially grown trees)
            else if (__instance.growthStage >= 1)
            {
                if (explosion > 0) // any amount of explosion can kill a sprout
                {
                    location.playSound("cut");
                    __result = true;
                    return false;
                }

                // ensure the tree can be damaged (based on map tile and tool level)
                var cutDownTree = true;
                if (__instance.modData.ContainsKey($"{ModEntry.Instance.ModManifest.UniqueID}/nonChoppable")) // no need to check the value as the presence of this key is enough
                    cutDownTree = false;
                if (treeFound && requiredToolLevel > (t?.UpgradeLevel ?? 0))
                {
                    // show message to say current tool is too weak
                    if (t is Axe)
                        Game1.drawObjectDialogue("Your axe isn't strong enough to break this sprout.");
                    else if (t is Pickaxe)
                        Game1.drawObjectDialogue("Your pickaxe isn't strong enough to break this sprout.");
                    else if (t is Hoe)
                        Game1.drawObjectDialogue("Your hoe isn't strong enough to break this sprout.");
                    cutDownTree = false;
                }
                if (!cutDownTree)
                {
                    __result = false;
                    return false;
                }

                if (t != null && t is Axe || t is Pickaxe || t is Hoe || t is MeleeWeapon)
                {
                    location.playSound("cut");
                    typeof(Tree).GetMethod("performSproutDestroy", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { t, tileLocation, location });
                    __result = true;
                    return false;
                }
            }

            // handle tool action on seeds
            else
            {
                if (explosion > 0) // any amount of explosion can kill a seed
                {
                    __result = true;
                    return false;
                }
                if (t != null && t is Axe || t is Pickaxe || t is Hoe)
                {
                    location.playSound("woodyHit");
                    location.playSound("axchop");
                    typeof(Tree).GetMethod("performSeedDestroy", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { t, tileLocation, location });
                    __result = true;
                    return false;
                }
            }

            return false;
        }

        /// <summary>The transpiler for the <see cref="Tree.tickUpdate(GameTime, Vector2, GameLocation)"/> method.</summary>
        /// <param name="instructions">The IL instructions.</param>
        /// <returns>The new IL instructions.</returns>
        /// <remarks>This is used to stop the tree from dropping wood and sap, when cut down (not as a stump).<br/>This is to make it drop the custom wood debris in <see cref="BarkingUpTheRightTree.Patches.TreePatch.TickUpdatePrefix(Microsoft.Xna.Framework.Vector2, StardewValley.GameLocation, StardewValley.TerrainFeatures.Tree)"/> patch, this was done as a <see cref="StardewValley.TerrainFeatures.Tree"/> instance can't be retrieved in a transpile but can in a prefix (and as such can't get the wood id for the tree being cut down).<br/>The debris type is set to 21, this is because <see cref="System.Reflection.Emit.OpCodes.Ldc_I4_S"/> only accepts an <see langword="sbyte"/> and <see cref="sbyte.MaxValue"/> isn't big enough to be outside of the game object ids range. As such 21 was used as it's an unused id and isn't an 'aliased' id (check the <see langword="switch"/> in <see cref="StardewValley.Debris(int, int, Microsoft.Xna.Framework.Vector2, Microsoft.Xna.Framework.Vector2, float)"/> constructor for the 'aliased' types).</remarks>
        internal static IEnumerable<CodeInstruction> TickUpdateTranspile(IEnumerable<CodeInstruction> instructions)
        {
            for (int i = 0; i < instructions.Count(); i++)
            {
                var instruction = instructions.ElementAt(i);

                // if the instruction is the last one, skip checking them for groups
                if (i >= instructions.Count() - 1)
                {
                    yield return instruction;
                    continue;
                }

                // check if the instruction is for the wood id
                {
                    var nextInstruction = instructions.ElementAt(i + 1);
                    if (instruction.opcode == OpCodes.Ldarg_3
                        && nextInstruction.opcode == OpCodes.Ldc_I4_S && Convert.ToInt32(nextInstruction.operand) == 12)
                    {
                        // edit the instruction to be the correct id and return them
                        nextInstruction.operand = 21;
                        yield return instruction;
                        yield return nextInstruction;

                        i++; // increment as the next instruction has been handled
                        continue;
                    }
                }

                // check if the instruction is for the sap id
                {
                    if (instruction.opcode == OpCodes.Ldc_I4_S && Convert.ToInt32(instruction.operand) == 92)
                    {
                        // edit the instruction to be the correct id and return them
                        instruction.operand = 21;
                        yield return instruction;
                        continue;
                    }
                }

                yield return instruction;
            }
        }

        /// <summary>The prefix for the <see cref="Tree.tickUpdate(GameTime, Vector2, GameLocation)"/> method.</summary>
        /// <param name="tileLocation">The tile location of the tree being patched.</param>
        /// <param name="location">The location of the tree being patched.</param>
        /// <param name="__instance">The <see cref="Tree"/> instance being patched.</param>
        /// <returns><see langword="true"/>, meaning the original method will get ran.</returns>
        /// <remarks>This is used to spawn the custom wood debris when cutting down a tree (that isn't a stump).</remarks>
        internal static bool TickUpdatePrefix(Vector2 tileLocation, GameLocation location, Tree __instance)
        {
            // run the same code as the game does to determine if the tree has fully fallen
            var falling = ((NetBool)typeof(Tree).GetField("falling", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance)).Value;
            var shakeRotation = (float)typeof(Tree).GetField("shakeRotation", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var maxShake = (float)typeof(Tree).GetField("maxShake", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            var shakeRotationModifier = maxShake * maxShake;
            var newShakeRotation = shakeRotation + (__instance.shakeLeft
                ? -shakeRotationModifier
                : shakeRotationModifier);

            if (!(falling && Math.Abs(newShakeRotation) > Math.PI / 2.0))
                return true;

            // tree has just fully fallen down, spawn wood and sap debris
            if (__instance.treeType == Tree.mushroomTree) // ensure not to spawn wood for mushroom trees
                return true;

            var treeFound = ModEntry.Instance.Api.GetTreeById(__instance.treeType, out _, out _, out _, out var wood, out var dropsSap, out var seed, out _, out _, out _, out _, out _, out _, out _);
            var woodId = Debris.woodDebris;
            if (__instance.treeType >= 20 && treeFound)
                woodId = wood;

            var lastPlayerToHit = ((NetLong)typeof(Tree).GetField("lastPlayerToHit", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance)).Value;
            var extraWoodCalculator = typeof(Tree).GetMethod("extraWoodCalculator", BindingFlags.NonPublic | BindingFlags.Instance);

            // drop wood
            if (woodId != -1)
                Game1.createRadialDebris(location, woodId, (int)tileLocation.X + (__instance.shakeLeft ? (-4) : 4), (int)tileLocation.Y, (int)((Game1.getFarmer(lastPlayerToHit).professions.Contains(12) ? 1.25 : 1.0) * (12 + (int)extraWoodCalculator.Invoke(__instance, new object[] { tileLocation }))), resource: true);

            // drop sap if the tree is either default or has it enabled in it's data
            if (__instance.treeType < 20 || dropsSap)
                Game1.createRadialDebris(location, 92, (int)tileLocation.X + (__instance.shakeLeft ? (-4) : 4), (int)tileLocation.Y, 5, resource: true);

            // drop tree seed
            var random = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + (int)tileLocation.X * 7 + (int)tileLocation.Y * 11);
            if (Game1.getFarmer(lastPlayerToHit).getEffectiveSkillLevel(Farmer.foragingSkill) >= 1
                && Game1.random.NextDouble() < 0.75
                && treeFound)
                Game1.createRadialDebris(location, seed, (int)tileLocation.X + (__instance.shakeLeft ? (-4) : 4), (int)tileLocation.Y, random.Next(1, 3), resource: true);

            return true;
        }

        /// <summary>The transpiler for the <see cref="Tree.performTreeFall(Tool, int, Vector2, GameLocation)"/> method.</summary>
        /// <param name="instructions">The IL instructions.</param>
        /// <returns>The new IL instructions.</returns>
        /// <remarks>This is used to stop the tree from dropping wood and sap, when cut down (as a stump).<br/>This is to make it drop the custom wood debris in <see cref="TreePatch.TickUpdatePrefix(Vector2, GameLocation, Tree)"/>, this was done as a <see cref="Tree"/> instance can't be retrieved in a transpile but can in a prefix (and as such can't get the wood id for the tree being cut down).<br/>The debris type is set to 21, this is because <see cref="OpCodes.Ldc_I4_S"/> only accepts an <see langword="sbyte"/> and <see cref="sbyte.MaxValue"/> isn't big enough to be outside of the game object ids range. As such 21 was used as it's an unused id and isn't an 'aliased' id (check the <see langword="switch"/> in <see cref="Debris(int, int, Vector2, Vector2, float)"/> constructor for the 'aliased' types).</remarks>
        internal static IEnumerable<CodeInstruction> PerformTreeFallTranspile(IEnumerable<CodeInstruction> instructions)
        {
            for (int i = 0; i < instructions.Count(); i++)
            {
                var instruction = instructions.ElementAt(i);

                // if the instruction is one of the last two, skip checking them for groups
                if (i >= instructions.Count() - 2)
                {
                    yield return instruction;
                    continue;
                }

                // check if the instruction is for the wood id
                {
                    var nextInstruction = instructions.ElementAt(i + 1);
                    if (instruction.opcode == OpCodes.Ldarg_S && Convert.ToInt32(instruction.operand) == 4
                        && nextInstruction.opcode == OpCodes.Ldc_I4_S && Convert.ToInt32(nextInstruction.operand) == 12)
                    {
                        // edit the instruction to be the correct id and return them
                        nextInstruction.operand = 21;
                        yield return instruction;
                        yield return nextInstruction;

                        i++; // increment as the next instruction has been handled
                        continue;
                    }
                }

                // check if the instruction is for the sap id
                {
                    // the nextNextInstruction had to be checked as the instruction before and after the ldc.i4.s instruction weren't able to checked properly (as they used labels)
                    var nextNextInstruction = instructions.ElementAt(i + 2);
                    if (instruction.opcode == OpCodes.Ldc_I4_S && Convert.ToInt32(instruction.operand) == 92
                        && nextNextInstruction.opcode == OpCodes.Ldc_I4 && Convert.ToInt32(nextNextInstruction.operand) == 709)
                    {
                        // edit the instruction to be the correct id and return them
                        instruction.operand = 21;
                        yield return instruction;
                        continue;
                    }
                }

                yield return instruction;
            }
        }

        /// <summary>The prefix for the <see cref="Tree.performTreeFall(Tool, int, Vector2, GameLocation)"/> method.</summary>
        /// <param name="t">The tool that was used to cut down the tree.</param>
        /// <param name="tileLocation">The tile location of the tree being patched.</param>
        /// <param name="location">The location of the tree being patched.</param>
        /// <param name="__instance">The <see cref="Tree"/> instance being patched.</param>
        /// <returns><see langword="true"/> if the original method should get ran; otherwise, <see langword="false"/> (depending on if the tree is allowed to be cut down based on tile data and tool level requirements).</returns>
        /// <remarks>This is used to spawn the custom wood debris when cutting down a tree (that's a stump) and to make sure the tree can only be cut down if allowed (based on map tile data and tool level).</remarks>
        internal static bool PerformTreeFallPrefix(Tool t, Vector2 tileLocation, GameLocation location, Tree __instance)
        {
            // ensure tree is allowed to be cut down (based on tile property and tool requirments)
            var cutDownTree = true;
            if (__instance.modData.ContainsKey($"{ModEntry.Instance.ModManifest.UniqueID}/nonChoppable")) // no need to check the value as the presence of this key is enough
                cutDownTree = false;
            var treeFound = ModEntry.Instance.Api.GetTreeById(__instance.treeType, out _, out _, out _, out var wood, out var dropsSap, out _, out var requiredToolLevel, out _, out _, out _, out _, out _, out _);
            if (treeFound && requiredToolLevel > (t?.UpgradeLevel ?? 0))
                cutDownTree = false;
            if (!cutDownTree)
            {
                __instance.health.Value = 1;
                return false;
            }

            // run the same code as the game does to determine if the tree has fully fallen
            if (!__instance.stump)
                return true;

            // stump has been cut down, spawn wood & sap debris
            var woodId = Debris.woodDebris;
            if (__instance.treeType >= 20 && treeFound)
                woodId = wood;

            var lastPlayerToHit = ((NetLong)typeof(Tree).GetField("lastPlayerToHit", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance)).Value;

            // drop wood
            if (woodId != -1)
                Game1.createRadialDebris(location, woodId, (int)tileLocation.X, (int)tileLocation.Y, (int)((Game1.getFarmer(lastPlayerToHit).professions.Contains(Farmer.forester) ? 1.25 : 1.0) * 4.0), resource: true);
            if (__instance.treeType < 20 || (treeFound && dropsSap)) // drop sap if the tree is either default or has it enabled in it's custom data
                Game1.createRadialDebris(location, 92, (int)tileLocation.X, (int)tileLocation.Y, 1, resource: true);

            return true;
        }

        /// <summary>The prefix for the <see cref="Tree.performBushDestroyPrefix(Vector2, GameLocation)"/> method.</summary>
        /// <param name="tileLocation">The tile location of the tree being patched.</param>
        /// <param name="location">The location of the tree being patched.</param>
        /// <param name="__instance">The <see cref="Tree"/> instance being patched.</param>
        /// <returns><see langword="true"/> if the original method should get ran; otherwise, <see langword="false"/> (depending on if the tree is custom).</returns>
        /// <remarks>This is used to spawn the custom wood debris when cutting down a tree bush.</remarks>
        internal static bool PerformBushDestroyPrefix(Vector2 tileLocation, GameLocation location, Tree __instance)
        {
            // if the tree type is a default one, let the original method handle it
            if (__instance.treeType < 20)
                return true;

            if (ModEntry.Instance.Api.GetTreeById(__instance.treeType, out _, out _, out _, out var woodId, out _, out _, out _, out _, out _, out _, out _, out _, out _))
            {
                var lastPlayerToHit = (NetLong)typeof(Tree).GetField("lastPlayerToHit", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                Game1.createDebris(woodId, (int)tileLocation.X, (int)tileLocation.Y, (int)((Game1.getFarmer(lastPlayerToHit).professions.Contains(Farmer.forester) ? 1.25f : 1) * 4), location);
            }

            return false;
        }

        /// <summary>The prefix for the <see cref="Tree.performBushDestroyPrefix(Vector2, GameLocation)"/> method.</summary>
        /// <param name="tileLocation">The tile location of the tree being patched.</param>
        /// <param name="__instance">The <see cref="Tree"/> instance being patched.</param>
        /// <returns><see langword="true"/> if the original method should get ran; otherwise, <see langword="false"/> (depending on if the tree is custom).</returns>
        /// <remarks>This is used to spawn the custom wood debris when cutting down a tree sprout.</remarks>
        internal static bool PerformSproutDestroyPrefix(Vector2 tileLocation, Tree __instance)
        {
            // if the tree type is a default one, let the original method handle it
            if (__instance.treeType < 20)
                return true;

            if (ModEntry.Instance.Api.GetTreeById(__instance.treeType, out _, out _, out _, out var woodId, out _, out _, out _, out _, out _, out _, out _, out _, out _))
                Game1.createDebris(woodId, (int)tileLocation.X, (int)tileLocation.Y, 1);

            return false;
        }

        /// <summary>The prefix for the <see cref="Tree.performSeedDestroy(Tool, Vector2, GameLocation)"/> method.</summary>
        /// <param name="t">The tool that was used to destroy the seed.</param>
        /// <param name="tileLocation">The tile location of the tree being patched.</param>
        /// <param name="location">The location of the tree being patched.</param>
        /// <param name="__instance">The <see cref="Tree"/> instance being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This reimplements the original method so the custom tree seeds will drop the item when they are destroyed and to make sure the seed can only be destroyed if allowed (based on map tile data and tool level).</remarks>
        internal static bool PerformSeedDestroyPrefix(Tool t, Vector2 tileLocation, GameLocation location, Tree __instance)
        {
            // ensure seed is allowed to be destroyed (based on tile property and tool requirments)
            if (__instance.modData.ContainsKey($"{ModEntry.Instance.ModManifest.UniqueID}/nonChoppable")) // no need to check the value as the presence of this key is enough
            {
                __instance.health.Value = 1;
                return false;
            }

            var multiplayer = (Multiplayer)typeof(Game1).GetField("multiplayer", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(17, tileLocation * 64f, Color.White));

            // ensure the player that destroyed the seed has at least level 1 foraging
            var lastPlayerToHit = (long)(NetLong)typeof(Tree).GetField("lastPlayerToHit", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var player = lastPlayerToHit != 0
                ? Game1.getFarmer(lastPlayerToHit)
                : Game1.player;
            if (player.getEffectiveSkillLevel(Farmer.foragingSkill) < 1)
                return false;

            // get the seed id
            var seedId = -1;
            if (__instance.treeType <= 3)
                seedId = 308 + __instance.treeType;
            else if (__instance.treeType == Tree.mahoganyTree)
                seedId = 292;
            else if (ModEntry.Instance.Api.GetTreeById(__instance.treeType, out _, out _, out _, out _, out _, out var seed, out _, out _, out _, out _, out _, out _, out _))
                seedId = seed;
            if (seedId == -1)
                return false;

            // spawn seed
            Game1.createMultipleObjectDebris(seedId, (int)tileLocation.X, (int)tileLocation.Y, 1, (t == null) ? Game1.player.uniqueMultiplayerID : t.getLastFarmerToUse().uniqueMultiplayerID, location);
            return false;
        }

        /// <summary>The prefix for the <see cref="Tree.UpdateTapperProduct(StardewValley.Object, StardewValley.Object)"/> method.</summary>
        /// <param name="tapper_instance">The tapper object on the tree.</param>
        /// <param name="__instance">The <see cref="Tree"/> instance being patched.</param>
        /// <returns><see langword="true"/> if the original method should get ran; otherwise, <see langword="false"/> (this depends on if the tree is custom).</returns>
        /// <remarks>This is used to add the custom tapper products.</remarks>
        internal static bool UpdateTapperProductPrefix(StardewValley.Object tapper_instance, Tree __instance)
        {
            // ensure tree is custom
            if (__instance.treeType < 20)
                return true;

            // get tree by data
            if (!ModEntry.Instance.Api.GetTreeById(__instance.treeType, out _, out _, out var tappedProduct, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _))
                return false;

            var timeMultiplier = 1f;
            if (tapper_instance != null && tapper_instance.ParentSheetIndex == 264) // half the time for a heavy tapper
                timeMultiplier = .5f;

            tapper_instance.heldObject.Value = new StardewValley.Object(tappedProduct.Product, tappedProduct.Amount);
            tapper_instance.minutesUntilReady.Value = (int)(tappedProduct.DaysBetweenProduce * 1600 * timeMultiplier);

            return false;
        }

        /// <summary>The prefix for the <see cref="Tree.draw(SpriteBatch, Vector2)"/> method.</summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> to draw the tree to.</param>
        /// <param name="tileLocation">The tile location of the tree being patched.</param>
        /// <param name="__instance">THe current <see cref="Tree"/> instance being patched.</param>
        /// <returns><see langword="true"/> if the original method should get ran; otherwise, <see langword="false"/> (this depends on if the tree is custom).</returns>
        /// <remarks>This is used to draw trees with the different tree sprite sheet layouts.</remarks>
        internal static bool DrawPrefix(SpriteBatch spriteBatch, Vector2 tileLocation, Tree __instance)
        {
            // ensure tree trying to be drawn is custom
            if (__instance.treeType < 20)
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
            var debarked = !ModEntry.Instance.Api.GetBarkState(__instance.currentLocation.Name, tileLocation);
            if (debarked)
            {
                stumpXOffset += 16;
                treeTopYOffset += 96;
            }

            // calculate season offset
            switch (Game1.currentSeason)
            {
                case "summer": seasonXOffset += 48; break;
                case "fall": seasonXOffset += 96; break;
                case "winter": seasonXOffset += 144; break;
            }

            // draw tree
            if (__instance.growthStage < 5) // tree is not fully grown
            {
                var sourceRectangle = Rectangle.Empty;

                // get the sourceRectangle for the current growth stage
                switch (__instance.growthStage)
                {
                    case 0: sourceRectangle = new Rectangle(32, 32, 16, 16); break;
                    case 1: sourceRectangle = new Rectangle(16, 32, 16, 16); break;
                    case 2: sourceRectangle = new Rectangle(0, 32, 16, 16); break;
                    default: sourceRectangle = new Rectangle(0, 0, 16, 32); break;
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
                    layerDepth: __instance.growthStage == 0 ? 0.0001f : __instance.getBoundingBox(tileLocation).Bottom / 10000f
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
                        layerDepth: (__instance.getBoundingBox(tileLocation).Bottom + 2) / 10000f - tileLocation.X / 1000000f
                    );
                }

                // draw the stump
                if (__instance.health >= 1 || !falling && __instance.health > -99)
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
                    layerDepth: (float)(__instance.getBoundingBox(tileLocation).Bottom / 10000f + 0.00999999977648258) // oddly specific number is to replicate the game (game also is this specific)
                );
            }

            return false;
        }
    }
}
