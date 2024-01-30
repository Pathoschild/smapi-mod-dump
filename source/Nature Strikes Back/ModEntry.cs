/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/clumsyjackdaw/StardewValley_NatureStrikesBack
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley.Tools;

namespace NatureStrikesBack
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        public static IMonitor GMonitor;
        /*********
         ** Public methods
         *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            GMonitor = this.Monitor;
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.PatchAll();
        }
        
        [HarmonyPatch(typeof(Bush), "performToolAction")]
        public class Bush_performToolAction_Patch
        {
            private static float bushHealth;

            public static void Prefix(Bush __instance)
            {
	            ModEntry.GMonitor.Log($"------------------------------------------------", LogLevel.Debug);
                bushHealth = (float)__instance.health;
            }
            public static void Postfix(Bush __instance, Tool t, int explosion, Vector2 tileLocation, GameLocation location, ref bool __result)
            {
                ModEntry.GMonitor.Log($"Holding {Game1.player.CurrentTool}", LogLevel.Debug);
                var newHealth = (float)__instance.health > 0f ? (float)__instance.health : 0f;
                var healthDiff = bushHealth > newHealth ? bushHealth - newHealth : newHealth - bushHealth;
                ModEntry.GMonitor.Log($"Health before: {bushHealth}, Health after: {(float)__instance.health}, Health diff: {healthDiff}.", LogLevel.Debug);
                var tile = t.InitialParentTileIndex;
                if (Game1.player.CurrentTool is MeleeWeapon)
                {
	                tile = 189;
                }
                if (healthDiff > 0f)
                {
                    Game1.player.health -= (int)healthDiff;
                    var facingDirection = Game1.player.FacingDirection;
                    
                    switch (facingDirection)
                    {
	                    // Facing up
                        case 0:
	                        // Show tool swinging down
	                        
                            break;
                        // Facing right
                        case 1:
	                        // Show tool swinging left
	                        tile += 2;
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(48f, -150f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 0f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 150
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(30f, -148f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 6f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 180
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(4f, -132f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 5.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 210
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-10f, -106f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 240
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 200f, 1, 0, Game1.player.Position + new Vector2(-10f, -76f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 4.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 270
	                        });
                            break;
                        // Facing down
                        case 2:
	                        // Don't show tool swinging up, item is in front of it
                            break;
                        // Facing left
                        case 3:
	                        // Don't show tool swinging down, farmer is in front of it
	                        tile += 2;
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-48f, -150f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 150
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-30f, -148f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -6f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 180
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-4f, -132f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -5.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 210
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(10f, -106f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 240
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 200f, 1, 0, Game1.player.Position + new Vector2(10f, -76f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -4.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 270
	                        });
                            break;
                        default:
                            ModEntry.GMonitor.Log($"Unknown farmer facing direction: {facingDirection}.");
                            break;
                    }

                }

            }
        }
        
        [HarmonyPatch(typeof(Tree), "performToolAction")]
        public class Tree_performToolAction_Patch
        {
            private static float treeHealth;

            public static void Prefix(Tree __instance)
            {
	            ModEntry.GMonitor.Log($"------------------------------------------------", LogLevel.Debug);
                treeHealth = (float)__instance.health;
            }
            public static void Postfix(Tree __instance, Tool t, int explosion, Vector2 tileLocation, GameLocation location, ref bool __result)
            {
                ModEntry.GMonitor.Log($"Holding {Game1.player.CurrentTool}", LogLevel.Debug);
                var newHealth = (float)__instance.health > 0f ? (float)__instance.health : 0f;
                var healthDiff = treeHealth > newHealth ? treeHealth - newHealth : newHealth - treeHealth;
                healthDiff = __instance.growthStage <= 3 ? 2f : healthDiff;
                ModEntry.GMonitor.Log($"Health before: {treeHealth}, Health after: {(float)__instance.health}, Health diff: {healthDiff}.", LogLevel.Debug);
                var tile = t.InitialParentTileIndex;
                if (Game1.player.CurrentTool is MeleeWeapon)
                {
	                tile = 189;
                }
                if (healthDiff > 0f)
                {
                    Game1.player.health -= (int)healthDiff;
                    var facingDirection = Game1.player.FacingDirection;
                    
                    switch (facingDirection)
                    {
	                    // Facing up
                        case 0:
	                        // Show tool swinging down
	                        
                            break;
                        // Facing right
                        case 1:
	                        // Show tool swinging left
	                        tile += 2;
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(48f, -150f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 0f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 150
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(30f, -148f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 6f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 180
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(4f, -132f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 5.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 210
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-10f, -106f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 240
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 200f, 1, 0, Game1.player.Position + new Vector2(-10f, -76f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 4.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 270
	                        });
                            break;
                        // Facing down
                        case 2:
	                        // Don't show tool swinging up, item is in front of it
                            break;
                        // Facing left
                        case 3:
	                        // Don't show tool swinging down, farmer is in front of it
	                        tile += 2;
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-48f, -150f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 150
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-30f, -148f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -6f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 180
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-4f, -132f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -5.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 210
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(10f, -106f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 240
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 200f, 1, 0, Game1.player.Position + new Vector2(10f, -76f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -4.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 270
	                        });
                            break;
                        default:
                            ModEntry.GMonitor.Log($"Unknown farmer facing direction: {facingDirection}.");
                            break;
                    }

                }

            }
        }
        
        [HarmonyPatch(typeof(CosmeticPlant), "performToolAction")]
        public class CosmeticPlant_performToolAction_Patch
        {
            private static float cosmeticPlantHealth;

            public static void Prefix(CosmeticPlant __instance)
            {
	            ModEntry.GMonitor.Log($"------------------------------------------------", LogLevel.Debug);
	            cosmeticPlantHealth = 2f;
            }
            public static void Postfix(CosmeticPlant __instance, Tool t, int explosion, Vector2 tileLocation, GameLocation location, ref bool __result)
            {
                ModEntry.GMonitor.Log($"Holding {Game1.player.CurrentTool}", LogLevel.Debug);
                var newHealth = 0f;
                var healthDiff = cosmeticPlantHealth - newHealth;
                ModEntry.GMonitor.Log($"Health before: {cosmeticPlantHealth}, Health after: {newHealth}, Health diff: {healthDiff}.", LogLevel.Debug);
                var tile = t.InitialParentTileIndex;
                if (Game1.player.CurrentTool is MeleeWeapon)
                {
	                tile = 189;
                }
                if (healthDiff > 0f)
                {
                    Game1.player.health -= (int)healthDiff;
                    var facingDirection = Game1.player.FacingDirection;
                    
                    switch (facingDirection)
                    {
	                    // Facing up
                        case 0:
	                        // Show tool swinging down
	                        
                            break;
                        // Facing right
                        case 1:
	                        // Show tool swinging left
	                        tile += 2;
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(48f, -150f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 0f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 150
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(30f, -148f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 6f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 180
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(4f, -132f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 5.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 210
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-10f, -106f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 240
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 200f, 1, 0, Game1.player.Position + new Vector2(-10f, -76f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 4.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 270
	                        });
                            break;
                        // Facing down
                        case 2:
	                        // Don't show tool swinging up, item is in front of it
                            break;
                        // Facing left
                        case 3:
	                        // Don't show tool swinging down, farmer is in front of it
	                        tile += 2;
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-48f, -150f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 150
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-30f, -148f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -6f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 180
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-4f, -132f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -5.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 210
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(10f, -106f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 240
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 200f, 1, 0, Game1.player.Position + new Vector2(10f, -76f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -4.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 270
	                        });
                            break;
                        default:
                            ModEntry.GMonitor.Log($"Unknown farmer facing direction: {facingDirection}.");
                            break;
                    }

                }

            }
        }
        
        [HarmonyPatch(typeof(FruitTree), "performToolAction")]
        public class FruitTree_performToolAction_Patch
        {
            private static float fruitTreeHealth;

            public static void Prefix(FruitTree __instance)
            {
	            ModEntry.GMonitor.Log($"------------------------------------------------", LogLevel.Debug);
	            fruitTreeHealth = (float)__instance.health;
            }
            public static void Postfix(FruitTree __instance, Tool t, int explosion, Vector2 tileLocation, GameLocation location, ref bool __result)
            {
                ModEntry.GMonitor.Log($"Holding {Game1.player.CurrentTool}", LogLevel.Debug);
                var newHealth = (float)__instance.health > 0f ? (float)__instance.health : 0f;
                var healthDiff = fruitTreeHealth > newHealth ? fruitTreeHealth - newHealth : newHealth - fruitTreeHealth;
                healthDiff = __instance.growthStage <= 3 ? 2f : healthDiff;
                ModEntry.GMonitor.Log($"Health before: {fruitTreeHealth}, Health after: {(float)__instance.health}, Health diff: {healthDiff}.", LogLevel.Debug);
                var tile = t.InitialParentTileIndex;
                if (Game1.player.CurrentTool is MeleeWeapon)
                {
	                tile = 189;
                }
                if (healthDiff > 0f)
                {
                    Game1.player.health -= (int)healthDiff;
                    var facingDirection = Game1.player.FacingDirection;
                    
                    switch (facingDirection)
                    {
	                    // Facing up
                        case 0:
	                        // Show tool swinging down
	                        
                            break;
                        // Facing right
                        case 1:
	                        // Show tool swinging left
	                        tile += 2;
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(48f, -150f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 0f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 150
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(30f, -148f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 6f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 180
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(4f, -132f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 5.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 210
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-10f, -106f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 240
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 200f, 1, 0, Game1.player.Position + new Vector2(-10f, -76f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 4.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 270
	                        });
                            break;
                        // Facing down
                        case 2:
	                        // Don't show tool swinging up, item is in front of it
                            break;
                        // Facing left
                        case 3:
	                        // Don't show tool swinging down, farmer is in front of it
	                        tile += 2;
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-48f, -150f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 150
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-30f, -148f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -6f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 180
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-4f, -132f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -5.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 210
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(10f, -106f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 240
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 200f, 1, 0, Game1.player.Position + new Vector2(10f, -76f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -4.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 270
	                        });
                            break;
                        default:
                            ModEntry.GMonitor.Log($"Unknown farmer facing direction: {facingDirection}.");
                            break;
                    }

                }

            }
        }
        
        [HarmonyPatch(typeof(GiantCrop), "performToolAction")]
        public class GiantCrop_performToolAction_Patch
        {
            private static float GiantCropHealth;

            public static void Prefix(GiantCrop __instance)
            {
	            ModEntry.GMonitor.Log($"------------------------------------------------", LogLevel.Debug);
	            GiantCropHealth = (float)__instance.health;
            }
            public static void Postfix(GiantCrop __instance, Tool t, int damage, Vector2 tileLocation, GameLocation location, ref bool __result)
            {
                ModEntry.GMonitor.Log($"Holding {Game1.player.CurrentTool}", LogLevel.Debug);
                var newHealth = (float)__instance.health > 0f ? (float)__instance.health : 0f;
                var healthDiff = GiantCropHealth > newHealth ? GiantCropHealth - newHealth : newHealth - GiantCropHealth;
                ModEntry.GMonitor.Log($"Health before: {GiantCropHealth}, Health after: {(float)__instance.health}, Health diff: {healthDiff}.", LogLevel.Debug);
                var tile = t.InitialParentTileIndex;
                if (Game1.player.CurrentTool is MeleeWeapon)
                {
	                tile = 189;
                }
                if (healthDiff > 0f)
                {
                    Game1.player.health -= (int)healthDiff;
                    var facingDirection = Game1.player.FacingDirection;
                    
                    switch (facingDirection)
                    {
	                    // Facing up
                        case 0:
	                        // Show tool swinging down
	                        
                            break;
                        // Facing right
                        case 1:
	                        // Show tool swinging left
	                        tile += 2;
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(48f, -150f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 0f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 150
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(30f, -148f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 6f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 180
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(4f, -132f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 5.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 210
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-10f, -106f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 240
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 200f, 1, 0, Game1.player.Position + new Vector2(-10f, -76f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 4.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 270
	                        });
                            break;
                        // Facing down
                        case 2:
	                        // Don't show tool swinging up, item is in front of it
                            break;
                        // Facing left
                        case 3:
	                        // Don't show tool swinging down, farmer is in front of it
	                        tile += 2;
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-48f, -150f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 150
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-30f, -148f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -6f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 180
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-4f, -132f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -5.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 210
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(10f, -106f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 240
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 200f, 1, 0, Game1.player.Position + new Vector2(10f, -76f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -4.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 270
	                        });
                            break;
                        default:
                            ModEntry.GMonitor.Log($"Unknown farmer facing direction: {facingDirection}.");
                            break;
                    }

                }

            }
        }
        
        [HarmonyPatch(typeof(Grass), "performToolAction")]
        public class Grass_performToolAction_Patch
        {
            private static float GrassHealth;

            public static void Prefix(Grass __instance)
            {
	            ModEntry.GMonitor.Log($"------------------------------------------------", LogLevel.Debug);
	            GrassHealth = 2f;
            }
            public static void Postfix(Grass __instance, Tool t, int explosion, Vector2 tileLocation, GameLocation location, ref bool __result)
            {
                ModEntry.GMonitor.Log($"Holding {Game1.player.CurrentTool}", LogLevel.Debug);
                var newHealth = 0f;
                var healthDiff = GrassHealth > newHealth ? GrassHealth - newHealth : newHealth - GrassHealth;
                ModEntry.GMonitor.Log($"Health before: {GrassHealth}, Health after: {newHealth}, Health diff: {healthDiff}.", LogLevel.Debug);
                var tile = t.InitialParentTileIndex;
                if (Game1.player.CurrentTool is MeleeWeapon)
                {
	                tile = 189;
                }
                if (healthDiff > 0f)
                {
                    Game1.player.health -= (int)healthDiff;
                    var facingDirection = Game1.player.FacingDirection;
                    
                    switch (facingDirection)
                    {
	                    // Facing up
                        case 0:
	                        // Show tool swinging down
	                        
                            break;
                        // Facing right
                        case 1:
	                        // Show tool swinging left
	                        tile += 2;
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(48f, -150f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 0f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 150
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(30f, -148f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 6f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 180
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(4f, -132f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 5.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 210
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-10f, -106f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 240
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 200f, 1, 0, Game1.player.Position + new Vector2(-10f, -76f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 4.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 270
	                        });
                            break;
                        // Facing down
                        case 2:
	                        // Don't show tool swinging up, item is in front of it
                            break;
                        // Facing left
                        case 3:
	                        // Don't show tool swinging down, farmer is in front of it
	                        tile += 2;
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-48f, -150f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 150
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-30f, -148f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -6f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 180
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-4f, -132f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -5.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 210
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(10f, -106f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 240
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 200f, 1, 0, Game1.player.Position + new Vector2(10f, -76f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -4.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 270
	                        });
                            break;
                        default:
                            ModEntry.GMonitor.Log($"Unknown farmer facing direction: {facingDirection}.");
                            break;
                    }

                }

            }
        }
        
        [HarmonyPatch(typeof(Quartz), "performToolAction")]
        public class Quartz_performToolAction_Patch
        {
            private static float QuartzHealth;

            public static void Prefix(Quartz __instance)
            {
	            ModEntry.GMonitor.Log($"------------------------------------------------", LogLevel.Debug);
	            QuartzHealth = (float)__instance.health;
            }
            public static void Postfix(Quartz __instance, Tool t, int explosion, Vector2 tileLocation, GameLocation location, ref bool __result)
            {
                ModEntry.GMonitor.Log($"Holding {Game1.player.CurrentTool}", LogLevel.Debug);
                var newHealth = (float)__instance.health > 0f ? (float)__instance.health : 0f;
                var healthDiff = QuartzHealth > newHealth ? QuartzHealth - newHealth : newHealth - QuartzHealth;
                ModEntry.GMonitor.Log($"Health before: {QuartzHealth}, Health after: {(float)__instance.health}, Health diff: {healthDiff}.", LogLevel.Debug);
                var tile = t.InitialParentTileIndex;
                if (Game1.player.CurrentTool is MeleeWeapon)
                {
	                tile = 189;
                }
                if (healthDiff > 0f)
                {
                    Game1.player.health -= (int)healthDiff;
                    var facingDirection = Game1.player.FacingDirection;
                    
                    switch (facingDirection)
                    {
	                    // Facing up
                        case 0:
	                        // Show tool swinging down
	                        
                            break;
                        // Facing right
                        case 1:
	                        // Show tool swinging left
	                        tile += 2;
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(48f, -150f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 0f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 150
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(30f, -148f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 6f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 180
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(4f, -132f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 5.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 210
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-10f, -106f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 240
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 200f, 1, 0, Game1.player.Position + new Vector2(-10f, -76f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 4.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 270
	                        });
                            break;
                        // Facing down
                        case 2:
	                        // Don't show tool swinging up, item is in front of it
                            break;
                        // Facing left
                        case 3:
	                        // Don't show tool swinging down, farmer is in front of it
	                        tile += 2;
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-48f, -150f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 150
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-30f, -148f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -6f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 180
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-4f, -132f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -5.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 210
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(10f, -106f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 240
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 200f, 1, 0, Game1.player.Position + new Vector2(10f, -76f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -4.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 270
	                        });
                            break;
                        default:
                            ModEntry.GMonitor.Log($"Unknown farmer facing direction: {facingDirection}.");
                            break;
                    }

                }

            }
        }
        
        [HarmonyPatch(typeof(ResourceClump), "performToolAction")]
        public class ResourceClump_performToolAction_Patch
        {
            private static float ResourceClumpHealth;

            public static void Prefix(ResourceClump __instance)
            {
	            ModEntry.GMonitor.Log($"------------------------------------------------", LogLevel.Debug);
	            ResourceClumpHealth = (float)__instance.health;
            }
            public static void Postfix(ResourceClump __instance, Tool t, int damage, Vector2 tileLocation, GameLocation location, ref bool __result)
            {
                ModEntry.GMonitor.Log($"Holding {Game1.player.CurrentTool}", LogLevel.Debug);
                var newHealth = (float)__instance.health > 0f ? (float)__instance.health : 0f;
                var healthDiff = ResourceClumpHealth > newHealth ? ResourceClumpHealth - newHealth : newHealth - ResourceClumpHealth;
                ModEntry.GMonitor.Log($"Health before: {ResourceClumpHealth}, Health after: {(float)__instance.health}, Health diff: {healthDiff}.", LogLevel.Debug);
                var tile = t.InitialParentTileIndex;
                if (Game1.player.CurrentTool is MeleeWeapon)
                {
	                tile = 189;
                }
                if (healthDiff > 0f)
                {
                    Game1.player.health -= (int)healthDiff;
                    var facingDirection = Game1.player.FacingDirection;
                    
                    switch (facingDirection)
                    {
	                    // Facing up
                        case 0:
	                        // Show tool swinging down
	                        
                            break;
                        // Facing right
                        case 1:
	                        // Show tool swinging left
	                        tile += 2;
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(48f, -150f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 0f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 150
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(30f, -148f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 6f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 180
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(4f, -132f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 5.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 210
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-10f, -106f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 240
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 200f, 1, 0, Game1.player.Position + new Vector2(-10f, -76f), flicker: false, flipped: true, 1f, 0f, Color.White, 4f, 0f, 4.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 270
	                        });
                            break;
                        // Facing down
                        case 2:
	                        // Don't show tool swinging up, item is in front of it
                            break;
                        // Facing left
                        case 3:
	                        // Don't show tool swinging down, farmer is in front of it
	                        tile += 2;
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-48f, -150f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 150
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-30f, -148f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -6f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 180
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(-4f, -132f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -5.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 210
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0, Game1.player.Position + new Vector2(10f, -106f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 240
	                        });
	                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width, tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 200f, 1, 0, Game1.player.Position + new Vector2(10f, -76f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, -4.5f, 0f) 
	                        {
		                        delayBeforeAnimationStart = 270
	                        });
                            break;
                        default:
                            ModEntry.GMonitor.Log($"Unknown farmer facing direction: {facingDirection}.");
                            break;
                    }

                }

            }
        }
        
        [HarmonyPatch(typeof(Tool), "DoFunction")]
        public class Tool_DoFunction_Patch
        {
	        public static void Prefix(Tool __instance, GameLocation location, int x, int y, int power, Farmer who)
	        {
		        Utility.clampToTile(new Vector2(x, y));
		        int tileX = x / 64;
		        int tileY = y / 64;
		        Vector2 tileLocation = new Vector2(tileX, tileY);

		        StardewValley.Object o = null;
		        location.Objects.TryGetValue(tileLocation, out o);

		        if (o != null)
		        {
			        ModEntry.GMonitor.Log($"Object name: {o.Name}", LogLevel.Debug);
		        }
	            if (o != null && (o.Name.Equals("Stone") || o.Name.Contains("Boulder") || o.Name.Contains("Weed") || o.Name.Contains("Twig") || o.Name.Equals("Stick")))
	            {
		            ModEntry.GMonitor.Log($"Holding {Game1.player.CurrentTool}", LogLevel.Debug);
		            var healthDiff = 2f;
		            ModEntry.GMonitor.Log($"Health diff: {healthDiff}.", LogLevel.Debug);
		            var tile = __instance.InitialParentTileIndex;
		            if (Game1.player.CurrentTool is MeleeWeapon)
		            {
			            tile = 189;
		            }

		            if (healthDiff > 0f)
		            {
			            Game1.player.health -= (int)healthDiff;
			            var facingDirection = Game1.player.FacingDirection;

			            switch (facingDirection)
			            {
				            // Facing up
				            case 0:
					            // Show tool swinging down

					            break;
				            // Facing right
				            case 1:
					            // Show tool swinging left
					            tile += 2;
					            Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
						            "TileSheets\\tools",
						            new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width,
							            tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0,
						            Game1.player.Position + new Vector2(48f, -150f), flicker: false, flipped: true, 1f,
						            0f, Color.White, 4f, 0f, 0f, 0f)
					            {
						            delayBeforeAnimationStart = 150
					            });
					            Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
						            "TileSheets\\tools",
						            new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width,
							            tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0,
						            Game1.player.Position + new Vector2(30f, -148f), flicker: false, flipped: true, 1f,
						            0f, Color.White, 4f, 0f, 6f, 0f)
					            {
						            delayBeforeAnimationStart = 180
					            });
					            Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
						            "TileSheets\\tools",
						            new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width,
							            tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0,
						            Game1.player.Position + new Vector2(4f, -132f), flicker: false, flipped: true, 1f,
						            0f, Color.White, 4f, 0f, 5.5f, 0f)
					            {
						            delayBeforeAnimationStart = 210
					            });
					            Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
						            "TileSheets\\tools",
						            new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width,
							            tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0,
						            Game1.player.Position + new Vector2(-10f, -106f), flicker: false, flipped: true, 1f,
						            0f, Color.White, 4f, 0f, 5f, 0f)
					            {
						            delayBeforeAnimationStart = 240
					            });
					            Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
						            "TileSheets\\tools",
						            new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width,
							            tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 200f, 1, 0,
						            Game1.player.Position + new Vector2(-10f, -76f), flicker: false, flipped: true, 1f,
						            0f, Color.White, 4f, 0f, 4.5f, 0f)
					            {
						            delayBeforeAnimationStart = 270
					            });
					            break;
				            // Facing down
				            case 2:
					            // Don't show tool swinging up, item is in front of it
					            break;
				            // Facing left
				            case 3:
					            // Don't show tool swinging down, farmer is in front of it
					            tile += 2;
					            Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
						            "TileSheets\\tools",
						            new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width,
							            tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0,
						            Game1.player.Position + new Vector2(-48f, -150f), flicker: false, flipped: false,
						            1f, 0f, Color.White, 4f, 0f, 0f, 0f)
					            {
						            delayBeforeAnimationStart = 150
					            });
					            Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
						            "TileSheets\\tools",
						            new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width,
							            tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0,
						            Game1.player.Position + new Vector2(-30f, -148f), flicker: false, flipped: false,
						            1f, 0f, Color.White, 4f, 0f, -6f, 0f)
					            {
						            delayBeforeAnimationStart = 180
					            });
					            Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
						            "TileSheets\\tools",
						            new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width,
							            tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0,
						            Game1.player.Position + new Vector2(-4f, -132f), flicker: false, flipped: false, 1f,
						            0f, Color.White, 4f, 0f, -5.5f, 0f)
					            {
						            delayBeforeAnimationStart = 210
					            });
					            Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
						            "TileSheets\\tools",
						            new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width,
							            tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 30f, 1, 0,
						            Game1.player.Position + new Vector2(10f, -106f), flicker: false, flipped: false, 1f,
						            0f, Color.White, 4f, 0f, -5f, 0f)
					            {
						            delayBeforeAnimationStart = 240
					            });
					            Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
						            "TileSheets\\tools",
						            new Microsoft.Xna.Framework.Rectangle(tile * 16 % Game1.toolSpriteSheet.Width,
							            tile * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32), 200f, 1, 0,
						            Game1.player.Position + new Vector2(10f, -76f), flicker: false, flipped: false, 1f,
						            0f, Color.White, 4f, 0f, -4.5f, 0f)
					            {
						            delayBeforeAnimationStart = 270
					            });
					            break;
				            default:
					            ModEntry.GMonitor.Log($"Unknown farmer facing direction: {facingDirection}.");
					            break;
			            }

		            }
	            }
            }
        }
    }
}