/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zamiell/stardew-valley-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.GameData.Crops;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Text.RegularExpressions;
using xTile.Dimensions;

namespace PlantHotkey
{
    public class ModEntry : Mod
    {
        // Enums
        /// From "Mineshaft.cs"
        enum TileType
        {
            Elevator = 112,
            LadderUp = 115,
            LadderDown = 173,
            Shaft = 174,
            CoalSackOrMineCart = 194,
        }

        // Variables
        private ModConfig config = new();
        bool warpingInThisLocation = false;

        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
            helper.Events.Player.Warped += this.OnWarped;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                return;
            }

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => config = new ModConfig(),
                save: () => this.Helper.WriteConfig(config)
            );

            configMenu.AddKeybindList(
                this.ModManifest,
                () => config.Hotkey,
                (KeybindList val) => config.Hotkey = val,
                () => "Plant Hotkey",
                () => "The hotkey to plant seeds + fertilizer."
            );
        }

        // We use the "ButtonsChanged" event instead of the "ButtonPressed" event because we want it to continually work while the button is being held down.
        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (Game1.activeClickableMenu is not null)
            {
                return;
            }

            if (config.Hotkey.IsDown())
            {
                SearchSurroundingTiles();
            }
        }

        private void SearchSurroundingTiles()
        {
            if (warpingInThisLocation || Game1.player.mount is Horse)
            {
                return;
            }

            Item? slot1Item = Game1.player.Items[0];
            Item? slot2Item = Game1.player.Items[1];

            // From: DebugCommand::ArtifactSpots
            GameLocation location = Game1.player.currentLocation;
            Vector2 playerTile = Game1.player.Tile;
            Vector2[] surroundingTileLocationsArray = Utility.getSurroundingTileLocationsArray(playerTile);
            Vector2[] tiles = surroundingTileLocationsArray.Concat(new[] { playerTile }).ToArray();

            // We return from each successful action to avoid race condition where we can plant more seeds than we have in our inventory.
            foreach (Vector2 tile in tiles)
            {
                Location tileLocation = new Location((int)tile.X, (int)tile.Y);

                // Terrain features
                if (location.terrainFeatures.TryGetValue(tile, out var terrainFeature))
                {
                    // Auto plant seeds + auto plant fertilizer + auto harvest
                    if (terrainFeature is HoeDirt hoeDirt)
                    {
                        if (IsSeed(slot1Item) && hoeDirt.crop is null)
                        {
                            bool success = hoeDirt.plant(slot1Item.ItemId, Game1.player, false);
                            if (success)
                            {
                                Game1.player.Items.ReduceId(slot1Item.ItemId, 1);
                                return;
                            }
                        }

                        if (IsFertilizer(slot2Item) && hoeDirt.fertilizer.Value is null)
                        {
                            bool success = hoeDirt.plant(slot2Item.ItemId, Game1.player, true);
                            if (success)
                            {
                                Game1.player.Items.ReduceId(slot2Item.ItemId, 1);
                                return;
                            }
                        }

                        if (hoeDirt.readyForHarvest() && hoeDirt.crop is not null && hoeDirt.crop.GetHarvestMethod() == HarvestMethod.Grab)
                        {
                            bool success = hoeDirt.performUseAction(hoeDirt.crop.tilePosition);
                            if (success)
                            {
                                return;
                            }
                        }
                    }

                    // Auto-shake normal trees (e.g. Oak Trees)
                    else if (terrainFeature is Tree tree)
                    {
                        // Don't shake trees that are tapped, because that is impossible in vanilla without removing the tapper.
                        if (!tree.tapped.Value)
                        {
                            bool success = tree.performUseAction(tree.Tile);
                            if (success)
                            {
                                // We explicitly do not return since then it will fail to shake trees that are beside each other.
                                // (This is because a success can happen on every frame.)
                            }
                        }
                    }

                    // Auto-shake fruit trees
                    else if (terrainFeature is FruitTree fruitTree)
                    {
                        bool success = fruitTree.performUseAction(fruitTree.Tile);
                        if (success)
                        {
                            // We explicitly do not return since then it will fail to shake trees that are beside each other.
                            // (This is because a success can happen on every frame.)
                        }
                    }
                }

                // Doors
                foreach (Building building in location.buildings)
                {
                    if (building.isActionableTile((int)tile.X, (int)tile.Y, Game1.player))
                    {
                        bool success = location.checkAction(tileLocation, Game1.viewport, Game1.player);
                        if (success)
                        {
                            warpingInThisLocation = true;
                            return;
                        }
                    }
                }

                // Objects
                StardewValley.Object obj = location.getObjectAtTile((int)tile.X, (int)tile.Y);
                if (obj is not null)
                {
                    Log(obj.Name);

                    // Auto empty nearby objects (e.g. Crystalariums)
                    if (obj.readyForHarvest.Value)
                    {
                        bool success = obj.checkForAction(Game1.player);
                        if (success)
                        {
                            return;
                        }
                    }

                    // Auto fill Kegs
                    if (obj.Name == "Keg" && IsFruitOrVegetable(slot1Item))
                    {
                        bool success = obj.performObjectDropInAction(slot1Item, false, Game1.player); // This automatically decrements the item stack.
                        if (success)
                        {
                            return;
                        }
                    }

                    // Auto fill Furnaces
                    if ((obj.Name == "Furnace" || obj.Name == "Heavy Furnace") && IsOre(slot1Item))
                    {
                        bool success = obj.performObjectDropInAction(slot1Item, false, Game1.player); // This automatically decrements the item stack.
                        if (success)
                        {
                            return;
                        }
                    }

                    // Auto-fill Crab Pots
                    if (obj.Name == "Crab Pot" && IsBait(slot1Item))
                    {
                        bool success = obj.performObjectDropInAction(slot1Item, false, Game1.player); // This automatically decrements the item stack.
                        if (success)
                        {
                            return;
                        }
                    }

                    // Auto-shake Tea Bushes
                    if (obj is IndoorPot indoorPot && indoorPot.bush.Value is not null)
                    {
                        bool success = indoorPot.bush.Value.performUseAction(indoorPot.TileLocation);
                        if (success)
                        {
                            // We explicitly do not return since then it will fail to shake trees that are beside each other.
                            // (This is because a success can happen on every frame.)
                        }
                    }

                    // Auto-forage
                    if (obj.CanBeGrabbed && obj.Name != "Anvil" && obj.Name != "Mini-Forge" && obj.Name != "Mini-Obelisk")
                    {
                        // "obj.performUseAction" does nothing for foragable items.
                        bool success = location.checkAction(tileLocation, Game1.viewport, Game1.player);
                        if (success)
                        {
                            return;
                        }
                    }
                }

                // Specific location things
                switch (location.Name)
                {
                    // Top floor of the mines.
                    case "Mine":
                        // This is the tile that the elevator is located at. You can also right-click on the tile below the elevator in order to bring up the menu,
                        // but this also involves a distance check. By checking for the elevator tile, the mod correctly emulates the vanilla behavior.
                        // (The below elevator code will not apply on the first floor of the mines because it does not count as a `MineShaft`.)
                        if (tile.X == 17 && tile.Y == 3)
                        {
                            bool success = location.performAction("MineElevator", Game1.player, tileLocation);
                            if (success)
                            {
                                warpingInThisLocation = true;
                                return;
                            }
                        }

                        if (tile.X == 12 && tile.Y == 10) // The mine cart tile to the left of the entrance.
                        {
                            bool success = location.performAction("MinecartTransport", Game1.player, tileLocation);
                            if (success)
                            {
                                warpingInThisLocation = true;
                                return;
                            }
                        }

                        break;

                    // The tiny entrance to Skull Cavern.
                    case "SkullCave":
                        if (tile.X == 3 && tile.Y == 4)
                        {
                            bool success = location.performAction("SkullDoor", Game1.player, tileLocation);
                            if (success)
                            {
                                warpingInThisLocation = true;
                                return;
                            }
                        }
                        break;

                    case "Sewer":
                        if (tile.X == 16 && tile.Y == 10) // The ladder tile to the left of Krobus.
                        {
                            bool success = location.checkAction(tileLocation, Game1.viewport, Game1.player);
                            if (success)
                            {
                                warpingInThisLocation = true;
                                return;
                            }
                        }
                        break;

                    case "Town":
                        if (tile.X == 105 && tile.Y == 79) // The mine cart tile next to the blacksmith.
                        {
                            bool success = location.performAction("MinecartTransport", Game1.player, tileLocation);
                            if (success)
                            {
                                warpingInThisLocation = true;
                                return;
                            }
                        }
                        break;

                    case "BusStop":
                        if (tile.X == 14 && tile.Y == 3) // The mine cart tile for the bus stop.
                        {
                            bool success = location.performAction("MinecartTransport", Game1.player, tileLocation);
                            if (success)
                            {
                                warpingInThisLocation = true;
                                return;
                            }
                        }
                        break;

                    case "Mountain":
                        if (tile.X == 124 && tile.Y == 11) // The mine cart tile for quarry.
                        {
                            // Intentionally disabled for the mountain because it interferes with gathering Oak Resin.
                            /*
                            bool success = location.performAction("MinecartTransport", Game1.player, tileLocation);
                            if (success)
                            {
                                usedLadderOnThisFloor = true;
                                return;
                            }
                            */
                        }
                        break;
                }

                // Mine shaft things
                if (location is MineShaft mineShaft)
                {
                    bool success = false;

                    int index = location.getTileIndexAt(new Point((int)tile.X, (int)tile.Y), "Buildings");
                    if (!location.Objects.ContainsKey(tile) && !location.terrainFeatures.ContainsKey(tile))
                    {
                        switch (index)
                        {
                            case (int)TileType.Elevator:
                                success = location.checkAction(tileLocation, Game1.viewport, Game1.player);
                                if (success)
                                {
                                    warpingInThisLocation = true;
                                    return;
                                }
                                break;
                            case (int)TileType.LadderUp:
                                // We only want to automatically go up ladders when farming ore in the mines (and not in Skull Cavern).
                                if (!IsSkullCavern(location.Name))
                                {
                                    warpingInThisLocation = true;
                                    location.answerDialogueAction("ExitMine_Leave", Array.Empty<string>()); // We want to skip the annoying dialog.
                                    return;
                                }
                                break;

                            case (int)TileType.LadderDown:
                                success = location.checkAction(tileLocation, Game1.viewport, Game1.player);
                                if (success)
                                {
                                    warpingInThisLocation = true;
                                    return;
                                }
                                break;

                            case (int)TileType.Shaft:
                                warpingInThisLocation = true;
                                mineShaft.enterMineShaft(); // We want to skip the annoying dialog.
                                return;

                            case (int)TileType.CoalSackOrMineCart:
                                success = location.checkAction(tileLocation, Game1.viewport, Game1.player);
                                if (success)
                                {
                                    return;
                                }
                                break;
                        }
                    }
                }
            }
        }

        private static bool IsSeed(Item? item)
        {
            return item is not null && item.Category == StardewValley.Object.SeedsCategory;
        }

        private static bool IsFertilizer(Item? item)
        {
            return item is not null && item.Category == StardewValley.Object.fertilizerCategory;
        }

        private static bool IsFruitOrVegetable(Item? item)
        {
            return item is not null && (item.Category == StardewValley.Object.FruitsCategory || item.Category == StardewValley.Object.VegetableCategory);
        }

        private static bool IsOre(Item? item)
        {
            if (item == null)
            {
                return false;
            }

            return (
                item.Name == "Copper Ore"
                || item.Name == "Iron Ore"
                || item.Name == "Gold Ore"
                || item.Name == "Iridium Ore"
                || item.Name == "Radioactive Ore"
            );
        }

        private static bool IsBait(Item? item)
        {
            // We intentionally do not use the category of bait to prevent accidentally using non-standard bait.
            return item is not null && item.Name == "Bait";
        }

        public static bool IsSkullCavern(string locationName)
        {
            string pattern = @"\d+";
            Match match = Regex.Match(locationName, pattern);

            if (!match.Success)
            {
                return false;
            }

            string numericPart = match.Value;

            if (!int.TryParse(numericPart, out int floorNum))
            {
                return false;
            }

            // In Skull Caverns, floor 121 is floor 1.
            return floorNum > 120;
        }

        private void OnWarped(object? sender, WarpedEventArgs e)
        {
            warpingInThisLocation = false;
        }

        private void Log(string msg)
        {
            this.Monitor.Log(msg, LogLevel.Debug);
        }
    }
}
