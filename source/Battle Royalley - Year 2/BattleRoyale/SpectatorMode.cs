/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale
{
    class SpectatorMode
    {
        //Each location UI component on the map has a myID field
        private static readonly Dictionary<int, string> pointIDsToLocation = new Dictionary<int, string>()
        {
            { -500, "Beach" },
            { 1001 ,"Desert" },
            { 1002 ,"Farm" },
            { 1003 ,"Backwoods" },
            { 1004 ,"BusStop" },
            { 1005 ,"WizardHouse" },
            { 1006 ,"AnimalShop" },
            { 1007 ,"LeahHouse" },
            { 1008 ,"SamHouse" },
            { 1009 ,"HaleyHouse" },
            { 1010 ,"Town" },
            { 1011 ,"Hospital" },
            { 1012 ,"SeedShop" },
            { 1013 ,"Blacksmith" },
            { 1014 ,"Saloon" },
            { 1015 ,"ManorHouse" },
            { 1016 ,"ArchaeologyHouse" },
            { 1017 ,"ElliottHouse" },
            { 1018 ,"Sewer" },
            { 1020 ,"Trailer" },
            { 1021 ,"JoshHouse" },
            { 1022 ,"ScienceHouse" },
            { 1023 ,"Tent" },
            { 1024 ,"Mine" },
            { 1025 ,"AdventureGuild" },
            { 1026, "Mountain" },
            { 1027 ,"JojaMart" },
            { 1028 ,"FishShop" },
            { 1029 ,"Railroad" },
            { 1030 ,"Woods" },
            { 1031, "Forest" },
            { 1032 ,"CommunityCenter" },
            { 1033 ,"Sewer" },
            { 1034 ,"Railroad" }
        };
        public static bool InSpectatorMode { get; private set; } = false;

        public static Farmer Following { get; private set; }
        public static SpectateToolbar FollowingToolbar { get; private set; }

        public static void EnterSpectatorMode(GameLocation gameLocation = null, xTile.Dimensions.Location? targetLocation = null)
        {
            InSpectatorMode = true;
            SendSpectatorToLocation(gameLocation ?? Game1.getFarm(), targetLocation);

            Game1.displayFarmer = false;
            Game1.viewportFreeze = true;
            Game1.displayHUD = false;

            Round round = ModEntry.BRGame.GetActiveRound();
            if (round != null && round.AlivePlayers.Count > 0)
                Following = round.AlivePlayers[0];
            else
                Following = null;
        }

        public static void ExitSpecatorMode()
        {
            InSpectatorMode = false;
            Game1.displayHUD = true;
            Following = null;
        }

        public static void SendSpectatorToLocation(GameLocation gameLocation, xTile.Dimensions.Location? targetLocation = null)
        {
            if (!InSpectatorMode)
                return;

            Game1.locationRequest = new LocationRequest(gameLocation.Name, gameLocation.isStructure, gameLocation);
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
            ModEntry.BRGame.Helper.Reflection.GetMethod(gameLocation, "resetLocalState").Invoke();

            if (Game1.IsServer)
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
                Multiplayer multiplayer = ModEntry.BRGame.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
                if (Game1.locationRequest.Location != null && Game1.locationRequest.Location.Root != null && multiplayer.isActiveLocation(Game1.locationRequest.Location))
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

        public static bool OnClickMapPoint(ClickableComponent point)
        {
            int id = point.myID;
            if (pointIDsToLocation.TryGetValue(id, out string locationName))
            {
                SendSpectatorToLocation(Game1.getLocationFromName(locationName));
                Following = null;
                return true;
            }

            return false;
        }

        public static Point GetViewportAtPlayer(Farmer farmer)
        {
            int x = (int)farmer.Position.X - (Game1.viewport.Width / 2);
            int y = (int)farmer.Position.Y - (Game1.viewport.Height / 2);

            return new Point(x, y);
        }

        public static void MoveScreen(int x, int y)
        {
            if (Game1.currentLocation == null)
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

        public static void Update()
        {
            if (!InSpectatorMode || ModEntry.Leaderboard.AlreadyDisplaying())
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

            Round round = ModEntry.BRGame.GetActiveRound();
            if (Game1.game1.IsActive && Game1.activeClickableMenu == null)
            {
                if (Following != null)
                {
                    if (!round.AlivePlayers.Contains(Following))
                        Following = null;
                    else
                    {
                        if (Following.currentLocation == null)
                            return;

                        if (Following.currentLocation != myLocation)
                        {
                            Game1.xLocationAfterWarp = Following.getTileX();
                            Game1.yLocationAfterWarp = Following.getTileY();
                            SendSpectatorToLocation(Following.currentLocation);
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

            if (Game1.activeClickableMenu == null && timer <= 0 && Game1.game1.IsActive && (Mouse.GetState().LeftButton == ButtonState.Pressed
                            || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.RightShoulder)))
            {
                currentIndex++;
                Console.WriteLine($"Next player spectate");
            }
            else if (Game1.activeClickableMenu == null && timer <= 0 && Game1.game1.IsActive && (Mouse.GetState().RightButton == ButtonState.Pressed
                || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftShoulder)))
            {
                currentIndex--;
                Console.WriteLine("Previous player spectate");
            }
            else
                return;

            timer = 30;

            var alivePlayers = round?.AlivePlayers.Where(a => a.Position.X >= -1000).ToList();

            if (alivePlayers.Count != 0)
                currentIndex %= alivePlayers.Count;
            if (currentIndex < 0)
                currentIndex += alivePlayers.Count;

            if (alivePlayers.Count > 0)
            {
                Farmer newTarget = alivePlayers[currentIndex];
                if (newTarget != Following)
                {
                    Following = newTarget;
                    FollowingToolbar = new SpectateToolbar(Following);
                    Game1.onScreenMenus.Add(FollowingToolbar);
                    return;
                }
                else if (newTarget == Following)
                    return;

                var warpTarget = alivePlayers[currentIndex];
                SendSpectatorToLocation(warpTarget.currentLocation,
                    new xTile.Dimensions.Location(
                        (int)warpTarget.Position.X - Game1.viewport.Width / 2,
                        (int)warpTarget.Position.Y - Game1.viewport.Height / 2));
            }
        }
    }
}
