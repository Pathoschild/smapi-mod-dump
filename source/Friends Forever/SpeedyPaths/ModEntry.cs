/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/isaacs-dev/Minidew-Mods
**
*************************************************/

/*
 * MIT License
 *
 * Copyright (c) 2017-2019 Isaac S.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace SpeedyPaths {
    public class Config {
        //!The config file for various speed boosts. If a boost is 0, it is considered disabled and won't be given.
        //!The general boost will be given instead if it is enabled.

        /// <summary>Whether any boost other than the general boost should show a status effect icon.</summary>
        public bool ShowStatusEffect = true;
        /// <summary>The boost given to a player when they aren't on top of anything.</summary>
        public int GeneralBoost = 0;
        /// <summary>The boost given to the player when inside of the bath house.</summary>
        public int BathHouseBoost = 1;
        /// <summary>The boost given when the player is on the road that leads to the desert.</summary>
        public int DesertRoadBoost = 1;
        /// <summary>The boost given when the player is on the dock (and the small bridge) at the beach.</summary>
        public int DockBoost = 1;
        /// <summary>The boost given when the player is on the big wooden bridge.</summary>
        public int WoodBridgeBoost = 1;
        /// <summary>The boost given when the player is on one of the FEW dirt paths (like the one by JojoMart).</summary>
        public int DirtPathBoost = 0;
        /// <summary>The boost given when the player is on the stone-path in the town-square.</summary>
        public int TownSquareBoost = 2;
        //!The following are boosts for specific paths/floors:
        public int GravelPathBoost = 0;
        public int StrawFloorBoost = 0;
        public int WoodPathBoost = 0;
        public int WeatheredFloorBoost = 1;
        public int WoodFloorBoost = 1;
        public int CobblePathBoost = 1;
        public int SteppingStoneBoost = 1;
        public int StoneFloorBoost = 2;
        public int CrystalPathBoost = 2;
        public int CrystalFloorBoost = 3;
        /// <summary>The speed given to flooring with an unknown id (for if stardew ever updates).</summary>
        public int UnknownFloorBoost = 1;
        /// <summary>Enables the sp_floorinfo command.</summary>
        public bool EnableCommand = false;

        /// <summary>The boost number for the given floor id. This is take from the above config values.</summary>
        /// <returns>The boost for the given floor type.</returns>
        /// <param name="floorType">The floor type (from FlooringObject.whichFloor).</param>
        public int BoostForFloor(int floorType) {
            switch (floorType) {
            case 0:
                return WoodFloorBoost;
            case 1:
                return StoneFloorBoost;
            case 2:
                return WeatheredFloorBoost;
            case 3:
                return CrystalFloorBoost;
            case 4:
                return StrawFloorBoost;
            case 5:
                return GravelPathBoost;
            case 6:
                return WoodPathBoost;
            case 7:
                return CrystalPathBoost;
            case 8:
                return CobblePathBoost;
            case 9:
                return SteppingStoneBoost;
            default:
                return UnknownFloorBoost;
            }
        }

        /// <summary>Makes the buff object for the given floor.</summary>
        /// <returns>The created buff object for use, or null if the boost value was 0.</returns>
        /// <param name="floorType">The floor type (from FlooringObject.whichFloor).</param>
        public Buff MakeBuffForFlooring(int floorType) {
            return MakeBuffForBoost(BoostForFloor(floorType));
        }

        /// <summary>Makes the buff object from the given boost value. Returns null if 0.</summary>
        /// <returns>The speed buff object.</returns>
        /// <param name="boost">The speed boost value.</param>
        public Buff MakeBuffForBoost(int boost) {
            if (boost == 0)
                return null;

            string description = "Benefit of a good walking surface.";
            if (boost < 0)
                description = "Difficult to walk on.";

            return new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, boost, 0, 0, 0, "", "Paths") {
                millisecondsDuration = 0,
                sheetIndex = (boost < 0) ? Buff.slimed : -1,
                description = description
            };
        }
    }

    public class ModEntry : Mod {
        Config config;
        /// <summary>The number of recognized floor types (floor ids are sequential).</summary>
        const int recognizedFloorTypes = 10;
        /// <summary>The valid map names in town that use the town tileset.</summary>
        const string townMapNames = "BusStop Town Backwoods Forest Mountain Railroad";
        /// <summary>The tile indexes of various town square tiles.</summary>
        readonly int[] townSquareIds = {
            641, 737, 736, 768, 769, 770, 646, 995, 996, //Roads around town
            858, 859, 860, 861, 862, 863, //Square Centerpiece Top Row
            890, 891, 892, 893, 894, 895, //Square Centerpiece 2nd Row
            922, 923, 924, 925, 926, 927, //Square Centerpiece 3rd Row
            954, 955, 956, 957, 958, 959, //Square Centerpiece 4th Row
            986, 987, 988, 989, 990, 991, //Square Centerpiece 5th Row
            1018, 1019, 1020, 1021, 1022, 1023, //Square Centerpiece 6th Row
        };
        /// <summary>The tile indexes of various dirt path tiles.</summary>
        readonly int[] dirtPathIds = {
            462, 549, 572, 573, 574, 596, 597, 620, 621, 622, 624,
        };
        /// <summary>The tile indexes of various road tiles.</summary>
        readonly int[] roadIds = {
            1026, 1030, 1054, 1056, 1082, 1628, 1629, 1675, 1678, 1680, 1679, 1700
        };
        /// <summary>The tile indexes of various dock tiles.</summary>
        readonly int[] dockIds = {
            419, 420, 421, 470, 471, 472, 487, 488, 489, 491, 504, 505, 506, 508, //Actual Dock
            107, 300, 302 //Small Beach Bridge
        };
        /// <summary>The tile indexes of sand paths tiles in the desert.</summary>
        readonly int[] desertPathIds = { 64, 66, 82, 49, 48, 80, 142 };
        /// <summary>The tile indexes of road tiles in the desert.</summary>
        readonly int[] desertRoadIds = { 174, 206 };
        //!The various buffs for usage. Null if they disabled:
        Buff[] flooringToBuff;
        Buff unknownFlooringBuff;
        Buff generalBuff;
        Buff townSquareBuff;
        Buff dirtPathBuff;
        Buff bathHouseBuff;
        Buff desertRoadBuff;
        Buff dockBuff;
        Buff woodBridgeBuff;
        /// <summary>Whether the general buff is currently active or not. The general buff is handled differently:
        /// it is added to the character directly and removed when needed (this way there isn't a constant effect on
        /// the player).</summary>
        bool generalBuffActive = false;
        /// <summary>The current buff being applied to the user. Only is used when ShowStatusEffect is false.</summary>
        Buff currentBuff = null;

        /// <summary>Generates the buff object from the config values. This function could later be used to
        /// allow for dynamic config editing. The only option that isn't dynamic is EnableCommand.</summary>
        void GenerateBuffs() {
            flooringToBuff = new Buff[recognizedFloorTypes];

            for (int i = 0; i < recognizedFloorTypes; i++) {
                flooringToBuff[i] = config.MakeBuffForFlooring(i);
            }
            unknownFlooringBuff = config.MakeBuffForBoost(config.UnknownFloorBoost);
            generalBuff = config.MakeBuffForBoost(config.GeneralBoost);
            townSquareBuff = config.MakeBuffForBoost(config.TownSquareBoost);
            dirtPathBuff = config.MakeBuffForBoost(config.DirtPathBoost);
            bathHouseBuff = config.MakeBuffForBoost(config.BathHouseBoost);
            desertRoadBuff = config.MakeBuffForBoost(config.DesertRoadBoost);
            dockBuff = config.MakeBuffForBoost(config.DockBoost);
            woodBridgeBuff = config.MakeBuffForBoost(config.WoodBridgeBoost);
        }

        /// <summary>Mod entry point. Reads the config, generates the buffs, and adds listeners.</summary>
        /// <param name="helper">Helper object for various mod functions (such as loading config files).</param>
        public override void Entry(IModHelper helper) {
            config = this.Helper.ReadConfig<Config>();
            GenerateBuffs();

            helper.Events.GameLoop.UpdateTicked += this.UpdateTick;
            helper.Events.GameLoop.DayStarted += this.EveryDay;

            if (config.EnableCommand)
                helper.ConsoleCommands.Add("sp_floorinfo", "Grab's some small info about the tile the player is "
                                           + "standing on and prints it to console. This is only useful to programmers "
				                           + "and debugging. Usage: sp_floorinfo",
                                           this.Command_FloorInfo);
        }

        /// <summary>Every day: the general buff has to be readded.</summary>
        private void EveryDay(object sender, EventArgs e) {
            //Buffs are cleared at the start of every day, so the general buff is definitely not active:
            generalBuffActive = false;
            currentBuff = null;
        }

        /// <summary>Updates the current speed boost based on the player's location.</summary>
        private void UpdateTick(object sender, EventArgs e) {
            //We don't want to modify anything if the player can't move, game is paused, or if the game is inactive:
            if (!Context.CanPlayerMove || Game1.paused || !Game1.game1.IsActive)
                return;

            Buff newBoost = GetBoost();

            if (newBoost == null) {
                //currentBuff is only set if we are managing the speed buffs differently due to ShowStatusEffect being
                //false.
                if (currentBuff != null && !config.ShowStatusEffect) {
                    currentBuff.removeBuff();
                    //Game1.player.buffs.Remove(currentBuff);
                    currentBuff = null;
                }
            
                //If the general buff isn't active, then that means we need to add it:
                if (!generalBuffActive && config.GeneralBoost != 0) {
                    generalBuff.addBuff();
                    //Game1.player.buffs.Add(generalBuff);
                    generalBuffActive = true;
                }
            } else {
                //Remove the general buff if it is active:
                if (generalBuffActive && config.GeneralBoost != 0) {
                    //Game1.player.buffs.Remove(generalBuff);
                    generalBuff.removeBuff();
                    generalBuffActive = false;
                }

                if (config.ShowStatusEffect) {
                    //To prevent the display from removing the buff:
                    newBoost.millisecondsDuration = 0;
                    //Add the buff, which the display will already remove the old one and add this one:
                    Game1.buffsDisplay.addOtherBuff(newBoost);
                    currentBuff = newBoost;
                //If we are not showing status effects to the player, we have to manage the boosts in a different way:
                } else if (currentBuff != newBoost) {
                    if (currentBuff != null) {
                        currentBuff.removeBuff();
                        //Game1.player.buffs.Remove(currentBuff);
                    }
                    
                    //Game1.player.buffs.Add(newBoost);
                    newBoost.addBuff();
                    currentBuff = newBoost;
                }
            }
        }

        /// <summary>Get the boost from the player's position. If no special boost if found, return null.</summary>
        private Buff GetBoost() {
            var position = Game1.player.getTileLocation();

            //There's no chance a player-placed path would exist if the terrain features
            //for that tile is missing:
            if (!Game1.player.currentLocation.terrainFeatures.ContainsKey(position))
                return GetMapBoost();

            var tile = Game1.player.currentLocation.terrainFeatures[position];

            //If the tile isn't a flooring tile, the last try is to see if the map layer is 'path':
            if (!(tile is Flooring))
                return GetMapBoost();

            var flooring = tile as Flooring;

            //Just in case any future floor types are added:
            if (flooring.whichFloor >= recognizedFloorTypes)
                return unknownFlooringBuff;

            return flooringToBuff[flooring.whichFloor];
        }

        /// <summary>Attempts to get the boost from the map's tiles ("Back" layer).</summary>
        private Buff GetMapBoost() {
            var worldLocation = Game1.player.currentLocation;

            //The Bathhouse gets its own boost because of how SLOW you walk in it:
            if (worldLocation.name.ToString().StartsWith("BathHouse", StringComparison.Ordinal)) //RECS0063
                return bathHouseBuff;

            //We don't make any indoor or farm map-paths fast:
            if (!worldLocation.IsOutdoors || worldLocation.IsFarm)
                return null;

            var playerLocation = Game1.player.getTileLocation().ToPoint();
            var tile = worldLocation.getTileIndexAt(playerLocation, "Back");

            //Make sure that the current map is from one recognized maps.
            if (townMapNames.Contains(worldLocation.name)) {
                foreach (int stoneId in townSquareIds) {
                    if (tile == stoneId)
                        return townSquareBuff;
                }

                foreach (int dirtId in dirtPathIds) {
                    if (tile == dirtId)
                        return dirtPathBuff;
                }

                foreach (int roadId in roadIds) {
                    if (tile == roadId)
                        return desertRoadBuff;
                }

                //The bridge's tile id's exist in two big ranges, so they're looked for this way:
                if ((1775 <= tile && tile <= 1791) || (1800 <= tile && tile <= 1816))
                    return woodBridgeBuff;
            } else if (worldLocation.name == "Desert") {
                foreach (int roadId in desertRoadIds) {
                    if (tile == roadId)
                        return desertRoadBuff;
                }

                foreach (int pathId in desertPathIds) {
                    if (tile == pathId)
                        return dirtPathBuff;
                }
            } else if (worldLocation.name.ToString().StartsWith("Beach", StringComparison.Ordinal)) {
                foreach (int dockId in dockIds) {
                    if (tile == dockId)
                        return dockBuff;
                }
            }

            return null;
        }

        /// <summary>The command that prints out player-placed floor-type (if there is one) and the map tile under the
        /// player. This is useful if I've missed a tile or </summary>
        private void Command_FloorInfo(string command, string[] args) {
            if (args.Length != 0)
                this.Monitor.Log("Note: This command ignores all arguments.");

            var worldLocation = Game1.player.currentLocation;
            var playerVec = Game1.player.getTileLocation();
            var playerLocation = playerVec.ToPoint();

            if (worldLocation == null)
                return;
            
            this.Monitor.Log($"Location name: {worldLocation.name.Get()}");

            if (Game1.player.currentLocation.terrainFeatures.ContainsKey(playerVec)) {
                var tile = worldLocation.terrainFeatures[playerVec];

                if (tile is Flooring) {
                    this.Monitor.Log($"Found a player-placed floor. Type: {(tile as Flooring).whichFloor}");
                } else {
                    this.Monitor.Log("Didn't find a player-placed floor.");
                }
            } else {
                this.Monitor.Log("Didn't find a player-placed floor.");
            }

            var tileId = Game1.player.currentLocation.getTileIndexAt(playerLocation, "Back");
            this.Monitor.Log($"Tile on the back layer has an id (index) of {tileId}");
        }
    }
}
