/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chiccenFL/StardewValleyMods
**
*************************************************/

using StardewValley;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Object = StardewValley.Object;
using StardewValley.TerrainFeatures;
using StardewValley.GameData.WildTrees;
using StardewValley.Extensions;
using StardewValley.Tools;
using StardewValley.Locations;
using StardewValley.Constants;
using StardewValley.Enchantments;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using xTile.Tiles;

namespace WildTreeTweaks
{
    public partial class ModEntry
    {

        /// <summary>
        /// Tree IDs:
        /// 1 - Oak
        /// 2 - Maple
        /// 3 - Pine
        /// 6 - Palm tree
        /// 7 - Mushroom tree
        /// 8 - Mahogany
        /// 9 - Palm tree 2 (Ginger Island variant)
        /// 10 - Wild Oak Tree/Green Rain Oak
        /// 11 - Wild Maple Tree/Green Rain Maple
        /// 12 - Wild Pine Tree/Green Rain Pine
        /// 13 - Mystic Tree
        /// </summary>
        
        public static Dictionary<GameLocation, Dictionary<Vector2, List<Leaf>>> leaves = new();

        [HarmonyPatch(typeof(Tree))]
        [HarmonyPatch(MethodType.Constructor, new Type[] { })]
        public class Tree__Patch1
        {
            public static void Postfix(Tree __instance)
            {
                if (!Config.EnableMod) return;
                __instance.health.Value = Config.Health;
            }

        }

        [HarmonyPatch(typeof(Tree), new Type[] {typeof(string), typeof(int), typeof(bool)})]
        [HarmonyPatch(MethodType.Constructor)]
        public class Tree__Patch2
        {
            public static void Postfix(Tree __instance, string id, int growthStage, bool isGreenRainTemporaryTree)
            {
                if (!Config.EnableMod || isGreenRainTemporaryTree) return;
                __instance.health.Value = Config.Health;
            }
        }

        [HarmonyPatch(typeof(Tree), new Type[] {typeof(string)})]
        [HarmonyPatch(MethodType.Constructor)]
        public class Tree__Patch3
        {
            public static void Postfix(Tree __instance, string id)
            {
                if (!Config.EnableMod) return;
            }
        }

        [HarmonyPatch(typeof(Tree), nameof(Tree.IsGrowthBlockedByNearbyTree))]
        public class Tree_IsGrowthBlockedByNearbyTree_Patch
        {
            public static void Postfix(Tree __instance, ref bool __result)
            {
                __result = !(Config.EnableMod && Config.GrowNearTrees);
                return;
            }
        }

        [HarmonyPatch(typeof(Object), nameof(Object.placementAction))]
        public class Object_placementAction_Patch
        {
            public static bool Prefix(Object __instance, GameLocation location, int x, int y, ref bool __result)
            {

                // TileIndexProperties((Type: Dirt), (Water: T)) for beach farm beach water

                if (!Config.EnableMod || !__instance.IsWildTreeSapling() || ((!location.IsFarm || !location.IsGreenhouse) && Config.OnlyOnFarm)) return true;

                Vector2 placementTile = new Vector2(x / 64, y / 64);
                
                if (!canPlaceWildTreeSeed(__instance, location, placementTile, out var deniedMessage))
                {
                    deniedMessage ??= Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13021");
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

                return true;

            }
        }

        [HarmonyPatch(typeof(Object), nameof(Object.canBePlacedHere))]
        public class Object_canBePlacedHere_Patch
        {
            public static void Postfix(Object __instance, GameLocation l, Vector2 tile, ref bool __result, bool showError = false)
            {
                if (!Config.EnableMod || !Object.isWildTreeSeed(__instance.ItemId) || __result || (!l.IsFarm && Config.OnlyOnFarm) || (!l.IsOutdoors && (!l.treatAsOutdoors.Value && !l.IsGreenhouse)))
                    return;

                if (!canPlaceWildTreeSeed(__instance, l, tile, out var deniedMessage))
                {
                    if (showError && deniedMessage is not null)
                        Game1.showRedMessage(deniedMessage);
                    return;
                }
                if (!l.isTileOnMap(tile)) return;
                if (l.GetHoeDirtAtTile(tile)?.crop is not null) return;

                __result = true;
            }
        }

