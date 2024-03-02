/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elizabethcd/DragonPearlLure
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Shops;
using StardewValley.Monsters;

namespace DragonPearlLure
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /// <summary>Monitor for logging purposes.</summary>
        private static IMonitor Mon;

        /// <summary>Helper for patching purposes.</summary>
        private static IModHelper Help;

        /// <summary>Game1.multiplayer from reflection.</summary>
        private static Multiplayer multiplayer;

        /// <summary>The name of the pearl lure.</summary>
        private static string pearlLureMonsterName = "Pearl Lure Monster";

        /// <summary>The mod unique ID.</summary>
        private static string modID;

        /// <summary>The item ID for the pearl lure.</summary>
        public static string PearlLureID = "violetlizabet.PearlLure";

        /// <summary>The item ID for the pearl lure.</summary>
        public static string lureImageLoc = "Mods/violetlizabet.DragonPearlLure/PearlLure";

        /// <summary>The magic number for the TAS</summary>
        private static int PearlLureMagicNumber = 15109;

        /// <summary>The dummy farmer for serpents to chase.</summary>
        private static Farmer dummyFarmer;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.Content.AssetRequested += this.OnAssetRequested;

            // Set up some things
            var harmony = new Harmony(this.ModManifest.UniqueID);
            var Game1_multiplayer = this.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            Mon = Monitor;
            Help = Helper;
            multiplayer = Game1_multiplayer;
            modID = this.ModManifest.UniqueID;

            // Allow pearl lures to be placed
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.canBePlacedHere)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.CanBePlacedHere_Postfix))
            );

            // Cause explosion when placed
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.placementAction)),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.PlacementAction_Prefix))
            );

            // Allow placeable
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.isPlaceable)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.IsPlaceable_Postfix))
            );

            // Spawn flying pearl when exploded
            harmony.Patch(
               original: AccessTools.Method(typeof(TemporaryAnimatedSprite), nameof(TemporaryAnimatedSprite.unload)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.TASUnload_Postfix))
            );

            // Make flying pearl not chase the farmer
            harmony.Patch(
               original: AccessTools.Method(typeof(Bat), nameof(Bat.behaviorAtGameTick)),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Bat_BehaviorAtGameTick_Prefix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(Bat), nameof(Bat.behaviorAtGameTick)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Bat_BehaviorAtGameTick_Postfix))
            );

            // Make serpent chase a fake player
            harmony.Patch(
               original: AccessTools.Method(typeof(Monster), "findPlayer"),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Monster_FindPlayer_Prefix))
            );
        }

        // Create dummy farmer
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            dummyFarmer = new Farmer(new FarmerSprite("Characters\\Farmer\\farmer_base"), new Vector2(192f, 192f), 0, "DummyPearlLureFarmer", Farmer.initialTools(), true);
        }

        // Load pearl lure monster asset
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            // Add the image for the pearl lure monster
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Monsters/" + pearlLureMonsterName))
            {
                e.LoadFromModFile<Texture2D>("assets/PearlLureMonster.png", AssetLoadPriority.Medium);
            }
            // Add the data for the pearl lure
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                var pearlLureObjectData = Helper.ModContent.Load<ObjectData>("assets/object.json");
                e.Edit(asset =>
                    {
                        var editor = asset.AsDictionary<string, ObjectData>();
                        editor.Data[PearlLureID] = pearlLureObjectData;
                    }
                );
            }
            // Add in the name and description for the pearl lure
            else if (e.NameWithoutLocale.IsEquivalentTo("Strings/Objects"))
            {
                e.Edit(asset => {
                    var dict = asset.AsDictionary<string, string>();
                    dict.Data[$"{PearlLureID}_Name"] =
                            Helper.Translation.Get("pearl-lure.name");
                    dict.Data[$"{PearlLureID}_Description"] =
                            Helper.Translation.Get("pearl-lure.description");
                });
            }
            // Add in the image for the pearl lure
            else if (e.NameWithoutLocale.IsEquivalentTo(lureImageLoc))
            {
                e.LoadFrom(() => {
                    return Helper.ModContent.Load<Texture2D>("assets/object.png");
                }, AssetLoadPriority.Medium);
            }
            // Add in the recipe for the pearl lure
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                e.Edit(asset => {
                    var dict = asset.AsDictionary<string, string>();
                    dict.Data[PearlLureID] = $"287 1 768 2/Field/{PearlLureID}/false/null/[LocalizedText Strings\\Objects:{PearlLureID}_Name]";
                });
            }
            // Sell the pearl lure at the Dwarf
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops"))
            {
                var dwarfShopEntries = Helper.ModContent.Load<Dictionary<string, ShopItemData>>("assets/shopEntries.json");
                e.Edit(asset => {
                    var dict = asset.AsDictionary<string, ShopData>();
                    foreach (var entry in dwarfShopEntries)
                    {
                        dict.Data["Dwarf"].Items.Add(entry.Value);
                    }
                });
            }
        }

        // Allow our object to be placed
        private static void CanBePlacedHere_Postfix(StardewValley.Object __instance, GameLocation l, Vector2 tile, ref bool __result)
        {
            // Not our item, we don't care
            if (!__instance.ItemId.Contains(PearlLureID, StringComparison.OrdinalIgnoreCase) || __instance.bigCraftable.Value)
            {
                return;
            }
            else
            {
                // If the tile is suitable, it can be placed
                if (!l.CanItemBePlacedHere(tile) || l.isTileOccupiedByFarmer(tile) != null)
                {
                    __result = true;
                }
            }
        }

        // Trigger explosion properly when placed
        private static bool PlacementAction_Prefix(StardewValley.Object __instance, GameLocation location, int x, int y, Farmer who, ref bool __result)
        {
            // Not our item, we don't care
            if (!__instance.ItemId.Contains(PearlLureID, StringComparison.OrdinalIgnoreCase) || __instance.bigCraftable.Value)
            {
                return true;
            }
            else 
            {
                bool success = DoPearlExplosionAnimation(location, x, y, who);
                if (success)
                {
                    __result = true;
                }
                return false;
            }
        }

        // Set placeable to true for our item
        private static void IsPlaceable_Postfix(StardewValley.Object __instance, ref bool __result)
        {
            if (__instance.ItemId.Contains(PearlLureID, StringComparison.OrdinalIgnoreCase))
            {
                __result = true;
            }
        }

        // Spawn a new pearl lure monster when unloaded
        private static void TASUnload_Postfix(TemporaryAnimatedSprite __instance)
        {
            if (__instance.initialParentTileIndex == PearlLureMagicNumber)
            {
                Bat newBat = new Bat(__instance.Position, -555);
                newBat.DamageToFarmer = 0;
                newBat.Name = pearlLureMonsterName;
                newBat.reloadSprite();
                newBat.Speed = 2;
                newBat.objectsToDrop.Clear();
                __instance.Parent.addCharacter(newBat);
            }
        }

        // Save some important things for the postfix
        private static void Bat_BehaviorAtGameTick_Prefix(Bat __instance, out float[] __state)
        {
            __state = new float[3];
            __state[0] = __instance.xVelocity;
            __state[1] = __instance.yVelocity;
            __state[2] = __instance.rotation;
        }

        // Make the pearl lure monster not chase the farmer
        private static void Bat_BehaviorAtGameTick_Postfix(Bat __instance, float[] __state, ref NetInt ___wasHitCounter, ref NetBool ___turningRight, ref float ___targetRotation)
        {
            // Not my monster, leave immediately
            if (!__instance.Name.Equals(pearlLureMonsterName,StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get the existing target, if there is one
            int targetX = -1;
            int targetY = -1;
            try
            {
                if (__instance.modData.ContainsKey(modID + "X"))
                {
                    targetX = Int32.Parse(__instance.modData[modID + "X"]);
                }
                if (__instance.modData.ContainsKey(modID + "Y"))
                {
                    targetY = Int32.Parse(__instance.modData[modID + "Y"]);
                }
            }
            catch (Exception ex)
            {
                Mon.Log($"Something very weird happened to the modData, causing {ex}", LogLevel.Error);
            }

            // Pick a new target location 1% of the time or if there is no current target location
            if (Game1.random.Next(100) == 0 || targetX < 0 || targetY < 0)
            {
                // Get new target location
                int newTargetX = Game1.random.Next(__instance.currentLocation.Map.DisplaySize.Width);
                int newTargetY = Game1.random.Next(__instance.currentLocation.Map.DisplaySize.Height);
                // Save new target to modData
                __instance.modData[modID + "X"] = newTargetX.ToString();
                __instance.modData[modID + "Y"] = newTargetY.ToString();
                targetX = newTargetX;
                targetY = newTargetY;
            }

            // Reset the bat stats before running the movement calcs
            try
            {
                __instance.xVelocity = __state[0];
                __instance.yVelocity = __state[1];
                __instance.rotation = __state[2];
            }
            catch (Exception ex)
            {
                Mon.Log($"Unable to get lure monster state from prefix due to {ex}",LogLevel.Error);
                return;
            }

            // Set max speed and extra velocity internally
            float maxSpeed = 2f;
            float extraVelocity = 1f;

            // Get the x and y slope towards the target and normalize
            float xSlope = -(targetX - __instance.GetBoundingBox().Center.X);
            float ySlope = targetY - __instance.GetBoundingBox().Center.Y;
            float t = Math.Max(1f, Math.Abs(xSlope) + Math.Abs(ySlope));
            if (t < (float)((extraVelocity > 0f) ? 192 : 64))
            {
                __instance.xVelocity = Math.Max(0f - maxSpeed, Math.Min(maxSpeed, __instance.xVelocity * 1.05f));
                __instance.yVelocity = Math.Max(0f - maxSpeed, Math.Min(maxSpeed, __instance.yVelocity * 1.05f));
            }
            xSlope /= t;
            ySlope /= t;

            if ((int)___wasHitCounter.Value <= 0)
            {
                ___targetRotation = (float)Math.Atan2(0f - ySlope, xSlope) - (float)Math.PI / 2f;
                if ((double)(Math.Abs(___targetRotation) - Math.Abs(__instance.rotation)) > Math.PI * 7.0 / 8.0 && Game1.random.NextDouble() < 0.5)
                {
                    ___turningRight.Value = true;
                }
                else if ((double)(Math.Abs(___targetRotation) - Math.Abs(__instance.rotation)) < Math.PI / 8.0)
                {
                    ___turningRight.Value = false;
                }
                if ((bool)___turningRight.Value)
                {
                    __instance.rotation -= (float)Math.Sign(___targetRotation - __instance.rotation) * ((float)Math.PI / 64f);
                }
                else
                {
                    __instance.rotation += (float)Math.Sign(___targetRotation - __instance.rotation) * ((float)Math.PI / 64f);
                }
                __instance.rotation %= (float)Math.PI * 2f;
                ___wasHitCounter.Value = 0;
            }
            float maxAccel = Math.Min(5f, Math.Max(1f, 5f - t / 64f / 2f)) + extraVelocity;
            xSlope = (float)Math.Cos((double)__instance.rotation + Math.PI / 2.0);
            ySlope = 0f - (float)Math.Sin((double)__instance.rotation + Math.PI / 2.0);
            __instance.xVelocity += (0f - xSlope) * maxAccel / 6f + (float)Game1.random.Next(-10, 10) / 100f;
            __instance.yVelocity += (0f - ySlope) * maxAccel / 6f + (float)Game1.random.Next(-10, 10) / 100f;
            if (Math.Abs(__instance.xVelocity) > Math.Abs((0f - xSlope) * maxSpeed))
            {
                __instance.xVelocity -= (0f - xSlope) * maxAccel / 6f;
            }
            if (Math.Abs(__instance.yVelocity) > Math.Abs((0f - ySlope) * maxSpeed))
            {
                __instance.yVelocity -= (0f - ySlope) * maxAccel / 6f;
            }
        }

        private static bool Monster_FindPlayer_Prefix(Monster __instance, ref Farmer __result)
        {
            // Not my monster, leave immediately
            if (!__instance.Name.Contains("Serpent", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            // Get the closest pearl lure monster, if there isn't one then quit
            Bat closestLure = getLure(__instance.currentLocation);
            if (closestLure == null)
            {
                return true;
            }

            // Make the monster chase a dummy farmer at the position of the closest lure
            dummyFarmer.Position = closestLure.Position;
            __result = dummyFarmer;
            //Mon.Log($"Chasing dummy farmer at {dummyFarmer.getTileLocation().X}, {dummyFarmer.getTileLocation().Y}", LogLevel.Debug);
            //Mon.Log($"When serpent is at {__instance.getTileLocation().X}, {__instance.getTileLocation().Y}", LogLevel.Debug);

            return false;
        }

        // Generate explosion animation when placed
        private static bool DoPearlExplosionAnimation(GameLocation location, int x, int y, Farmer who)
        {
            Vector2 placementTile = new Vector2(x / 64, y / 64);
            foreach (TemporaryAnimatedSprite temporarySprite2 in location.temporarySprites)
            {
                if (temporarySprite2.position.Equals(placementTile * 64f))
                {
                    return false;
                }
            }
            int idNum = Game1.random.Next();
            location.playSound("thudStep");
            TemporaryAnimatedSprite pearlTAS = new TemporaryAnimatedSprite(PearlLureMagicNumber, 100f, 1, 24, placementTile * 64f, flicker: true, flipped: false, location, who)
            {
                bombRadius = 3,
                bombDamage = 1,
                shakeIntensity = 0.5f,
                shakeIntensityChange = 0.002f,
                extraInfoForEndBehavior = idNum,
                endFunction = location.removeTemporarySpritesWithID,
                sourceRect = new Rectangle(0, 0, 16, 16),
                scale = 4f
            };

            // Try forcing the texture name
            Help.Reflection.GetField<string>(pearlTAS, "textureName").SetValue(lureImageLoc);
            Help.Reflection.GetMethod(pearlTAS, "loadTexture").Invoke();

            multiplayer.broadcastSprites(location, pearlTAS);
            location.netAudio.StartPlaying("fuse");
            return true;
        }

        private static Bat getLure(GameLocation location)
        {
            foreach (Character charact in location.characters)
            {
                if (charact is Bat batChar && batChar.Name.Equals(pearlLureMonsterName, StringComparison.OrdinalIgnoreCase))
                {
                    return batChar;
                }
            }
            return null;
        }
    }
}