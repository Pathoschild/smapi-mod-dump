/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zamiell/stardew-valley-mods
**
*************************************************/

using Force.DeepCloner;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System.Text.RegularExpressions;

namespace Notifier
{
    public class ModEntry : Mod
    {
        // Constants
        /// 48 is the highest amount of damage; see: https://stardewvalleywiki.com/Skull_Cavern#Monsters
        private const int DANGEROUS_HEALTH = 48;

        // Enums
        /// From "Mineshaft.cs"
        enum TileType
        {
            Ladder = 173,
            Shaft = 174,
            CoalSackOrMineCart = 194,
        }

        // Variables
        private int currentWateringCanWater = 0;
        public int currentBubblesX = 0;
        public int currentBubblesY = 0;
        public GameLocation currentBubblesLocation = Game1.currentLocation;
        private Netcode.NetStringList currentBuffIDs = new Netcode.NetStringList();
        private int currentHealth = 100; // Default starting health on a new character.
        private GameLocation lastLocation = Game1.currentLocation;
        private Vector2 lastLadderPos = Vector2.Zero;
        private Vector2 lastShaftPos = Vector2.Zero;
        private int lastNumStaircases = 0;
        private int lastNumBombs = 0;
        private int lastNumProjectiles = 0;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += this.OnWarped;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }

        private void OnWarped(object? sender, WarpedEventArgs e)
        {
            CheckForArtifactSpots();
            CheckDungeonPause();
            CheckCoalNode();
        }

        private void CheckForArtifactSpots()
        {
            foreach (StardewValley.Object node in Game1.currentLocation.objects.Values)
            {
                if (node.Name == "Artifact Spot")
                {
                    Notify($"Artifact spot detected in {Game1.currentLocation.Name} at: {node.TileLocation.X}, {node.TileLocation.Y}", "");
                }

                if (node.Name == "Seed Spot")
                {
                    Notify($"Seed spot detected in {Game1.currentLocation.Name} at: {node.TileLocation.X}, {node.TileLocation.Y}", "");
                }
            }
        }

        private void CheckDungeonPause()
        {
            if (Game1.currentLocation is not MineShaft mineShaft)
            {
                return;
            }

            if (!IsDungeonBattleFloor(mineShaft))
            {
                return;
            }

            EmulatePause();
        }

        private void CheckCoalNode()
        {
            if (Game1.currentLocation is not MineShaft mineShaft)
            {
                return;
            }

            if (!IsDungeonBattleFloor(mineShaft))
            {
                return;
            }

            var potentialTuple = GetDungeonFloorNum(mineShaft.Name);
            if (potentialTuple is not (int, bool) tuple)
            {
                return;
            }

            var (floorNum, mines) = tuple;

            var ladderPos = GetTilePosition(mineShaft, TileType.Ladder);
            if (ladderPos != Vector2.Zero)
            {
                Notify($"Floor {floorNum} has an pre-existing ladder at: {ladderPos.X}, {ladderPos.Y}", "cowboy_gunload");
            }

            var shaftPos = GetTilePosition(mineShaft, TileType.Shaft);
            if (shaftPos != Vector2.Zero)
            {
                Notify($"Floor {floorNum} has an pre-existing shaft at: {shaftPos.X}, {shaftPos.Y}", "cowboy_gunload");
            }

            var coalPos = GetTilePosition(mineShaft, TileType.CoalSackOrMineCart);
            if (coalPos != Vector2.Zero)
            {
                Notify($"Floor {floorNum} has a coal node: {coalPos.X}, {coalPos.Y}", "cowboy_gunload");
            }
        }

        public bool IsDungeonBattleFloor(MineShaft mineShaft)
        {
            // e.g. "UndergroundMine1" is the first floor of the mines.
            return mineShaft.Name.StartsWith("UndergroundMine") && !IsMineEmptyFloor(mineShaft);
        }

        private bool IsMineEmptyFloor(MineShaft mineShaft)
        {
            var potentialTuple = GetDungeonFloorNum(mineShaft.Name);
            if (potentialTuple is not (int, bool) tuple) {
                return false;
            }

            var (floorNum, mines) = tuple;
            return mines ? floorNum % 10 == 0 : false;
        }

