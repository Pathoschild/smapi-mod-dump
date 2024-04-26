/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/scayze/multiprairie
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MultiPlayerPrairie;
using MultiplayerPrairieKing.Utility;
using StardewValley;
using System;
using System.Collections.Generic;
using static MultiPlayerPrairie.GameMultiplayerPrairieKing;

namespace MultiplayerPrairieKing.Components
{
    public class UI
    {
        readonly GameMultiplayerPrairieKing gameInstance;
        public static Texture2D StartScreenTexture { get; set; }
        public static Texture2D StartScreenPoppetjesTexture { get; set; }
        public static Texture2D CheckMarkTexture { get; set; }

        public bool onStartMenu;

        public int currentGameOverOption;

        public UI(GameMultiplayerPrairieKing gameInstance)
        {
            this.gameInstance = gameInstance;
        }

        public void Draw(SpriteBatch b)
        {
            if (onStartMenu)
            {
                DrawStartMenu(b);
            }
            else if ((gameInstance.gameOver || gameInstance.gamerestartTimer > 0) && !gameInstance.endCutscene)
            {
                DrawGameOverMenu(b);
            }
            else if (gameInstance.endCutscene) //Draw the final cutscene
            {

            }
            else if(!gameInstance.gopherRunning)
            {
                DrawGameOverlay(b);
            }
        }

        public void ProcessInputs(Dictionary<GameKeys, int> _buttonHeldFrames)
        {
            //Start the game after startmenu
            if (_buttonHeldFrames[GameKeys.UsePowerup] == 1 && onStartMenu && gameInstance.IsHost)
            {
                SaveState saveState = gameInstance.modInstance.GetSaveState();
                if (saveState == null || gameInstance.modInstance.playerList.Value.Count == saveState.playerSaveStates.Count)
                {
                    onStartMenu = false;
                    gameInstance.InstantiatePlayers();
                    Game1.playSound("Pickup_Coin15");
                    PK_StartNewGame mNewGame = new();
                    gameInstance.modInstance.SyncMessage(mNewGame);
                    _buttonHeldFrames[GameKeys.UsePowerup] = 2;
                }
            }

            if (_buttonHeldFrames[GameKeys.MoveUp] == 1 && gameInstance.gameOver)
            {
                currentGameOverOption = Math.Max(0, currentGameOverOption - 1);
                Game1.playSound("Cowboy_gunshot");
            }

            if (_buttonHeldFrames[GameKeys.MoveDown] == 1 && gameInstance.gameOver)
            {
                currentGameOverOption = Math.Min(1, currentGameOverOption + 1);
                Game1.playSound("Cowboy_gunshot");
            }

            if (_buttonHeldFrames[GameKeys.SelectOption] == 1 && gameInstance.gameOver)
            {
                if (currentGameOverOption == 1)
                {
                    gameInstance.quit = true;
                }
                else
                {
                    if(gameInstance.IsHost)
                    {
                        gameInstance.gamerestartTimer = 1500;
                        gameInstance.gameOver = false;
                        currentGameOverOption = 0;
                        Game1.playSound("Pickup_Coin15");

                        PK_RestartGame message = new();
                        gameInstance.modInstance.SyncMessage(message);
                    }
                }
            }
        }

