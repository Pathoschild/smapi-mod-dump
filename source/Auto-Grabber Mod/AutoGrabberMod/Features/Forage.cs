using System;
using System.Collections.Generic;
using System.Linq;
using AutoGrabberMod.Models;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace AutoGrabberMod.Features
{
    using SVObject = StardewValley.Object;

    class Forage : Feature
    {
        public override string FeatureName { get => "Auto Forage / Coop / Truffles"; }

        public override string FeatureConfig => "forage";

        public override int Order => 1;

        public override bool IsAllowed => Utilities.Config.AllowAutoForage;

        public Forage()
        {
            Value = false;
        }

        public override void Action()
        {
            if (!IsAllowed || !(bool)Value) return;

            Utilities.Monitor.Log($"  {Grabber.InstanceName} Attempting to forage items", StardewModdingAPI.LogLevel.Trace);
            Vector2[] nearbyTiles = (Grabber.RangeEntireMap ? Utilities.GetLocationObjectTiles(Grabber.Location) : Grabber.NearbyTilesRange).ToArray();
            Random random = new Random();

            //Handle animal houses                       
            foreach (Vector2 tile in nearbyTiles.Where(tile => Grabber.Location.Objects.ContainsKey(tile) && Utilities.IsGrabbableCoop(Grabber.Location.Objects[tile])).ToArray())
            {
                if (Grabber.IsChestFull) break;

                if (tile != null && Grabber.Location.objects.ContainsKey(tile) 
                    && Grabber.Location.objects[tile].Name != null
                    && Grabber.Location.objects[tile].Name.Contains("Slime Ball"))
                {
                    Random rr = new Random((int)Game1.stats.daysPlayed + (int)Game1.uniqueIDForThisGame + (int)tile.X * 77 + (int)tile.Y * 777 + 2);
                    Grabber.GrabberChest.addItem(new SVObject(766, random.Next(10, 21), false, -1, 0));
                    int i = 0;
                    while (random.NextDouble() < 0.33) i++;
                    if (i > 0) Grabber.GrabberChest.addItem(new SVObject(557, i, false, -1, 0));
                }
                else if (Grabber.GrabberChest.addItem(Grabber.Location.Objects[tile]) != null) continue;
                //Utilities.Monitor.Log($"    {Grabber.InstanceName} foraged: {Grabber.Location.Objects[tile].Name} {tile.X},{tile.Y}", StardewModdingAPI.LogLevel.Trace);
                Grabber.Location.Objects.Remove(tile);
                if (Grabber.GainExperience) Utilities.GainExperience(Grabber.FORAGING, 5);
            }

            //Handle Spring onions
            foreach (Vector2 tile in nearbyTiles.Where((tile) => Grabber.Location.terrainFeatures.ContainsKey(tile) && Grabber.Location.terrainFeatures[tile] is HoeDirt dirt && dirt.crop != null && dirt.crop.forageCrop.Value && dirt.crop.whichForageCrop.Value == 1))
            {
                if (Grabber.IsChestFull) break;
                if (tile == null || !Grabber.Location.terrainFeatures.ContainsKey(tile)) continue;
                SVObject onion = new SVObject(399, 1, false, -1, 0);

                if (Game1.player.professions.Contains(16)) onion.Quality = 4;
                else if (random.NextDouble() < Game1.player.ForagingLevel / 30.0) onion.Quality = 2;
                else if (random.NextDouble() < Game1.player.ForagingLevel / 15.0) onion.Quality = 1;

                if (Game1.player.professions.Contains(13))
                {
                    while (random.NextDouble() < 0.2) onion.Stack += 1;
                }
                Utilities.Monitor.Log($"    {Grabber.InstanceName} foraged: {onion.Name} {onion.Stack} {tile.X}, {tile.Y}", StardewModdingAPI.LogLevel.Trace);
                Grabber.GrabberChest.addItem(onion);
                (Grabber.Location.terrainFeatures[tile] as HoeDirt).crop = null;
                if (Grabber.GainExperience) Utilities.GainExperience(Grabber.FORAGING, 3);
            }

            //Handle world forageables            
            foreach (Vector2 tile in nearbyTiles.Where(tile => Grabber.Location.Objects.ContainsKey(tile) && (Utilities.IsGrabbableWorld(Grabber.Location.Objects[tile]) || Grabber.Location.Objects[tile].isForage(null))).ToArray())
            {
                if (Grabber.IsChestFull) break;
                if (tile == null || !Grabber.Location.Objects.ContainsKey(tile)) continue;
                SVObject obj = Grabber.Location.Objects[tile];
                if (Game1.player.professions.Contains(16)) obj.Quality = 4;
                else if (random.NextDouble() < Game1.player.ForagingLevel / 30.0) obj.Quality = 2;
                else if (random.NextDouble() < Game1.player.ForagingLevel / 15.0) obj.Quality = 1;

                if (Game1.player.professions.Contains(13))
                {
                    while (random.NextDouble() < 0.2) obj.Stack += 1;
                }
                Utilities.Monitor.Log($"    {Grabber.InstanceName} foraged: {obj.Name} {obj.Stack} {tile.X},{tile.Y}", StardewModdingAPI.LogLevel.Trace);
                Item item = Grabber.GrabberChest.addItem(obj);
                Grabber.Location.Objects.Remove(tile);
                if (Grabber.GainExperience) Utilities.GainExperience(Grabber.FORAGING, 7);
            }

            //Handle berry bushes
            if (Grabber.RangeEntireMap)
            {
                int berryIndex;
                foreach (LargeTerrainFeature feature in Grabber.Location.largeTerrainFeatures)
                {
                    if (Grabber.IsChestFull) break;
                    if (Game1.currentSeason == "spring") berryIndex = 296;
                    else if (Game1.currentSeason == "fall") berryIndex = 410;
                    else break;

                    if (feature is Bush bush)
                    {
                        if (bush.inBloom(Game1.currentSeason, Game1.dayOfMonth) && bush.tileSheetOffset.Value == 1)
                        {
                            SVObject berry = new SVObject(berryIndex, 1 + Game1.player.FarmingLevel / 4, false, -1, 0);
                            if (Game1.player.professions.Contains(16))
                            {
                                berry.Quality = 4;
                            }
                            Utilities.Monitor.Log($"    {Grabber.InstanceName} foraged: {berry.Name} {berry.Stack}", StardewModdingAPI.LogLevel.Trace);
                            bush.tileSheetOffset.Value = 0;
                            bush.setUpSourceRect();
                            Grabber.GrabberChest.addItem(berry);
                        }
                    }
                }
            }

            Grabber.Grabber.showNextIndex.Value |= Grabber.GrabberChest.items.Count != 0;
        }

        public void ActionItemAddedRemoved(object sender, EventArgsLocationObjectsChanged e)
        {
            if (!IsAllowed || !(bool)Value || Grabber.IsChestFull) return;
            //Utilities.Monitor.Log($"  {Grabber.InstanceName} Attempting to forage truffle items", StardewModdingAPI.LogLevel.Trace);
            System.Random random = new System.Random();
            Vector2[] nearbyTiles = Grabber.RangeEntireMap ? Utilities.GetLocationObjectTiles(Grabber.Location).ToArray() : Grabber.NearbyTilesRange;

            foreach (KeyValuePair<Vector2, SVObject> pair in e.Added)
            {
                if (pair.Value.ParentSheetIndex != 430 || pair.Value.bigCraftable.Value || !nearbyTiles.Contains(pair.Key)) continue;

                SVObject obj = pair.Value;
                if (obj.Stack == 0) obj.Stack = 1;

                //make sure its a forageable and grabable
                if (!obj.isForage(null) && !Utilities.IsGrabbableWorld(obj)) continue;

                if (Game1.player.professions.Contains(16)) obj.Quality = 4;
                else if (random.NextDouble() < Game1.player.ForagingLevel / 30.0) obj.Quality = 2;
                else if (random.NextDouble() < Game1.player.ForagingLevel / 15.0) obj.Quality = 1;

                if (Game1.player.professions.Contains(13))
                {
                    while (random.NextDouble() < 0.2) obj.Stack += 1;
                }
                //Utilities.Monitor.Log($"    {Grabber.InstanceName} foraged: {obj.Name} {obj.Stack} {pair.Key.X},{pair.Key.Y}", StardewModdingAPI.LogLevel.Trace);
                Grabber.GrabberChest.addItem(obj);
                e.Location.Objects.Remove(pair.Key);
                if (Grabber.GainExperience) Utilities.GainExperience(Grabber.FORAGING, 7);
            }
        }
    }
}
