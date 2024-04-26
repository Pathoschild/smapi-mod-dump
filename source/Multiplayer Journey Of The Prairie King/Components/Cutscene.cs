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
using Microsoft.Xna.Framework.Input;
using MultiPlayerPrairie;
using MultiplayerPrairieKing.Entities;
using MultiplayerPrairieKing.Utility;
using MultiplayerPrairieKing.Utility;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;
using static MultiPlayerPrairie.GameMultiplayerPrairieKing;

namespace MultiplayerPrairieKing.Components
{
    public class Cutscene
    {
        readonly GameMultiplayerPrairieKing gameInstance;

        public int endCutsceneTimer;

        int endCutscenePhase;

        public Cutscene(GameMultiplayerPrairieKing gameInstance)
        {
            this.gameInstance = gameInstance;
        }

        public void Reset()
        {
            endCutscenePhase = 0;
            endCutsceneTimer = 0;
        }

        public void Tick(GameTime time)
        {
            endCutsceneTimer -= time.ElapsedGameTime.Milliseconds;
            if (endCutsceneTimer < 0)
            {
                endCutscenePhase++;
                if (endCutscenePhase > 5)
                {
                    endCutscenePhase = 5;
                }
                switch (endCutscenePhase)
                {
                    case 1:
                        if (gameInstance.modInstance.Config.AchievementPrairieKingEnabled)
                        {
                            Game1.getSteamAchievement("Achievement_PrairieKing");
                        }

                        if (!gameInstance.died && gameInstance.modInstance.Config.AchievementFectorsChallengeEnabled)
                        {
                            Game1.getSteamAchievement("Achievement_FectorsChallenge");
                        }
                        //Game1.multiplayer.globalChatInfoMessage("PrairieKing", Game1.player.Name);
                        // TODO: Replacement with SMAPI?

                        endCutsceneTimer = 15500;
                        Game1.playSound("Cowboy_singing");
                        gameInstance.map = MapLoader.GetMap(-1);
                        break;
                    case 2:
                        gameInstance.player.position = new Vector2(0f, 8 * TileSize);

                        //NET Player Move
                        gameInstance.NETmovePlayer(gameInstance.player.position);

                        endCutsceneTimer = 12000;
                        break;
                    case 3:
                        endCutsceneTimer = 5000;
                        break;
                    case 4:
                        endCutsceneTimer = 1000;
                        break;
                    case 5:
                        if (Game1.input.GetKeyboardState().GetPressedKeys().Length == 0)
                        {
                            Game1.input.GetGamePadState();
                            if (Game1.input.GetGamePadState().Buttons.X != ButtonState.Pressed && Game1.input.GetGamePadState().Buttons.Start != ButtonState.Pressed && Game1.input.GetGamePadState().Buttons.A != ButtonState.Pressed)
                            {
                                break;
                            }
                        }
                        if (gameInstance.gamerestartTimer <= 0)
                        {
                            if (gameInstance.IsHost)
                            {
                                gameInstance.StartNewRound();
                                PK_StartNewGamePlus message = new();
                                gameInstance.modInstance.SyncMessage(message);
                            }

                        }
                        break;
                }
            }
            if (endCutscenePhase == 2 && gameInstance.player.position.X < (float)(9 * TileSize))
            {
                gameInstance.player.position.X += 1f;
                gameInstance.player.motionAnimationTimer += time.ElapsedGameTime.Milliseconds;
                gameInstance.player.motionAnimationTimer %= 400;
            }
        }

