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
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
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
            Elevator = 112,
            LadderUp = 115,
            LadderDown = 173,
            Shaft = 174,
            CoalSackOrMineCart = 194,
        }

        // Variables
        private ModConfig config = new();

        private int currentWateringCanWater = 0;
        public int currentBubblesX = 0;
        public int currentBubblesY = 0;
        public GameLocation currentBubblesLocation = Game1.currentLocation;

        public int currentPanPointX = 0;
        public int currentPanPointY = 0;
        public GameLocation currentPanPointLocation = Game1.currentLocation;

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
            config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Player.Warped += this.OnWarped;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
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

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.ArtifactSpots,
                (bool val) => config.ArtifactSpots = val,
                () => "Artifact Spots",
                () => "Whether to notify on artifact spots."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.SeedSpots,
                (bool val) => config.SeedSpots = val,
                () => "Seed Spots",
                () => "Whether to notify on seed spots."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.Bubbles,
                (bool val) => config.Bubbles = val,
                () => "Bubbles",
                () => "Whether to notify on bubbles."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.PanPoints,
                (bool val) => config.PanPoints = val,
                () => "Panning Points",
                () => "Whether to notify on panning points."
            );

            configMenu.AddKeybindList(
                this.ModManifest,
                () => config.DebugHotkey,
                (KeybindList val) => config.DebugHotkey = val,
                () => "Debug Hotkey",
                () => "The hotkey to execute the debug function."
            );
        }

        private void OnWarped(object? sender, WarpedEventArgs e)
        {
            CheckForArtifactSpots();
            CheckBubblesOnWarp();
            CheckPanPointOnWarp();
            CheckDungeonPause();
            CheckCoalNode();
            CheckChests();
        }

        private void CheckForArtifactSpots()
        {
             foreach (StardewValley.Object obj in Game1.currentLocation.objects.Values)
            {
                if (obj.Name == "Artifact Spot" && config.ArtifactSpots)
                {
                    Notify($"Artifact spot detected in {Game1.currentLocation.Name} at: (X: {obj.TileLocation.X}, Y: {obj.TileLocation.Y})", "");
                }
                else if (obj.Name == "Seed Spot" && config.SeedSpots)
                {
                    Notify($"Seed spot detected in {Game1.currentLocation.Name} at: (X: {obj.TileLocation.X}, Y: {obj.TileLocation.Y})", "");
                }
            }
        }

        private void CheckBubblesOnWarp()
        {
            if (Game1.currentLocation.fishSplashPoint is not NetPoint bubbles)
            {
                return;
            }

            bool bubblesExist = bubbles.X != 0 || bubbles.Y != 0;
            if (bubblesExist && config.Bubbles)
            {
                Notify($"Bubbles detected in {Game1.currentLocation.Name} at: (X: {bubbles.X}, Y: {bubbles.Y})", "");
            }
        }

        private void CheckPanPointOnWarp()
        {
            if (Game1.currentLocation.orePanPoint is not NetPoint panPoint)
            {
                return;
            }

            bool panPointExists = panPoint.X != 0 || panPoint.Y != 0;
            if (panPointExists && config.PanPoints)
            {
                Notify($"Pan point detected in {Game1.currentLocation.Name} at: (X: {panPoint.X}, Y: {panPoint.Y})", "");
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

            var ladderPos = GetTilePosition(mineShaft, TileType.LadderDown);
            if (ladderPos != Vector2.Zero)
            {
                Notify($"Floor {floorNum} has an pre-existing ladder at: (X: {ladderPos.X}, Y: {ladderPos.Y})", "cowboy_gunload");
            }

            var shaftPos = GetTilePosition(mineShaft, TileType.Shaft);
            if (shaftPos != Vector2.Zero)
            {
                Notify($"Floor {floorNum} has an pre-existing shaft at: (X: {shaftPos.X}, Y: {shaftPos.Y})", "cowboy_gunload");
            }

            var coalPos = GetTilePosition(mineShaft, TileType.CoalSackOrMineCart);
            if (coalPos != Vector2.Zero)
            {
                Notify($"Floor {floorNum} has a coal node: (X: {coalPos.X}, Y: {coalPos.Y})", "cowboy_gunload");
            }
        }

        public void CheckChests()
        {
            foreach (StardewValley.Object obj in Game1.currentLocation.objects.Values)
            {
                if (obj is Chest chest && !chest.playerChest.Value && !IsChestOpened(chest))
                {
                    Notify($"Location {Game1.currentLocation.Name} has a chest: (X: {obj.TileLocation.X}, Y: {obj.TileLocation.Y})", "cowboy_gunload");
                }
            }
        }

        private bool IsChestOpened(Chest chest)
        {
            // "currentLidFrame" is private, so we have to use reflection.
            int currentLidFrame = this.Helper.Reflection.GetField<int>(chest, "currentLidFrame").GetValue();

            // currentLidFrame is 224 on a closed chest.
            // currentLidFrame is 226 on an opened chest.
            return currentLidFrame != 224;
        }

        public bool IsDungeonBattleFloor(GameLocation location)
        {
            // e.g. "UndergroundMine1" is the first floor of the mines.
            return location is MineShaft mineShaft && mineShaft.Name.StartsWith("UndergroundMine") && !IsMineEmptyFloor(mineShaft);
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
                    if (IsTileTypeOnTile(tileType, i, j, mineShaft))
                    {
                        return new Vector2(i, j);
                    }
                }
            }

            return Vector2.Zero;
        }

        private static bool IsTileTypeOnTile(TileType tileType, int i, int j, GameLocation location)
        {
            int index = location.getTileIndexAt(new Point(i, j), "Buildings");
            Vector2 loc = new Vector2(i, j);

            return (
                index == (int)tileType
                && !location.Objects.ContainsKey(loc)
                && !location.terrainFeatures.ContainsKey(loc)
            );
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            CheckWateringCan();
            CheckBubblesOnTick();
            CheckPanPointOnTick();
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

        private void CheckBubblesOnTick()
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

            if (config.Bubbles)
            {
                Notify(msg, soundName);
            }
        }

        private void CheckPanPointOnTick()
        {
            var oldPanPointX = currentPanPointX;
            var newPanPointX = Game1.currentLocation.orePanPoint.X;
            currentPanPointX = newPanPointX;

            var oldPanPointY = currentPanPointY;
            var newPanPointY = Game1.currentLocation.orePanPoint.Y;
            currentPanPointY = newPanPointY;

            var oldPanPointLocation = currentPanPointLocation;
            var newPanPointLocation = Game1.currentLocation;
            currentPanPointLocation = newPanPointLocation;

            if (oldPanPointLocation != newPanPointLocation)
            {
                return;
            }

            if (oldPanPointX == newPanPointX && oldPanPointY == newPanPointY)
            {
                return;
            }

            bool disappear = newPanPointX == 0 && newPanPointY == 0;
            string msg = disappear ? $"Pan point disappeared in {Game1.currentLocation.Name}." : $"Pan point appeared in {Game1.currentLocation.Name}: {newPanPointX}, {newPanPointY}";

            if (config.PanPoints)
            {
                Notify(msg, "cowboy_gunload");
            }
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
            var ladderPos = GetTilePosition(mineShaft, TileType.LadderDown);
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
                // The parent tile indexes are hard coded to match the 3 types of bombs.
                // See: TemporaryAnimatedSprite::GetTemporaryAnimatedSprite
                if (sprite.initialParentTileIndex == 286 || sprite.initialParentTileIndex == 287 || sprite.initialParentTileIndex == 288)
                {
                    numBombs++;
                }
            }

            return numBombs;
        }

        // We just check for any player ammo being removed (assuming that all ammo is exploding ammo).
        private void CheckExplodingAmmoExploded(GameLocation location, bool isNewLocation)
        {
            var playerProjectiles = location.projectiles.Where(projectile => projectile.damagesMonsters.Value).ToArray();

            int numProjectiles = playerProjectiles.Length;
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

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (config.DebugHotkey.IsDown())
            {
                DebugFunction();
            }
        }

        private void DebugFunction()
        {
            DestroyItemCursorIsOver();
        }

        private void DestroyItemCursorIsOver()
        {
            if (Game1.activeClickableMenu is not GameMenu gameMenu)
            {
                return;
            }

            if (gameMenu.pages.Count == 0)
            {
                return;
            }

            var firstPage = gameMenu.pages[0];
            if (firstPage is not InventoryPage inventoryPage)
            {
                return;
            }

            if (inventoryPage.hoveredItem is not StardewValley.Object obj)
            {
                return;
            }

            DecrementStack(obj);
        }

        private void DecrementStack(StardewValley.Object obj)
        {
            if (obj.Stack > 1)
            {
                obj.Stack--;
            }
            else
            {
                // Cannot use "Items.Remove" since it causes other items to slide around.
                Game1.player.Items.RemoveButKeepEmptySlot(obj);
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
            if (Game1.activeClickableMenu is null && !IsFishing() && !IsDayEnding())
            {
                Game1.activeClickableMenu = new GameMenu();
            }
        }

        private bool IsFishing() {
            return Game1.player.UsingTool && Game1.player.CurrentTool is FishingRod;
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
