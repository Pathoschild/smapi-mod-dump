using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace SDVLife
{
    public class ModEntry : StardewModdingAPI.Mod
    {
        /// <summary>The mod settings.</summary>
        private ModConfig Config;

        private bool Running = false;

        private readonly uint[] TicksPerUpdate = new uint[] { 600, 300, 120, 60, 45, 30, 15, 10, 8, 6, 5, 4, 3 };

        private bool modKeyUsed = false;

        public override void Entry(IModHelper helper)
        {
            // read config
            this.Config = helper.ReadConfig<ModConfig>();
            Config.Speed = Math.Max(0, Math.Min(TicksPerUpdate.Length, Config.Speed));
            this.Monitor.Log($"Started with mod key {this.Config.ModKey}.", LogLevel.Trace);

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Input.ButtonReleased += this.OnButtonReleased;
            helper.Events.GameLoop.UpdateTicked += this.Update;
        }

        private void Update(object sender, UpdateTickedEventArgs e)
        {
            if (Running && e.IsMultipleOf(TicksPerUpdate[Config.Speed])  && Context.IsPlayerFree)
            {
                RunGeneration();
            }
        }

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;
            if (e.Button.Equals(Config.ModKey) && !modKeyUsed)
            {
                Game1.addHUDMessage(new HUDMessage("Game of Life: Hold '" + Config.ModKey + "' and press: 'P' to Play/Pause, '+' to speed up, '-' to slow down."));
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;
            if (e.Button.Equals(Config.ModKey))
            {
                modKeyUsed = false;
                return;
            }
            if (!this.Helper.Input.IsDown(Config.ModKey))
                return;

            this.Monitor.Log("Input detected");
            switch (e.Button)
            {
                case SButton.Add:
                case SButton.OemPlus:
                    NotifySpeedIncrease();
                    Config.Speed = Math.Min(Config.Speed + 1, TicksPerUpdate.Length - 1);
                    modKeyUsed = true;
                    break;
                case SButton.Subtract:
                case SButton.OemMinus:
                    NotifySpeedDecrease();
                    Config.Speed = Math.Max(Config.Speed - 1, 0);
                    modKeyUsed = true;
                    break;
                case SButton.P:
                    if (CheckLocation())
                    {
                        Running = !Running;
                        NotifyStartStop();
                    }
                    modKeyUsed = true;
                    break;
                case SButton.OemPeriod:
                    RunGeneration();
                    modKeyUsed = true;
                    break;
            }
        }

        private void NotifyStartStop()
        {
            if (Running) {
                Game1.addHUDMessage(new HUDMessage("Game of Life started.  Press '" + Config.ModKey + "'+'P' to Pause.", 2));
            } 
            else
            {
                Game1.addHUDMessage(new HUDMessage("Game of Life paused.  Press '" + Config.ModKey + "'+'P' to Play.", 3));
            }
        }

        private void NotifySpeedDecrease()
        {
            if (Config.Speed == 0)
            {
                Game1.addHUDMessage(new HUDMessage("Game of Life speed at minimum."));
            }
            else
            {
                Game1.addHUDMessage(new HUDMessage("Game of Life speed down."));
            }
        }

        private void NotifySpeedIncrease()
        {
            if (Config.Speed == TicksPerUpdate.Length - 1)
            {
                Game1.addHUDMessage(new HUDMessage("Game of Life speed at maximum."));
            }
            else
            {
                Game1.addHUDMessage(new HUDMessage("Game of Life speed up."));
            }
        }

        private bool CheckLocation()
        {
            if (Game1.currentLocation != null && Game1.currentLocation is Farm)
            {
                return true;
            }
            else
            {
                if (Running)
                {
                    Running = false;
                }
                Game1.addHUDMessage(new HUDMessage("Game of Life can only be played on your farm.", 3));
                return false;
            }
        }

        private void RunGeneration()
        {
            if (CheckLocation())
            {
                IEnumerable<KeyValuePair<Vector2, TerrainFeature>> featurePairs = Game1.currentLocation.terrainFeatures.Pairs;
                Dictionary<Vector2, Cell> liveNeighbors = CountLiveNeighbors(featurePairs);
                UpdateGameForNextGen(liveNeighbors, ConwayGenerationFunction);
                UpdateAllHoeDirt(featurePairs);
            }
        }

        private static bool ConwayGenerationFunction(int liveNeighborCount, bool isAlive)
        {
            return (liveNeighborCount == 3 || (isAlive && liveNeighborCount == 2));
        }

        private static Dictionary<Vector2, Cell> CountLiveNeighbors(IEnumerable<KeyValuePair<Vector2, TerrainFeature>> featurePairs)
        {
            Dictionary<Vector2, Cell> liveNeighbors = new Dictionary<Vector2, Cell>();
            foreach (KeyValuePair<Vector2, TerrainFeature> keyValuePair in featurePairs)
            {
                if (keyValuePair.Value is HoeDirt hoeDirt)
                {
                    Vector2 tilePos = keyValuePair.Key;
                    if ((int)(NetFieldBase<int, NetInt>)hoeDirt.state == 1)
                    {
                        foreach (Vector2 offset in ModEntry.neighborhoodOffsets)
                        {
                            Vector2 key = tilePos + offset;
                            liveNeighbors.TryGetValue(key, out Cell neighborCell);
                            neighborCell.liveNeighborCount++;
                            liveNeighbors[key] = neighborCell;
                        }
                        liveNeighbors.TryGetValue(tilePos, out Cell thisCell);
                        thisCell.alive = true;
                        liveNeighbors[tilePos] = thisCell;
                        hoeDirt.state.Value = 0;
                    }
                }
            }

            return liveNeighbors;
        }

        private void UpdateGameForNextGen(Dictionary<Vector2, Cell> liveNeighbors, Func<int, bool, bool> generationFunction)
        {
            foreach (KeyValuePair<Vector2, Cell> keyValuePair in liveNeighbors.AsEnumerable())
            {
                Vector2 tilePos = keyValuePair.Key;
                int liveNeighborCount = keyValuePair.Value.liveNeighborCount;
                bool isAlive = keyValuePair.Value.alive;
                bool aliveNextGen = generationFunction(liveNeighborCount, isAlive);
                bool diggable = Game1.currentLocation.doesTileHaveProperty((int)tilePos.X, (int)tilePos.Y, "Diggable", "Back") != null;
                if (aliveNextGen && diggable)
                {
                    bool hasNoBlockingFeature = HandleBlockingFeatures(tilePos);
                    bool hasNoBlockingObject = HandleBlockingObjects(tilePos);

                    if (Config.HoeDirt && hasNoBlockingFeature && (hasNoBlockingObject || Config.HoeUnderObjects))
                    {
                        Game1.currentLocation.makeHoeDirt(tilePos, true);
                    }
                    if (Game1.currentLocation.terrainFeatures.TryGetValue(tilePos, out TerrainFeature feature) && feature is HoeDirt dirt)
                    {
                        dirt.state.Value = 1;
                    }
                }
            }
        }

        private bool HandleBlockingFeatures(Vector2 tilePos)
        {
            bool destroy = Config.HoeDirt;
            if (Game1.currentLocation.terrainFeatures.TryGetValue(tilePos, out TerrainFeature feature))
            {
                if (feature is HoeDirt)
                {
                    return false;
                }
                else if (feature is Tree tree)
                {
                    destroy &= !tree.tapped.Value;
                    if (tree.stump.Value)
                    {
                        destroy &= Config.DestroyStumps;
                    }
                    else
                    {
                        if (tree.growthStage.Value < 5)
                        {
                            destroy &= Config.DestroyImmatureTrees;
                        }
                        else
                        {
                            destroy &= Config.DestroyMatureTrees;
                        }
                    }
                    if (destroy || Config.DestroyEverything)
                    {
                        tree.instantDestroy(tilePos, Game1.currentLocation);
                    }
                }
                else if (feature is Grass grass)
                {
                    destroy &= Config.DestroyGrass;
                }
                else
                {
                    destroy = false;
                }
                destroy |= Config.DestroyEverything;
                if (destroy)
                {
                    this.Monitor.Log($"Removing feature from {tilePos}", LogLevel.Trace);
                    Game1.currentLocation.terrainFeatures.Remove(tilePos);
                }
            }

            return destroy;
        }

        private bool HandleBlockingObjects(Vector2 tilePos)
        {
            bool destroy = Config.HoeDirt;
            if (Game1.currentLocation.objects.TryGetValue(tilePos, out Object obj))
            {
                if (obj.Name.Equals("Stone"))
                {
                    destroy &= Config.DestroyRocks;
                }
                else if (obj.Name.Equals("Twig"))
                {
                    destroy &= Config.DestroyTwigs;
                }
                else if (obj.Name.Equals("Weeds"))
                {
                    destroy &= Config.DestroyWeeds;
                }
                else
                {
                    destroy = false;
                }
                destroy |= Config.DestroyEverything;
                if (destroy)
                {
                    this.Monitor.Log($"Removing object from {tilePos}", LogLevel.Trace);
                    DestroyObject(tilePos);
                }
            }

            return destroy;
        }

        private static void UpdateAllHoeDirt(IEnumerable<KeyValuePair<Vector2, TerrainFeature>> featurePairs)
        {
            foreach (KeyValuePair<Vector2, TerrainFeature> keyValuePair in featurePairs)
            {
                if (keyValuePair.Value is HoeDirt hoeDirt)
                {
                    hoeDirt.updateNeighbors(Game1.currentLocation, keyValuePair.Key);
                }
            }
        }

        private void RunGenerationSimple()
        {
            KeyValuePair<Vector2, TerrainFeature>[] array = Game1.currentLocation.terrainFeatures.Pairs.ToArray<KeyValuePair<Vector2, TerrainFeature>>();
            Dictionary<Vector2, bool> nextGen = new Dictionary<Vector2, bool>();
            int i = 0;
            foreach (KeyValuePair<Vector2, TerrainFeature> keyValuePair in array)
            {
                if (keyValuePair.Value is HoeDirt hoeDirt)
                {
                    i++;
                    int liveNeighbors = 0;
                    Vector2 tilePos = keyValuePair.Key;
                    foreach (Vector2 offset in ModEntry.neighborhoodOffsets)
                    {
                        Vector2 key = tilePos + offset;
                        if (Game1.currentLocation.terrainFeatures.TryGetValue(key, out TerrainFeature terrainFeature) && terrainFeature != null && terrainFeature is HoeDirt neighbor)
                        {
                            if ((int)(NetFieldBase<int, NetInt>)neighbor.state == 1) liveNeighbors++;
                        }
                    }
                    nextGen.Add(tilePos, (liveNeighbors == 3 || ((int)(NetFieldBase<int, NetInt>)hoeDirt.state == 1 && liveNeighbors == 2)));
                }
            }
            foreach (KeyValuePair<Vector2, TerrainFeature> keyValuePair in array)
            {
                if (keyValuePair.Value is HoeDirt hoeDirt)
                {
                    Vector2 tilePos = keyValuePair.Key;
                    if (nextGen.TryGetValue(tilePos, out bool alive))
                    {
                        hoeDirt.state.Value = alive ? 1 : 0;
                    }
                    else
                    {
                        this.Monitor.Log("Couldn't find nextGen at " + tilePos, LogLevel.Debug);
                    }
                }
            }
            foreach (KeyValuePair<Vector2, TerrainFeature> keyValuePair in array)
            {
                if (keyValuePair.Value is HoeDirt hoeDirt)
                {
                    hoeDirt.updateNeighbors(Game1.currentLocation, keyValuePair.Key);
                }
            }
        }

        private void DestroyObject(Vector2 key)
        {
            StardewValley.Object @object = Game1.currentLocation.objects[key];
            @object.performRemoveAction((Vector2)(NetFieldBase<Vector2, NetVector2>)@object.tileLocation, Game1.currentLocation);
            @object.dropItem(Game1.currentLocation, new Vector2(key.X * 64f, key.Y * 64f), new Vector2(key.X * 64f, key.Y * 64f));
            Game1.currentLocation.Objects.Remove(key);
        }

        private static readonly Vector2[] neighborhoodOffsets = new Vector2[8]
        {
              HoeDirt.N_Offset,
              HoeDirt.N_Offset + HoeDirt.E_Offset,
              HoeDirt.E_Offset,
              HoeDirt.E_Offset + HoeDirt.S_Offset,
              HoeDirt.S_Offset,
              HoeDirt.S_Offset + HoeDirt.W_Offset,
              HoeDirt.W_Offset,
              HoeDirt.W_Offset + HoeDirt.N_Offset
        };

        private struct Cell
        {
            public int liveNeighborCount;
            public bool alive;
        }
    }
}
