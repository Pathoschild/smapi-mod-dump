/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kakashigr/stardew-radioactivetools
**
*************************************************/

using System.Collections.Generic;
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace RadioactiveTools.Framework {
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static class RadioactivePatches {
        /*********
        ** Public methods
        *********/
        /****
        ** Furnace patches
        ****/
        public static void Farmer_GetTallyOfObject(ref int __result, int index, bool bigCraftable) {
            if (index == 382 && !bigCraftable && __result <= 0)
                __result = 666666;
        }

        public static bool Object_PerformObjectDropInAction(ref SObject __instance, ref bool __result, ref Item dropInItem, bool probe, Farmer who) {
            if (!(dropInItem is SObject object1))
                return false;

            if (object1.ParentSheetIndex != 74)
                return true;

            if (__instance.name.Equals("Furnace")) {
                if (who.IsLocalPlayer && who.getTallyOfObject(382, false) == 666666) {
                    if (!probe && who.IsLocalPlayer)
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12772"));
                    return false;
                }
                if (__instance.heldObject.Value == null && !probe) {
                    __instance.heldObject.Value = new SObject(910, 5);
                    __instance.MinutesUntilReady = 2400;
                    who.currentLocation.playSound("furnace");
                    __instance.initializeLightSource(__instance.TileLocation);
                    __instance.showNextIndex.Value = true;

                    Multiplayer multiplayer = ModEntry.ModHelper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
                    multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(30, __instance.TileLocation * 64f + new Vector2(0.0f, -16f), Color.White, 4, false, 50f, 10, 64, (float)((__instance.TileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05)) {
                        alphaFade = 0.005f
                    });
                    for (int index = who.Items.Count - 1; index >= 0; --index) {
                        if (who.Items[index] is SObject obj && obj.ParentSheetIndex == 382) {
                            --who.Items[index].Stack;
                            if (who.Items[index].Stack <= 0) {
                                who.Items[index] = null;
                                break;
                            }
                            break;
                        }
                    }
                    object1.Stack -= 1;
                    __result = object1.Stack <= 0;
                    return false;
                }
                if (__instance.heldObject.Value == null & probe) {
                    if (object1.ParentSheetIndex == 74) {
                        __instance.heldObject.Value = new SObject();
                        __result = true;
                        return false;
                    }
                }
            }
            __result = false;
            return false;
        }

        /****
        ** Sprinkler patches
        ****/
        public static bool Farm_AddCrows(ref Farm __instance) {
            int num1 = 0;
            foreach (KeyValuePair<Vector2, TerrainFeature> pair in __instance.terrainFeatures.Pairs) {
                if (pair.Value is HoeDirt dirt && dirt.crop != null)
                    ++num1;
            }
            List<Vector2> vector2List = new List<Vector2>();
            foreach (KeyValuePair<Vector2, SObject> pair in __instance.objects.Pairs) {
                if (pair.Value.Name.Contains("arecrow")) {
                    vector2List.Add(pair.Key);
                }
            }
            int num2 = System.Math.Min(4, num1 / 16);
            for (int index1 = 0; index1 < num2; ++index1) {
                if (Game1.random.NextDouble() < 1.0) {
                    for (int index2 = 0; index2 < 10; ++index2) {
                        Vector2 key = __instance.terrainFeatures.Pairs.ElementAt(Game1.random.Next(__instance.terrainFeatures.Count())).Key;
                        if (__instance.terrainFeatures[key] is HoeDirt dirt && dirt.crop?.currentPhase.Value > 1) {
                            bool flag = false;
                            foreach (Vector2 index3 in vector2List) {
                                if (Vector2.Distance(index3, key) < 9.0) {
                                    flag = true;
                                    ++__instance.objects[index3].SpecialVariable;
                                    break;
                                }
                            }
                            if (!flag)
                                dirt.crop = null;
                            break;
                        }
                    }
                }
            }
            return false;
        }

        public static void After_Object_IsSprinkler(ref SObject __instance, ref bool __result) {
            if (__instance.ParentSheetIndex == RadioactiveSprinklerItem.INDEX)
                __result = true;
        }
        public static void After_Object_IsFishingRod(ref SObject __instance, ref bool __result) {
            if (__instance.ParentSheetIndex == RadioactiveRodItem.id)
                __result = true;
        }

        public static void After_Object_GetBaseRadiusForSprinkler(ref SObject __instance, ref int __result) {
            if (__instance.ParentSheetIndex == RadioactiveSprinklerItem.INDEX)
                __result = ModEntry.Config.SprinklerRange;
        }

        public static bool Object_UpdatingWhenCurrentLocation(ref SObject __instance, GameTime time, GameLocation environment) {
            var obj = __instance;

            // enable sprinkler scarecrow/light
            if (obj.ParentSheetIndex == RadioactiveSprinklerItem.INDEX)
                TryEnableRadioactiveSprinkler(environment, obj.TileLocation, obj);

            return true;
        }

        public static bool Object_OnPlacing(ref SObject __instance, GameLocation location, int x, int y) {
            var obj = __instance;

            // enable sprinkler scarecrow/light
            if (obj.ParentSheetIndex == RadioactiveSprinklerItem.INDEX)
                TryEnableRadioactiveSprinkler(location, new Vector2(x, y), obj);

            return true;
        }

        /****
        ** Tool patches
        ****/
        public static void Tree_PerformToolAction(ref Tree __instance, Tool t, int explosion) {
            if (t is Axe axe && axe.UpgradeLevel == 5 && explosion <= 0 && ModEntry.ModHelper.Reflection.GetField<NetFloat>(__instance, "health").GetValue() > -99f) {
                __instance.health.Value = 0.0f;
            }
        }

        public static void FruitTree_PerformToolAction(ref FruitTree __instance, Tool t, int explosion) {
            if (t is Axe axe && axe.UpgradeLevel == 5 && explosion <= 0 && ModEntry.ModHelper.Reflection.GetField<NetFloat>(__instance, "health").GetValue() > -99f) {
                __instance.health.Value = 0.0f;
            }
        }

        public static void Pickaxe_DoFunction(ref Pickaxe __instance, GameLocation location, int x, int y, int power, Farmer who) {
            if (__instance.UpgradeLevel == 5) {
                if (location.Objects.TryGetValue(new Vector2(x / 64, y / 64), out SObject obj)) {
                    if (obj.Name == "Stone") {
                        obj.MinutesUntilReady = 0;
                    }
                }
            }
        }

        public static void ResourceClump_PerformToolAction(ref ResourceClump __instance, Tool t, int damage, Vector2 tileLocation, GameLocation location) {
            if (t is Axe && t.UpgradeLevel == 5 && (__instance.parentSheetIndex.Value == 600 || __instance.parentSheetIndex.Value == 602)) {
                __instance.health.Value = 0;
            }
        }

        public static void Tool_TilesAffected_Postfix(ref List<Vector2> __result, Vector2 tileLocation, int power, Farmer who) {
            if (power >= 6) {
                __result.Clear();
                Vector2 direction;
                Vector2 orth;
                int radius = ModEntry.Config.RadioactiveToolWidth;
                int length = ModEntry.Config.RadioactiveToolLength;
                switch (who.FacingDirection) {
                    case 0: direction = new Vector2(0, -1); orth = new Vector2(1, 0); break;
                    case 1: direction = new Vector2(1, 0); orth = new Vector2(0, 1); break;
                    case 2: direction = new Vector2(0, 1); orth = new Vector2(-1, 0); break;
                    case 3: direction = new Vector2(-1, 0); orth = new Vector2(0, -1); break;
                    default: direction = Vector2.Zero; orth = Vector2.Zero; break;
                }
                for (int i = 0; i < length; i++) {
                    __result.Add(direction * i + tileLocation);
                    for (int j = 1; j <= radius; j++) {
                        __result.Add(direction * i + orth * j + tileLocation);
                        __result.Add(direction * i + orth * -j + tileLocation);
                    }
                }
            }
        }

        public static bool Tool_Name(Tool __instance, ref string __result) {
            if (__instance.UpgradeLevel == 5) {
                switch (__instance.BaseName) {
                    case "Axe": __result = ModEntry.ModHelper.Translation.Get("radioactiveAxe"); break;
                    case "Pickaxe": __result = ModEntry.ModHelper.Translation.Get("radioactivePickaxe"); break;
                    case "Watering Can": __result = ModEntry.ModHelper.Translation.Get("radioactiveWatercan"); break;
                    case "Hoe": __result = ModEntry.ModHelper.Translation.Get("radioactiveHoe"); break;
                }
                return false;
            }
            return true;
        }

        public static bool Tool_DisplayName(Tool __instance, ref string __result) {
            if (__instance.UpgradeLevel == 5) {
                __result = __instance.Name;
                return false;
            }
            return true;
        }

        public static bool Fishrod_Name(FishingRod __instance, ref string __result) {
            if (__instance.UpgradeLevel == 9) {
                __result = ModEntry.ModHelper.Translation.Get("radioactiveRod.name");
                return false;
            }
            return true;
        }

        public static bool Fishrod_DisplayName(FishingRod __instance, ref string __result) {
            if (__instance.UpgradeLevel == 9) {
                __result = __instance.Name;
                return false;
            }
            return true;
        }

        public static bool FishrodColor(FishingRod __instance, ref Color __result) {
            switch (__instance.UpgradeLevel) {
                case 0:
                    __result = Color.Goldenrod; return false;
                case 1:
                    __result = Color.OliveDrab; return false;
                case 2:
                    __result = Color.White; return false;
                case 3:
                    __result = Color.Violet; return false;
                case 9:
                    __result = Color.GreenYellow; return false;
                default:
                    __result = Color.GreenYellow; return false;
            }
        }

        public static bool Radioactive_isScythe(MeleeWeapon __instance, ref bool __result) {
            if (__instance.BaseName == "Radioactive Scythe") {
                __result = true;
                return false;
            }
            return true;
        }

        public static void RScythe_performToolAction(ref Grass __instance, Tool t, Vector2 tileLocation) {
            
            if (t != null && t is MeleeWeapon && t.BaseName == "Radioactive Scythe") {
           
                int numberOfWeedsToDestroy2 = 4;
                if ((byte)__instance.grassType.Value == 6 && Game1.random.NextDouble() < 0.3) {
                    numberOfWeedsToDestroy2 = 0;
                }
                __instance.numberOfWeeds.Value = (int)__instance.numberOfWeeds.Value - numberOfWeedsToDestroy2;
                
                if ((int)__instance.numberOfWeeds.Value <= 0) {
                    Random obj = Game1.IsMultiplayer ? Game1.recentMultiplayerRandom : new Random((int)((float)(double)Game1.uniqueIDForThisGame + tileLocation.X * 1000f + tileLocation.Y * 11f));
                    double chance = 0.90;
                    if (obj.NextDouble() < chance && (Game1.getLocationFromName("Farm") as Farm).tryToAddHay(1) == 0) {
                        TemporaryAnimatedSprite tmpSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 178, 16, 16), 750f, 1, 0, t.getLastFarmerToUse().Position - new Vector2(0f, 128f), flicker: false, flipped: false, t.getLastFarmerToUse().Position.Y / 10000f, 0.005f, Color.White, 4f, -0.005f, 0f, 0f);
                        tmpSprite.motion.Y = -1f;
                        tmpSprite.layerDepth = 1f - (float)Game1.random.Next(100) / 10000f;
                        tmpSprite.delayBeforeAnimationStart = Game1.random.Next(350);
                        Game1.addHUDMessage(new HUDMessage("Hay", 1, add: true, Color.LightGoldenrodYellow, new SObject(178, 1)));
                    }
                }
            }
        }

        /*
         * Radioactive Ferrtilizer patches 
         */
        //public static void RadioactiveFert_plant(HoeDirt __instance ,ref int index, ref int tileX, ref int tileY, ref Farmer who, ref bool isFertilizer) {

        //    Crop crop = new Crop(index, tileX, tileY);

        //    if (__instance.fertilizer == 465 || __instance.fertilizer == 466 || who.professions.Contains(5)) {
        //        int num1 = 0;
        //        for (int index1 = 0; index1 < crop.phaseDays.Count - 1; ++index1)
        //            num1 += crop.phaseDays[index1];
        //        float num2 = __instance.fertilizer == 465 ? 0.1f : (__instance.fertilizer == 466 ? 0.25f : 0.0f);
        //        if (who.professions.Contains(5))
        //            num2 += 0.1f;
        //        int num3 = (int)Math.Ceiling((double)num1 * (double)num2);
        //        for (int index1 = 0; num3 > 0 && index1 < 3; ++index1) {
        //            for (int index2 = 0; index2 < crop.phaseDays.Count; ++index2) {
        //                if (index2 > 0 || crop.phaseDays[index2] > 1) {
        //                    List<int> phaseDays = crop.phaseDays;
        //                    int num4 = index2;
        //                    int index3 = num4;
        //                    int num5 = phaseDays[index3];
        //                    int index4 = num4;
        //                    int num6 = num5 - 1;
        //                    phaseDays[index4] = num6;
        //                    --num3;
        //                }
        //                if (num3 <= 0)
        //                    break;
        //            }
        //        }
        //    }

        //}


        /*********
        ** Private methods
        *********/
        /// <summary>Try to add the light source for a radioactive sprinkler, if applicable.</summary>
        /// <param name="location">The location containing the sprinkler.</param>
        /// <param name="tile">The sprinkler's tile coordinate within the location.</param>
        /// <param name="obj">The object to check.</param>
        private static void TryEnableRadioactiveSprinkler(GameLocation location, Vector2 tile, SObject obj) {
            if (obj.ParentSheetIndex != RadioactiveSprinklerItem.INDEX)
                return;

            // set name
            obj.Name = ModEntry.Config.UseSprinklersAsScarecrows
                ? "Radioactive Scarecrow Sprinkler"
                : "Radioactive Sprinkler";

            // add light source
            if (ModEntry.Config.UseSprinklersAsLamps) {
                int id = (int)tile.X * 4000 + (int)tile.Y;
                if (!location.sharedLights.ContainsKey(id)) {
                    obj.lightSource = new LightSource(4, tile * Game1.tileSize, 2.0f, Color.Black, id);
                    location.sharedLights.Add(id, obj.lightSource);
                }
            }
        }
    }
}
