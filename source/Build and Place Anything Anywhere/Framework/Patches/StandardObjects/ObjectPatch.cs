/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Fences;
using StardewValley.GameData.WildTrees;
using StardewValley.GameData;
using StardewValley.Inventories;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Object = StardewValley.Object;
using xTile.Tiles;
using StardewValley.ItemTypeDefinitions;
using System.Diagnostics.CodeAnalysis;
using StardewValley.Extensions;

namespace AnythingAnywhere.Framework.Patches.StandardObjects
{
    internal class ObjectPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Object);

        internal ObjectPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Object.placementAction), new[] { typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(PlacementActionPrefix)));
            //harmony.Patch(AccessTools.Method(_object, nameof(Object.canBePlacedHere), new[] { typeof(GameLocation), typeof(Vector2), typeof(CollisionMask), typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(CanBePlacedHerePostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Object.canBePlacedHere), new[] { typeof(GameLocation), typeof(Vector2), typeof(CollisionMask), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(CanBePlacedHerePrefix)));

        }

        // Lets the placement of some special items including the mini fridge and obelisk
        // NEED TO FIX
        private static bool PlacementActionPrefix(Object __instance, GameLocation location, int x, int y, ref bool __result, Farmer who = null)
        {
            if (!ModEntry.modConfig.EnablePlacing)
                return true;

            Vector2 placementTile = new Vector2(x / 64, y / 64);
            __instance.setHealth(10);
            __instance.Location = location;
            __instance.TileLocation = placementTile;
            __instance.owner.Value = who?.UniqueMultiplayerID ?? Game1.player.UniqueMultiplayerID;
            if (!__instance.bigCraftable.Value && !(__instance is Furniture))
            {
                if (__instance.IsSprinkler() && location.doesTileHavePropertyNoNull((int)placementTile.X, (int)placementTile.Y, "NoSprinklers", "Back") == "T")
                {
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:NoSprinklers"));
                    __result = false;
                    return false;
                }
                // Bypass wild tree check
                if (__instance.IsWildTreeSapling())
                {
                    if (!canPlaceWildTreeSeed(__instance, location, placementTile, out var deniedMessage))
                    {
                        if (deniedMessage == null)
                        {
                            deniedMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13021");
                        }
                        Game1.showRedMessage(deniedMessage);
                        __result = false;
                        return false;
                    }
                    string treeType = Tree.ResolveTreeTypeFromSeed(__instance.QualifiedItemId);
                    if (treeType != null)
                    {
                        Game1.stats.Increment("wildtreesplanted");
                        location.terrainFeatures.Remove(placementTile);
                        location.terrainFeatures.Add(placementTile, new Tree(treeType, 0));
                        location.playSound("dirtyHit");
                        __result = true;
                        return false;
                    }
                    __result = false;
                    return false;
                }
                if (__instance.IsFloorPathItem())
                {
                    if (location.terrainFeatures.ContainsKey(placementTile))
                    {
                        __result = false;
                        return false;
                    }
                    string key = Flooring.GetFloorPathItemLookup()[__instance.ItemId];
                    location.terrainFeatures.Add(placementTile, new Flooring(key));
                    if (Game1.floorPathData.TryGetValue(key, out var floorData) && floorData.PlacementSound != null)
                    {
                        location.playSound(floorData.PlacementSound);
                    }
                    __result = true;
                    return false;
                }
                if (ItemContextTagManager.HasBaseTag(__instance.QualifiedItemId, "torch_item"))
                {
                    if (location.objects.ContainsKey(placementTile))
                    {
                        __result = false;
                        return false;
                    }
                    location.removeLightSource((int)(__instance.TileLocation.X * 2000f + __instance.TileLocation.Y));
                    location.removeLightSource((int)Game1.player.UniqueMultiplayerID);
                    new Torch(1, __instance.ItemId).placementAction(location, x, y, who ?? Game1.player);
                    __result = true;
                    return false;
                }
                if (__instance.IsFenceItem())
                {
                    if (location.objects.ContainsKey(placementTile))
                    {
                        __result = false;
                        return false;
                    }
                    FenceData fenceData = Fence.GetFenceLookup()[__instance.ItemId];
                    location.objects.Add(placementTile, new Fence(placementTile, __instance.ItemId, __instance.ItemId == "325"));
                    if (fenceData.PlacementSound != null)
                    {
                        location.playSound(fenceData.PlacementSound);
                    }
                    __result = true;
                    return false;
                }
                switch (__instance.QualifiedItemId)
                {
                    case "(O)TentKit":
                        {
                            if (location == null || !location.IsOutdoors)
                            {
                                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture_Outdoors_Message"));
                                __result = false;
                                return false;
                            }
                            if (Utility.isFestivalDay((Game1.dayOfMonth + 1) % 28, (Game1.dayOfMonth == 28) ? ((Season)((int)(Game1.season + 1) % 4)) : Game1.season, location.GetLocationContextId()))
                            {
                                Game1.showRedMessage(Game1.content.LoadString("Strings\\1_6_Strings:FestivalTentWarning"));
                                __result = false;
                                return false;
                            }
                            PassiveFestivalData passiveFestival = null;
                            string passiveFestivalID = null;
                            if (Utility.TryGetPassiveFestivalDataForDay((Game1.dayOfMonth + 1) % 28, (Game1.dayOfMonth == 28) ? ((Season)((int)(Game1.season + 1) % 4)) : Game1.season, null, out passiveFestivalID, out passiveFestival) && passiveFestival != null)
                            {
                                if (passiveFestival.MapReplacements != null)
                                {
                                    foreach (string key2 in passiveFestival.MapReplacements.Keys)
                                    {
                                        if (key2.Equals(location.Name))
                                        {
                                            Game1.showRedMessage(Game1.content.LoadString("Strings\\1_6_Strings:FestivalTentWarning"));
                                            __result = false;
                                            return false;
                                        }
                                    }
                                }
                                if (((passiveFestivalID.Equals("TroutDerby") && location.Name.Equals("Forest")) || (passiveFestivalID.Equals("SquidFest") && location.Name.Equals("Beach"))) && passiveFestival.StartDay > Game1.dayOfMonth)
                                {
                                    Game1.showRedMessage(Game1.content.LoadString("Strings\\1_6_Strings:FestivalTentWarning"));
                                    __result = false;
                                    return false;
                                }
                            }
                            if (who != null)
                            {
                                Rectangle area = Rectangle.Empty;
                                switch (Utility.getDirectionFromChange(placementTile, who.Tile))
                                {
                                    case 0:
                                        area = new Rectangle((int)(placementTile.X - 1f), (int)(placementTile.Y - 1f), 3, 2);
                                        break;
                                    case 1:
                                        area = new Rectangle((int)placementTile.X, (int)(placementTile.Y - 1f), 3, 2);
                                        break;
                                    case 2:
                                        area = new Rectangle((int)(placementTile.X - 1f), (int)placementTile.Y, 3, 2);
                                        break;
                                    case 3:
                                        area = new Rectangle((int)(placementTile.X - 2f), (int)(placementTile.Y - 1f), 3, 2);
                                        break;
                                }
                                if (area != Rectangle.Empty && location.isAreaClear(area))
                                {
                                    location.largeTerrainFeatures.Add(new Tent(new Vector2(area.X + 1, area.Y + 1)));
                                    Game1.playSound("moss_cut");
                                    Game1.playSound("woodyHit");
                                    new Rectangle(area.X * 64, area.Y * 64, 192, 128);
                                    Utility.addDirtPuffs(location, area.X, area.Y, 3, 2, 9);
                                    __result = true;
                                    return false;
                                }
                                Game1.showRedMessage(Game1.content.LoadString("Strings\\1_6_Strings:Tent_Blocked"));
                                __result = false;
                                return false;
                            }
                            break;
                        }
                    case "(O)926":
                        if (location.objects.ContainsKey(placementTile) || location.terrainFeatures.ContainsKey(placementTile))
                        {
                            __result = false;
                            return false;
                        }
                        location.objects.Add(placementTile, new Torch("278", bigCraftable: true)
                        {
                            Fragility = 1,
                            destroyOvernight = true
                        });
                        Utility.addSmokePuff(location, new Vector2(x, y));
                        Utility.addSmokePuff(location, new Vector2(x + 16, y + 16));
                        Utility.addSmokePuff(location, new Vector2(x + 32, y));
                        Utility.addSmokePuff(location, new Vector2(x + 48, y + 16));
                        Utility.addSmokePuff(location, new Vector2(x + 32, y + 32));
                        Game1.playSound("fireball");
                        __result = true;
                        return false;
                    case "(O)286":
                        {
                            foreach (TemporaryAnimatedSprite temporarySprite in location.temporarySprites)
                            {
                                if (temporarySprite.position.Equals(placementTile * 64f))
                                {
                                    __result = false;
                                    return false;
                                }
                            }
                            int idNum = Game1.random.Next();
                            location.playSound("thudStep");
                            ModEntry.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(__instance.ParentSheetIndex, 100f, 1, 24, placementTile * 64f, flicker: true, flipped: false, location, who)
                            {
                                shakeIntensity = 0.5f,
                                shakeIntensityChange = 0.002f,
                                extraInfoForEndBehavior = idNum,
                                endFunction = location.removeTemporarySpritesWithID
                            });
                            ModEntry.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279, 3, 4), 53f, 5, 9, placementTile * 64f + new Vector2(5f, 3f) * 4f, flicker: true, flipped: false, (float)(y + 7) / 10000f, 0f, Color.Yellow, 4f, 0f, 0f, 0f)
                            {
                                id = idNum
                            });
                            ModEntry.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279, 3, 4), 53f, 5, 9, placementTile * 64f + new Vector2(5f, 3f) * 4f, flicker: true, flipped: true, (float)(y + 7) / 10000f, 0f, Color.Orange, 4f, 0f, 0f, 0f)
                            {
                                delayBeforeAnimationStart = 100,
                                id = idNum
                            });
                            ModEntry.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279, 3, 4), 53f, 5, 9, placementTile * 64f + new Vector2(5f, 3f) * 4f, flicker: true, flipped: false, (float)(y + 7) / 10000f, 0f, Color.White, 3f, 0f, 0f, 0f)
                            {
                                delayBeforeAnimationStart = 200,
                                id = idNum
                            });
                            location.netAudio.StartPlaying("fuse");
                            __result = true;
                            return false;
                        }
                    case "(O)287":
                        {
                            foreach (TemporaryAnimatedSprite temporarySprite2 in location.temporarySprites)
                            {
                                if (temporarySprite2.position.Equals(placementTile * 64f))
                                {
                                    __result = false;
                                    return false;
                                }
                            }
                            int idNum = Game1.random.Next();
                            location.playSound("thudStep");
                            ModEntry.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(__instance.ParentSheetIndex, 100f, 1, 24, placementTile * 64f, flicker: true, flipped: false, location, who)
                            {
                                shakeIntensity = 0.5f,
                                shakeIntensityChange = 0.002f,
                                extraInfoForEndBehavior = idNum,
                                endFunction = location.removeTemporarySpritesWithID
                            });
                            ModEntry.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279, 3, 4), 53f, 5, 9, placementTile * 64f, flicker: true, flipped: false, (float)(y + 7) / 10000f, 0f, Color.Yellow, 4f, 0f, 0f, 0f)
                            {
                                id = idNum
                            });
                            ModEntry.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279, 3, 4), 53f, 5, 9, placementTile * 64f, flicker: true, flipped: false, (float)(y + 7) / 10000f, 0f, Color.Orange, 4f, 0f, 0f, 0f)
                            {
                                delayBeforeAnimationStart = 100,
                                id = idNum
                            });
                            ModEntry.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279, 3, 4), 53f, 5, 9, placementTile * 64f, flicker: true, flipped: false, (float)(y + 7) / 10000f, 0f, Color.White, 3f, 0f, 0f, 0f)
                            {
                                delayBeforeAnimationStart = 200,
                                id = idNum
                            });
                            location.netAudio.StartPlaying("fuse");
                            __result = true;
                            return false;
                        }
                    case "(O)288":
                        {
                            foreach (TemporaryAnimatedSprite temporarySprite3 in location.temporarySprites)
                            {
                                if (temporarySprite3.position.Equals(placementTile * 64f))
                                {
                                    __result = false;
                                    return false;
                                }
                            }
                            int idNum = Game1.random.Next();
                            location.playSound("thudStep");
                            ModEntry.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(__instance.ParentSheetIndex, 100f, 1, 24, placementTile * 64f, flicker: true, flipped: false, location, who)
                            {
                                shakeIntensity = 0.5f,
                                shakeIntensityChange = 0.002f,
                                extraInfoForEndBehavior = idNum,
                                endFunction = location.removeTemporarySpritesWithID
                            });
                            ModEntry.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279, 3, 4), 53f, 5, 9, placementTile * 64f + new Vector2(5f, 0f) * 4f, flicker: true, flipped: false, (float)(y + 7) / 10000f, 0f, Color.Yellow, 4f, 0f, 0f, 0f)
                            {
                                id = idNum
                            });
                            ModEntry.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279, 3, 4), 53f, 5, 9, placementTile * 64f + new Vector2(5f, 0f) * 4f, flicker: true, flipped: true, (float)(y + 7) / 10000f, 0f, Color.Orange, 4f, 0f, 0f, 0f)
                            {
                                delayBeforeAnimationStart = 100,
                                id = idNum
                            });
                            ModEntry.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279, 3, 4), 53f, 5, 9, placementTile * 64f + new Vector2(5f, 0f) * 4f, flicker: true, flipped: false, (float)(y + 7) / 10000f, 0f, Color.White, 3f, 0f, 0f, 0f)
                            {
                                delayBeforeAnimationStart = 200,
                                id = idNum
                            });
                            location.netAudio.StartPlaying("fuse");
                            __result = true;
                            return false;
                        }
                    case "(O)893":
                    case "(O)894":
                    case "(O)895":
                        {
                            int fireworkType = __instance.ParentSheetIndex - 893;
                            int spriteX = 256 + fireworkType * 16;
                            foreach (TemporaryAnimatedSprite temporarySprite4 in location.temporarySprites)
                            {
                                if (temporarySprite4.position.Equals(placementTile * 64f))
                                {
                                    __result = false;
                                    return false;
                                }
                            }
                            int idNum = Game1.random.Next();
                            int idNumFirework = Game1.random.Next();
                            location.playSound("thudStep");
                            ModEntry.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(spriteX, 397, 16, 16), 2400f, 1, 1, placementTile * 64f, flicker: false, flipped: false, -1f, 0f, Color.White, 4f, 0f, 0f, 0f)
                            {
                                shakeIntensity = 0.5f,
                                shakeIntensityChange = 0.002f,
                                extraInfoForEndBehavior = idNum,
                                endFunction = location.removeTemporarySpritesWithID,
                                layerDepth = (placementTile.Y * 64f + 64f - 16f) / 10000f
                            });
                            ModEntry.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(spriteX, 397, 16, 16), 800f, 1, 0, placementTile * 64f, flicker: false, flipped: false, -1f, 0f, Color.White, 4f, 0f, 0f, 0f)
                            {
                                fireworkType = fireworkType,
                                delayBeforeAnimationStart = 2400,
                                acceleration = new Vector2(0f, -0.36f + (float)Game1.random.Next(10) / 100f),
                                drawAboveAlwaysFront = true,
                                startSound = "firework",
                                shakeIntensity = 0.5f,
                                shakeIntensityChange = 0.002f,
                                extraInfoForEndBehavior = idNumFirework,
                                endFunction = location.removeTemporarySpritesWithID,
                                id = Game1.random.Next(20, 31),
                                Parent = location,
                                owner = who
                            });
                            ModEntry.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279, 3, 4), 40f, 5, 5, placementTile * 64f + new Vector2(11f, 12f) * 4f, flicker: true, flipped: false, (float)(y + 7) / 10000f, 0f, Color.Yellow, 4f, 0f, 0f, 0f)
                            {
                                id = idNum
                            });
                            ModEntry.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279, 3, 4), 40f, 5, 5, placementTile * 64f + new Vector2(11f, 12f) * 4f, flicker: true, flipped: true, (float)(y + 7) / 10000f, 0f, Color.Orange, 4f, 0f, 0f, 0f)
                            {
                                delayBeforeAnimationStart = 100,
                                id = idNum
                            });
                            ModEntry.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279, 3, 4), 40f, 5, 5, placementTile * 64f + new Vector2(11f, 12f) * 4f, flicker: true, flipped: false, (float)(y + 7) / 10000f, 0f, Color.White, 3f, 0f, 0f, 0f)
                            {
                                delayBeforeAnimationStart = 200,
                                id = idNum
                            });
                            location.netAudio.StartPlaying("fuse");
                            DelayedAction.functionAfterDelay(delegate
                            {
                                location.netAudio.StopPlaying("fuse");
                            }, 2400);
                            __result = true;
                            return false;
                        }
                    case "(O)297":
                        if (location.objects.ContainsKey(placementTile) || location.terrainFeatures.ContainsKey(placementTile))
                        {
                            __result = false;
                            return false;
                        }
                        location.terrainFeatures.Add(placementTile, new Grass(1, 4));
                        location.playSound("dirtyHit");
                        __result = true;
                        return false;
                    case "(O)BlueGrassStarter":
                        if (location.objects.ContainsKey(placementTile) || location.terrainFeatures.ContainsKey(placementTile))
                        {
                            __result = false;
                            return false;
                        }
                        location.terrainFeatures.Add(placementTile, new Grass(7, 4));
                        location.playSound("dirtyHit");
                        __result = true;
                        return false;
                    case "(O)710":
                        if (!CrabPot.IsValidCrabPotLocationTile(location, (int)placementTile.X, (int)placementTile.Y))
                        {
                            __result = false;
                            return false;
                        }
                        new CrabPot().placementAction(location, x, y, who);
                        __result = true;
                        return false;
                    case "(O)805":
                        {
                            if (location.terrainFeatures.TryGetValue(placementTile, out var terrainFeature4) && terrainFeature4 is Tree tree)
                            {
                                __result = tree.fertilize();
                                return false;
                            }
                            __result = false;
                            return false;
                        }
                }
            }
            else
            {
                if (__instance.IsTapper())
                {
                    if (location.terrainFeatures.TryGetValue(placementTile, out var terrainFeature5) && terrainFeature5 is Tree tree2 && (int)tree2.growthStage.Value >= 5 && !tree2.stump.Value && !location.objects.ContainsKey(placementTile) && (!tree2.isTemporaryGreenRainTree.Value || Game1.season != Season.Summer))
                    {
                        WildTreeData data = tree2.GetData();
                        if (data != null && data.CanBeTapped())
                        {
                            Object tapper = (Object)__instance.getOne();
                            tapper.heldObject.Value = null;
                            tapper.TileLocation = placementTile;
                            location.objects.Add(placementTile, tapper);
                            tree2.tapped.Value = true;
                            tree2.UpdateTapperProduct(tapper);
                            location.playSound("axe");
                            __result = true;
                            return false;
                        }
                    }
                    __result = false;
                    return false;
                }
                if (__instance.HasContextTag("sign_item"))
                {
                    if (location.objects.ContainsKey(placementTile))
                    {
                        __result = false;
                        return false;
                    }
                    location.objects.Add(placementTile, new Sign(placementTile, __instance.ItemId));
                    location.playSound("axe");
                    __result = true;
                    return false;
                }
                if (__instance.HasContextTag("torch_item"))
                {
                    if (location.objects.ContainsKey(placementTile))
                    {
                        __result = false;
                        return false;
                    }
                    Torch torch = new Torch(__instance.ItemId, bigCraftable: true);
                    torch.shakeTimer = 25;
                    torch.placementAction(location, x, y, who);
                    __result = true;
                    return false;
                }
                switch (__instance.QualifiedItemId)
                {
                    case "(BC)108":
                    case "(BC)109":
                        {
                            Object tub = (Object)__instance.getOne();
                            tub.ResetParentSheetIndex();
                            Season season = location.GetSeason();
                            if (__instance.Location.IsOutdoors && (season == Season.Winter || season == Season.Fall))
                            {
                                tub.ParentSheetIndex = 109;
                            }
                            location.Objects.Add(placementTile, tub);
                            Game1.playSound("axe");
                            __result = true;
                            return false;
                        }
                    case "(BC)62":
                        location.objects.Add(placementTile, new IndoorPot(placementTile));
                        break;
                    case "(BC)71":
                        if (location is MineShaft mine)
                        {
                            if (mine.shouldCreateLadderOnThisLevel() && mine.recursiveTryToCreateLadderDown(placementTile))
                            {
                                MineShaft.numberOfCraftedStairsUsedThisRun++;
                                __result = true;
                                return false;
                            }
                            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                        }
                        else if (location.Name.Equals("ManorHouse") && x >= 1088)
                        {
                            Game1.warpFarmer("LewisBasement", 4, 4, 2);
                            Game1.playSound("stairsdown");
                            Game1.screenGlowOnce(Color.Black, hold: true, 1f, 1f);
                            __result = true;
                            return false;
                        }
                        __result = false;
                        return false;
                    case "(BC)232":
                    case "(BC)130":
                        if (location.objects.ContainsKey(placementTile) || location is MineShaft || location is VolcanoDungeon)
                        {
                            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                            __result = false;
                            return false;
                        }
                        location.objects.Add(placementTile, new Chest(playerChest: true, placementTile, __instance.ItemId)
                        {
                            name = __instance.name,
                            shakeTimer = 50
                        });
                        location.playSound((__instance.QualifiedItemId == "(BC)130") ? "axe" : "hammer");
                        __result = true;
                        return false;
                    case "(BC)BigChest":
                    case "(BC)BigStoneChest":
                        {
                            if (location.objects.ContainsKey(placementTile) || location is MineShaft || location is VolcanoDungeon)
                            {
                                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                                __result = false;
                                return false;
                            }
                            Chest bigchest = new Chest(playerChest: true, placementTile, __instance.ItemId)
                            {
                                shakeTimer = 50,
                                SpecialChestType = Chest.SpecialChestTypes.BigChest
                            };
                            location.objects.Add(placementTile, bigchest);
                            location.playSound((__instance.QualifiedItemId == "(BC)BigChest") ? "axe" : "hammer");
                            __result = true;
                            return false;
                        }
                    case "(BC)163":
                        location.objects.Add(placementTile, new Cask(placementTile));
                        location.playSound("hammer");
                        break;
                    case "(BC)165":
                        {
                            Object autoGrabber = ItemRegistry.Create<Object>("(BC)165");
                            location.objects.Add(placementTile, autoGrabber);
                            autoGrabber.heldObject.Value = new Chest();
                            location.playSound("axe");
                            __result = true;
                            return false;
                        }
                    case "(BC)208":
                        location.objects.Add(placementTile, new Workbench(placementTile));
                        location.playSound("axe");
                        __result = true;
                        return false;
                    case "(BC)209":
                        {
                            MiniJukebox mini_jukebox = (__instance as MiniJukebox) ?? new MiniJukebox(placementTile);
                            location.objects.Add(placementTile, mini_jukebox);
                            mini_jukebox.RegisterToLocation();
                            location.playSound("hammer");
                            __result = true;
                            return false;
                        }
                    case "(BC)211":
                        {
                            WoodChipper wood_chipper = (__instance as WoodChipper) ?? new WoodChipper(placementTile);
                            wood_chipper.placementAction(location, x, y);
                            location.objects.Add(placementTile, wood_chipper);
                            location.playSound("hammer");
                            __result = true;
                            return false;
                        }
                    case "(BC)214":
                        {
                            Phone phone = (__instance as Phone) ?? new Phone(placementTile);
                            location.objects.Add(placementTile, phone);
                            location.playSound("hammer");
                            __result = true;
                            return false;
                        }
                    case "(BC)216": // Mini-Fridge
                        {
                            Chest fridge = new Chest("216", placementTile, 217, 2)
                            {
                                shakeTimer = 50
                            };
                            fridge.fridge.Value = true;
                            location.objects.Add(placementTile, fridge);
                            location.playSound("hammer");
                            __result = true;
                            return false;
                        }
                    case "(BC)248":
                        if (location.objects.ContainsKey(placementTile) || location is MineShaft || location is VolcanoDungeon)
                        {
                            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                            __result = false;
                            return false;
                        }
                        location.objects.Add(placementTile, new Chest(playerChest: true, placementTile, __instance.ItemId)
                        {
                            name = __instance.name,
                            shakeTimer = 50,
                            SpecialChestType = Chest.SpecialChestTypes.MiniShippingBin
                        });
                        location.playSound("axe");
                        __result = true;
                        return false;
                    case "(BC)238": // Mini-Obelisk
                        {
                            if (!(location is Farm) && !ModEntry.modConfig.AllowMiniObelisksAnywhere)
                            {
                                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:OnlyPlaceOnFarm"));
                                __result = false;
                                return false;
                            }
                            Vector2 obelisk1 = Vector2.Zero;
                            Vector2 obelisk2 = Vector2.Zero;

                            foreach (KeyValuePair<Vector2, Object> o2 in location.objects.Pairs)
                            {
                                if (o2.Value.QualifiedItemId == "(BC)238")
                                {
                                    if (obelisk1.Equals(Vector2.Zero))
                                    {
                                        obelisk1 = o2.Key;
                                    }
                                    else if (obelisk2.Equals(Vector2.Zero))
                                    {
                                        obelisk2 = o2.Key;
                                        break;
                                    }
                                }
                            }
                            if (!obelisk1.Equals(Vector2.Zero) && !obelisk2.Equals(Vector2.Zero))
                            {
                                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:OnlyPlaceTwo"));
                                __result = false;
                                return false; //skip original method
                            }
                            break;
                        }
                    case "(BC)254": // Ostrich Incubator
                        break;
                    case "(BC)256":
                        if (location.objects.ContainsKey(placementTile) || location is MineShaft || location is VolcanoDungeon)
                        {
                            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                            __result = false;
                            return false;
                        }
                        location.objects.Add(placementTile, new Chest(playerChest: true, placementTile, __instance.ItemId)
                        {
                            name = __instance.name,
                            shakeTimer = 50,
                            SpecialChestType = Chest.SpecialChestTypes.JunimoChest
                        });
                        location.playSound("axe");
                        __result = true;
                        return false;
                    case "(BC)275":
                        {
                            if (location.objects.ContainsKey(placementTile) || location is MineShaft || location is VolcanoDungeon)
                            {
                                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                                __result = false;
                                return false;
                            }
                            Chest chest2 = new Chest(playerChest: true, placementTile, __instance.ItemId)
                            {
                                name = __instance.name,
                                shakeTimer = 50,
                                SpecialChestType = Chest.SpecialChestTypes.AutoLoader
                            };
                            chest2.lidFrameCount.Value = 2;
                            location.objects.Add(placementTile, chest2);
                            location.playSound("axe");
                            __result = true;
                            return false;
                        }
                }
            }
            if (__instance.Category == -19 && location.terrainFeatures.TryGetValue(placementTile, out var terrainFeature3) && terrainFeature3 is HoeDirt { crop: not null } dirt3 && (__instance.QualifiedItemId == "(O)369" || __instance.QualifiedItemId == "(O)368") && (int)dirt3.crop.currentPhase.Value != 0)
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13916"));
                __result = false;
                return false;
            }
            // Bypass fruit tree placement checks
            if (__instance.isSapling())
            {
                if ((__instance.IsWildTreeSapling() /*&& !ModEntry.modConfig.EnableWildTreeTweaks*/) || (__instance.IsFruitTreeSapling() && !ModEntry.modConfig.EnablePlacing))
                {
                    if (FruitTree.IsTooCloseToAnotherTree(new Vector2(x / 64, y / 64), location))
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13060"));
                        __result = false;
                        return false;
                    }
                    if (FruitTree.IsGrowthBlocked(new Vector2(x / 64, y / 64), location))
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:FruitTree_PlacementWarning", __instance.DisplayName));
                        __result = false;
                        return false;
                    }
                }
                if (location.terrainFeatures.TryGetValue(placementTile, out var terrainFeature2))
                {
                    if (!(terrainFeature2 is HoeDirt { crop: null }))
                    {
                        __result = false;
                        return false;
                    }
                    location.terrainFeatures.Remove(placementTile);
                }
                string deniedMessage2 = null;
                bool canDig = location.doesTileHaveProperty((int)placementTile.X, (int)placementTile.Y, "Diggable", "Back") != null;
                string tileType = location.doesTileHaveProperty((int)placementTile.X, (int)placementTile.Y, "Type", "Back");
                bool canPlantTrees = location.doesEitherTileOrTileIndexPropertyEqual((int)placementTile.X, (int)placementTile.Y, "CanPlantTrees", "Back", "T");
                // Remove location needing to be Farm
                if ((((canDig || tileType == "Grass" || tileType == "Dirt" || canPlantTrees) && (!location.IsNoSpawnTile(placementTile, "Tree") || canPlantTrees)) || ((canDig || tileType == "Stone") && location.CanPlantTreesHere(__instance.ItemId, (int)placementTile.X, (int)placementTile.Y, out deniedMessage2))) || ModEntry.modConfig.EnablePlacing)
                {
                    location.playSound("dirtyHit");
                    DelayedAction.playSoundAfterDelay("coin", 100);
                    if (__instance.IsTeaSapling())
                    {
                        location.terrainFeatures.Add(placementTile, new Bush(placementTile, 3, location));
                        __result = true;
                        return false;
                    }
                    FruitTree fruitTree = new FruitTree(__instance.ItemId)
                    {
                        GreenHouseTileTree = (location.IsGreenhouse && tileType == "Stone")
                    };
                    fruitTree.growthRate.Value = Math.Max(1, __instance.Quality + 1);
                    location.terrainFeatures.Add(placementTile, fruitTree);
                    __result = true;
                    return false;
                }
                if (deniedMessage2 == null)
                {
                    deniedMessage2 = Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13068");
                }
                Game1.showRedMessage(deniedMessage2);
                __result = false;
                return false;
            }
            if (__instance.Category == -74 || __instance.Category == -19)
            {
                if (location.terrainFeatures.TryGetValue(placementTile, out var terrainFeature) && terrainFeature is HoeDirt dirt2)
                {
                    string seedId = Crop.ResolveSeedId(who.ActiveObject.ItemId, location);
                    if (dirt2.canPlantThisSeedHere(seedId, who.ActiveObject.Category == -19))
                    {
                        if (dirt2.plant(seedId, who, who.ActiveObject.Category == -19) && who.IsLocalPlayer)
                        {
                            if (__instance.Category == -74)
                            {
                                foreach (Object o in location.Objects.Values)
                                {
                                    if (!o.IsSprinkler() || o.heldObject.Value == null || !(o.heldObject.Value.QualifiedItemId == "(O)913") || !o.IsInSprinklerRangeBroadphase(placementTile))
                                    {
                                        continue;
                                    }
                                    if (!o.GetSprinklerTiles().Contains(placementTile))
                                    {
                                        continue;
                                    }
                                    Object value = o.heldObject.Value.heldObject.Value;
                                    Chest chest = value as Chest;
                                    if (chest == null)
                                    {
                                        continue;
                                    }
                                    IInventory items = chest.Items;
                                    if (items.Count <= 0 || items[0] == null || chest.GetMutex().IsLocked())
                                    {
                                        continue;
                                    }
                                    chest.GetMutex().RequestLock(delegate
                                    {
                                        if (items.Count > 0 && items[0] != null)
                                        {
                                            Item item = items[0];
                                            if (item.Category == -19 && ((HoeDirt)terrainFeature).plant(item.ItemId, who, isFertilizer: true))
                                            {
                                                item.Stack--;
                                                if (item.Stack <= 0)
                                                {
                                                    items[0] = null;
                                                }
                                            }
                                        }
                                        chest.GetMutex().ReleaseLock();
                                    });
                                    break;
                                }
                            }
                            Game1.haltAfterCheck = false;
                            __result = true;
                            return false;
                        }
                        __result = false;
                        return false;
                    }
                    __result = false;
                    return false;
                }
                __result = false;
                return false;
            }
            if (!__instance.performDropDownAction(who))
            {
                Object toPlace = (Object)__instance.getOne();
                bool place_furniture_instance_instead = false;
                if (toPlace.GetType() == typeof(Furniture) && Furniture.GetFurnitureInstance(__instance.ItemId, new Vector2(x / 64, y / 64)).GetType() != toPlace.GetType())
                {
                    StorageFurniture storageFurniture = new StorageFurniture(__instance.ItemId, new Vector2(x / 64, y / 64));
                    storageFurniture.currentRotation.Value = (__instance as Furniture).currentRotation.Value;
                    storageFurniture.updateRotation();
                    toPlace = storageFurniture;
                    place_furniture_instance_instead = true;
                }
                toPlace.shakeTimer = 50;
                toPlace.Location = location;
                toPlace.TileLocation = placementTile;
                toPlace.performDropDownAction(who);
                if (toPlace.QualifiedItemId == "(BC)TextSign")
                {
                    toPlace.signText.Value = null;
                    toPlace.showNextIndex.Value = true;
                }
                if (toPlace.name.Contains("Seasonal"))
                {
                    int baseIndex = toPlace.ParentSheetIndex - toPlace.ParentSheetIndex % 4;
                    toPlace.ParentSheetIndex = baseIndex + location.GetSeasonIndex();
                }
                if (!(toPlace is Furniture) && !ModEntry.modConfig.EnableFreePlace && location.objects.TryGetValue(placementTile, out var tileObj))
                {
                    if (tileObj.QualifiedItemId != __instance.QualifiedItemId)
                    {
                        Game1.createItemDebris(tileObj, placementTile * 64f, Game1.random.Next(4));
                        location.objects[placementTile] = toPlace;
                    }
                }
                else if (toPlace is Furniture furniture)
                {
                    if (place_furniture_instance_instead)
                    {
                        location.furniture.Add(furniture);
                    }
                    else
                    {
                        location.furniture.Add(__instance as Furniture);
                    }
                }
                else
                {
                    location.objects.Add(placementTile, toPlace);
                }
                toPlace.initializeLightSource(placementTile);
            }
            location.playSound("woodyStep");
            __result = true;
            return false;
        }




        // Lets objects be placed inside of walls
        // NEED TO FIX
        private static bool CanBePlacedHerePrefix(Object __instance, GameLocation l, Vector2 tile, ref bool __result, CollisionMask collisionMask = CollisionMask.All, bool showError = false)
        {
            if (ModEntry.modConfig.EnableFreePlace)
            {
                __result = true;
                return false;
            }



            if (__instance.QualifiedItemId == "(O)710")
            {
                return true;
            }
            if (__instance.IsTapper() && l.terrainFeatures.TryGetValue(tile, out var terrainFeature) && terrainFeature is Tree tree && !l.objects.ContainsKey(tile) && (tree.GetData()?.CanBeTapped() ?? false))
            {
                return true;
            }
            if (__instance.QualifiedItemId == "(O)805" && l.terrainFeatures.TryGetValue(tile, out var terrainFeature2) && terrainFeature2 is Tree)
            {
                return true;
            }
            if (Object.isWildTreeSeed(__instance.ItemId))
            {
                if (!l.CanItemBePlacedHere(tile, itemIsPassable: true, collisionMask))
                {
                    __result = false;
                    return false;
                }
                if (!canPlaceWildTreeSeed(__instance, l, tile, out var deniedMessage) && !ModEntry.modConfig.EnablePlacing)
                {
                    if (showError && deniedMessage != null)
                    {
                        Game1.showRedMessage(deniedMessage);
                    }
                    __result = false;
                    return false;
                }
                __result = true;
                return false;
            }
            if ((int)__instance.Category == -74)
            {
                HoeDirt dirt = l.GetHoeDirtAtTile(tile);
                Object obj = l.getObjectAtTile((int)tile.X, (int)tile.Y);
                IndoorPot pot = obj as IndoorPot;
                if (dirt?.crop != null || (dirt == null && l.terrainFeatures.TryGetValue(tile, out var _)))
                {
                    __result = false;
                    return false;
                }
                if (__instance.IsFruitTreeSapling())
                {
                    if (obj != null)
                    {
                        __result = false;
                        return false;
                    }
                    if (dirt == null)
                    {
                        if ((FruitTree.IsTooCloseToAnotherTree(tile, l, !__instance.IsFruitTreeSapling())) && !ModEntry.modConfig.EnablePlacing)
                        {
                            if (showError)
                            {
                                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13060"));
                            }
                            __result = false;
                            return false;
                        }
                        if (FruitTree.IsGrowthBlocked(tile, l))
                        {
                            if (showError)
                            {
                                Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:FruitTree_PlacementWarning", __instance.DisplayName));
                            }
                            __result = false;
                            return false;
                        }
                        if (!l.CanItemBePlacedHere(tile, itemIsPassable: true, collisionMask))
                        {
                            __result = false;
                            return false;
                        }
                        if (!l.CanPlantTreesHere(__instance.ItemId, (int)tile.X, (int)tile.Y, out var deniedMessage2) && !ModEntry.modConfig.EnablePlacing)
                        {
                            if (showError && deniedMessage2 != null)
                            {
                                Game1.showRedMessage(deniedMessage2);
                            }
                            __result = false;
                            return false;
                        }
                        __result = true;
                        return false;
                    }
                    __result = false;
                    return false;
                }
                if (__instance.IsTeaSapling())
                {
                    return true;
                }
                if (__instance.IsWildTreeSapling() )
                {
                    return true;
                }
                if (__instance.HasTypeObject())
                {
                    return true;
                }
                return true;
            }
            if ((int)__instance.Category == -19)
            {
                return true;
            }
            if (l != null)
            {
                return true;
            }
            if (__instance.IsFloorPathItem())
            {
                return true;
            }
            return true;
        }

        // Reimpmenting canPlaceWildTreeSeed, as its private and can't reference.
        internal static bool canPlaceWildTreeSeed(Object __instance, GameLocation location, Vector2 tile, out string deniedMessage)
        {
            if (location.IsNoSpawnTile(tile, "Tree", ignoreTileSheetProperties: true))
            {
                deniedMessage = null;
                return false;
            }
            if (location.IsNoSpawnTile(tile, "Tree") && !location.doesEitherTileOrTileIndexPropertyEqual((int)tile.X, (int)tile.Y, "CanPlantTrees", "Back", "T"))
            {
                deniedMessage = null;
                return false;
            }
            if (location.objects.ContainsKey(tile))
            {
                deniedMessage = null;
                return false;
            }
            if (location.terrainFeatures.TryGetValue(tile, out var terrainFeature) && !(terrainFeature is HoeDirt))
            {
                deniedMessage = null;
                return false;
            }
            if (!location.CanPlantTreesHere(__instance.ItemId, (int)tile.X, (int)tile.Y, out deniedMessage) && !ModEntry.modConfig.EnablePlacing)
            {
                return false;
            }
            if (ModEntry.modConfig.EnablePlacing)
            {
                return true;
            }
            return location.CheckItemPlantRules(__instance.QualifiedItemId, isGardenPot: false, location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Diggable", "Back") != null || location.doesEitherTileOrTileIndexPropertyEqual((int)tile.X, (int)tile.Y, "CanPlantTrees", "Back", "T"), out deniedMessage);
        }

    }
}