        public void Draw(SpriteBatch b)
        {
            switch (endCutscenePhase)
            {
                case 0:
                    b.Draw(Game1.staminaRect, new Rectangle((int)topLeftScreenCoordinate.X, (int)topLeftScreenCoordinate.Y, 16 * TileSize, 16 * TileSize), Game1.staminaRect.Bounds, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 0.0001f);
                    b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + gameInstance.player.position + new Vector2(0f, -TileSize / 4), new Rectangle(384, 1760, 16, 16), Color.White * ((endCutsceneTimer < 2000) ? (1f * ((float)endCutsceneTimer / 2000f)) : 1f), 0f, Vector2.Zero, 3f, SpriteEffects.None, gameInstance.player.position.Y / 10000f + 0.001f);
                    b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + gameInstance.player.position + new Vector2(0f, -TileSize * 2 / 3) + new Vector2(0f, -TileSize / 4), new Rectangle(320 + (int)gameInstance.player.GetHeldItem() * 16, 1776, 16, 16), Color.White * ((endCutsceneTimer < 2000) ? (1f * ((float)endCutsceneTimer / 2000f)) : 1f), 0f, Vector2.Zero, 3f, SpriteEffects.None, gameInstance.player.position.Y / 10000f + 0.002f);
                    break;
                case 4:
                case 5:
                    b.Draw(Game1.staminaRect, new Rectangle((int)topLeftScreenCoordinate.X, (int)topLeftScreenCoordinate.Y, 16 * TileSize, 16 * TileSize), Game1.staminaRect.Bounds, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 0.97f);
                    b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(6 * TileSize, 3 * TileSize), new Rectangle(224, 1744, 64, 48), Color.White * ((endCutsceneTimer > 0) ? (1f - ((float)endCutsceneTimer - 2000f) / 2000f) : 1f), 0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
                    if (endCutscenePhase == 5 && gameInstance.gamerestartTimer <= 0)
                    {
                        b.DrawString(Game1.smallFont, Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_PK_NewGame+"), topLeftScreenCoordinate + new Vector2(3f, 10f) * TileSize, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    }
                    break;
                case 1:
                case 2:
                case 3:
                    {
                        for (int x = 0; x < 16; x++)
                        {
                            for (int y = 0; y < 16; y++)
                            {
                                b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(x, y) * 16f * 3f + new Vector2(0f, gameInstance.newMapPosition - 16 * TileSize), new Rectangle(464 + 16 * (int)gameInstance.map[x, y] + ((gameInstance.map[x, y] == MAP_TILE.CACTUS && gameInstance.cactusDanceTimer > 800f) ? 16 : 0), 1680 - (int)gameInstance.world * 16, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0f);
                            }
                        }
                        b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(6 * TileSize, 3 * TileSize), new Rectangle(288, 1697, 64, 80), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.01f);
                        if (endCutscenePhase == 3)
                        {
                            b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(9 * TileSize, 7 * TileSize), new Rectangle(544, 1792, 32, 32), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.05f);
                            if (endCutsceneTimer < 3000)
                            {
                                b.Draw(Game1.staminaRect, new Rectangle((int)topLeftScreenCoordinate.X, (int)topLeftScreenCoordinate.Y, 16 * TileSize, 16 * TileSize), Game1.staminaRect.Bounds, Color.Black * (1f - (float)endCutsceneTimer / 3000f), 0f, Vector2.Zero, SpriteEffects.None, 1f);
                            }
                            break;
                        }
                        b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(10 * TileSize, 8 * TileSize), new Rectangle(272 - endCutsceneTimer / 300 % 4 * 16, 1792, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.02f);
                        if (endCutscenePhase == 2)
                        {
                            b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + gameInstance.player.position + new Vector2(4f, 13f) * 3f, new Rectangle(484, 1760 + (int)(gameInstance.player.motionAnimationTimer / 100f) * 3, 8, 3), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, gameInstance.player.position.Y / 10000f + 0.001f + 0.001f);
                            b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + gameInstance.player.position, new Rectangle(384, 1760, 16, 13), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, gameInstance.player.position.Y / 10000f + 0.002f + 0.001f);
                            b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + gameInstance.player.position + new Vector2(0f, -TileSize * 2 / 3 - TileSize / 4), new Rectangle(320 + (int)gameInstance.player.GetHeldItem() * 16, 1776, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, gameInstance.player.position.Y / 10000f + 0.005f);
                        }
                        b.Draw(Game1.staminaRect, new Rectangle((int)topLeftScreenCoordinate.X, (int)topLeftScreenCoordinate.Y, 16 * TileSize, 16 * TileSize), Game1.staminaRect.Bounds, Color.Black * ((endCutscenePhase == 1 && endCutsceneTimer > 12500) ? ((float)((endCutsceneTimer - 12500) / 3000)) : 0f), 0f, Vector2.Zero, SpriteEffects.None, 1f);
                        break;
                    }
            }
        }
    }
}
