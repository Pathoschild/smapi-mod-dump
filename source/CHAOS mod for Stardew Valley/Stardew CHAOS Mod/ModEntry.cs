/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DevWithMaj/Stardew-CHAOS
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Network;
using xTile.Dimensions;
using xTile;
using xTile.Tiles;
using StardewValley.Locations;
using System.Runtime.CompilerServices;
using Netcode;

namespace Stardew_CHAOS_Mod
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {

        public const int ChaosTimeInterval = 60;
        private int timer = 0;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.OneSecondUpdateTicked += ChaosTimeChecker;
            helper.Events.Player.Warped += Warped;
        }


        private void ChaosTimeChecker(object sender, OneSecondUpdateTickedEventArgs e)
        {
            // if not in game do not do yet
            if (!Context.IsWorldReady)
                return;
            if (Game1.paused || Game1.player.hasMenuOpen == true)
                return;
            if (Game1.isFestival()) // festival days are immune to chaos, enjoy the festivities
                return;
            if (Game1.CurrentEvent != null)
                return;

            timer++; // this ensures that it only calls method once when it is greater, then it will reset it down
            if (timer < ChaosTimeInterval)
            {
                return;
            }

            timer = 0; 

            /* Game1.currentLocation.monsterDrop(bat, (int)playerPos.X, (int)playerPos.Y, Game1.player); */
            // click on addBuffAttributes and you can find the indexes of the different buffs to add
            // Game1.player.addBuffAttributes();
            Random random = new Random();
            int number = random.Next(18);
            switch (number) // currently I just do this to determine which one to do. There are more elegant solutions such as making a weighted map, if someone reading this wants to do it, go for it
            {
                case 0:
                    AdvanceSeasonByOne();
                    break;
                case 1:
                    ChangeTime(120);
                    break;
                case 2:
                    ExplodePlayer(50);
                    break;
                case 3:
                    HalfPlayerEnergy();
                    break;
                case 4:
                    HalfPlayerHealth();
                    break;
                case 5:
                    DeleteHeldItem();
                    break;
                case 6:
                case 7:
                case 8:
                    TeleportPlayerToRandomLocation();
                    break;
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                    Random rand2 = new Random();
                    int val = rand2.Next(6);
                    switch (val)
                    {
                        case 0: BuffHandler.GiveBuff(BuffHandler.Buffs.DARKNESS, 20000, 1); break;
                        case 1: BuffHandler.GiveBuff(BuffHandler.Buffs.SLIME, 10000, 1); break;
                        case 2: BuffHandler.GiveBuff(BuffHandler.Buffs.FROZEN, 5000, 1); break;
                        case 3: BuffHandler.GiveBuff(BuffHandler.Buffs.BURNED, 20000, 1); break;
                        case 4: BuffHandler.GiveBuff(BuffHandler.Buffs.SPEED, 20000, 1); break;
                        case 5: BuffHandler.GiveBuff(BuffHandler.Buffs.TIPSY, 20000, 1); break;
                    }
                    break;
                case 14:
                case 15:
                case 16:
                case 17:
                    Random rand3 = new Random();
                    int val2 = rand3.Next(5);
                    // Game1.player.hasItemOfType();
                    switch (val2)
                    {
                        case 0: MonsterSpawner.SpawnBats(Game1.currentLocation, 5, 5); break;
                        case 1: MonsterSpawner.SpawnSlimes(Game1.currentLocation, 5, 5); break;
                        case 2: MonsterSpawner.SpawnSerpents(Game1.currentLocation, 3, 5); break;
                        case 3: MonsterSpawner.SpawnSlimes(Game1.currentLocation, 10, 5); break;
                        case 4: MonsterSpawner.SpawnBats(Game1.currentLocation, 10, 5); break;
                    }
                    break;
                case 18:
                    StardewValley.Object diamond = new StardewValley.Object(72, 1, false, -1, 0);
                    DropItem(diamond, Game1.player.GetDropLocation());
                    break;
                case 19:
                    StardewValley.Object iridium = new StardewValley.Object(337, 1, false, -1, 0);
                    DropItem(iridium, Game1.player.GetDropLocation());
                    break;
                case 20:
                    StardewValley.Object gold = new StardewValley.Object(336, 1, false, -1, 0);
                    DropItem(gold, Game1.player.GetDropLocation());
                    break;
            }
        }
        
        // goes to 28th day then forces sleep so that tilesets change
        private void AdvanceSeasonByOne()
        {
            Game1.dayOfMonth = 28;
            ChangeTime(4000);

            // this chunk actually advances the season by one in game but it is weird... you should sleep to completely reset tilemap and npc behavior
            /*int currentSeason = SDate.Now().SeasonIndex;
            currentSeason += 1;
            if (currentSeason >= 4) currentSeason = 0;
            this.Monitor.Log($"Season before: {Game1.currentSeason}", LogLevel.Debug);
            switch (currentSeason)
            {
                case 0:
                    Game1.currentSeason = "spring";
                    break;
                case 1:
                    Game1.currentSeason = "summer";
                    break;
                case 2:
                    Game1.currentSeason = "fall";
                    break;
                case 3:
                    Game1.currentSeason = "winter";
                    break;
            }
            this.Monitor.Log($"Season now: {Game1.currentSeason}", LogLevel.Debug);
            Game1.dayOfMonth = 1;*/
        }

        // TODO - prevent negative time
        // currently allows for huge numbers. If you add huge positive it skips the day, character passes out
        // if you add huge negative, time just goes negative. You have a super super long day.
        private void ChangeTime(int amount)
        {
            int hours = amount / 60;
            int minutes = amount % 60;
            Game1.timeOfDay += (100 * hours) + minutes;
        }

        // explodes the player,
        private void ExplodePlayer(int damage)
        {
            Game1.playSound("explosion");
            Game1.currentLocation.explode(Game1.player.getTileLocation(), 3, Game1.player, true, damage);
        }

        public enum ItemIds : int
        {
            DIAMOND = 72,
            IRIDIUM_BAR = 337,
            GOLD_BAR = 336
        }

        // used to create an item drop at a location
        // used like this: 
        // StardewValley.Object diamond = new StardewValley.Object(72, 1, false, -1, 0);
        // DropItem(diamond, Game1.player.GetDropLocation());
        private void DropItem(StardewValley.Object item, Vector2 dropLocation)
        {
            Game1.playSound("give_gift");
            Game1.createItemDebris(item, dropLocation, 1);
            // useful method, finds a location near the player that is suitable for drops!
            // Game1.player.GetDropLocation()
        }

        // used to spawn an item that can be picked up by the player (right click on it)
        // collides and blocks player, npcs destroy when walking through
        private void SpawnItem(StardewValley.Object item, Vector2 dropLocation)
        {
            // itemId, initialStack, bool isRecipe, price, quality
            // StardewValley.Object lol = new StardewValley.Object(itemId, 1, false, -1, 0);
            Game1.currentLocation.dropObject(item, dropLocation, Game1.viewport, true, null);
            Game1.player.GetDropLocation();
        }

        private void HalfPlayerEnergy()
        {
            Game1.playSound("UFO");
            Game1.player.Stamina /= 2;
        }

        private void HalfPlayerHealth()
        {
            // TODO - play sound here!
            Game1.playSound("UFO");
            Game1.player.health /= 2;
        }

        private void DeleteHeldItem()
        {
            Game1.playSound("woodWhack");
            Item currentItem = Game1.player.CurrentItem;
            if (currentItem != null) {
                Game1.player.removeItemFromInventory(currentItem);

            }
        }

        private void DeleteInventory()
        {
            Game1.playSound("woodWhack");
            Game1.player.clearBackpack();
        }

        private void TeleportPlayerToRandomLocation()
        {
            Game1.playSound("wand");
            // generate random location from list of all locations
            List<GameLocation> locations = new List<GameLocation>(Game1.locations);
            // 0 farm, 1 farm house, 3 town, 7 blacksmith, 4568 houses, 10 salloon, 24 bus stop, 25 mine, 19 tent, 20 forest, 22 animal shop, 23 leah house, 50 tuinnel, woods 34,
            GameLocation[] locationArray = new GameLocation[] { locations[0], locations[1], locations[3], locations[7], locations[4], locations[5], locations[6], locations[8], locations[10], locations[24], locations[25], locations[19], locations[20], locations[22], locations[23], locations[50], locations[34] };
            Random random = new Random();
            int locationIndex = random.Next(locationArray.Length);
            GameLocation teleportLocation = locationArray[locationIndex];
            Map map = teleportLocation.map;
            Vector2 vector = new Vector2(Game1.random.Next(0, map.GetLayer("Back").LayerWidth), Game1.random.Next(0, map.GetLayer("Back").LayerHeight));
            int i;
            for (i = 0; i < 12; i++)
            {
                // adding the isTile____IgnoreFloors with the other stuff removes the spawning in walkable walls sometimes!!
                // still will teleport to places that are soft locks sometimes! Use at your warning!
                if (teleportLocation.isTileLocationTotallyClearAndPlaceableIgnoreFloors(vector) 
                    && !teleportLocation.isTileOccupied(vector) 
                    && teleportLocation.isTilePassable(new Location((int)vector.X, (int)vector.Y), Game1.viewport) 
                    && map.GetLayer("Back").Tiles[(int)vector.X, (int)vector.Y] != null 
                    && !map.GetLayer("Back").Tiles[(int)vector.X, (int)vector.Y].Properties.ContainsKey("NPCBarrier"))
                {
                    break;
                }
                vector = new Vector2(Game1.random.Next(0, map.GetLayer("Back").LayerWidth), Game1.random.Next(0, map.GetLayer("Back").LayerHeight));
            }
            // if it passed the bounds check
            if (i < 12)
            {
                // warp the farmer there
                Game1.warpFarmer(teleportLocation.Name, (int)vector.X, (int)vector.Y, false);
            }

        }

        // TODO - add decrease levels for different stats!!!!
        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
        }

        private void Warped(object sender, WarpedEventArgs e)
        {
            MonsterSpawner.ClearMonsters();
            this.Monitor.Log($"Killed them???", LogLevel.Debug);
        }


    }
}