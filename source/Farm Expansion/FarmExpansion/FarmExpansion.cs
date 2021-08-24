/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AdvizeGH/FarmExpansion
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xTile;
using xTile.Dimensions;
using Object = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using FarmExpansion.Framework;
using StardewValley.Locations;
using Netcode;
using StardewValley.Network;

namespace FarmExpansion
{
    
    public class FarmExpansion : Farm
    {

        [System.Xml.Serialization.XmlIgnore]
        private FEFramework framework;

        public FarmExpansion()
        {
        }

        public FarmExpansion(Map m, string name, FEFramework framework) : base(/*"FarmExpansion", name*/)
        {
            this.framework = framework;
            this.Map = m;
            this.name.Value = name;
        }

        public override void DayUpdate(int dayOfMonth)
        {
            {
                // skip Farm.dayUpdate, call BuildableGameLocation.dayUpdate and anything further
                // up the chain of inheritance.
                // Don't try this at home, kids, this is ultimate example of how NOT to do things.
                // thanks to https://stackoverflow.com/a/32562464 for this ugly workaround.
                var ptr = typeof(BuildableGameLocation).GetMethod("DayUpdate").MethodHandle.GetFunctionPointer();
                var grandparentDayUpdate = (Action<int>)Activator.CreateInstance(typeof(Action<int>), this, ptr);
                grandparentDayUpdate(dayOfMonth);
            }

            for (int index = this.animals.Count() - 1; index >= 0; --index)
                this.animals.Pairs.ElementAt<KeyValuePair<long, FarmAnimal>>(index).Value.dayUpdate((GameLocation)this);
            /*if (Game1.whichFarm == 4 && !Game1.player.mailReceived.Contains("henchmanGone"))
            {
                Game1.spawnMonstersAtNight = true;
            }*/
            this.lastItemShipped = null;
            for (int index = this.characters.Count - 1; index >= 0; --index)
            {
                if (this.characters[index] is JunimoHarvester)
                    this.characters.RemoveAt(index);
            }
            for (int index = this.characters.Count - 1; index >= 0; --index)
            {
                if (this.characters[index] is Monster && (this.characters[index] as Monster).wildernessFarmMonster)
                    this.characters.RemoveAt(index);
            }
            if (this.characters.Count > 5)
            {
                int num = 0;
                for (int index = this.characters.Count - 1; index >= 0; --index)
                {
                    if (this.characters[index] is GreenSlime && Game1.random.NextDouble() < 0.035)
                    {
                        this.characters.RemoveAt(index);
                        ++num;
                    }
                }
                if (num > 0)
                    Game1.showGlobalMessage(Game1.content.LoadString(num == 1 ? "Strings\\Locations:Farm_1SlimeEscaped" : "Strings\\Locations:Farm_NSlimesEscaped", (object)num));
            }
            // stuff for handling specific Game1.whichFarm (hardwood stump/geode spawning) - ommited, doesn't concern expansion

            ICollection<Vector2> source = (ICollection<Vector2>)new List<Vector2>(this.terrainFeatures.Keys);
            for (int index = source.Count - 1; index >= 0; --index)
            {
                if (this.terrainFeatures[source.ElementAt<Vector2>(index)] is HoeDirt && (this.terrainFeatures[source.ElementAt<Vector2>(index)] as HoeDirt).crop == null && Game1.random.NextDouble() <= 0.1)
                    this.terrainFeatures.Remove(source.ElementAt<Vector2>(index));
            }
            if (this.terrainFeatures.Count() > 0 && Game1.currentSeason.Equals("fall") && (Game1.dayOfMonth > 1 && Game1.random.NextDouble() < 0.05))
            {
                for (int index = 0; index < 10; ++index)
                {
                    TerrainFeature terrainFeature = this.terrainFeatures.Pairs.ElementAt<KeyValuePair<Vector2, TerrainFeature>>(Game1.random.Next(this.terrainFeatures.Count())).Value;
                    if (terrainFeature is Tree && (int)((NetFieldBase<int, NetInt>)(terrainFeature as Tree).growthStage) >= 5 && !(bool)((NetFieldBase<bool, NetBool>)(terrainFeature as Tree).tapped))
                    {
                        (terrainFeature as Tree).treeType.Value = 7;
                        (terrainFeature as Tree).loadSprite();
                        break;
                    }
                }
            }
            if (this.framework.config.enableCrows)
            {
                this.addCrows();
            }
            if (!Game1.currentSeason.Equals("winter"))
            {
                spawnWeedsAndStones(Game1.currentSeason.Equals("summer") ? 30 : 20, false, true);
            }
            spawnWeeds(false);
            if (dayOfMonth == 1)
            {
                for (int index = this.terrainFeatures.Count() - 1; index >= 0; --index)
                {
                    KeyValuePair<Vector2, TerrainFeature> keyValuePair = this.terrainFeatures.Pairs.ElementAt<KeyValuePair<Vector2, TerrainFeature>>(index);
                    if (keyValuePair.Value is HoeDirt)
                    {
                        keyValuePair = this.terrainFeatures.Pairs.ElementAt<KeyValuePair<Vector2, TerrainFeature>>(index);
                        if ((keyValuePair.Value as HoeDirt).crop == null && Game1.random.NextDouble() < 0.8)
                        {
                            NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>> terrainFeatures = this.terrainFeatures;
                            keyValuePair = this.terrainFeatures.Pairs.ElementAt<KeyValuePair<Vector2, TerrainFeature>>(index);
                            Vector2 key = keyValuePair.Key;
                            terrainFeatures.Remove(key);
                        }
                    }
                }
                spawnWeedsAndStones(20, false, false);
                if (Game1.currentSeason.Equals("spring") && Game1.stats.DaysPlayed > 1U)
                {
                    spawnWeedsAndStones(40, false, false);
                    spawnWeedsAndStones(40, true, false);
                    for (int index = 0; index < 15; ++index)
                    {
                        int xTile = Game1.random.Next(this.map.DisplayWidth / 64);
                        int yTile = Game1.random.Next(this.map.DisplayHeight / 64);
                        Vector2 vector2 = new Vector2((float)xTile, (float)yTile);
                        Object @object;
                        this.objects.TryGetValue(vector2, out @object);
                        if (@object == null && this.doesTileHaveProperty(xTile, yTile, "Diggable", "Back") != null && (this.isTileLocationOpen(new Location(xTile * 64, yTile * 64)) && !this.isTileOccupied(vector2, "")) && this.doesTileHaveProperty(xTile, yTile, "Water", "Back") == null)
                            this.terrainFeatures.Add(vector2, (TerrainFeature)new Grass(1, 4));
                    }
                    this.growWeedGrass(40);
                }
            }
            base.growWeedGrass(1);
        }

