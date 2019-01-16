using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace CustomFarmTypes
{
    class CustomFarm : Farm
    {
        private static void swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public readonly FarmType type;

        public static void swapFarms( Farm a, Farm b )
        {
            swap(ref a.characters, ref b.characters);
            swap(ref a.objects, ref b.objects);
            swap(ref a.temporarySprites, ref b.temporarySprites);
            swap(ref a.farmers, ref b.farmers);
            swap(ref a.projectiles, ref b.projectiles);
            swap(ref a.terrainFeatures, ref b.terrainFeatures);
            swap(ref a.debris, ref b.debris);
            swap(ref a.buildings, ref b.buildings);
            swap(ref a.animals, ref b.animals);
            swap(ref a.resourceClumps, ref b.resourceClumps);
            swap(ref a.piecesOfHay, ref b.piecesOfHay);
            swap(ref a.grandpaScore, ref b.grandpaScore);
            swap(ref a.hasSeenGrandpaNote, ref b.hasSeenGrandpaNote);

            foreach (var c in a.characters)
                c.currentLocation = a;
            foreach (var f in a.farmers)
                f.currentLocation = a;
            foreach (var c in b.characters)
                c.currentLocation = b;
            foreach (var f in b.farmers)
                f.currentLocation = b;
        }

        public CustomFarm( FarmType theType, string locationName )
            : base(theType.loadMap(), locationName )
        {
            type = theType;
        }

        public override void DayUpdate(int dayOfMonth)
        {
            Log.debug("Custom farm " + type.ID + " DayUpdate");
            base.DayUpdate(dayOfMonth);

            if (type.Behavior.RepopulateStumps)
            {
                for (int x = 0; x < this.map.Layers[0].LayerWidth; ++x)
                {
                    for (int y = 0; y < this.map.Layers[0].LayerHeight; ++y)
                    {
                        if (this.map.GetLayer("Paths").Tiles[x, y] != null && this.map.GetLayer("Paths").Tiles[x, y].TileIndex == 21 && (this.isTileLocationTotallyClearAndPlaceable(x, y) && this.isTileLocationTotallyClearAndPlaceable(x + 1, y)) && (this.isTileLocationTotallyClearAndPlaceable(x + 1, y + 1) && this.isTileLocationTotallyClearAndPlaceable(x, y + 1)))
                            this.resourceClumps.Add(new ResourceClump(600, 2, 2, new Vector2((float)x, (float)y)));
                    }
                }
            }
            
            if ( type.Behavior.ForageableSpawnBehavior != null && type.Behavior.ForageableSpawnBehavior.ContainsKey(Game1.currentSeason))
            {
                double d = type.Behavior.ForageableSpawnChanceBase;
                while (Game1.random.NextDouble() < d)
                {
                    var area = FarmType.FarmBehavior.chooseSpawnArea(type.Behavior.ForageableSpawnBehavior[Game1.currentSeason]);
                    if (area != null)
                    {
                        var entry = FarmType.FarmBehavior.SpawnBehaviorEntry.chooseEntry(area.Entries);
                        if (entry != null)
                        {
                            var obj = entry.getObject();
                            obj.CanBeSetDown = false;
                            if (obj.ParentSheetIndex < 75 || obj.ParentSheetIndex > 77)
                                obj.IsSpawnedObject = true;
                            obj.TileLocation = new Vector2(area.Area.x + Game1.random.Next(area.Area.w), area.Area.y + Game1.random.Next(area.Area.h));
                            obj.getBoundingBox(obj.TileLocation);
                            dropObject(obj, new Vector2(obj.TileLocation.X * Game1.tileSize, obj.TileLocation.Y * Game1.tileSize), Game1.viewport, true, null);
                            if ( !entry.SkipChanceDecrease)
                                d *= type.Behavior.ForageableSpawnChanceMultiplier;
                            continue;
                        }
                    }
                    d *= type.Behavior.ForageableSpawnChanceMultiplier;
                }
            }

            if (type.Behavior.SpecialWeedCount > 0 && !Game1.IsWinter)
            {
                if (this.Objects.Any())
                {
                    for (int index = 0; index < type.Behavior.SpecialWeedCount; ++index)
                    {
                        SObject @object = this.objects.ElementAt(Game1.random.Next(this.objects.Count)).Value;
                        if (@object.name.Equals("Weeds"))
                            @object.ParentSheetIndex = 792 + Utility.getSeasonNumber(Game1.currentSeason);
                    }
                }
            }

            doOreSpawns();
            
            if (type.Behavior.SpawnMonsters && !Game1.player.mailReceived.Contains("henchmanGone"))
                Game1.spawnMonstersAtNight = true;
        }

        public override int getFishingLocation(Vector2 pos)
        {
            foreach (var pool in type.Behavior.FishPoolToDrawFrom)
                if (pool.Area.contains((int)pos.X / Game1.tileSize, (int)pos.Y / Game1.tileSize))
                    return pool.ListFromLocationID;
            return -1;
        }

        public override SObject getFish(float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency, string locationName = null)
        {
            var fishingRod = who.CurrentTool as FishingRod;
            var bobber = Mod.instance.Helper.Reflection.GetField<Vector2>(fishingRod, "bobber").GetValue();
            Point bobberTile = new Point((int)bobber.X / Game1.tileSize, (int)bobber.Y / Game1.tileSize);

            int poolFrom = -1;
            foreach (var poolTest in type.Behavior.FishPoolToDrawFrom)
                if (poolTest.Area.contains((int)bobberTile.X, (int)bobberTile.Y))
                    poolFrom = poolTest.PoolId;

            var pool = type.Behavior.FishPools[poolFrom];
            var entry = FarmType.FarmBehavior.FishPoolEntry.chooseEntry(pool);
            if (entry == null || entry.LocationPreset == null)
                return entry.getObject();

            locationName = entry == null ? null : entry.LocationPreset;
            
            // We're copying the vanilla function because I feel the need to make it operate based
            // on tile the bobber is on, NOT the tile of the player
            int parentSheetIndex = -1;
            Dictionary<string, string> dictionary1 = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
            bool flag1 = false;
            string key = locationName ?? this.Name;
            if (this.Name.Equals("WitchSwamp") && !Game1.player.mailReceived.Contains("henchmanGone") && (Game1.random.NextDouble() < 0.25 && !Game1.player.hasItemInInventory(308, 1, 0)))
                return new SObject(308, 1, false, -1, 0);
            if (dictionary1.ContainsKey(key))
            {
                string[] strArray1 = dictionary1[key].Split('/')[4 + Utility.getSeasonNumber(Game1.currentSeason)].Split(' ');
                Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
                if (strArray1.Length > 1)
                {
                    int index = 0;
                    while (index < strArray1.Length)
                    {
                        dictionary2.Add(strArray1[index], strArray1[index + 1]);
                        index += 2;
                    }
                }
                string[] array = dictionary2.Keys.ToArray<string>();
                Dictionary<int, string> dictionary3 = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
                Utility.Shuffle<string>(Game1.random, array);
                for (int index1 = 0; index1 < array.Length; ++index1)
                {
                    bool flag2 = true;
                    string[] strArray2 = dictionary3[Convert.ToInt32(array[index1])].Split('/');
                    string[] strArray3 = strArray2[5].Split(' ');
                    int int32 = Convert.ToInt32(dictionary2[array[index1]]);
                    if (int32 == -1 || this.getFishingLocation(bobber) == int32)
                    {
                        int index2 = 0;
                        while (index2 < strArray3.Length)
                        {
                            if (Game1.timeOfDay >= Convert.ToInt32(strArray3[index2]) && Game1.timeOfDay < Convert.ToInt32(strArray3[index2 + 1]))
                            {
                                flag2 = false;
                                break;
                            }
                            index2 += 2;
                        }
                    }
                    if (!strArray2[7].Equals("both"))
                    {
                        if (strArray2[7].Equals("rainy") && !Game1.isRaining)
                            flag2 = true;
                        else if (strArray2[7].Equals("sunny") && Game1.isRaining)
                            flag2 = true;
                    }
                    if (who.FishingLevel < Convert.ToInt32(strArray2[12]))
                        flag2 = true;
                    if (!flag2)
                    {
                        double num1 = Convert.ToDouble(strArray2[10]);
                        double num2 = Convert.ToDouble(strArray2[11]) * num1;
                        double num3 = Math.Min(num1 - (double)Math.Max(0, Convert.ToInt32(strArray2[9]) - waterDepth) * num2 + (double)who.FishingLevel / 50.0, 0.899999976158142);
                        if (Game1.random.NextDouble() <= num3)
                        {
                            parentSheetIndex = Convert.ToInt32(array[index1]);
                            break;
                        }
                    }
                }
            }
            if (parentSheetIndex == -1)
                parentSheetIndex = Game1.random.Next(167, 173);
            if ((who.fishCaught == null || who.fishCaught.Count == 0) && parentSheetIndex >= 152)
                parentSheetIndex = 145;
            SObject @object = new SObject(parentSheetIndex, 1, false, -1, 0);
            if (flag1)
                @object.scale.X = 1f;
            return @object;
        }

        public override void performTenMinuteUpdate(int timeOfDay)
        {
            base.performTenMinuteUpdate(timeOfDay);

            Random random = new Random(timeOfDay + (int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed);

            // Fishing
            if (this.fishSplashPoint.Value.Equals(Point.Zero) && random.NextDouble() < type.Behavior.FishingSplashChance)
            {
                for (int index = 0; index < 2; ++index)
                {
                    Point point = new Point(random.Next(0, this.map.GetLayer("Back").LayerWidth), random.Next(0, this.map.GetLayer("Back").LayerHeight));
                    if (this.isOpenWater(point.X, point.Y))
                    {
                        int land = FishingRod.distanceToLand(point.X, point.Y, this);
                        if (land > 1 && land <= 5)
                        {
                            if (Game1.player.currentLocation.Equals((object)this))
                                Game1.playSound("waterSlosh");
                            this.fishSplashPoint.Value = point;
                            this.fishSplashAnimation = new TemporaryAnimatedSprite(51, new Vector2((float)(point.X * Game1.tileSize), (float)(point.Y * Game1.tileSize)), Color.White, 10, false, 80f, 999999, -1, -1f, -1, 0);
                            break;
                        }
                    }
                }
            }
        }

        public void doOreSpawns()
        {
            if (type.Behavior.OreSpawnBehavior == null)
                return;

            double d = type.Behavior.OreSpawnChanceBase;
            while (Game1.random.NextDouble() < d)
            {
                var area = FarmType.FarmBehavior.chooseSpawnArea(type.Behavior.OreSpawnBehavior);
                if (area != null)
                {
                    var entry = FarmType.FarmBehavior.SpawnBehaviorEntry.chooseEntry(area.Entries);
                    if (entry != null)
                    {
                        Vector2 pos = new Vector2(area.Area.x + Game1.random.Next(area.Area.w), area.Area.y + Game1.random.Next(area.Area.h));
                        if (doesTileHavePropertyNoNull((int)pos.X, (int)pos.Y, "Type", "Back").Equals("Dirt") && this.isTileLocationTotallyClearAndPlaceable(pos))
                        {
                            var obj = entry.getObject();
                            obj.TileLocation = pos;
                            obj.getBoundingBox(obj.TileLocation);
                            objects.Add(pos, obj);
                        }
                        if (!entry.SkipChanceDecrease)
                            d *= type.Behavior.OreSpawnChanceMultiplier;
                        continue;
                    }
                }
                d *= type.Behavior.OreSpawnChanceMultiplier;
            }
        }
    }
}