        public (int floorNum, bool mines)? GetDungeonFloorNum(string locationName)
        {
            string pattern = @"\d+";
            Match match = Regex.Match(locationName, pattern);

            if (!match.Success)
            {
                return null;
            }

            string numericPart = match.Value;

            if (!int.TryParse(numericPart, out int floorNum))
            {
                return null;
            }

            // In Skull Caverns, floor 121 is floor 1.
            bool mines = true;
            if (floorNum > 120)
            {
                floorNum -= 120;
                mines = false;
            }

            return (floorNum, mines);
        }

        // Based on the "Joys of Efficiency" mod with some changes:
        // https://github.com/pomepome/JoysOfEfficiency/blob/master/JoysOfEfficiency/Huds/MineHud.cs
        private static Vector2 GetTilePosition(MineShaft mineShaft, TileType tileType)
        {
            for (int i = 0; i < mineShaft.Map.GetLayer("Buildings").LayerWidth; i++)
            {
                for (int j = 0; j < mineShaft.Map.GetLayer("Buildings").LayerHeight; j++)
                {
                    int index = mineShaft.getTileIndexAt(new Point(i, j), "Buildings");
                    Vector2 loc = new Vector2(i, j);
                    if (mineShaft.Objects.ContainsKey(loc) || mineShaft.terrainFeatures.ContainsKey(loc))
                    {
                        continue;
                    }

                    if (index == (int)tileType)
                    {
                        return loc;
                    }
                }
            }

            return Vector2.Zero;
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            CheckWateringCan();
            CheckBubbles();
            CheckBuffWornOff();
            CheckHP();

            if (Game1.player.currentLocation is MineShaft mineShaft)
            {
                // We track if this is a new location to prevent duplicating alerts.
                // This is necessary because the `UpdateTicked` event fires before the `Warped` event.
                bool isNewLocation = false;
                if (!Game1.currentLocation.Equals(lastLocation))
                {
                    lastLocation = Game1.currentLocation;
                    isNewLocation = true;
                }

                CheckNewLadderOrShaft(mineShaft, isNewLocation);
                CheckPlacedBombExploded(mineShaft, isNewLocation);
                CheckExplodingAmmoExploded(mineShaft, isNewLocation);
            }
        }

        private void CheckWateringCan()
        {
            if (Game1.player.CurrentTool is not StardewValley.Tools.WateringCan wateringCan)
            {
                return;
            }

            int oldWaterLeft = currentWateringCanWater;
            int newWaterLeft = wateringCan.WaterLeft;
            currentWateringCanWater = newWaterLeft;

            // Water can go into negative values.
            if (oldWaterLeft != newWaterLeft && newWaterLeft <= 0)
            {
                Notify("Watering can out of water!", "cowboy_gunload");
            }
        }

        private void CheckBubbles()
        {
            var oldBubblesX = currentBubblesX;
            var newBubblesX = Game1.currentLocation.fishSplashPoint.X;
            currentBubblesX = newBubblesX;

            var oldBubblesY = currentBubblesY;
            var newBubblesY = Game1.currentLocation.fishSplashPoint.Y;
            currentBubblesY = newBubblesY;

            var oldBubblesLocation = currentBubblesLocation;
            var newBubblesLocation = Game1.currentLocation;
            currentBubblesLocation = newBubblesLocation;

            if (oldBubblesLocation != newBubblesLocation)
            {
                return;
            }

            if (oldBubblesX == newBubblesX && oldBubblesY == newBubblesY)
            {
                return;
            }

            bool disappear = newBubblesX == 0 && newBubblesY == 0;
            string msg = disappear ? $"Bubbles disappeared in {Game1.currentLocation.Name}." : $"Bubbles appeared in {Game1.currentLocation.Name}: {newBubblesX}, {newBubblesY}";
            string soundName = disappear ? "cowboy_gunload" : "Cowboy_Secret";
            Notify(msg, soundName);
        }

        private void CheckBuffWornOff()
        {
            if (currentBuffIDs.SequenceEqual(Game1.player.buffs.AppliedBuffIds))
            {
                return;
            }

            Netcode.NetStringList oldBuffIDs = currentBuffIDs.DeepClone();
            Netcode.NetStringList newBuffIDs = Game1.player.buffs.AppliedBuffIds;
            currentBuffIDs = newBuffIDs.DeepClone();

            if (oldBuffIDs.Contains("food") && !newBuffIDs.Contains("food"))
            {
                Notify("Food buff worn off!", "Cowboy_Secret");
                EmulatePause();
            }

            if (oldBuffIDs.Contains("drink") && !newBuffIDs.Contains("drink"))
            {
                Notify("Drink buff worn off!", "cowboy_gunload");
                EmulatePause();
            }
        }

