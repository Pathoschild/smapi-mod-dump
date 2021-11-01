/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using BattleRoyale.Utils;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace BattleRoyale
{
    public class DoorOrWarp
    {
        public GameLocation Location { get; set; }

        public Point Position { get; set; }

        public GameLocation TargetLocation { get; set; }

        public DoorOrWarp(GameLocation location, Point position, GameLocation targetLocation)
        {
            Location = location;
            Position = position;
            TargetLocation = targetLocation;
        }
    }
    class Game
    {
        public IModHelper Helper { get; private set; }
        public IMonitor Monitor { get; private set; }

        public DateTime? WhenToStartNextRound = null;
        public bool waitingForNextRoundToStart = false;

        public HashSet<long> spectatingPlayers = new HashSet<long>();

        public bool isSpectating = false;
        public bool lastRound = false;
        public bool inLobby = true;

        public static int RedTeamHatId   = 57; // Red party hat
        public static int GreenTeamHatId = 59; // Green party hat
        public static int BlueTeamHatId  = 58; // Blue party hat

        public static int ForcedWinnerHatId = 39; // Cone hat;

        public bool ForceSpecialRound = false;
        public static int SpecialRoundsEvery = 3;
        public List<SpecialRoundType> SpecialRoundHistory = new List<SpecialRoundType>();
        public int SpecialRoundHistorySize = 4;

        public List<int> StormIndexHistory = new List<int>();
        public int StormIndexHistorySize = 5;

        public List<Round> Rounds = new List<Round>();

        public bool InProgress
        {
            get { return Rounds.Count > 0 && GetActiveRound().InProgress; }
        }

        public static Dictionary<GameLocation, List<DoorOrWarp>> AllWarps = new Dictionary<GameLocation, List<DoorOrWarp>>();

        public Game(IModHelper helper, IMonitor monitor)
        {
            this.Helper = helper;
            this.Monitor = monitor;

            //Ban people from hiding in the elevator building.

            ModEntry.Events.Display.MenuChanged += (o, e) =>
            {
                if (InProgress && e.NewMenu != null && e.NewMenu.GetType().Name == "ElevatorMenu")
                    e.NewMenu.exitThisMenu(false);
            };

            ModEntry.Events.GameLoop.UpdateTicked += (o, e) =>
            {
                if (InProgress && Game1.IsServer && e.IsMultipleOf(8))
                {
                    Storm.QuarterSecUpdate(GetActiveRound().AlivePlayers);
                    EquipmentDrops.Check();
                    Monsters.Check();
                }
            };
        }

        public Round GetActiveRound()
        {
            if (Rounds.Count == 0)
                return null;

            return Rounds[Rounds.Count - 1];
        }

        public bool IsSpecialRoundType(SpecialRoundType roundType)
        {
            Round round = GetActiveRound();
            return round != null && round.IsSpecialRoundType(roundType);
        }

        public Round CreateNewRound()
        {
            List<Farmer> participants = GetAllPlayers();
            Round round = new Round(participants);
            Rounds.Add(round);
            return round;
        }

        public void Play()
        {
            if (!Game1.IsServer || InProgress || waitingForNextRoundToStart)
                return;

            LoadAllWarps();

            CreateNewRound().Start();
        }

        public List<Farmer> GetAllPlayers()
        {
            return Game1.getOnlineFarmers().Where(farmer => !spectatingPlayers.Contains(farmer.UniqueMultiplayerID)).ToList();
        }

        public void ReturnToLobby()
        {
            ModEntry.BRGame.waitingForNextRoundToStart = false;
            ModEntry.BRGame.WhenToStartNextRound = null;

            lastRound = false;
            inLobby = true;

            if (Game1.IsServer)
            {
                TimeUtils.SetTime("spring", 600);
                NetworkUtils.SynchronizeTimeData();
            }

            SpectatorMode.ExitSpecatorMode();

            FarmerUtils.ClearInventory();

            NetworkUtils.WarpFarmer(Game1.player, new TileLocation("Mountain", 117, 30));
        }

        public void ProcessPlayerJoin(NetFarmerRoot farmerRoot)
        {
            Storm.SendReachedLocationData();

            ModEntry.Leaderboard.GetPlayer(farmerRoot.Value);

            foreach (long playerId in spectatingPlayers)
            {
                NetworkMessage.Send(
                    NetworkUtils.MessageTypes.TOGGLE_SPECTATE,
                    NetworkMessageDestination.SPECIFIC_PEER,
                    new List<object> { playerId, true },
                    targetPeer: farmerRoot.Value.UniqueMultiplayerID
                );
            }

            // If you run immediately it does nothing
            DelayedAction.functionAfterDelay(() =>
            {
                ModEntry.Leaderboard.SendFarmerSpecificData(farmerRoot.Value);
                ModEntry.Leaderboard.SendData(farmerRoot.Value.UniqueMultiplayerID);

                NetworkMessage.Send(
                    NetworkUtils.MessageTypes.ON_JOIN,
                    NetworkMessageDestination.SPECIFIC_PEER,
                    new List<object>() { },
                    farmerRoot.Value.UniqueMultiplayerID
                );

                NetworkUtils.WarpFarmer(farmerRoot.Value, new TileLocation("Mountain", 117, 30));
                NetworkUtils.SynchronizeTimeData();
            }, 1000);

            Round round = GetActiveRound();
            if (InProgress && round?.AlivePlayers.Count <= 1)
            {
                //Restart the game, or nothing will ever happen
                round?.HandleWin(null, null);
            }
        }

        public void Update()
        {
            Game1.gameTimeInterval = 0;

            Game1.player.Stamina = Game1.player.MaxStamina;

            if (Game1.currentLocation != null && Game1.currentLocation is StardewValley.Locations.Desert && Game1.player.CanMove)
            {
                if (Game1.player.position.X > 3020 && Game1.player.position.Y > 1350 && Game1.player.position.Y < 2100)
                {
                    Console.WriteLine("Exit desert by road");
                    Game1.warpFarmer("Backwoods", 25, 30, false);
                }
            }

            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ShopMenu)
                Game1.activeClickableMenu = null;

            if (waitingForNextRoundToStart && WhenToStartNextRound != null && Game1.IsServer && DateTime.Now >= WhenToStartNextRound)
            {
                if (lastRound)
                {
                    NetworkMessage.Send(
                        NetworkUtils.MessageTypes.RETURN_TO_LOBBY,
                        NetworkMessageDestination.ALL,
                        new List<object>()
                    );
                    return;
                }

                waitingForNextRoundToStart = false;
                Play();
            }
        }

        public static void LoadAllWarps()
        {
            AllWarps.Clear();

            foreach (GameLocation location in Game1.locations)
            {
                foreach (var doorPair in location.doors.Pairs)
                {
                    Point position = doorPair.Key;
                    string targetLocationName = doorPair.Value;
                    position.X *= 64;
                    position.Y *= 64;

                    GameLocation targetLocation;
                    try
                    {
                        targetLocation = Game1.getLocationFromName(targetLocationName);
                    }
                    catch (InvalidOperationException)
                    {
                        continue;
                    }
                    if (targetLocation == null)
                        continue;

                    var item = new DoorOrWarp(location, position, targetLocation);

                    if (AllWarps.ContainsKey(location))
                        AllWarps[location].Add(item);
                    else
                        AllWarps[location] = new List<DoorOrWarp>() { item };
                }

                foreach (Warp warp in location.warps)
                {
                    Point position = new Point(warp.X * 64, warp.Y * 64);

                    GameLocation targetLocation;
                    try
                    {
                        targetLocation = Game1.getLocationFromName(warp.TargetName);
                    }
                    catch (InvalidOperationException)
                    {
                        continue;
                    }
                    if (targetLocation == null)
                        continue;

                    var item = new DoorOrWarp(location, position, targetLocation);

                    if (AllWarps.ContainsKey(location))
                        AllWarps[location].Add(item);
                    else
                        AllWarps[location] = new List<DoorOrWarp>() { item };
                }

                if (location is IslandWest)
                {
                    DoorOrWarp item = new DoorOrWarp(
                        location,
                        new Point(4930, 2500),
                        Game1.getLocationFromName("IslandFarmHouse")
                    );
                    if (AllWarps.ContainsKey(location))
                        AllWarps[location].Add(item);
                    else
                        AllWarps[location] = new List<DoorOrWarp>() { item };
                }
                else if (location is Town)
                {
                    DoorOrWarp item = new DoorOrWarp(
                        location,
                        new Point(2200, 6125),
                        Game1.getLocationFromName("Sewer")
                    );
                    if (AllWarps.ContainsKey(location))
                        AllWarps[location].Add(item);
                    else
                        AllWarps[location] = new List<DoorOrWarp>() { item };
                }
            }
        }
    }
}
