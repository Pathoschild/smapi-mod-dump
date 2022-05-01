/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.Tools;
using System.Collections.Generic;
using BetterJunimos.Abilities;
using StardewModdingAPI;
using SObject = StardewValley.Object;

// bits of this are from Tractor Mod; https://github.com/Pathoschild/StardewMods/blob/68628a40f992288278b724984c0ade200e6e4296/TractorMod/Framework/BaseAttachment.cs#L132

namespace BetterJunimosForestry.Abilities {
    public class CollectDroppedObjectsAbility : IJunimoAbility {

        private readonly IMonitor Monitor;
        private readonly Axe FakeAxe = new Axe();

        internal CollectDroppedObjectsAbility(IMonitor Monitor) {
            this.Monitor = Monitor;
        }

        public string AbilityName() {
            return "CollectDroppedObjects";
        }

        public bool IsActionAvailable(GameLocation location, Vector2 pos, Guid guid) {
            // Monitor.Log($"CollectDroppedObjectsAbility IsActionAvailable {pos}", LogLevel.Debug);

            Vector2 up = new Vector2(pos.X, pos.Y + 1);
            Vector2 right = new Vector2(pos.X + 1, pos.Y);
            Vector2 down = new Vector2(pos.X, pos.Y - 1);
            Vector2 left = new Vector2(pos.X - 1, pos.Y);

            Vector2[] positions = { up, right, down, left };
            foreach (Vector2 nextPos in positions) {
                if (!Util.IsWithinRadius(location, Util.GetHutFromId(guid), nextPos)) continue;
                if (IsDebrisAtTile(nextPos)) {
                    // Monitor.Log($"Pos {nextPos} contains debris", LogLevel.Debug);
                    return true;
                }
            }
            return false;
        }

        public bool PerformAction(GameLocation location, Vector2 pos, JunimoHarvester junimo, Guid guid) {
            var chest = Util.GetHutFromId(guid).output.Value;

            var (x, y) = pos;
            var up = new Vector2(x, y + 1);
            var right = new Vector2(x + 1, y);
            var down = new Vector2(x, y - 1);
            var left = new Vector2(x - 1, y);

            var direction = 0;
            Vector2[] positions = { up, right, down, left };
            foreach (var nextPos in positions) {
                if (!Util.IsWithinRadius(location, Util.GetHutFromId(guid), nextPos)) continue;
                if (DebrisIndexAtTile(nextPos) > 0) {
                    junimo.faceDirection(direction);
                    return MoveDebrisFromTileToChest(nextPos, location, chest);
                }
                direction++;
            }
            return false;
        }

        protected bool IsDebrisAtTile(Vector2 tile) {
            return DebrisIndexAtTile(tile) > 0;
        }

        protected int DebrisIndexAtTile(Vector2 tile) { 
            // Monitor.Log($"IsDebrisAtTile {tile}", LogLevel.Debug);
            if (Game1.currentLocation.debris is null) {
                return -1;
            }
            foreach (Debris d in Game1.currentLocation.debris) {
                foreach (Chunk c in d.Chunks) {
                    int dx = (int)(c.position.X + Game1.tileSize / 2f) / Game1.tileSize;
                    int dy = (int)(c.position.Y + Game1.tileSize / 2f) / Game1.tileSize;
                    if (dx == tile.X && dy == tile.Y) {
                        // Monitor.Log($"        Debris chunks: {d.Chunks.Count} type: {d.debrisType} at {dx},{dy}", LogLevel.Debug);
                        if (d.item is not null) {
                            // Monitor.Log($"            {d.item.Name} [{d.item.ParentSheetIndex}]", LogLevel.Debug);
                            return d.item.ParentSheetIndex;
                        }
                        else {
                            // Monitor.Log($"            non-item debris [{c.debrisType}]", LogLevel.Debug);
                            return c.debrisType;
                        }
                    }
                }
            }
            return 0;
        }

        protected bool MoveDebrisFromTileToChest(Vector2 tile, GameLocation farm, Chest chest) {
            // Monitor.Log($"MoveDebrisFromTileToChest {tile}", LogLevel.Debug);
            if (Game1.currentLocation.debris is null) return false;
            List<Debris> to_remove = new List<Debris>();

            foreach (Debris d in Game1.currentLocation.debris) {
                foreach (Chunk c in d.Chunks) {
                    int dx = (int)(c.position.X + Game1.tileSize / 2f) / Game1.tileSize;
                    int dy = (int)(c.position.Y + Game1.tileSize / 2f) / Game1.tileSize;
                    if (dx == tile.X && dy == tile.Y) {
                        // Monitor.Log($"        Adding debris chunks to removal list: {d.Chunks.Count} at {dx},{dy}", LogLevel.Debug);
                        to_remove.Add(d);
                        break;
                    }
                }
            }

            foreach (Debris d in to_remove) {
                // Monitor.Log($"        removing Debris // chunks: {d.Chunks.Count} type: {d.debrisType}", LogLevel.Debug);
                MoveDebrisToChest(d, farm, chest);
                Game1.currentLocation.debris.Remove(d);
            }
            return (to_remove.Count > 0);
        }

        protected void MoveDebrisToChest(Debris d, GameLocation farm, Chest chest) {
            foreach (Chunk c in d.Chunks) {
                if (d.item is not null) {
                    SObject item = new SObject(d.item.ParentSheetIndex, 1);
                    Util.AddItemToChest(farm, chest, item);
                    //Monitor.Log($"            MoveDebrisToChest {d.item.Name} [{d.item.ParentSheetIndex}]", LogLevel.Debug);
                } else {
                    SObject item = new SObject(c.debrisType, 1);
                    if (item.Name != "Error Item") {
                        Util.AddItemToChest(farm, chest, item);
                        //Monitor.Log($"            MoveDebrisToChest {item.Name} [{c.debrisType}]", LogLevel.Debug);
                    }
                }
            }
        }
        
        public List<int> RequiredItems() {
            return new();
        }
        
        
        /* older API compat */
        public bool IsActionAvailable(Farm farm, Vector2 pos, Guid guid) {
            return IsActionAvailable((GameLocation) farm, pos, guid);
        }
        
        public bool PerformAction(Farm farm, Vector2 pos, JunimoHarvester junimo, Guid guid) {
            return PerformAction((GameLocation) farm, pos, junimo, guid);
        }
    }
}