        // not needed, does exactly the same as GameLocation.spawnWeedsAndStones
        //new public void spawnWeedsAndStones(int numDebris = -1, bool weedsOnly = false, bool spawnFromOldWeeds = true)
        //{
        //    if ((this as Farm).isBuildingConstructed("Gold Clock"))
        //    {
        //        return;
        //    }
        //    if (!Game1.currentSeason.Equals("winter"))
        //    {
        //        int num = (numDebris != -1) ? numDebris : ((Game1.random.NextDouble() < 0.95) ? ((Game1.random.NextDouble() < 0.25) ? Game1.random.Next(10, 21) : Game1.random.Next(5, 11)) : 0);
        //        if (Game1.isRaining)
        //        {
        //            num *= 2;
        //        }
        //        if (Game1.dayOfMonth == 1)
        //        {
        //            num *= 5;
        //        }
        //        if (this.objects.Count() <= 0 & spawnFromOldWeeds)
        //        {
        //            return;
        //        }
        //        for (int i = 0; i < num; i++)
        //        {
        //            Vector2 vector = spawnFromOldWeeds ? new Vector2((float)Game1.random.Next(-1, 2), (float)Game1.random.Next(-1, 2)) : new Vector2((float)Game1.random.Next(this.map.Layers[0].LayerWidth), (float)Game1.random.Next(this.map.Layers[0].LayerHeight));
        //            while (spawnFromOldWeeds && vector.Equals(Vector2.Zero))
        //            {
        //                vector = new Vector2((float)Game1.random.Next(-1, 2), (float)Game1.random.Next(-1, 2));
        //            }
        //            KeyValuePair<Vector2, Object> keyValuePair = new KeyValuePair<Vector2, Object>(Vector2.Zero, null);
        //            if (spawnFromOldWeeds)
        //            {
        //                keyValuePair = this.objects.Pairs.ElementAt(Game1.random.Next(this.objects.Count()));
        //            }
        //            Vector2 vector2 = spawnFromOldWeeds ? keyValuePair.Key : Vector2.Zero;
        //            if (((this.doesTileHaveProperty((int)(vector.X + vector2.X), (int)(vector.Y + vector2.Y), "Diggable", "Back") != null) || (this.doesTileHaveProperty((int)(vector.X + vector2.X), (int)(vector.Y + vector2.Y), "Diggable", "Back") == null)) && (this.doesTileHaveProperty((int)(vector.X + vector2.X), (int)(vector.Y + vector2.Y), "Type", "Back") == null || !this.doesTileHaveProperty((int)(vector.X + vector2.X), (int)(vector.Y + vector2.Y), "Type", "Back").Equals("Wood")) && (this.isTileLocationTotallyClearAndPlaceable(vector + vector2) || (spawnFromOldWeeds && ((this.objects.ContainsKey(vector + vector2) && this.objects[vector + vector2].ParentSheetIndex != 105) || (this.terrainFeatures.ContainsKey(vector + vector2) && (this.terrainFeatures[vector + vector2] is HoeDirt || this.terrainFeatures[vector + vector2] is Flooring))))) && this.doesTileHaveProperty((int)(vector.X + vector2.X), (int)(vector.Y + vector2.Y), "NoSpawn", "Back") == null && (spawnFromOldWeeds || !this.objects.ContainsKey(vector + vector2)))
        //            {
        //                int parentSheetIndex = -1;
                        
