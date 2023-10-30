/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewRoguelike.Netcode;
using StardewRoguelike.UI;
using StardewRoguelike.VirtualProperties;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewRoguelike
{

    class SpectatorMode
    {
        public static Farmer? Following { get; private set; }
        public static SpectateToolbar? FollowingToolbar { get; private set; }

        private readonly static List<Item> StoredItems = new();

        public static void EnterSpectatorMode()
        {
            if (!Context.IsMultiplayer)
                return;

            xTile.Dimensions.Location targetLocation = new(
                (int)Game1.player.Position.X - Game1.viewport.Width / 2,
                (int)Game1.player.Position.Y - Game1.viewport.Height / 2
            );

            StoreItems();

            Game1.player.get_FarmerIsSpectating().Value = true;
            SendToLocation(Game1.player.currentLocation, targetLocation);

            Game1.displayFarmer = false;
            Game1.viewportFreeze = true;
            Game1.displayHUD = false;

            List<Farmer> aliveFarmers = GetAliveFarmers();
            if (aliveFarmers.Count > 0)
                Following = aliveFarmers[Game1.random.Next(aliveFarmers.Count)];
            else
                Following = null;
        }

        public static void ExitSpectatorMode(int? level = null)
        {
            if (!Context.IsMultiplayer)
                return;

            Game1.player.get_FarmerIsSpectating().Value = false;
            Game1.displayHUD = true;
            Following = null;
            Game1.onScreenMenus = Game1.onScreenMenus.Where(m => m is not SpectateToolbar).ToList();

            PopItems();

            if (level is null)
                return;

            Roguelike.CurrentLevel = level.Value;
            Game1.warpFarmer($"UndergroundMine0/{level}", 6, 6, 2);
        }

        public static void StoreItems()
        {
            for (int i = 0; i < Game1.player.MaxItems; i++)
                StoredItems.Add(Game1.player.Items[i]);

            Game1.player.clearBackpack();
        }

        public static void PopItems()
        {
            foreach (Item item in StoredItems)
                Game1.player.addItemToInventory(item);

            StoredItems.Clear();
        }

        public static void RespawnPlayers(object? sender, WarpedEventArgs e)
        {
            if (e.OldLocation.GetType() != typeof(MineShaft))
                return;

            MineShaft previousMine = (MineShaft)e.OldLocation;
            int level = Roguelike.GetLevelFromMineshaft(previousMine);

            // Avoid this logic if the warped player is dead or not a merchant floor
            if (e.Player.get_FarmerIsSpectating().Value || !Merchant.IsMerchantFloor(level + 1))
                return;

            long[] deadFarmerIds = GetDeadFarmers().Select(f => f.UniqueMultiplayerID).ToArray();
            ModEntry.MultiplayerHelper.SendMessage(
                new RespawnMessage(level + 1),
                "RespawnMessage",
                playerIDs: deadFarmerIds
            );
        }

        public static List<Farmer> GetAliveFarmers()
        {
            List<Farmer> aliveFarmers = new();
            foreach (Farmer farmer in Game1.getOnlineFarmers())
            {
                if (!farmer.get_FarmerIsSpectating().Value)
                    aliveFarmers.Add(farmer);
            }

            return aliveFarmers;
        }

        public static List<Farmer> GetDeadFarmers()
        {
            List<Farmer> deadFarmers = new();
            foreach (Farmer farmer in Game1.getOnlineFarmers())
            {
                if (farmer.get_FarmerIsSpectating().Value)
                    deadFarmers.Add(farmer);
            }

            return deadFarmers;
        }

        public static void SendToLocation(GameLocation gameLocation, xTile.Dimensions.Location? targetLocation = null)
        {
            if (!Game1.player.get_FarmerIsSpectating().Value)
                return;

            Game1.locationRequest = new LocationRequest(gameLocation.Name, gameLocation.isStructure.Value, gameLocation);
            Game1.locationRequest.OnWarp += delegate
            {
                Game1.displayFarmer = false;
                Game1.viewportFreeze = true;
                Game1.displayHUD = false;

                int locationWidth = gameLocation.Map.Layers[0].LayerWidth * Game1.tileSize;
                int locationHeight = gameLocation.Map.Layers[0].LayerHeight * Game1.tileSize;
                Game1.viewport.Location = targetLocation ?? new xTile.Dimensions.Location(
                    locationWidth / 2 - Game1.viewport.Width / 2,
                    locationHeight / 2 - Game1.viewport.Height / 2
                );

                Game1.panScreen(0, 0);

                Game1.player.Position = new Vector2(-10000, -10000); // So they don't accidentally show up
            };
            ModEntry.ReflectionHelper.GetMethod(gameLocation, "resetLocalState").Invoke();

            if (Context.IsMainPlayer)
            {
                Game1.locationRequest.Loaded(Game1.locationRequest.Location);
                Game1.currentLocation.resetForPlayerEntry();
                Game1.currentLocation.Map.LoadTileSheets(Game1.mapDisplayDevice);

                Game1.currentLocation = Game1.locationRequest.Location;
                Game1.locationRequest.Warped(Game1.locationRequest.Location);
                Game1.locationRequest = null;
            }
            else
            {
                Multiplayer multiplayer = ModEntry.ReflectionHelper.GetField<Multiplayer>(typeof(Game1), "multiplayer", required: true).GetValue();
                if (Game1.locationRequest.Location is not null && Game1.locationRequest.Location.Root is not null && Game1.locationRequest.Location.Root.Value is not null && multiplayer.isActiveLocation(Game1.locationRequest.Location))
                {
                    Game1.currentLocation = Game1.locationRequest.Location;
                    Game1.locationRequest.Loaded(Game1.locationRequest.Location);
                    Game1.currentLocation.resetForPlayerEntry();

                    Game1.player.currentLocation = Game1.currentLocation;
                    Game1.locationRequest.Warped(Game1.currentLocation);
                    Game1.currentLocation.updateSeasonalTileSheets();
                    if (Game1.IsDebrisWeatherHere())
                        Game1.populateDebrisWeatherArray();

                    Game1.warpingForForcedRemoteEvent = false;
                    Game1.locationRequest = null;
                }
                else
                    Game1.requestLocationInfoFromServer();
            }
        }

        public static Point GetViewportAtPlayer(Farmer farmer)
        {
            int x = (int)farmer.Position.X - (Game1.viewport.Width / 2);
            int y = (int)farmer.Position.Y - (Game1.viewport.Height / 2);

            return new Point(x, y);
        }

        public static void MoveScreen(int x, int y)
        {
            if (Game1.currentLocation is null)
                return;

            int old_ui_mode_count = Game1.uiModeCount;
            while (Game1.uiModeCount > 0)
            {
                Game1.PopUIMode();
            }
            Game1.previousViewportPosition.X = Game1.viewport.Location.X;
            Game1.previousViewportPosition.Y = Game1.viewport.Location.Y;
            Game1.viewport.X = x;
            Game1.viewport.Y = y;
            Game1.clampViewportToGameMap();
            Game1.updateRaindropPosition();
            for (int i = 0; i < old_ui_mode_count; i++)
            {
                Game1.PushUIMode();
            }
        }

        static int currentIndex = 0;
        static int timer = 0;

        public static void Update(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !Game1.player.get_FarmerIsSpectating().Value)
                return;

            #region Camera movement
            float speed = 8;
            float x = 0;
            float y = 0;

            y += Game1.options.moveDownButton.Where(a => Keyboard.GetState().GetPressedKeys().Contains(a.key)).Count();
            x += Game1.options.moveRightButton.Where(a => Keyboard.GetState().GetPressedKeys().Contains(a.key)).Count();
            y -= Game1.options.moveUpButton.Where(a => Keyboard.GetState().GetPressedKeys().Contains(a.key)).Count();
            x -= Game1.options.moveLeftButton.Where(a => Keyboard.GetState().GetPressedKeys().Contains(a.key)).Count();

            var stick = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
            x += stick.X;
            y -= stick.Y;

            int mouseX = Game1.getOldMouseX() + Game1.viewport.X;
            int mouseY = Game1.getOldMouseY() + Game1.viewport.Y;
            if (mouseX - Game1.viewport.X < 64)
                x -= 1;
            else if (mouseX - (Game1.viewport.X + Game1.viewport.Width) >= -128)
                x += 1;
            if (mouseY - Game1.viewport.Y < 64)
                y -= 1;
            else if (mouseY - (Game1.viewport.Y + Game1.viewport.Height) >= -64)
                y += 1;

            x *= speed;
            y *= speed;

            bool moved = false;
            moved = (int)x != 0 || (int)y != 0;

            if (moved)
                Following = null;

            GameLocation myLocation;
            if (Game1.IsServer)
                myLocation = Game1.currentLocation;
            else
                myLocation = Game1.player.currentLocation;

            List<Farmer> aliveFarmers = GetAliveFarmers();
            if (Game1.game1.IsActive && Game1.activeClickableMenu is null)
            {
                if (Following is not null)
                {
                    if (!aliveFarmers.Contains(Following))
                        Following = null;
                    else
                    {
                        if (Following.currentLocation is null)
                            return;

                        if (Following.currentLocation != myLocation)
                        {
                            Game1.xLocationAfterWarp = Following.getTileX();
                            Game1.yLocationAfterWarp = Following.getTileY();
                            SendToLocation(Following.currentLocation);
                        }
                        else
                        {
                            Point newPoint = GetViewportAtPlayer(Following);
                            MoveScreen(newPoint.X, newPoint.Y);
                        }
                    }
                }
                else
                    Game1.panScreen((int)x, (int)y);
            }
            #endregion

            timer--;

            if (Game1.activeClickableMenu is null && timer <= 0 && Game1.game1.IsActive && (Mouse.GetState().LeftButton == ButtonState.Pressed
                            || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.RightShoulder)))
            {
                currentIndex++;
                Console.WriteLine($"Next player spectate");
            }
            else if (Game1.activeClickableMenu is null && timer <= 0 && Game1.game1.IsActive && (Mouse.GetState().RightButton == ButtonState.Pressed
                || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftShoulder)))
            {
                currentIndex--;
                Console.WriteLine("Previous player spectate");
            }
            else
                return;

            timer = 30;

            if (aliveFarmers.Count != 0)
                currentIndex %= aliveFarmers.Count;
            if (currentIndex < 0)
                currentIndex += aliveFarmers.Count;

            if (aliveFarmers.Count > 0)
            {
                Farmer newTarget = aliveFarmers[currentIndex];
                if (newTarget != Following)
                {
                    Following = newTarget;
                    FollowingToolbar = new SpectateToolbar(Following);
                    Game1.onScreenMenus.Add(FollowingToolbar);
                    return;
                }
            }
        }
    }
}