        [HarmonyPatch(typeof(Tree), nameof(Tree.TryGetData))]
        public class Tree_TryGetData_Patch
        {
            public static bool Prefix(string id, out WildTreeData data, ref bool __result)
            {
                if (!Config.EnableMod || id is null)
                {
                    Log("TryGetData: mod disabled or id is null.", debugOnly: true);
                    data = null;
                    return true;
                }

                if (!(Tree.GetWildTreeDataDictionary().TryGetValue(id, out data))) return true;

                data.GrowthChance = Config.GrowthChance; // 1f = always true, called in Tree.dayUpdate()
                data.GrowsInWinter = Config.GrowInWinter; // called in IsInSeason() => GetMaxSizeHere() => dayUpdate()
                data.IsStumpDuringWinter = !Config.GrowInWinter; // called in dayUpdate()
                data.SeedSpreadChance = Config.SeedSpreadChance; // 1f = always true, called in Tree.dayUpdate()
                float difChance = (data.SeedOnShakeChance - Config.SeedChance) * 10f; // seed chop scales with seed shake. lowest possible val for seed chop = 0.25 = 25% chance
                data.SeedOnShakeChance = Config.SeedChance; // 1f = always true, called in Tree.dayUpdate()
                data.SeedOnChopChance = (difChance + data.SeedOnChopChance) > 1f ? 1f : data.SeedOnChopChance + difChance;

                List<WildTreeChopItemData> chopItems = data.ChopItems;

                __result = true;
                return false;
                //data.SeedOnChopChance = Config.SeedChance * 15f;
            }
        }