        private void CheckHP()
        {
            int health = Game1.player.health;
            int oldHealth = currentHealth;
            int newHealth = health;
            currentHealth = health;

            if (oldHealth != newHealth && newHealth <= DANGEROUS_HEALTH)
            {
                Notify("Health is at dangerous levels! (" + oldHealth + " is equal to or less than " + DANGEROUS_HEALTH + ")", "fallDown");
                EmulatePause();
            }
        }

        private void CheckNewLadderOrShaft(MineShaft mineShaft, bool isNewLocation)
        {
            var ladderPos = GetTilePosition(mineShaft, TileType.Ladder);
            Vector2 oldLadderPos = lastLadderPos;
            Vector2 newLadderPos = ladderPos;
            lastLadderPos = ladderPos;

            var shaftPos = GetTilePosition(mineShaft, TileType.Shaft);
            Vector2 oldShaftPos = lastShaftPos;
            Vector2 newShaftPos = shaftPos;
            lastShaftPos = shaftPos;

            int numStaircases = Game1.player.Items.CountId("(BC)71"); // Staircase ID
            int oldNumStaircases = lastNumStaircases;
            int newNumStaircases = numStaircases;
            lastNumStaircases = numStaircases;

            if (isNewLocation)
            {
                return;
            }

            // Do not send the alert if we are using a crafted staircase.
            if (oldNumStaircases != newNumStaircases)
            {
                return;
            }

            var potentialTuple = GetDungeonFloorNum(mineShaft.Name);
            if (potentialTuple is not (int, bool) tuple)
            {
                return;
            }

            var (floorNum, mines) = tuple;

            if (!oldLadderPos.Equals(newLadderPos))
            {
                Notify("Ladder spawned on floor " + floorNum + "!", "cowboy_gunload");
            }

            if (!oldShaftPos.Equals(newShaftPos))
            {
                Notify("Shaft spawned on floor " + floorNum + "!", "cowboy_gunload");
            }
        }

        private void CheckPlacedBombExploded(MineShaft mineShaft, bool isNewLocation)
        {
            int numBombs = GetNumTemporaryBombs(mineShaft);
            int oldNumBombs = lastNumBombs;
            int newNumBombs = numBombs;
            lastNumBombs = numBombs;

            if (isNewLocation)
            {
                return;
            }

            if (oldNumBombs > newNumBombs)
            {
                EmulatePause();
            }
        }

        private int GetNumTemporaryBombs(GameLocation location)
        {
            int numBombs = 0;

            foreach (var sprite in location.TemporarySprites)
            {
                // The parent tile indexes are harded to match the 3 types of bombs.
                // See: TemporaryAnimatedSprite.GetTemporaryAnimatedSprite
                if (sprite.initialParentTileIndex == 286 || sprite.initialParentTileIndex == 287 || sprite.initialParentTileIndex == 288)
                {
                    numBombs++;
                }
            }

            return numBombs;
        }

        // We just check for any ammo being removed (assuming that all ammo is exploding ammo).
        private void CheckExplodingAmmoExploded(GameLocation location, bool isNewLocation)
        {
            int numProjectiles = location.projectiles.Count;
            int oldNumProjectiles = lastNumProjectiles;
            int newNumProjectiles = numProjectiles;
            lastNumProjectiles = numProjectiles;

            if (isNewLocation)
            {
                return;
            }

            if (oldNumProjectiles > 0 && newNumProjectiles == 0)
            {
                EmulatePause();
            }
        }

        private void Notify(string msg, string soundName)
        {
            if (IsDayEnding())
            {
                return;
            }

            Game1.chatBox.addMessage(msg, Color.Red);

            if (soundName != "")
            {
                Game1.playSound(soundName);
            }
        }

        private void EmulatePause()
        {
            if (IsDayEnding())
            {
                return;
            }

            if (Game1.activeClickableMenu is null)
            {
                Game1.activeClickableMenu = new GameMenu();
            }
        }

        private bool IsDayEnding()
        {
            return Game1.timeOfDay == 600 && Game1.gameTimeInterval == 0;
        }

        private void Log(string msg)
        {
            this.Monitor.Log(msg, LogLevel.Debug);
        }
    }
}