        //                if (Game1.random.NextDouble() < 0.5 && !weedsOnly && (!spawnFromOldWeeds || keyValuePair.Value.Name.Equals("Stone") || keyValuePair.Value.Name.Contains("Twig")))
        //                {
        //                    if (Game1.random.NextDouble() < 0.5)
        //                    {
        //                        parentSheetIndex = ((Game1.random.NextDouble() < 0.5) ? 294 : 295);
        //                    }
        //                    else
        //                    {
        //                        parentSheetIndex = ((Game1.random.NextDouble() < 0.5) ? 343 : 450);
        //                    }
        //                }
        //                else if (!spawnFromOldWeeds || keyValuePair.Value.Name.Contains("Weed"))
        //                {
        //                    parentSheetIndex = GameLocation.getWeedForSeason(Game1.random, Game1.currentSeason);
        //                }
        //                if (!spawnFromOldWeeds && Game1.random.NextDouble() < 0.05)
        //                {
        //                    this.terrainFeatures.Add(vector + vector2, new Tree(Game1.random.Next(3) + 1, Game1.random.Next(3)));
        //                    continue;
        //                }
        //                if (parentSheetIndex != -1)
        //                {
        //                    bool flag2 = false;
        //                    if (this.objects.ContainsKey(vector + vector2))
        //                    {
        //                        Object @object = this.objects[vector + vector2];
        //                        if (@object is Fence || @object is Chest)
        //                        {
        //                            continue;
        //                        }
        //                        if (@object.name != null && !@object.Name.Contains("Weed") && !@object.Name.Equals("Stone") && !@object.name.Contains("Twig") && @object.name.Length > 0)
        //                        {
        //                            flag2 = true;
        //                            Game1.debugOutput = @object.Name + " was destroyed";
        //                        }
        //                        this.objects.Remove(vector + vector2);
        //                    }
        //                    else if (this.terrainFeatures.ContainsKey(vector + vector2))
        //                    {
        //                        try
        //                        {
        //                            flag2 = (this.terrainFeatures[vector + vector2] is HoeDirt || this.terrainFeatures[vector + vector2] is Flooring);
        //                        }
        //                        catch (Exception)
        //                        {
        //                        }
        //                        if (!flag2)
        //                        {
        //                            return;
        //                        }
        //                        this.terrainFeatures.Remove(vector + vector2);
        //                    }
        //                    if (flag2)
        //                    {
        //                        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Locations:Farm_WeedsDestruction", new object[0]));
        //                    }
        //                    this.objects.Add(vector + vector2, new Object(vector + vector2, parentSheetIndex, 1));
        //                }
        //            }
        //        }
        //    }
        //}