        void DrawGameOverlay(SpriteBatch b)
        {
            b.Draw(Game1.mouseCursors, topLeftScreenCoordinate - new Vector2(TileSize + 27, 0f), new Rectangle(294, 1782, 22, 22), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.25f);
            if (gameInstance.player.heldItem != null)
            {
                b.Draw(Game1.mouseCursors, topLeftScreenCoordinate - new Vector2(TileSize + 18, -9f), new Rectangle(272 + (int)gameInstance.player.heldItem.type * 16, 1808, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.5f);
            }
            b.Draw(Game1.mouseCursors, topLeftScreenCoordinate - new Vector2(TileSize * 2, -TileSize - 18), new Rectangle(400, 1776, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.5f);
            b.DrawString(Game1.smallFont, "x" + Math.Max(gameInstance.lives, 0), topLeftScreenCoordinate - new Vector2(TileSize, -TileSize - TileSize / 4 - 18), Color.White);
            b.Draw(Game1.mouseCursors, topLeftScreenCoordinate - new Vector2(TileSize * 2, -TileSize * 2 - 18), new Rectangle(272, 1808, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.5f);
            b.DrawString(Game1.smallFont, "x" + gameInstance.Coins, topLeftScreenCoordinate - new Vector2(TileSize, -TileSize * 2 - TileSize / 4 - 18), Color.White);
            for (int j = 0; j < gameInstance.currentLevel + gameInstance.newGamePlus * 12; j++)
            {
                b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(TileSize * 16 + 3, j * 3 * 6), new Rectangle(512, 1760, 5, 5), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.5f);
            }
            b.Draw(Game1.mouseCursors, new Vector2((int)topLeftScreenCoordinate.X, (int)topLeftScreenCoordinate.Y - TileSize / 2 - 12), new Rectangle(595, 1748, 9, 11), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.5f);
            if (!gameInstance.shootoutLevel)
            {
                b.Draw(Game1.staminaRect, new Rectangle((int)topLeftScreenCoordinate.X + 30, (int)topLeftScreenCoordinate.Y - TileSize / 2 + 3, (int)((float)(16 * TileSize - 30) * ((float)gameInstance.waveTimer / 80000f)), TileSize / 4), (gameInstance.waveTimer < 8000) ? new Color(188, 51, 74) : new Color(147, 177, 38));
            }
            if (gameInstance.betweenWaveTimer > 0 && gameInstance.currentLevel == 0 && !gameInstance.scrollingMap)
            {
                Vector2 pos = new(Game1.viewport.Width / 2 - 120, Game1.viewport.Height - 144 - 3);
                if (!Game1.options.gamepadControls)
                {
                    b.Draw(Game1.mouseCursors, pos, new Rectangle(352, 1648, 80, 48), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.99f);
                }
                else
                {
                    b.Draw(Game1.controllerMaps, pos, StardewValley.Utility.controllerMapSourceRect(new Rectangle(681, 157, 160, 96)), Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0.99f);
                }
            }

            //Draw Powerup Icons
            if (gameInstance.player.bulletDamage > 1)
            {
                b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(-TileSize - 3, 16 * TileSize - TileSize), new Rectangle(416 + (gameInstance.player.ammoLevel - 1) * 16, 1776, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.5f);
            }
            if (gameInstance.player.fireSpeedLevel > 0)
            {
                b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(-TileSize - 3, 16 * TileSize - TileSize * 2), new Rectangle(320 + (gameInstance.player.fireSpeedLevel - 1) * 16, 1776, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.5f);
            }
            if (gameInstance.player.runSpeedLevel > 0)
            {
                b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(-TileSize - 3, 16 * TileSize - TileSize * 3), new Rectangle(368 + (gameInstance.player.runSpeedLevel - 1) * 16, 1776, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.5f);
            }
            if (gameInstance.player.spreadPistol)
            {
                b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(-TileSize - 3, 16 * TileSize - TileSize * 4), new Rectangle(464, 1776, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.5f);
            }
        }

        void DrawGameOverMenu(SpriteBatch b)
        {
            b.Draw(Game1.staminaRect, new Rectangle((int)topLeftScreenCoordinate.X, (int)topLeftScreenCoordinate.Y, 16 * TileSize, 16 * TileSize), Game1.staminaRect.Bounds, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 0.0001f);
            b.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:AbigailGame.cs.11914"), topLeftScreenCoordinate + new Vector2(6f, 7f) * TileSize, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            b.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:AbigailGame.cs.11914"), topLeftScreenCoordinate + new Vector2(6f, 7f) * TileSize + new Vector2(-1f, 0f), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            b.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:AbigailGame.cs.11914"), topLeftScreenCoordinate + new Vector2(6f, 7f) * TileSize + new Vector2(1f, 0f), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            string option = Game1.content.LoadString("Strings\\StringsFromCSFiles:AbigailGame.cs.11917");
            if ((OPTION_TYPE)currentGameOverOption == OPTION_TYPE.RETRY)
            {
                option = "> " + option;
            }
            string option2 = Game1.content.LoadString("Strings\\StringsFromCSFiles:AbigailGame.cs.11919");
            if ((OPTION_TYPE)currentGameOverOption == OPTION_TYPE.QUIT)
            {
                option2 = "> " + option2;
            }
            if (gameInstance.gamerestartTimer <= 0 || gameInstance.gamerestartTimer / 500 % 2 == 0)
            {
                b.DrawString(Game1.smallFont, option, topLeftScreenCoordinate + new Vector2(6f, 9f) * TileSize, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }
            b.DrawString(Game1.smallFont, option2, topLeftScreenCoordinate + new Vector2(6f, 9f) * TileSize + new Vector2(0f, TileSize * 2 / 3), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }

        void DrawStartMenu(SpriteBatch b)
        {
            //Draw start menu

            b.Draw(
                Game1.staminaRect,
                new Rectangle((int)topLeftScreenCoordinate.X, (int)topLeftScreenCoordinate.Y, 16 * TileSize, 16 * TileSize),
                Game1.staminaRect.Bounds,
                Color.Black,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.97f
            );
            b.Draw(
                StartScreenTexture,
                new Vector2(Game1.viewport.Width / 2, Game1.viewport.Height / 2),
                new Rectangle(0, 0, 256, 256),
                Color.White,
                0f,
                new Vector2(128, 128),
                3f,
                SpriteEffects.None,
                1f
            );

            //Draw the four poppetjes symbolizing the four joined players
            List<Vector2> poppetjePositions = new()
            {
                new Vector2(65, 134),
                new Vector2(82, 134),
                new Vector2(159, 134),
                new Vector2(176, 134)
            };

            for (int i = 0; i < 4; i++)
            {
                bool playerJoined = i < gameInstance.modInstance.playerList.Value.Count;
                Rectangle sourceRect;

                if (playerJoined) sourceRect = new Rectangle(16 * i, 0, 16, 16);
                else sourceRect = new Rectangle(16 * 4, 0, 16, 16);

                b.Draw(
                    StartScreenPoppetjesTexture,
                    topLeftScreenCoordinate + poppetjePositions[i] * 3f,
                    sourceRect,
                    playerJoined ? Color.White : new Color(255, 255, 255, 20),
                    0f,
                    Vector2.Zero,
                    3f,
                    SpriteEffects.None,
                    1f
                );
            }

            //When continueing save game, display which characters should be visible
            ModMultiPlayerPrairieKing modInstance = gameInstance.modInstance;

            //If this player is the host, there needs to be a savefile present to indicate a continued game
            if (modInstance.isHost.Value && modInstance.GetSaveState() == null) return;
            //If this player joined a host, multiplayerSaveState has to be set to indicate a continued game
            if (!modInstance.isHost.Value && gameInstance.multiplayerSaveState == null) return;

            int iterator = 0;
            foreach (PlayerSaveState ps in modInstance.GetSaveState().playerSaveStates)
            {
                Rectangle sourceRect;
                bool playerJoined = modInstance.playerList.Value.Contains(ps.PlayerID);

                if (playerJoined) sourceRect = new Rectangle(0, 0, 7, 7);
                else sourceRect = new Rectangle(7, 0, 7, 7);

                b.Draw(
                    CheckMarkTexture,
                    topLeftScreenCoordinate + new Vector2(315 - 21 - 30, 550 + 40 * iterator),
                    sourceRect,
                    new Color(255, 255, 255, 20),
                    0f,
                    Vector2.Zero,
                    3f,
                    SpriteEffects.None,
                    1f
                );

                b.DrawString(
                    Game1.smallFont,
                    ps.PlayerName,
                    topLeftScreenCoordinate + new Vector2(310, 547 + 40 * iterator),
                    new Color(242, 188, 82),
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    1f
                );

                iterator += 1;
            }
            
        }
    }
}