        [HarmonyPatch(typeof(Tree), nameof(Tree.performToolAction))]
        public class Tree_performToolAction_Patch
        {
            public static bool Prefix(Tree __instance, Tool t, int explosion, Vector2 tileLocation, ref bool __result)
            {
                if (!Config.EnableMod || (!__instance.Location.IsFarm && Config.OnlyOnFarm) || __instance.growthStage.Value < 5 || (Config.BookChance == 0.0005f || !Config.BookChanceBool || Config.MysteryBoxChance == 0.005f)) return true;

                GameLocation location = __instance.Location ?? Game1.currentLocation;
                if ((int)__instance.growthStage.Value >= 5)
                {
                    if ((bool)__instance.hasMoss.Value)
                    {
                        Item moss = Tree.CreateMossItem();
                        if (t.getLastFarmerToUse() != null)
                        {
                            t.getLastFarmerToUse().gainExperience(2, moss.Stack);
                        }
                        __instance.hasMoss.Value = false;
                        Game1.createMultipleItemDebris(moss, new Vector2(tileLocation.X, tileLocation.Y - 1f) * 64f, -1, location, Game1.player.StandingPixel.Y - 32);
                        Game1.stats.Increment("mossHarvested");
                        __instance.shake(tileLocation, doEvenIfStillShaking: true);
                        __instance.growthStage.Value = 12 - moss.Stack;
                        Game1.playSound("moss_cut");
                        for (int i = 0; i < 6; i++)
                        {
                            location.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\debris", new Microsoft.Xna.Framework.Rectangle(Game1.random.Choose(16, 0), 96, 16, 16), new Vector2(tileLocation.X + (float)Game1.random.NextDouble() - 0.15f, tileLocation.Y - 1f + (float)Game1.random.NextDouble()) * 64f, flipped: false, 0.025f, Color.Green)
                            {
                                drawAboveAlwaysFront = true,
                                motion = new Vector2((float)Game1.random.Next(-10, 11) / 10f, -4f),
                                acceleration = new Vector2(0f, 0.3f + (float)Game1.random.Next(-10, 11) / 200f),
                                animationLength = 1,
                                interval = 1000f,
                                sourceRectStartingPos = new Vector2(0f, 96f),
                                alpha = 1f,
                                layerDepth = 1f,
                                scale = 4f
                            });
                        }
                    }
                    if ((bool)__instance.tapped.Value)
                    {
                        __result = false;
                        return false;
                    }
                    if (t is Axe)
                    {
                        location.playSound("axchop", tileLocation);
                        __instance.lastPlayerToHit.Value = t.getLastFarmerToUse().UniqueMultiplayerID;
                        location.debris.Add(new Debris(12, Game1.random.Next(1, 3), t.getLastFarmerToUse().GetToolLocation() + new Vector2(16f, 0f), t.getLastFarmerToUse().Position, 0, __instance.GetChopDebrisColor()));
                        if (location is Town && tileLocation.X < 100f && !__instance.isTemporaryGreenRainTree.Value)
                        {
                            int pathsIndex = location.getTileIndexAt((int)tileLocation.X, (int)tileLocation.Y, "Paths");
                            if (pathsIndex >= 9 || pathsIndex <= 11)
                            {
                                __instance.shake(tileLocation, doEvenIfStillShaking: true);
                                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:TownTreeWarning"));
                                __result = false;
                                return false;
                            }
                        }
                        if (!__instance.stump.Value)
                        {
                            if (t.getLastFarmerToUse() != null && location.HasUnlockedAreaSecretNotes(t.getLastFarmerToUse()) && Game1.random.NextDouble() < 0.005)
                            {
                                Object o = location.tryToCreateUnseenSecretNote(t.getLastFarmerToUse());
                                if (o is not null)
                                {
                                    Game1.createItemDebris(o, new Vector2(tileLocation.X, tileLocation.Y - 3f) * 64f, -1, location, Game1.player.StandingPixel.Y - 32);
                                }
                            }
                            else if (t.getLastFarmerToUse() != null && Utility.tryRollMysteryBox((double)Config.MysteryBoxChance))
                            {
                                Game1.createItemDebris(ItemRegistry.Create((t.getLastFarmerToUse().stats.Get(StatKeys.Mastery(2)) != 0) ? "(O)GoldenMysteryBox" : "(O)MysteryBox"), new Vector2(tileLocation.X, tileLocation.Y - 3f) * 64f, -1, location, Game1.player.StandingPixel.Y - 32);
                            }
                            else if (t.getLastFarmerToUse() != null && t.getLastFarmerToUse().stats.Get("TreesChopped") > 20)
                            {
                                if (Config.BookChanceBool && Game1.random.NextDouble() < (double)Config.BookChance)
                                {
                                    Game1.createItemDebris(ItemRegistry.Create("(O)Book_Woodcutting"), new Vector2(tileLocation.X, tileLocation.Y - 3f) * 64f, -1, location, Game1.player.StandingPixel.Y - 32);
                                    t.getLastFarmerToUse().mailReceived.Add("GotWoodcuttingBook");
                                }
                                else if (Game1.random.NextDouble() < 0.0003 + (t.getLastFarmerToUse().mailReceived.Contains("GotWoodcuttingBook") ? 0.0007 : ((double)t.getLastFarmerToUse().stats.Get("TreesChopped") * 1E-05)))
                                {
                                    Game1.createItemDebris(ItemRegistry.Create("(O)Book_Woodcutting"), new Vector2(tileLocation.X, tileLocation.Y - 3f) * 64f, -1, location, Game1.player.StandingPixel.Y - 32);
                                    t.getLastFarmerToUse().mailReceived.Add("GotWoodcuttingBook");
                                }
                            }
                            Utility.trySpawnRareObject(Game1.player, new Vector2(tileLocation.X, tileLocation.Y - 3f) * 64f, location, 0.33, 1.0, Game1.player.StandingPixel.Y - 32);
                        }
                    }
                    else if (explosion <= 0)
                    {
                        __result = false;
                        return false;
                    }
                    __instance.shake(tileLocation, doEvenIfStillShaking: true);
                    float damage;
                    if (explosion > 0) return true;
                    else
                    {
                        if (t is null)
                        {
                            __result = false;
                            return false;
                        }
                        damage = t.UpgradeLevel switch
                        {
                            0 => 1f,
                            1 => 1.25f,
                            2 => 1.67f,
                            3 => 2.5f,
                            4 => 5f,
                            _ => (int)t.UpgradeLevel + 1,
                        };
                    }
                    if (t is Axe && t.hasEnchantmentOfType<ShavingEnchantment>() && Game1.random.NextDouble() <= (double)(damage / 5f))
                    {
                        Debris d = ((__instance.treeType.Value == "12") ? new Debris("(O)259", new Vector2(tileLocation.X * 64f + 32f, (tileLocation.Y - 0.5f) * 64f + 32f), Game1.player.getStandingPosition()) : ((__instance.treeType.Value == "7") ? new Debris("(O)420", new Vector2(tileLocation.X * 64f + 32f, (tileLocation.Y - 0.5f) * 64f + 32f), Game1.player.getStandingPosition()) : ((!(__instance.treeType.Value == "8")) ? new Debris("388", new Vector2(tileLocation.X * 64f + 32f, (tileLocation.Y - 0.5f) * 64f + 32f), Game1.player.getStandingPosition()) : new Debris("(O)709", new Vector2(tileLocation.X * 64f + 32f, (tileLocation.Y - 0.5f) * 64f + 32f), Game1.player.getStandingPosition()))));
                        d.Chunks[0].xVelocity.Value += (float)Game1.random.Next(-10, 11) / 10f;
                        d.chunkFinalYLevel = (int)(tileLocation.Y * 64f + 64f);
                        location.debris.Add(d);
                    }
                    //__instance.health.Value -= damage;
                    if (__instance.health.Value <= 0f && performTreeFall(__instance, t, explosion, tileLocation)) 
                    {
                        __result = true;
                        return false;
                    }
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(Tree), nameof(Tree.tickUpdate))]
        public class Tree_tickUpdate_Patch
        {
            public static bool Prefix(Tree __instance, GameTime time, ref bool __result)
            {
                if (!Config.EnableMod || !__instance.falling.Value || (bool)__instance.destroy.Value || Config.WoodMultiplier == 1f || (__instance.Location.IsFarm && Config.OnlyOnFarm)) return true;

                GameLocation location = __instance.Location;
                __instance.alpha = Math.Min(1f, __instance.alpha + 0.05f);
                Vector2 tileLocation = __instance.Tile;

                if (!leaves.ContainsKey(__instance.Location))
                    leaves.Add(__instance.Location, new Dictionary<Vector2, List<Leaf>>() { { __instance.Tile, new List<Leaf>() } });
                if (!leaves.TryGetValue(__instance.Location, out var dict) || !dict.TryGetValue(__instance.Tile, out var list))
                    dict.Add(__instance.Tile, new List<Leaf>());

                leaves.TryGetValue(__instance.Location, out Dictionary<Vector2, List<Leaf>> trees);
                trees.TryGetValue(__instance.Tile, out List<Leaf> leafs);

                if (__instance.falling.Value)
                {
                    __instance.shakeRotation += (__instance.shakeLeft.Value ? (0f - __instance.maxShake * __instance.maxShake) : (__instance.maxShake * __instance.maxShake));
                    __instance.maxShake += 0.0015339808f;
                    WildTreeData data = __instance.GetData();
                    if (data != null && Game1.random.NextDouble() < 0.01 && __instance.IsLeafy())
                    {
                        location.localSound("leafrustle");
                    }
                    if ((double)Math.Abs(__instance.shakeRotation) > Math.PI / 2.0)
                    {
                        __instance.falling.Value = false;
                        __instance.maxShake = 0f;
                        if (data != null)
                        {
                            location.localSound("treethud");
                            if (__instance.IsLeafy())
                            {
                                int leavesToAdd = Game1.random.Next(90, 120);
                                for (int j = 0; j < leavesToAdd; j++)
                                {
                                    leafs.Add(new Leaf(new Vector2(Game1.random.Next((int)(tileLocation.X * 64f), (int)(tileLocation.X * 64f + 192f)) + (__instance.shakeLeft.Value ? (-320) : 256), tileLocation.Y * 64f - 64f), (float)Game1.random.Next(-10, 10) / 100f, Game1.random.Next(4), (float)Game1.random.Next(10, 40) / 10f));
                                }
                            }
                            Random r;
                            if (Game1.IsMultiplayer)
                            {
                                Game1.recentMultiplayerRandom = Utility.CreateRandom((double)tileLocation.X * 1000.0, tileLocation.Y);
                                r = Game1.recentMultiplayerRandom;
                            }
                            else
                            {
                                r = Utility.CreateRandom(Game1.uniqueIDForThisGame, Game1.stats.DaysPlayed, (double)tileLocation.X * 7.0, (double)tileLocation.Y * 11.0);
                            }
                            if (data.DropWoodOnChop)
                            {
                                int numToDrop = (int)((Game1.getFarmer(__instance.lastPlayerToHit.Value).professions.Contains(12) ? 1.25 : 1.0) * (double)((12 + extraWoodCalculator(tileLocation)) * Config.WoodMultiplier));
                                if (Game1.getFarmer(__instance.lastPlayerToHit.Value).stats.Get("Book_Woodcutting") != 0 && r.NextDouble() < 0.05)
                                {
                                    numToDrop *= 2;
                                }
                                Game1.createRadialDebris(location, 12, (int)tileLocation.X + (__instance.shakeLeft.Value ? (-4) : 4), (int)tileLocation.Y, numToDrop, resource: true);
                                Game1.createRadialDebris(location, 12, (int)tileLocation.X + (__instance.shakeLeft.Value ? (-4) : 4), (int)tileLocation.Y, (int)((Game1.getFarmer(__instance.lastPlayerToHit.Value).professions.Contains(12) ? 1.25 : 1.0) * (double)(12 + extraWoodCalculator(tileLocation))), resource: false);
                            }
                            Farmer targetFarmer = Game1.getFarmer(__instance.lastPlayerToHit.Value);
                            if (data.DropWoodOnChop)
                            {
                                Game1.createMultipleObjectDebris("(O)92", (int)tileLocation.X + (__instance.shakeLeft.Value ? (-4) : 4), (int)tileLocation.Y, 5, __instance.lastPlayerToHit.Value, location);
                            }
                            int numHardwood = 0;
                            if (data.DropHardwoodOnLumberChop && targetFarmer != null)
                            {
                                while (targetFarmer.professions.Contains(14) && r.NextBool())
                                {
                                    numHardwood++;
                                }
                            }
                            List<WildTreeChopItemData> chopItems = data.ChopItems;
                            if (chopItems != null && chopItems.Count > 0 && targetFarmer is not null)
                            {
                                bool addedAdditionalHardwood = false;
                                foreach (WildTreeChopItemData drop in data.ChopItems)
                                {
                                    Item item = TryGetDrop(__instance, drop, r, targetFarmer, "ChopItems", null, false);
                                    if (item != null)
                                    {
                                        if (drop.ItemId == "709")
                                        {
                                            numHardwood += item.Stack;
                                            addedAdditionalHardwood = true;
                                        }
                                        else
                                        {
                                            Game1.createMultipleItemDebris(item, new Vector2(tileLocation.X + (float)(__instance.shakeLeft.Value ? (-4) : 4), tileLocation.Y) * 64f, -2, location);
                                        }
                                    }
                                }
                                if (addedAdditionalHardwood && targetFarmer != null && targetFarmer.professions.Contains(14))
                                {
                                    numHardwood += (int)((float)numHardwood * 0.25f + 0.9f);
                                }
                            }
                            if (numHardwood > 0)
                            {
                                Game1.createMultipleObjectDebris("(O)709", (int)tileLocation.X + (__instance.shakeLeft.Value ? (-4) : 4), (int)tileLocation.Y, numHardwood, __instance.lastPlayerToHit.Value, location);
                            }
                            float seedOnChopChance = data.SeedOnChopChance;
                            if (Game1.getFarmer(__instance.lastPlayerToHit.Value).getEffectiveSkillLevel(2) >= 1 && data != null && data.SeedItemId != null && r.NextDouble() < (double)seedOnChopChance)
                            {
                                Game1.createMultipleObjectDebris(data.SeedItemId, (int)tileLocation.X + (__instance.shakeLeft.Value ? (-4) : 4), (int)tileLocation.Y, r.Next(1, 3), __instance.lastPlayerToHit.Value, location);
                            }
                        }
                        if (__instance.health.Value == -100f)
                        {
                            __result = true;
                            return false;
                        }
                        if (__instance.health.Value <= 0f)
                        {
                            __instance.health.Value = -100f;
                        }
                    }
                }
                for (int i = leafs.Count - 1; i >= 0; i--)
                {
                    Leaf leaf = leafs[i];
                    leaf.position.Y -= leaf.yVelocity - 3f;
                    leaf.yVelocity = Math.Max(0f, leaf.yVelocity - 0.01f);
                    leaf.rotation += leaf.rotationRate;
                    if (leaf.position.Y >= tileLocation.Y * 64f + 64f)
                    {
                        leafs.RemoveAt(i);
                    }
                }
                return true;
            }
        }
    }
}