        //new public void spawnWeeds(bool weedsOnly)
        // shouldn't be needed (didn't check too thoroughly though)
        //{
        //    int num = Game1.random.Next(5, 12);
        //    if (Game1.dayOfMonth == 1 && Game1.currentSeason.Equals("spring"))
        //    {
        //        num *= 15;
        //    }
        //    for (int i = 0; i < num; i++)
        //    {
        //        for (int j = 0; j < 3; j++)
        //        {
        //            int num2 = Game1.random.Next(this.map.DisplayWidth / Game1.tileSize);
        //            int num3 = Game1.random.Next(this.map.DisplayHeight / Game1.tileSize);
        //            Vector2 vector = new Vector2((float)num2, (float)num3);
        //            Object @object;
        //            this.objects.TryGetValue(vector, out @object);
        //            int num4 = -1;
        //            int num5 = -1;
        //            if (Game1.random.NextDouble() < 0.15 + (weedsOnly ? 0.05 : 0.0))
        //            {
        //                num4 = 1;
        //            }
        //            else if (!weedsOnly && Game1.random.NextDouble() < 0.35)
        //            {
        //                num5 = 1;
        //            }
        //            if (num5 != -1)
        //            {
        //                if (Game1.random.NextDouble() < 0.25)
        //                {
        //                    return;
        //                }
        //            }
        //            else if (@object == null && this.doesTileHaveProperty(num2, num3, "Diggable", "Back") != null && this.isTileLocationOpen(new Location(num2 * Game1.tileSize, num3 * Game1.tileSize)) && !this.isTileOccupied(vector, "") && this.doesTileHaveProperty(num2, num3, "Water", "Back") == null)
        //            {
        //                string text = this.doesTileHaveProperty(num2, num3, "NoSpawn", "Back");
        //                if (text != null && (text.Equals("Grass") || text.Equals("All") || text.Equals("True")))
        //                {
        //                    continue;
        //                }
        //                if (num4 != -1 && !Game1.currentSeason.Equals("winter"))
        //                {
        //                    int numberOfWeeds = Game1.random.Next(1, 3);
        //                    this.terrainFeatures.Add(vector, new Grass(num4, numberOfWeeds));
        //                }
        //            }
        //        }
        //    }
        //}

        public override void draw(SpriteBatch b)
        {
            {
                // skip Farm.draw, call BuildableGameLocation.draw and anything further
                // up the chain of inheritance.
                // Don't try this at home, kids, this is ultimate example of how NOT to do things.
                // thanks to https://stackoverflow.com/a/32562464 for this ugly workaround.
                var ptr = typeof(BuildableGameLocation).GetMethod("draw").MethodHandle.GetFunctionPointer();
                var grandparentDraw = (Action<SpriteBatch>)Activator.CreateInstance(typeof(Action<SpriteBatch>), this, ptr);
                grandparentDraw(b);
            }

            foreach (ResourceClump current in this.resourceClumps)
            {
                current.draw(b, current.tile.Value);
            }
            foreach (KeyValuePair<long, FarmAnimal> current in this.animals.Pairs)
            {
                current.Value.draw(b);
            }
            if (Game1.player.currentUpgrade != null && Game1.player.currentUpgrade.daysLeftTillUpgradeDone <= 3)
            {
                b.Draw(Game1.player.currentUpgrade.workerTexture, Game1.GlobalToLocal(Game1.viewport, Game1.player.currentUpgrade.positionOfCarpenter), new Rectangle?(Game1.player.currentUpgrade.getSourceRectangle()), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, (Game1.player.currentUpgrade.positionOfCarpenter.Y + (float)(Game1.tileSize * 3 / 4)) / 10000f);
            }
            /*b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(3776f, 1088f)), new Rectangle?(Building.leftShadow), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1E-05f);
            for (int i = 1; i < 8; i++)
            {
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(3776 + i * 64), 1088f)), new Rectangle?(Building.middleShadow), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1E-05f);
            }
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(4288f, 1088f)), new Rectangle?(Building.rightShadow), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1E-05f);
            b.Draw(this.houseTextures, Game1.GlobalToLocal(Game1.viewport, new Vector2(3712f, 520f)), new Rectangle?(this.houseSource), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.075f);
            b.Draw(this.houseTextures, Game1.GlobalToLocal(Game1.viewport, new Vector2(1600f, 384f)), new Rectangle?(this.greenhouseSource), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0704f);
            if (Game1.mailbox.Count > 0)
            {
                float num = 4f * (float)Math.Round(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 250.0), 2);
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(68 * Game1.tileSize), (float)(16 * Game1.tileSize - Game1.tileSize * 3 / 2 - 48) + num)), new Rectangle?(new Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(17 * Game1.tileSize) / 10000f + 1E-06f + 0.0068f);
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(68 * Game1.tileSize + Game1.tileSize / 2 + Game1.pixelZoom), (float)(16 * Game1.tileSize - Game1.tileSize - 24 - Game1.tileSize / 8) + num)), new Rectangle?(new Rectangle(189, 423, 15, 13)), Color.White, 0f, new Vector2(7f, 6f), 4f, SpriteEffects.None, (float)(17 * Game1.tileSize) / 10000f + 1E-05f + 0.0068f);
            }
            if (this.shippingBinLid != null)
            {
                this.shippingBinLid.draw(b, false, 0, 0);
            }
            if (!this.hasSeenGrandpaNote)
            {
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(9 * Game1.tileSize), (float)(7 * Game1.tileSize))), new Rectangle?(new Rectangle(575, 1972, 11, 8)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(7 * Game1.tileSize) / 10000f + 1E-06f);
            }*/
        }

