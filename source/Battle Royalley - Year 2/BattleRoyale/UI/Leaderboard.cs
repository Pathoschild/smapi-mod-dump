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
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace BattleRoyale.UI
{
    class LeaderboardPlayer : IComparable<LeaderboardPlayer>
    {
        public long PlayerId;
        public int Kills = 0;
        public int Deaths = 0;
        public int Wins = 0;

        public Farmer Player
        {
            get { return Game1.getFarmerMaybeOffline(PlayerId); }
        }

        public LeaderboardPlayer(Farmer player, int kills, int deaths, int wins)
        {
            PlayerId = player.UniqueMultiplayerID;
            Kills = kills;
            Deaths = deaths;
            Wins = wins;
        }

        public int CompareTo(LeaderboardPlayer other)
        {
            if (other == null)
                return -1;

            string sortMethod = ModEntry.Leaderboard.SortMethod;
            bool ascending = sortMethod.EndsWith("+");
            sortMethod = sortMethod.Replace("+", "");

            switch (sortMethod)
            {
                case "Player":
                    return ascending ? other.Player.Name.CompareTo(Player.Name) : Player.Name.CompareTo(other.Player.Name);
                case "Wins":
                    return ascending ? Wins.CompareTo(other.Wins) : other.Wins.CompareTo(Wins);
                case "Kills":
                    return ascending ? Kills.CompareTo(other.Kills) : other.Kills.CompareTo(Kills);
                case "Deaths":
                    return ascending ? Deaths.CompareTo(other.Deaths) : other.Deaths.CompareTo(Deaths);
                default:
                    break;
            }

            return 0;
        }
    }
    class Leaderboard : IClickableMenu
    {
        public List<LeaderboardPlayer> Players = new();
        public string SortMethod = "Wins";

        private int currentDrawHeight;
        private readonly List<ClickableComponent> headerComponents = new();

        private readonly float percentOfViewportWide = 0.33f;
        private readonly float percentOfViewportTall = 0.66f;

        private readonly int rowTopPadding = 4;

        private readonly int cornerSize = 60 / 3;

        private readonly Texture2D titleTexture;
        private readonly float titleScale = 0.65f;

        private readonly Rectangle horizontalLineRectangle = new(0, 256, 60, 60);

        public Leaderboard()
        {
            titleTexture = ModEntry.BRGame.Helper.Content.Load<Texture2D>("Assets/leaderboard.png", ContentSource.ModFolder);
            calculateLeaderboardPosition();
        }

        public void InitPlayer(Farmer farmer)
        {
            LeaderboardPlayer newPlayer = new(farmer, 0, 0, 0);
            Players.Add(newPlayer);
        }

        public LeaderboardPlayer GetPlayer(Farmer farmer)
        {
            foreach (LeaderboardPlayer player in Players)
            {
                if (player.PlayerId == farmer.UniqueMultiplayerID)
                    return player;
            }

            LeaderboardPlayer newPlayer = new(farmer, 0, 0, 0);
            Players.Add(newPlayer);
            return newPlayer;
        }

        public LeaderboardPlayer GetWinner()
        {
            LeaderboardPlayer winner = null;
            foreach (LeaderboardPlayer player in Players)
            {
                if (!FarmerUtils.IsOnline(player.Player))
                    continue;

                if (winner == null)
                {
                    winner = player;
                    continue;
                }
                if (player.Wins > winner.Wins)
                {
                    winner = player;
                    continue;
                }
                else if (player.Wins == winner.Wins)
                {
                    if (player.Kills > winner.Kills)
                    {
                        winner = player;
                        continue;
                    }
                    else if (player.Kills == winner.Kills && player.Deaths < winner.Deaths)
                    {
                        winner = player;
                        continue;
                    }
                }
            }

            return winner;
        }

        public void SendFarmerSpecificData(Farmer farmer, long? targetId = null)
        {
            LeaderboardPlayer player = GetPlayer(farmer);
            NetworkMessageDestination destination = targetId == null ? NetworkMessageDestination.ALL : NetworkMessageDestination.SPECIFIC_PEER;

            NetworkMessage.Send(
                NetworkUtils.MessageTypes.LEADERBOARD_DATA_SYNC,
                destination,
                new List<object> { player.PlayerId, player.Kills, player.Deaths, player.Wins },
                targetId
            );
        }

        public void SendData(long? targetId = null)
        {
            NetworkMessageDestination destination = targetId == null ? NetworkMessageDestination.ALL : NetworkMessageDestination.SPECIFIC_PEER;

            foreach (LeaderboardPlayer player in Players)
            {
                NetworkMessage.Send(
                    NetworkUtils.MessageTypes.LEADERBOARD_DATA_SYNC,
                    destination,
                    new List<object> { player.PlayerId, player.Kills, player.Deaths, player.Wins },
                    targetId
                );
            }
        }

        public static bool AlreadyDisplaying()
        {
            foreach (var menu in Game1.onScreenMenus)
            {
                if (menu is Leaderboard)
                    return true;
            }

            return false;
        }

        public static void TryShow()
        {
            if (!AlreadyDisplaying())
            {
                if (SpectatorMode.InSpectatorMode)
                    Game1.displayHUD = true;
                Game1.onScreenMenus.Add(ModEntry.Leaderboard);
            }
        }

        public static void TryRemove()
        {
            if (AlreadyDisplaying())
            {
                if (SpectatorMode.InSpectatorMode)
                    Game1.displayHUD = false;
                Game1.onScreenMenus.Remove(ModEntry.Leaderboard);
            }
        }

        private void calculateLeaderboardPosition()
        {
            width = (int)(Game1.uiViewport.Width * percentOfViewportWide);
            height = (int)(Game1.uiViewport.Height * percentOfViewportTall);

            xPositionOnScreen = Game1.uiViewport.Width / 2 - width / 2;
            yPositionOnScreen = 100;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach (ClickableComponent header in headerComponents)
            {
                if (header.containsPoint(x, y))
                {
                    if (SortMethod != header.name)
                        SortMethod = header.name;
                    else
                        SortMethod += "+";

                    Players.Sort();
                    Game1.playSound("toolSwap");
                    return;
                }
            }
        }

        public void drawLeaderboardTitle(SpriteBatch spriteBatch)
        {
            Rectangle scaledBounds = new(
                (int)(titleTexture.Bounds.X * titleScale),
                (int)(titleTexture.Bounds.Y * titleScale),
                (int)(titleTexture.Bounds.Width * titleScale),
                (int)(titleTexture.Bounds.Height * titleScale)
            );
            ClickableTextureComponent Logo = new(
                "Logo",
                new Rectangle(xPositionOnScreen + (width / 2 - (scaledBounds.Width / 2)), currentDrawHeight, width - cornerSize, 80),
                "",
                "",
                titleTexture,
                titleTexture.Bounds,
                titleScale
            );
            Logo.draw(spriteBatch);

            currentDrawHeight -= 30;

            drawLeaderboardHorizontalLine(spriteBatch, currentDrawHeight);

            currentDrawHeight += cornerSize;
        }

        public void drawLeaderboardHorizontalLine(SpriteBatch spriteBatch, int y)
        {
            drawTextureBox(
                spriteBatch,
                Game1.menuTexture,
                horizontalLineRectangle,
                xPositionOnScreen,
                yPositionOnScreen - cornerSize + y,
                width,
                cornerSize,
                Color.White,
                drawShadow: false
            );
        }

        public void drawPlayer(SpriteBatch spriteBatch, LeaderboardPlayer player)
        {
            string name = (player.Player != null && player.Player.Name.Length == 0) ? "Unnamed Farmhand" : player.Player.Name;
            int textHeight = (int)Game1.smallFont.MeasureString(name).Y;

            if (textHeight + cornerSize + currentDrawHeight > height || !FarmerUtils.IsOnline(player.Player))
                return;

            Utility.drawTextWithShadow(
                spriteBatch,
                name,
                Game1.smallFont,
                new Vector2(
                    xPositionOnScreen + cornerSize,
                    yPositionOnScreen - cornerSize + currentDrawHeight + rowTopPadding
                ),
                Color.Black
            );

            int valueWidth = (int)Game1.smallFont.MeasureString(player.Kills.ToString()).X;

            Utility.drawTextWithShadow(
                spriteBatch,
                player.Kills.ToString(),
                Game1.smallFont,
                new Vector2(
                    xPositionOnScreen + cornerSize + width * 6 / 12 - (valueWidth / 2),
                    yPositionOnScreen - cornerSize + currentDrawHeight + rowTopPadding
                ),
                Color.Black
            );

            valueWidth = (int)Game1.smallFont.MeasureString(player.Deaths.ToString()).X;

            Utility.drawTextWithShadow(
                spriteBatch,
                player.Deaths.ToString(),
                Game1.smallFont,
                new Vector2(
                    xPositionOnScreen + cornerSize + width * 8 / 12 - (valueWidth / 2),
                    yPositionOnScreen - cornerSize + currentDrawHeight + rowTopPadding
                ),
                Color.Black
            );

            valueWidth = (int)Game1.smallFont.MeasureString(player.Wins.ToString()).X;

            Utility.drawTextWithShadow(
                spriteBatch,
                player.Wins.ToString(),
                Game1.smallFont,
                new Vector2(
                    xPositionOnScreen + cornerSize + width * 10 / 12 - (valueWidth / 2),
                    yPositionOnScreen - cornerSize + currentDrawHeight + rowTopPadding
                ),
                Color.Black
            );

            currentDrawHeight += textHeight;

            drawLeaderboardHorizontalLine(spriteBatch, currentDrawHeight);

            currentDrawHeight += cornerSize;
        }

        public void drawHeader(SpriteBatch spriteBatch)
        {
            headerComponents.Clear();

            Vector2 textSize = Game1.smallFont.MeasureString("Player");

            headerComponents.Add(new ClickableComponent(
                new Rectangle(
                    xPositionOnScreen + cornerSize,
                    yPositionOnScreen - cornerSize + currentDrawHeight + rowTopPadding,
                    (int)textSize.X,
                    (int)textSize.Y
                ),
                "Player"
             ));

            Utility.drawBoldText(
                spriteBatch,
                "Player",
                Game1.smallFont,
                new Vector2(
                    xPositionOnScreen + cornerSize,
                    yPositionOnScreen - cornerSize + currentDrawHeight + rowTopPadding
                ),
                Color.Black
            );

            textSize = Game1.smallFont.MeasureString("Kills");

            headerComponents.Add(new ClickableComponent(
                new Rectangle(
                    (int)(xPositionOnScreen + cornerSize + width * 6 / 12 - (textSize.X / 2)),
                    yPositionOnScreen - cornerSize + currentDrawHeight + rowTopPadding,
                    (int)textSize.X,
                    (int)textSize.Y
                ),
                "Kills"
             ));

            Utility.drawBoldText(
                spriteBatch,
                "Kills",
                Game1.smallFont,
                new Vector2(
                    xPositionOnScreen + cornerSize + width * 6 / 12 - (textSize.X / 2),
                    yPositionOnScreen - cornerSize + currentDrawHeight + rowTopPadding
                ),
                Color.Black
            );

            textSize = Game1.smallFont.MeasureString("Deaths");

            headerComponents.Add(new ClickableComponent(
                new Rectangle(
                    (int)(xPositionOnScreen + cornerSize + width * 8 / 12 - (textSize.X / 2)),
                    yPositionOnScreen - cornerSize + currentDrawHeight + rowTopPadding,
                    (int)textSize.X,
                    (int)textSize.Y
                ),
                "Deaths"
             ));

            Utility.drawBoldText(
                spriteBatch,
                "Deaths",
                Game1.smallFont,
                new Vector2(
                    xPositionOnScreen + cornerSize + width * 8 / 12 - (textSize.X / 2),
                    yPositionOnScreen - cornerSize + currentDrawHeight + rowTopPadding
                ),
                Color.Black
            );

            textSize = Game1.smallFont.MeasureString("Wins");

            headerComponents.Add(new ClickableComponent(
                new Rectangle(
                    (int)(xPositionOnScreen + cornerSize + width * 10 / 12 - (textSize.X / 2)),
                    yPositionOnScreen - cornerSize + currentDrawHeight + rowTopPadding,
                    (int)textSize.X,
                    (int)textSize.Y
                ),
                "Wins"
             ));

            Utility.drawBoldText(
                spriteBatch,
                "Wins",
                Game1.smallFont,
                new Vector2(
                    xPositionOnScreen + cornerSize + width * 10 / 12 - (textSize.X / 2),
                    yPositionOnScreen - cornerSize + currentDrawHeight + rowTopPadding
                ),
                Color.Black
            );

            currentDrawHeight += (int)textSize.Y;

            drawLeaderboardHorizontalLine(spriteBatch, currentDrawHeight);

            currentDrawHeight += cornerSize;
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            calculateLeaderboardPosition();

            drawTextureBox(
                spriteBatch,
                xPositionOnScreen,
                yPositionOnScreen,
                width,
                height,
                Color.White
            );

            currentDrawHeight = yPositionOnScreen + cornerSize;

            drawLeaderboardTitle(spriteBatch);
            drawHeader(spriteBatch);

            foreach (LeaderboardPlayer player in Players)
                drawPlayer(spriteBatch, player);
        }
    }
}
