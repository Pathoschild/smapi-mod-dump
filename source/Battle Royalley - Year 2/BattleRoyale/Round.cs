/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using BattleRoyale.UI;
using BattleRoyale.Utils;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale
{
    enum SpecialRoundType : int
    {
        SLINGSHOT_ONLY = 0,
        BOMBS_ONLY = 1,
        NO_FOOD = 2,
        MONSTERS = 3,
        TEAMS = 4,
        SLUGFEST = 5,
        TRAVELLING_CART = 6
    }

    class Round
    {
        public long? WinnerId;

        public List<Farmer> Participants;
        public List<Farmer> AlivePlayers;

        public DateTime? StartTime;
        public DateTime? EndTime;

        public SpecialRoundType? SpecialRound = null;

        public bool GingerIsland = false;

        private readonly int[] timesOfDay = new int[4] { 900, 1200, 2000, 2400 };
        private readonly string[] seasons = new string[4] { "spring", "summer", "fall", "winter" };

        private readonly Dictionary<SpecialRoundType, string> AnnounceMessages = new Dictionary<SpecialRoundType, string>()
        {
            { SpecialRoundType.SLINGSHOT_ONLY, "Practice your aim! It's time to get shootin'!" },
            { SpecialRoundType.BOMBS_ONLY, "Explosives detected in Stardew Valley! Watch your step!" },
            { SpecialRoundType.NO_FOOD, "Supply shortage! Not enough crops were harvested this year to feed everyone." },
            { SpecialRoundType.MONSTERS, "Uh-oh, monsters have emerged from the mines!" },
            { SpecialRoundType.TEAMS, "Partner up, it's safer to go with a buddy!" },
            { SpecialRoundType.SLUGFEST, "Grab an axe and get in the battle! It isn't over till only one person is standing." },
            { SpecialRoundType.TRAVELLING_CART, "Your travelling cart shipment has arrived! Make good use of it!" }
        };

        public OverlayUI overlayUI;

        public bool InProgress
        {
            get { return StartTime != null && EndTime == null; }
        }

        public int Index
        {
            get { return ModEntry.BRGame.Rounds.Count - 1; }
        }

        public Round(List<Farmer> participants)
        {
            Participants = participants;
            AlivePlayers = participants;
        }

        public bool IsSpecialRoundType(SpecialRoundType roundType)
        {
            return SpecialRound == roundType;
        }

        public void SetupTime()
        {
            string season = seasons[Game1.random.Next(0, seasons.Length)];
            int time = timesOfDay[Game1.random.Next(0, timesOfDay.Length)];

            if (IsSpecialRoundType(SpecialRoundType.MONSTERS))
                time = 2400;

            TimeUtils.SetTime(season, time);
            NetworkUtils.SynchronizeTimeData();
        }

        public bool ShouldDoSpecialRound()
        {
            if (ModEntry.BRGame.ForceSpecialRound)
            {
                ModEntry.BRGame.ForceSpecialRound = false;
                return true;
            }
            return (Index > 0) && ((Index + 1) % Game.SpecialRoundsEvery == 0);
        }

        public SpecialRoundType? GetSpecialRoundType()
        {
            if (!ShouldDoSpecialRound())
                return null;

            List<SpecialRoundType> roundTypes = new List<SpecialRoundType>();
            foreach (SpecialRoundType roundType in Enum.GetValues(typeof(SpecialRoundType)))
                roundTypes.Add(roundType);

            SpecialRoundType selected = roundTypes[Game1.random.Next(roundTypes.Count)];
            int attempts = 0;
            while (ModEntry.BRGame.SpecialRoundHistory.Contains(selected) && attempts < 10)
            {
                selected = roundTypes[Game1.random.Next(roundTypes.Count)];
                if (selected == SpecialRoundType.TEAMS && Participants.Count < 4)
                    continue;
                attempts++;
            }

            ModEntry.BRGame.SpecialRoundHistory.Add(selected);
            if (ModEntry.BRGame.SpecialRoundHistory.Count > ModEntry.BRGame.SpecialRoundHistorySize)
                ModEntry.BRGame.SpecialRoundHistory.RemoveAt(0);

            return selected;
        }

        public void Start()
        {
            if (!Game1.IsServer || InProgress || Game1.activeClickableMenu != null)
                return;

            SpecialRound = GetSpecialRoundType();

            int stormIndex = Storm.GetRandomStormIndex();
            GingerIsland = Storm.IsGingerIslandPhase();

            MapUtils.RefreshMap();
            EquipmentDrops.Reset(GingerIsland);
            Monsters.Reset();

            new Chests().SpawnAndFillChests(GingerIsland);

            SetupTime();

            //Spawn players in & Tell the clients to start game
            var chosenSpawns = new Spawns().ScatterPlayers(Participants, GingerIsland);
            foreach (Farmer player in chosenSpawns.Keys)
                NetworkUtils.WarpFarmer(player, chosenSpawns[player]);

            if (SpecialRound == SpecialRoundType.TEAMS)
            {
                Participants.Shuffle();
                string lastChosen = "";
                foreach (Farmer player in Participants)
                {
                    string team;
                    switch (lastChosen)
                    {
                        case "red":
                            team = "green";
                            break;
                        case "green":
                            team = "blue";
                            break;
                        case "blue":
                        default:
                            team = "red";
                            break;
                    }
                    lastChosen = team;

                    NetworkMessage.Send(
                        NetworkUtils.MessageTypes.BROADCAST_TEAM,
                        NetworkMessageDestination.SPECIFIC_PEER,
                        new List<object>() { team },
                        player.UniqueMultiplayerID
                    );
                }
            }

            NetworkUtils.BroadcastRoundStart(AlivePlayers.Count, stormIndex, SpecialRound);

            NetworkUtils.SendChatMessageToAllPlayers("Game starting!");

            if (SpecialRound != null)
                NetworkUtils.SendChatMessageToAllPlayers(AnnounceMessages[(SpecialRoundType)SpecialRound]);
        }

        public void LoadClient(int numberOfPlayers, int stormIndex, SpecialRoundType? specialRoundType)
        {
            StartTime = DateTime.Now;

            SpecialRound = specialRoundType;
            ModEntry.BRGame.inLobby = false;
            Game1.currentMinigame = null;

            if (!ModEntry.BRGame.isSpectating)
                SpectatorMode.ExitSpecatorMode();
            else
                SpectatorMode.EnterSpectatorMode();

            //Prevent draw issues if they were holding a slingshot before round ended/started
            foreach (Farmer p in Participants)
                p.forceCanMove();

            FarmerUtils.ResetPlayer();

            Storm.StartStorm(stormIndex);
            SetupUI(numberOfPlayers);

            DelayedAction.functionAfterDelay(() =>
            {
                if (Index > 0 && ModEntry.Leaderboard.GetWinner()?.Player == Game1.player && specialRoundType != SpecialRoundType.TEAMS)
                    Game1.player.changeHat(Game.ForcedWinnerHatId);

                FarmerUtils.SetupSpecialRound();

                switch (SpecialRound)
                {
                    case SpecialRoundType.SLUGFEST:
                        Game1.changeMusicTrack("honkytonky");
                        break;
                    case SpecialRoundType.MONSTERS:
                        Game1.changeMusicTrack("spirits_eve");
                        break;
                }
            }, 1000);
        }
        public void EndRound(long? winnerId)
        {
            WinnerId = winnerId;
            EndTime = DateTime.Now;
            Game1.player.changeHat(-1);
            SpecialRound = null;

            if (WinnerId != null)
            {
                Farmer winner = Game1.getFarmer((long)WinnerId);
                Game1.onScreenMenus.Add(new VictoryRoyale(winner));
                Game1.playSound("stardrop");
            }

            TeardownUI();
        }

        public void SetupUI(int numberOfPlayers)
        {
            // Setup UI
            if (!(Game1.activeClickableMenu is CharacterCustomization))
                Game1.activeClickableMenu?.exitThisMenu(false);

            foreach (IClickableMenu menu in Game1.onScreenMenus.ToList())
            {
                if (menu is DayTimeMoneyBox)
                {
                    Game1.onScreenMenus.Remove(menu);
                    break;
                }
            }

            overlayUI = new OverlayUI(numberOfPlayers);
            Game1.onScreenMenus.Add(overlayUI);
        }

        public void TeardownUI()
        {
            // Remove the overlay
            foreach (IClickableMenu menu in Game1.onScreenMenus.ToList())
            {
                if (menu is OverlayUI)
                {
                    Game1.onScreenMenus.Remove(menu);
                    break;
                }
            }
        }

        public void HandleWin(Farmer winner, Farmer whoDied)
        {
            if (whoDied == null && winner == null)
                NetworkUtils.SendChatMessageToAllPlayers($"Ending game to avoid eternal wait.");
            else if (winner == null)
                NetworkUtils.SendChatMessageToAllPlayers($"'{whoDied.Name}' died, but no one was present to claim the victory.");
            else
            {
                if (!IsSpecialRoundType(SpecialRoundType.TEAMS))
                {
                    LeaderboardPlayer winningPlayer = ModEntry.Leaderboard.GetPlayer(winner);
                    winningPlayer.Wins++;
                    ModEntry.Leaderboard.SendFarmerSpecificData(winner);
                }
                else
                {
                    foreach (Farmer farmer in Participants)
                    {
                        if (!FarmerUtils.IsOnSameTeamAs(winner, farmer))
                            continue;

                        LeaderboardPlayer winningFarmer = ModEntry.Leaderboard.GetPlayer(farmer);
                        winningFarmer.Wins++;
                        ModEntry.Leaderboard.SendFarmerSpecificData(farmer);
                    }
                }


                NetworkUtils.SendChatMessageToAllPlayers($"[orange]#1 VALLEY ROYALE '{winner.Name}'");
                winner.netDoEmote("heart");
            }

            NetworkUtils.BroadcastRoundEnd(winner?.UniqueMultiplayerID);

            int timeBetweenRound = 15;
            if (timeBetweenRound >= 0)
            {
                if (!ModEntry.BRGame.lastRound)
                    NetworkUtils.SendChatMessageToAllPlayers($"Starting new game in {timeBetweenRound} seconds...");
                else
                    NetworkUtils.SendChatMessageToAllPlayers($"Returning to lobby in {timeBetweenRound} seconds...");

                ModEntry.BRGame.waitingForNextRoundToStart = true;
                ModEntry.BRGame.WhenToStartNextRound = DateTime.Now + new TimeSpan(hours: 0, minutes: 0, seconds: timeBetweenRound);
            }
        }

        public void HandleDeath(DamageSource damageSource, Farmer whoDied, Farmer killer = null, string monster = "")
        {
            if (!InProgress)
                return;

            if (damageSource == DamageSource.PLAYER || damageSource == DamageSource.THORNS)
            {
                if (killer != null && killer != whoDied)
                {
                    // [124] is the sword e-mote icon
                    NetworkUtils.SendChatMessageToAllPlayers($"[peach]{killer.Name} [124] {whoDied.Name}");

                    LeaderboardPlayer killerPlayer = ModEntry.Leaderboard.GetPlayer(killer);
                    killerPlayer.Kills++;
                    ModEntry.Leaderboard.SendFarmerSpecificData(killer);
                }
                else if (killer != null && killer == whoDied)
                    NetworkUtils.SendChatMessageToAllPlayers($"[peach]{killer.Name} killed themself");
                else if (killer == null)
                    NetworkUtils.SendChatMessageToAllPlayers($"[peach]{whoDied.Name} was killed");
            }
            else if (damageSource == DamageSource.STORM)
                NetworkUtils.SendChatMessageToAllPlayers($"[peach]{whoDied.Name} was killed by the storm");
            else if (damageSource == DamageSource.WORLD)
                NetworkUtils.SendChatMessageToAllPlayers($"[peach]{whoDied.Name} was killed by the world");
            else if (damageSource == DamageSource.MONSTER)
            {
                if (monster.Length != 0)
                    NetworkUtils.SendChatMessageToAllPlayers($"[peach]{monster} [70] {whoDied.Name}");
                else
                    NetworkUtils.SendChatMessageToAllPlayers($"[peach]{whoDied.Name} was killed by a monster");
            }

            LeaderboardPlayer deadPlayer = ModEntry.Leaderboard.GetPlayer(whoDied);
            deadPlayer.Deaths++;
            ModEntry.Leaderboard.SendFarmerSpecificData(whoDied);

            AlivePlayers.Remove(whoDied);

            NetworkMessage.Send(
                NetworkUtils.MessageTypes.BROADCAST_ALIVE_COUNT,
                NetworkMessageDestination.ALL_OTHERS,
                new List<object>() { AlivePlayers.Count }
            );

            if (IsSpecialRoundType(SpecialRoundType.TEAMS) && AlivePlayers.Count > 0)
            {
                Farmer sampleFarmer = null;
                bool allSameTeam = true;
                foreach (Farmer farmer in AlivePlayers)
                {
                    if (sampleFarmer == null)
                    {
                        sampleFarmer = farmer;
                        continue;
                    }
                    if (!FarmerUtils.IsOnSameTeamAs(sampleFarmer, farmer))
                    {
                        allSameTeam = false;
                        break;
                    }
                }

                if (allSameTeam)
                {
                    Farmer winner = AlivePlayers[0];
                    HandleWin(winner, whoDied);
                }

            }

            if (AlivePlayers.Count == 1)
            {
                Farmer winner = AlivePlayers[0];
                HandleWin(winner, whoDied);
            }
            else if (AlivePlayers.Count == 0)
                HandleWin(null, whoDied);
            else
                NetworkUtils.SendChatMessageToAllPlayers($"'{whoDied.Name}' has died! {AlivePlayers.Count} players remain...");
        }
    }
}