        public override void UpdateWhenCurrentLocation(GameTime time)
        {
            if (wasUpdated && Game1.gameMode != 0)
            {
                return;
            }
            if (Game1.player.currentUpgrade != null && Game1.currentLocation.Name.Equals("FarmExpansion") && Game1.player.currentUpgrade.daysLeftTillUpgradeDone <= 3)
            {
                Game1.player.currentUpgrade.update((float)time.ElapsedGameTime.Milliseconds);
            }
            base.UpdateWhenCurrentLocation(time);
            fixAnimalLocation();
            if (Game1.player.currentUpgrade != null && Game1.player.currentUpgrade.daysLeftTillUpgradeDone <= 3)
            {
                Game1.player.currentUpgrade.update((float)time.ElapsedGameTime.Milliseconds);
            }
        }

        public override void updateEvenIfFarmerIsntHere(GameTime time, bool skipWasUpdatedFlush = false)
        {
            base.updateEvenIfFarmerIsntHere(time, skipWasUpdatedFlush);
            fixAnimalLocation();
        }

        public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, StardewValley.Farmer who)
        {
            Vector2 vector = new Vector2((float)tileLocation.X, (float)tileLocation.Y);
            if (this.objects.ContainsKey(vector) && this.objects[vector].Type != null)
            {
                if ((this.objects[vector].Type.Equals("Crafting") || this.objects[vector].Type.Equals("interactive")) && this.objects[vector].name.Equals("Bee House"))
                {
                    Object beeHive = this.objects[vector];
                    if (!beeHive.readyForHarvest.Value)
                    {
                        return false;
                    }
                    beeHive.honeyType.Value = new Object.HoneyType?(Object.HoneyType.Wild);
                    string str = "Wild";
                    int num6 = 0;
                        
                    Crop crop = this.findCloseFlower(beeHive.TileLocation);
                    if (crop != null)
                    {
                        str = Game1.objectInformation[crop.indexOfHarvest.Value].Split(new char[]
                        {
                            '/'
                        })[0];
                        int indexOfHarvest = crop.indexOfHarvest.Value;
                        if (indexOfHarvest != 376)
                        {
                            switch (indexOfHarvest)
                            {
                                case 591:
                                    beeHive.honeyType.Value = new Object.HoneyType?(Object.HoneyType.Tulip);
                                    break;
                                case 593:
                                    beeHive.honeyType.Value = new Object.HoneyType?(Object.HoneyType.SummerSpangle);
                                    break;
                                case 595:
                                    beeHive.honeyType.Value = new Object.HoneyType?(Object.HoneyType.FairyRose);
                                    break;
                                case 597:
                                    beeHive.honeyType.Value = new Object.HoneyType?(Object.HoneyType.BlueJazz);
                                    break;
                            }
                        }
                        else
                        {
                            beeHive.honeyType.Value = new Object.HoneyType?(Object.HoneyType.Poppy);
                        }
                        num6 = Convert.ToInt32(Game1.objectInformation[crop.indexOfHarvest.Value].Split(new char[]
                        {
                            '/'
                        })[1]) * 2;
                    }
                    if (beeHive.heldObject.Value != null)
                    {
                        beeHive.heldObject.Value.name = str + " Honey";
                        string displayName = framework.helper.Reflection.GetMethod(beeHive, "loadDisplayName").Invoke<string>();
                        beeHive.heldObject.Value.displayName = displayName;
                        beeHive.heldObject.Value.Price += num6;
                        if (Game1.currentSeason.Equals("winter"))
                        {
                            beeHive.heldObject.Value = null;
                            beeHive.readyForHarvest.Value = false;
                            beeHive.showNextIndex.Value = false;
                            return false;
                        }
                        if (who.IsMainPlayer && !who.addItemToInventoryBool(beeHive.heldObject.Value, false))
                        {
                            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588", new object[0]));
                            return false;
                        }
                        Game1.playSound("coin");
                    }
                    beeHive.readyForHarvest.Value = false;
                    beeHive.showNextIndex.Value = false;
                    if (!Game1.currentSeason.Equals("winter"))
                    {
                        beeHive.heldObject.Value = new Object(Vector2.Zero, 340, null, false, true, false, false);
                        beeHive.MinutesUntilReady = 2400 - Game1.timeOfDay + 4320;
                    }
                    return true;
                }
            }

            return base.checkAction(tileLocation, viewport, who);
        }

        private void fixAnimalLocation()
        {
            foreach (FarmAnimal animal in Game1.getFarm().animals.Values.ToList())
            {
                if (buildings.Contains(animal.home))
                {
                    Game1.getFarm().animals.Remove(animal.myID.Value);
                    animals.Add(animal.myID.Value, animal);
                }
            }
            foreach (Building building in buildings)
            {
                if (building.indoors.Value == null)
                    continue;

                if (building.indoors.Value is AnimalHouse)
                {
                    List<FarmAnimal> stuckAnimals = new List<FarmAnimal>();
                    foreach (FarmAnimal animal in ((AnimalHouse)building.indoors.Value).animals.Values)
                    {
                        if (Game1.getFarm().isCollidingPosition(
                                new Rectangle(
                                    (building.tileX.Value + building.animalDoor.X) * Game1.tileSize + 2,
                                    (building.tileY.Value + building.animalDoor.Y) * Game1.tileSize + 2,
                                    (animal.isCoopDweller() ? Game1.tileSize : (Game1.tileSize * 2)) - 4,
                                    Game1.tileSize - 4),
                                Game1.viewport, false, 0, false, animal, false, false, false
                            )
                            || Game1.getFarm().isCollidingPosition(
                                new Rectangle(
                                    (building.tileX.Value + building.animalDoor.X) * Game1.tileSize + 2,
                                    (building.tileY.Value + building.animalDoor.Y + 1) * Game1.tileSize + 2,
                                    (animal.isCoopDweller() ? Game1.tileSize : (Game1.tileSize * 2)) - 4,
                                    Game1.tileSize - 4),
                                Game1.viewport, false, 0, false, animal, false, false, false))
                        {
                            if (Game1.random.NextDouble() < 0.002
                                && building.animalDoorOpen.Value
                                && Game1.timeOfDay < 1630
                                && !Game1.isRaining
                                && !Game1.currentSeason.Equals("winter")
                                && building.indoors.Value.farmers.Count() == 0
                                && !building.indoors.Equals(Game1.currentLocation))
                            {
                                if (isCollidingPosition(new Rectangle((building.tileX.Value + building.animalDoor.X) * Game1.tileSize + 2, (building.tileY.Value + building.animalDoor.Y) * Game1.tileSize + 2, (animal.isCoopDweller() ? Game1.tileSize : (Game1.tileSize * 2)) - 4, Game1.tileSize - 4), Game1.viewport, false, 0, false, animal, false, false, false) || isCollidingPosition(new Rectangle((building.tileX.Value + building.animalDoor.X) * Game1.tileSize + 2, (building.tileY.Value + building.animalDoor.Y + 1) * Game1.tileSize + 2, (animal.isCoopDweller() ? Game1.tileSize : (Game1.tileSize * 2)) - 4, Game1.tileSize - 4), Game1.viewport, false, 0, false, animal, false, false, false))
                                {
                                    break;
                                }
                                else
                                {
                                    stuckAnimals.Add(animal);
                                }
                            }
                        }
                    }
                    foreach (FarmAnimal localBuildingAnimal in stuckAnimals)
                    {
                        if (localBuildingAnimal.noWarpTimer <= 0)
                        {
                            localBuildingAnimal.noWarpTimer = 9000;
                            ((AnimalHouse)building.indoors.Value).animals.Remove(localBuildingAnimal.myID.Value);
                            building.currentOccupants.Value--;
                            animals.Add(localBuildingAnimal.myID.Value, localBuildingAnimal);
                            localBuildingAnimal.faceDirection(2);
                            localBuildingAnimal.SetMovingDown(true);
                            localBuildingAnimal.position.Value = new Vector2(building.getRectForAnimalDoor().X, (building.tileY.Value + building.animalDoor.Y) * Game1.tileSize - (localBuildingAnimal.Sprite.getHeight() * Game1.pixelZoom - localBuildingAnimal.GetBoundingBox().Height) + Game1.tileSize / 2);
                        }
                    }
                }
            }
            //Gets animals back inside
            foreach (FarmAnimal localAnimal in animals.Values.ToList())
            {
                if (localAnimal.noWarpTimer <= 0 && localAnimal.home != null && localAnimal.home.getRectForAnimalDoor().Contains(localAnimal.GetBoundingBox().Center.X, localAnimal.GetBoundingBox().Top))
                {
                    if (Utility.isOnScreen(localAnimal.getTileLocationPoint(), Game1.tileSize * 3, this))
                    {
                        Game1.playSound("dwoop");
                    }
                    localAnimal.noWarpTimer = 3000;
                    localAnimal.home.currentOccupants.Value++;
                    animals.Remove(localAnimal.myID.Value);
                    ((AnimalHouse)localAnimal.home.indoors.Value).animals.Add(localAnimal.myID.Value, localAnimal);

                    localAnimal.setRandomPosition(localAnimal.home.indoors.Value);
                    localAnimal.faceDirection(Game1.random.Next(4));
                    localAnimal.controller = null;
                }
            }
        }

        internal void removeCarpenter()
        {
            if (!isThereABuildingUnderConstruction())
            {
                List<NPC> npcsToRemove = new List<NPC>();
                foreach (Building building in buildings)
                {
                    npcsToRemove.Clear();
                    if (building.indoors.Value != null)
                    {
                        foreach (NPC npc in building.indoors.Value.characters)
                        {
                            if (npc.Name.Equals("Robin"))
                            {
                                npcsToRemove.Add(npc);
                            }
                        }
                        foreach (NPC carpenter in npcsToRemove)
                        {
                            building.indoors.Value.characters.Remove(carpenter);
                        }
                    }
                }
            }
        }

        private Crop findCloseFlower(Vector2 startTileLocation)
        {
            Queue<Vector2> queue = new Queue<Vector2>();
            HashSet<Vector2> hashSet = new HashSet<Vector2>();
            queue.Enqueue(startTileLocation);
            int num = 0;
            while (num <= 150 && queue.Count > 0)
            {
                Vector2 vector = queue.Dequeue();
                if (this.terrainFeatures.ContainsKey(vector) && this.terrainFeatures[vector] is HoeDirt && (this.terrainFeatures[vector] as HoeDirt).crop != null && (this.terrainFeatures[vector] as HoeDirt).crop.programColored.Value && (this.terrainFeatures[vector] as HoeDirt).crop.currentPhase.Value >= (this.terrainFeatures[vector] as HoeDirt).crop.phaseDays.Count - 1 && !(this.terrainFeatures[vector] as HoeDirt).crop.dead.Value)
                {
                    return (this.terrainFeatures[vector] as HoeDirt).crop;
                }
                foreach (Vector2 current in Utility.getAdjacentTileLocations(vector))
                {
                    if (!hashSet.Contains(current))
                    {
                        queue.Enqueue(current);
                    }
                }
                hashSet.Add(vector);
                num++;
            }
            return null;
        }

    }
}
