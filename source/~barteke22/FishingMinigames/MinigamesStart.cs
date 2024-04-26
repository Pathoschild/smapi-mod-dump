/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/barteke22/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Minigames;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FishingMinigames
{
    class MinigamesStart
    {
        private IMonitor Monitor;
        private Farmer who;
        private int fishingLevel;
        private int screen;
        public Action EmergencyCancel;
        private int whichFish;
        private bool bossFish;
        private string behavior;



        private bool debug;
        public int fishingFestivalMinigame;        //0 none, 1 fall16, 2 winter8
        public int festivalTimer;

        public int minigameStage = 0;          //0 before, 1 fade in, 2 playing, 3 show score, 4 cancel, 5 fail, 7-10 = score
        private int minigameTimer;
        private string[] minigameArrowData;    //0 arrow direction, 1 colour, 2 offset, 3 current distance
        public int[] minigameData;            //0 current arrow, 1 perfect area?, 2 score, 3 last arrow to vanish, 4 treasure arrow, 5 fade
        public int minigameDiff;
        private List<string> minigameText;

        //public float oldZoom;
        //public bool oldViewportFollow;


        public static int[] minigameStyle;
        public static Texture2D[] minigameTextures;
        public static Color minigameColor;
        public static float startMinigameScale;
        public static bool bossTransparency;
        public static float[] minigameDifficulty;
        public static bool[] tutorialSkip;

        private Minigames data;


        public MinigamesStart(Minigames data, IMonitor Monitor)
        {
            this.data = data;
            this.Monitor = Monitor;
            whichFish = data.whichFish;
            behavior = data.behavior;
            debug = data.debug;

            who = Game1.player;
            fishingLevel = who.FishingLevel;
            bossFish = FishingRod.isFishBossFish(whichFish);
            screen = Context.ScreenId;
        }


        public void StartMinigameSetup()
        {
            //starting minigame init
            //oldZoom = Game1.options.zoomLevel;
            //oldViewportFollow = who.currentLocation.forceViewportPlayerFollow;
            //Game1.viewportFreeze = true;
            //who.currentLocation.forceViewportPlayerFollow = false;

            minigameData = new int[6];

            if (minigameStyle[screen] > 1) data.Helper.Multiplayer.SendMessage(true, "hideText", modIDs: new[] { "barteke22.FishingInfoOverlays" }, new[] { who.UniqueMultiplayerID });//hide overlay text (for text based)

            minigameText = new List<string>();
            foreach (string s in data.translate.Get("Minigame.InfoDDR" + ((fishingFestivalMinigame == 0) ? "" : "_Festival")).ToString().Split(new string[] { "\n" }, StringSplitOptions.None)) minigameText.Add(s);

            if (fishingFestivalMinigame == 0) minigameArrowData = new string[1 + (int)Math.Abs(minigameDiff * 7f) + ((bossFish) ? 20 : 0)];
            else minigameArrowData = new string[999];

            Random r = new Random();
            int arrow = 0;
            int offset = 0;
            for (int i = 0; i < minigameArrowData.Length; i++)
            {
                switch (behavior)
                {
                    case "smooth":
                        if (i > 0 && r.Next(0, 2) != 0) arrow = r.Next(0, 4);
                        break;
                    case "sinker":
                        if (i > 0 && r.Next(0, 3) == 0) arrow = 2;
                        else arrow = r.Next(0, 4);
                        break;
                    case "floater":
                        if (i > 0 && r.Next(0, 3) == 0) arrow = 0;
                        else arrow = r.Next(0, 4);
                        break;
                    case "dart":
                        int newR = r.Next(0, 4);
                        if (i > 0 && arrow == newR && r.Next(0, 3) == 0) arrow = newR;
                        else arrow = r.Next(0, 4);
                        break;
                    default:
                        arrow = r.Next(0, 4);
                        break;
                }
                minigameArrowData[i] = arrow + "/0/" + offset + "/9999";//0 arrow direction, 1 colour, 2 offset, 3 current distance
                if (r.Next(0, 3) == 0) offset += (int)(250 * startMinigameScale);
                else offset += (int)(140 * startMinigameScale);
            }
            //vanilla treasure chance calculation
            if (fishingFestivalMinigame == 0                                                    // Not in a festival
                && who.fishCaught != null && who.fishCaught.Count() > 1                         // Caught a fish
                && Game1.random.NextDouble()                                                    // RNG
                < (   who.LuckLevel * 0.005                                                     // Consumables + Rings
                    + data.effects["TREASURE"]                                                  // The modifier on rods
                    + who.DailyLuck / 2.0                                                       // Daily Luck
                    + 0.07                                                                      // Flat amount - in vanilla this is 15%, but we can fish 2-3x as fast so make it 7%
                    + ((who.professions.Contains(9) ? FishingRod.baseChanceForTreasure : 0))    // The Pirate profession
                  )
               )
            {
                minigameData[4] = Game1.random.Next(minigameArrowData.Length / 2, minigameArrowData.Length - 1);
            }
            else minigameData[4] = -1;
            minigameData[0] = -2;
            minigameData[5] = 0;

            minigameStage = 1;
            minigameTimer = 0;
        }


        public void MainInput(ButtonsChangedEventArgs e)//true cancels
        {
            if (minigameStage > 0 && minigameStage < 5)//already in startMinigame
            {
                if (minigameData[5] > 70 && minigameStage < 3 && (e.Pressed.Contains(SButton.Escape) || e.Pressed.Contains(SButton.ControllerB))) //cancel
                {
                    data.Helper.Input.Suppress(SButton.Escape);
                    data.Helper.Input.Suppress(SButton.ControllerB);
                    minigameStage = 4;
                    minigameData[5] = 70;
                }
                else if (minigameStage > 1 && minigameStage != 4) InputDDR();
            }
        }
        public void MainUpdateTicked(UpdateTickingEventArgs e)
        {
            if (minigameStage > 0 && minigameStage < 5 && minigameData[5] > 70 && e.Ticks % 240 == 0) who.netDoEmote("game");
        }
        public void MainRendered(SpriteBatch batch)
        {
            if (minigameStyle[screen] == 1 && ((minigameStage > 0 && minigameStage < 5) || (minigameStage > 4 && minigameData[5] > 0))) DrawDDR(batch);
            if (minigameStyle[screen] == 2 && ((minigameStage > 0 && minigameStage < 5) || (minigameStage > 4 && minigameData[5] > 0))) DrawHangman(batch);
        }

        private float DrawIntro(SpriteBatch batch, Vector2 screenMid, float scale)
        {
            if (minigameStage == 1)//fade in (opacity calc)
            {
                minigameData[5]++;

                //if (minigameData[5] < 70)
                //{
                //    float f = (minigameData[5] / 250f) + 0.01f;
                //    if (Context.IsSplitScreen) Game1.options.localCoopBaseZoomLevel += f;
                //    else Game1.options.singlePlayerBaseZoomLevel += f;

                //    Game1.viewport.X = Math.Max(who.getStandingX() - (Game1.viewport.Width / 2), 0);
                //    Game1.viewport.Y = Math.Max(who.getStandingY() - Math.Min(minigameData[5] * 2, 80) - (Game1.viewport.Height / 2), 0);
                //}
                //else 
                if (minigameData[5] == 300)
                {
                    if (fishingFestivalMinigame == 0)
                    {
                        data.Helper.Multiplayer.SendMessage((whichFish < 167 || whichFish > 172) ? whichFish : 168, "whichFish", modIDs: new[] { "barteke22.FishingInfoOverlays" }, new[] { who.UniqueMultiplayerID });//notify overlay of which fish
                    }
                    Game1.activeClickableMenu = new DummyMenu();
                    minigameStage = 2;
                    Game1.playSound("FishHit");
                }
            }
            else if (fishingFestivalMinigame != 0 && festivalTimer <= 2000)
            {
                data.Helper.Multiplayer.SendMessage(false, "hideText", modIDs: new[] { "barteke22.FishingInfoOverlays" }, new[] { who.UniqueMultiplayerID });//clear overlay
                minigameData[5] -= 4;
                minigameStage = 4;
                if ((Game1.currentMinigame as FishingGame).perfections == 0)
                {
                    if (fishingFestivalMinigame == 1)
                    {
                        for (int i = 0; i < minigameData[2] / 30; i++)
                        {
                            Game1.CurrentEvent.perfectFishing();
                        }
                    }
                }
            }
            float opacity = 0f;

            Texture2D animTexture = Game1.content.Load<Texture2D>("TileSheets\\animations");




            Vector2 whoPos = who.getStandingPosition() + new Vector2(-Game1.viewport.X, -110f - Game1.viewport.Y);

            float s = Math.Min(minigameData[5] / 23f, 3f);
            Vector2 offsetToMid = new Vector2(screenMid.X - whoPos.X + 32f, screenMid.Y - whoPos.Y + 110f);
            if (fishingFestivalMinigame != 1)
            {
                offsetToMid = new Vector2(screenMid.X - Utility.ModifyCoordinateForUIScale(whoPos.X - 32f), screenMid.Y - Utility.ModifyCoordinateForUIScale(whoPos.Y + 110f));
                whoPos = Utility.ModifyCoordinatesForUIScale(whoPos);
                s = Utility.ModifyCoordinateForUIScale(s);
            }

            batch.Draw(animTexture, whoPos + (offsetToMid * 0.05f), new Rectangle(0, 320, 64, 64), Color.White, 0f, new Vector2(30f, 46f), s, SpriteEffects.None, 0.0f);
            if (minigameData[5] > 15)
            {
                batch.Draw(animTexture, whoPos + (offsetToMid * 0.1f) + new Vector2(0f, -10f), new Rectangle(64, 320, 64, 64), Color.White, 0f, new Vector2(33f, 41f), s, SpriteEffects.None, 0.0f);
            }
            if (minigameData[5] > 30)
            {
                batch.Draw(animTexture, whoPos + (offsetToMid * 0.18f) + new Vector2(0f, -20f), new Rectangle(128, 320, 64, 64), Color.White, 0f, new Vector2(37f, 38f), s, SpriteEffects.None, 0.0f);
            }
            if (minigameData[5] > 45)
            {
                batch.Draw(animTexture, whoPos + (offsetToMid * 0.4f), new Rectangle(192, 320, 64, 64), Color.White, 0f, new Vector2(32f), s, SpriteEffects.None, 0.0f);
            }
            if (minigameData[5] > 70)
            {
                opacity = (minigameData[5] - 30) / 100f;
            }


            //board
            if (opacity < 2.7f)
            {
                batch.Draw(minigameTextures[3], new Rectangle((int)screenMid.X, (int)screenMid.Y, (int)(scale * 324f * Math.Min(opacity, 0.5f)), (int)(scale * 176f * Math.Min(opacity, 0.5f))), new Rectangle(0, 0, 84, 48), Color.White * opacity, 0f, new Vector2(42f, 24f), SpriteEffects.None, 0.1f);

                batch.Draw(minigameTextures[0], screenMid, null, minigameColor * Math.Min((opacity / 1.5f), 1f), 0f, new Vector2(69f, 37f), scale * 2f * Math.Min(opacity, 0.5f), SpriteEffects.None, 0.3f);
            }
            else
            {
                batch.Draw(minigameTextures[3], new Rectangle((int)screenMid.X, (int)screenMid.Y, (int)(scale * 162f), (int)(scale * 88f)), new Rectangle(0, 0, 84, 48), Color.White, 0f, new Vector2(42f, 24f), SpriteEffects.None, 0.1f);

                batch.Draw(minigameTextures[0], screenMid, null, minigameColor, 0f, new Vector2(69f, 37f), scale, SpriteEffects.None, 0.3f);
            }
            return opacity;
        }



        private void DrawDDR(SpriteBatch batch)
        {
            //scale/middle/bounds calculation
            float scale = 7f * startMinigameScale;
            if (fishingFestivalMinigame == 1) scale *= (1f / Game1.options.zoomLevel) * Game1.options.uiScale;
            int width = (int)Math.Round(138f * scale);
            int height = (int)Math.Round(74f * scale);
            Vector2 screenMid = new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width / 2, Game1.graphics.GraphicsDevice.Viewport.Height / 2);

            float opacity = DrawIntro(batch, screenMid, scale);//intro

            if (minigameData[3] != minigameArrowData.Length)//minigame not done
            {
                int festivalDifficulty = 1;//festival stuff
                if (fishingFestivalMinigame == 1) festivalDifficulty += (Game1.currentMinigame as FishingGame).fishCaught;
                else if (fishingFestivalMinigame == 2) festivalDifficulty += who.festivalScore;

                //info
                Vector2 textLoc = new Vector2(0f, height * -0.44f);
                for (int i = 0; i < minigameText.Count; i++)
                {
                    DrawStringWithBorder(batch, Game1.smallFont, minigameText[i], screenMid + (textLoc += new Vector2(0f, height * 0.07f)), minigameColor * Math.Min(opacity, 1.5f), 0f, new Vector2(Game1.smallFont.MeasureString(minigameText[i]).X / 2f, 0f), scale * 0.16f, SpriteEffects.None, 0.4f);
                }
                if (fishingFestivalMinigame != 0)
                {
                    DrawStringWithBorder(batch, Game1.smallFont, (festivalDifficulty * 20 - minigameData[2]).ToString(), screenMid + (textLoc += new Vector2(0f, height * 0.05f)), minigameColor * Math.Min(opacity, 1f), 0f, new Vector2(Game1.smallFont.MeasureString((festivalDifficulty * 20 - minigameData[2]).ToString()).X / 2f, 0f), scale * 0.4f, SpriteEffects.None, 0.4f);
                }

                //if paused/out of focus:
                if ((Game1.paused || !Game1.game1.IsActiveNoOverlay) && (Game1.options == null || Game1.options.pauseWhenOutOfFocus || Game1.paused))
                {
                    batch.Draw(Game1.mouseCursors, screenMid, new Rectangle(322, 498, 12, 12), Color.Brown, 0f, new Vector2(6f), scale * 2f, SpriteEffects.None, 0.4f);
                    return;
                }
                //hit area rings
                Vector2 hitAreaMid = screenMid + new Vector2((int)(width * -0.2f), height * 0.18f);
                batch.Draw(minigameTextures[1], hitAreaMid, new Rectangle(355, 86, 26, 26), minigameColor * (opacity / 3f), 0f, new Vector2(13f), scale * 0.7f, SpriteEffects.None, 0.4f);
                batch.Draw(minigameTextures[1], hitAreaMid, new Rectangle(355, 86, 26, 26), new Color(Color.Brown.ToVector3() + (minigameColor.ToVector3() * 0.6f)) * (opacity / 3f), 0f, new Vector2(13f), scale * 0.5f, SpriteEffects.None, 0.41f);

                //arrows
                Vector2 firstArrowLoc = new Vector2((int)(screenMid.X + (width / 2f)) + minigameTimer, screenMid.Y + (height * 0.18f));

                float speed = 2f;
                if (minigameStage == 2)
                {
                    if ((fishingFestivalMinigame == 0 && tutorialSkip[screen]) || debug) speed -= (minigameDiff * data.effects["SPEED"]);
                    else if (fishingFestivalMinigame == 1) speed += (festivalDifficulty / 2f - (fishingLevel / 10f)) * minigameDifficulty[screen];
                    else if (fishingFestivalMinigame == 2) speed += (festivalDifficulty / 1.6f - (fishingLevel / 10f)) * minigameDifficulty[screen];

                    if (speed <= 1) speed = 1f;
                    minigameTimer -= (int)(startMinigameScale * speed);
                }


                minigameData[0] = -2;//-2 to not clash with treasure arrow
                minigameData[1] = 0;
                int arrowsLeft = minigameArrowData.Length;

                for (int i = 0; i < minigameArrowData.Length; i++)
                {
                    if (i == minigameData[4] && minigameTimer % (speed * 200) == 0)//flip treasure arrow clockwise
                    {
                        if (int.Parse(minigameArrowData[i][0].ToString()) == 3) minigameArrowData[i] = minigameArrowData[i].Remove(0, 1).Insert(0, "0");
                        else minigameArrowData[i] = minigameArrowData[i].Remove(0, 1).Insert(0, (int.Parse(minigameArrowData[i][0].ToString()) + 1).ToString());
                    }

                    float[] arrowData = minigameArrowData[i].Split('/').Select(float.Parse).ToArray();//data = 0 direction, 1 colour, 2 offset from first, 3 current loc

                    if (arrowData[1] == 0f)//if empty arrow
                    {
                        if (hitAreaMid.X - (13f * scale * 0.5f) > firstArrowLoc.X + arrowData[2])//too late - fail
                        {
                            if (data.effects["LIFE"] > 0 && i != minigameData[4] && Game1.random.Next(0, 2) == 0)//saved by tackle?
                            {
                                data.effects["LIFE"]--;
                                arrowData[1] = 1.1f;
                                minigameArrowData[i] = minigameArrowData[i].Replace("/0/", "/1.1/");
                                Game1.playSound("button1");
                            }
                            else
                            {
                                arrowData[1] = -1f;
                                minigameArrowData[i] = minigameArrowData[i].Replace("/0/", "/-1/");
                                Game1.playSound("crit");
                            }
                        }
                        else if (hitAreaMid.X - (13f * scale * 0.5f) <= firstArrowLoc.X + arrowData[2] &&
                                 hitAreaMid.X + (13f * scale * 0.5f) >= firstArrowLoc.X + arrowData[2])      //in regular hit area
                        {
                            minigameData[0] = i;

                            if (hitAreaMid.X - (13f * scale * 0.1f * data.effects["AREA"]) <= firstArrowLoc.X + arrowData[2] &&
                                hitAreaMid.X + (13f * scale * 0.1f * data.effects["AREA"]) >= firstArrowLoc.X + arrowData[2]) minigameData[1] = 1; //+ in perfect area
                        }
                    }


                    if (firstArrowLoc.X + arrowData[2] + (6f * scale) <= screenMid.X + (width * 0.464f))//arrow passed start
                    {
                        arrowsLeft--;

                        if (firstArrowLoc.X + arrowData[2] - (6f * scale) >= screenMid.X - (width * 0.464f))//arrow didn't pass end
                        {
                            float arrowOpacity = opacity > 2.6f ? (bossFish && bossTransparency && i % 10 > 4 ? 0.5f : 1f) : 0f;

                            Color color = (arrowData[1] == 2f) ? Color.LimeGreen : ((int)arrowData[1] == 1) ? Color.Orange : (arrowData[1] == -1f) ? Color.Red : (i == minigameData[4]) ? Color.LightPink : minigameColor;
                            batch.Draw(minigameTextures[1], firstArrowLoc + new Vector2((arrowData[2]), 0), new Rectangle((arrowData[0] == 0f || arrowData[0] == 2f) ? 338 : 322, 82, 12, 12),
                                color * arrowOpacity, 0f, new Vector2(6f), scale, (arrowData[0] == 0f) ? SpriteEffects.FlipVertically : (arrowData[0] == 3f) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0.4f);
                            //treasure arrow
                            if (i == minigameData[4]) batch.Draw(Game1.mouseCursors, firstArrowLoc + new Vector2((arrowData[2]), 0), new Rectangle((data.treasureCaught) ? 104 : (arrowData[1] == -1f) ? 167 : 71, 1926, 20, 26),
                                Color.White * arrowOpacity, 0f, new Vector2(9f, 14f), scale * 0.2f, SpriteEffects.None, 0.41f);
                            //saved arrow
                            if (arrowData[1] == 1.1f && who.CurrentTool.attachments[1] != null) batch.Draw(Game1.objectSpriteSheet, firstArrowLoc + new Vector2((arrowData[2]), 0),
                                Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, who.CurrentTool.attachments[1].ParentSheetIndex, 16, 16), Color.White * arrowOpacity, 0f, new Vector2(8f), scale * 0.2f, SpriteEffects.None, 0.42f);
                        }
                        else if (i + 1 > minigameData[3])//update score when new arrow passes end
                        {
                            minigameData[3] = i + 1;
                            minigameData[2] = 0;
                            for (int j = 0; j < i + 1; j++)
                            {
                                minigameData[2] += (int)float.Parse(minigameArrowData[j].Split('/')[1]);
                            }
                            if (fishingFestivalMinigame != 0 && festivalDifficulty * 20 <= minigameData[2])
                            {
                                Game1.CurrentEvent.caughtFish(137, festivalDifficulty * 2, who);
                                if (Minigames.voices.TryGetValue(Minigames.voiceType[screen], out SoundEffect sfx)) sfx.Play(Minigames.voiceVolume * 0.5f, Minigames.voicePitch[screen], 0);
                            }
                        }
                    }
                }

                //arrow 'dispensers'
                batch.Draw(Game1.mouseCursors, screenMid + new Vector2(width * 0.464f, height * 0.18f), new Rectangle(301, 288, 15, 15), minigameColor * Math.Min(opacity, 0.95f), 0f, new Vector2(15f, 7.5f), scale, SpriteEffects.None, 0.5f);
                batch.Draw(Game1.mouseCursors, screenMid + new Vector2(width * -0.464f, height * 0.18f), new Rectangle(301, 288, 15, 15), minigameColor * Math.Min(opacity, 0.95f), 0f, new Vector2(0f, 7.5f), scale, SpriteEffects.FlipHorizontally, 0.5f);
                //score count
                DrawStringWithBorder(batch, Game1.smallFont, minigameData[2].ToString(), screenMid + new Vector2(width * -0.41f, height * 0.19f),
                    ((minigameData[2] < minigameArrowData.Length * 0.38f) ? Color.Crimson :
                    (minigameData[2] < minigameArrowData.Length * 0.76f) ? Color.DarkOrange :
                    (minigameData[2] < minigameArrowData.Length * 1.14f) ? Color.Yellow :
                    (minigameData[2] < minigameArrowData.Length * 1.52f) ? Color.GreenYellow :
                    (minigameData[2] < minigameArrowData.Length * 1.9f) ? Color.LimeGreen : Color.Purple) * (opacity / 3f),
                    0f, Game1.smallFont.MeasureString(minigameData[2].ToString()) / 2f, scale * 0.28f, SpriteEffects.None, 0.51f);
                //arrows left count
                batch.DrawString(Game1.smallFont, arrowsLeft.ToString(), screenMid + new Vector2(width * 0.41f, height * 0.19f), minigameColor * Math.Min(opacity, 1f), 0f, Game1.smallFont.MeasureString(arrowsLeft.ToString()) / 2f, scale * 0.28f, SpriteEffects.None, 0.51f);
            }
            else//final score screen
            {
                Color color = (minigameData[2] < minigameArrowData.Length * 0.38f) ? Color.Crimson :
                    (minigameData[2] < minigameArrowData.Length * 0.76f) ? Color.DarkOrange :
                    (minigameData[2] < minigameArrowData.Length * 1.14f) ? Color.Yellow :
                    (minigameData[2] < minigameArrowData.Length * 1.52f) ? Color.GreenYellow :
                    (minigameData[2] < minigameArrowData.Length * 1.9f) ? Color.LimeGreen : Color.Purple;

                if ((minigameStage < 3) && color != Color.Crimson && color != Color.DarkOrange) Game1.playSound("reward");

                //text
                string score = data.translate.Get("Minigame.Score") + " " + ((minigameData[2] < 0) ? "@ 0" : minigameData[2].ToString());
                string score2 = data.translate.Get("Minigame.Score2") + " " + (int)Math.Ceiling(minigameArrowData.Length * 0.76f);
                string scoreX = (color == Color.Purple) ? Game1.content.LoadString("Strings\\UI:BobberBar_Perfect") : data.translate.Get("Minigame.Score_" + ((color == Color.Crimson) ? 0 : (color == Color.Yellow || color == Color.GreenYellow) ? 2 : (color == Color.LimeGreen) ? 3 : 1));
                DrawStringWithBorder(batch, Game1.smallFont, score, screenMid + new Vector2(0f, -0.28f * height), color * (opacity / 3f), 0f, Game1.smallFont.MeasureString(score) / 2f, scale * 0.4f, SpriteEffects.None, 0.4f);
                DrawStringWithBorder(batch, Game1.smallFont, score2, screenMid + new Vector2(0f, -0.14f * height), color * (opacity / 3f), 0f, Game1.smallFont.MeasureString(score2) / 2f, scale * 0.3f, SpriteEffects.None, 0.4f);
                DrawStringWithBorder(batch, Game1.smallFont, scoreX, screenMid + new Vector2(0f, 0.02f * height), color * (opacity / 3f), 0f, Game1.smallFont.MeasureString(scoreX) / 2f, scale * 0.3f, SpriteEffects.None, 0.4f);

                //bar
                Rectangle whitePixel = new Rectangle(36, 1875, 1, 1);
                batch.Draw(Game1.mouseCursors, new Rectangle((int)(screenMid.X + (width * -0.403f)), (int)(screenMid.Y + (height * 0.184f)), (int)(0.806f * width), (int)(7f * scale)),
                    whitePixel, Color.Black * (opacity / 3f), 0f, Vector2.Zero, SpriteEffects.None, 0.4f);

                Rectangle bounds = new Rectangle((int)(screenMid.X + (width * -0.4f)), (int)(screenMid.Y + (height * 0.19f)), (int)(0.38f * 0.4f * width), (int)(6f * scale));
                batch.Draw(Game1.mouseCursors, bounds, whitePixel, Color.Crimson * (opacity / 3f), 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                bounds.X += bounds.Width;
                batch.Draw(Game1.mouseCursors, bounds, whitePixel, Color.DarkOrange * (opacity / 3f), 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                bounds.X += bounds.Width;
                batch.Draw(Game1.mouseCursors, new Rectangle(bounds.X, (int)(screenMid.Y + (height * 0.185f)), (int)(0.3f * scale), (int)(6.8f * scale)),
                    whitePixel, Color.LimeGreen * (opacity / 3f), 0f, Vector2.Zero, SpriteEffects.None, 0.4f);
                batch.Draw(Game1.mouseCursors, bounds, whitePixel, Color.Yellow * (opacity / 3f), 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                bounds.X += bounds.Width;
                batch.Draw(Game1.mouseCursors, bounds, whitePixel, Color.GreenYellow * (opacity / 3f), 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                bounds.X += bounds.Width;
                batch.Draw(Game1.mouseCursors, bounds, whitePixel, Color.LimeGreen * (opacity / 3f), 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                bounds.X += bounds.Width;
                bounds.Width = (int)(0.1f * 0.4f * width);
                batch.Draw(Game1.mouseCursors, bounds, whitePixel, Color.Purple * (opacity / 3f), 0f, Vector2.Zero, SpriteEffects.None, 0.5f);

                for (float i = 0; i < 0.799f * width; i += (1f / (minigameArrowData.Length * 2f)) * 0.799f * width)
                {
                    batch.Draw(Game1.mouseCursors, new Rectangle((int)(screenMid.X + (width * -0.4f) + i), (int)(screenMid.Y + (height * 0.19f)), (int)(0.15f * scale), (int)(6f * scale)),
                        whitePixel, Color.Gray * (opacity / 3f), 0f, Vector2.Zero, SpriteEffects.None, 0.4f);
                }
                batch.Draw(Game1.mouseCursors, screenMid + new Vector2(width * -0.4f + ((minigameData[2] < 1) ? 0 : (int)((minigameData[2] / (minigameArrowData.Length * 2f)) * 0.799f * width)), height * 0.2f),
                    new Rectangle(146, 1830, 9, 17), Color.Black * (opacity / 3f), 0f, new Vector2(4.5f, 17), 0.4f * scale, SpriteEffects.None, 0.5f);
                batch.Draw(Game1.mouseCursors, screenMid + new Vector2(width * -0.4f + ((minigameData[2] < 1) ? 0 : (int)((minigameData[2] / (minigameArrowData.Length * 2f)) * 0.799f * width)), height * 0.19f),
                    new Rectangle(146, 1830, 9, 17), color * (opacity / 3f), 0f, new Vector2(4.5f, 17), 0.3f * scale, SpriteEffects.None, 0.6f);

                if (minigameStage < 5) minigameStage = 3;
            }
        }
        private void InputDDR()
        {
            if (minigameStage == 2)
            {
                if (minigameData[0] >= 0)
                {
                    bool passed = false;
                    switch (minigameArrowData[minigameData[0]].Split('/')[0])
                    {
                        case "0"://up
                            if (KeybindList.Parse("W, Up, DPadUp").JustPressed()) passed = true;
                            break;
                        case "1"://right
                            if (KeybindList.Parse("D, Right, DPadRight").JustPressed()) passed = true;
                            break;
                        case "2"://down
                            if (KeybindList.Parse("S, Down, DPadDown").JustPressed()) passed = true;
                            break;
                        case "3"://left
                            if (KeybindList.Parse("A, Left, DPadLeft").JustPressed()) passed = true;
                            break;
                    }
                    if (passed)
                    {
                        if (minigameData[0] == minigameData[4])//treasure arrow?
                        {
                            data.treasureCaught = true;
                            Game1.playSound("coin");
                        }
                        if (minigameData[1] == 1)//perfect?
                        {
                            minigameArrowData[minigameData[0]] = minigameArrowData[minigameData[0]].Replace("/0/", "/2/");
                            Game1.playSound("newArtifact");
                        }
                        else
                        {
                            minigameArrowData[minigameData[0]] = minigameArrowData[minigameData[0]].Replace("/0/", "/1/");
                            Game1.playSound("jingle1");
                        }
                    }
                    else if (data.effects["LIFE"] > 0 && minigameData[0] != minigameData[4] && Game1.random.Next(0, 2) == 0)//saved by tackle?
                    {
                        data.effects["LIFE"]--;
                        minigameArrowData[minigameData[0]] = minigameArrowData[minigameData[0]].Replace("/0/", "/1.1/");
                        Game1.playSound("button1");
                    }
                    else
                    {
                        minigameArrowData[minigameData[0]] = minigameArrowData[minigameData[0]].Replace("/0/", "/-1/");
                        Game1.playSound("crit");
                    }
                }
                //else negative point if hit when arrow outside box? maybe if 2 in a row to avoid spam cheeze? can do it by changing data into list and adding red arrows
            }
            else
            {
                if (debug) minigameData[2] = 0;

                Game1.exitActiveMenu();
                minigameData[5] = 70;

                if (minigameData[2] < minigameArrowData.Length * 0.76f)
                {
                    minigameStage = 5;
                    Game1.playSound("fishEscape");
                }
                else
                {
                    if (minigameData[2] < minigameArrowData.Length * 1.14f) minigameStage = 7;
                    else if (minigameData[2] < minigameArrowData.Length * 1.52f) minigameStage = 8;
                    else if (minigameData[2] < minigameArrowData.Length * 1.9f) minigameStage = 9;
                    else minigameStage = 10;
                }
            }
        }


        private void DrawHangman(SpriteBatch batch)
        {
            //scale/middle/bounds calculation
            float scale = 7f * startMinigameScale;
            if (fishingFestivalMinigame == 1) scale *= (1f / Game1.options.zoomLevel) * Game1.options.uiScale;
            int width = (int)Math.Round(138f * scale);
            int height = (int)Math.Round(74f * scale);
            Vector2 screenMid = new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width / 2, Game1.graphics.GraphicsDevice.Viewport.Height / 2);

            float opacity = DrawIntro(batch, screenMid, scale);//intro

            if (minigameData[3] != minigameArrowData.Length)//minigame not done
            {
                int festivalDifficulty = 1;//festival stuff
                if (fishingFestivalMinigame == 1) festivalDifficulty += (Game1.currentMinigame as FishingGame).fishCaught;
                else if (fishingFestivalMinigame == 2) festivalDifficulty += who.festivalScore;

                //info
                Vector2 textLoc = new Vector2(0f, height * -0.44f);
                for (int i = 0; i < minigameText.Count; i++)
                {
                    DrawStringWithBorder(batch, Game1.smallFont, minigameText[i], screenMid + (textLoc += new Vector2(0f, height * 0.07f)), opacity > 2.6f ? minigameColor * 1.6f : Color.White * Math.Min(opacity, 1.5f), 0f, new Vector2(Game1.smallFont.MeasureString(minigameText[i]).X / 2f, 0f), scale * 0.16f, SpriteEffects.None, 0.4f);
                }
                if (fishingFestivalMinigame != 0)
                {
                    DrawStringWithBorder(batch, Game1.smallFont, (festivalDifficulty * 20 - minigameData[2]).ToString(), screenMid + (textLoc += new Vector2(0f, height * 0.05f)), minigameColor * Math.Min(opacity, 1f), 0f, new Vector2(Game1.smallFont.MeasureString((festivalDifficulty * 20 - minigameData[2]).ToString()).X / 2f, 0f), scale * 0.4f, SpriteEffects.None, 0.4f);
                }

                //if paused/out of focus:
                if (Game1.multiplayerMode == 0 && (Game1.paused || (!Game1.game1.IsActiveNoOverlay && Program.releaseBuild) && (Game1.options == null || Game1.options.pauseWhenOutOfFocus)))
                {
                    batch.Draw(Game1.mouseCursors, screenMid, new Rectangle(322, 498, 12, 12), Color.Brown, 0f, new Vector2(6f), scale * 2f, SpriteEffects.None, 0.4f);
                    return;
                }

                float speed = 2f;
                if (minigameStage == 2)
                {
                    if ((fishingFestivalMinigame == 0 && tutorialSkip[screen]) || debug) speed -= (minigameDiff * data.effects["SPEED"]);
                    else if (fishingFestivalMinigame == 1) speed += (festivalDifficulty / 2f - (fishingLevel / 10f)) * minigameDifficulty[screen];
                    else if (fishingFestivalMinigame == 2) speed += (festivalDifficulty / 1.6f - (fishingLevel / 10f)) * minigameDifficulty[screen];

                    if (speed <= 1) speed = 1f;
                    minigameTimer -= (int)(startMinigameScale * speed);
                }

            }
            else//final score screen
            {
                Color color = (minigameData[2] < minigameArrowData.Length * 0.38f) ? Color.Crimson :
                    (minigameData[2] < minigameArrowData.Length * 0.76f) ? Color.DarkOrange :
                    (minigameData[2] < minigameArrowData.Length * 1.14f) ? Color.Yellow :
                    (minigameData[2] < minigameArrowData.Length * 1.52f) ? Color.GreenYellow :
                    (minigameData[2] < minigameArrowData.Length * 1.9f) ? Color.LimeGreen : Color.Purple;

                if ((minigameStage < 3) && color != Color.Crimson && color != Color.DarkOrange) Game1.playSound("reward");

                //text
                string score = data.translate.Get("Minigame.Score") + " " + ((minigameData[2] < 0) ? "@ 0" : minigameData[2].ToString());
                string score2 = data.translate.Get("Minigame.Score2") + " " + (int)Math.Ceiling(minigameArrowData.Length * 0.76f);
                string scoreX = (color == Color.Purple) ? Game1.content.LoadString("Strings\\UI:BobberBar_Perfect") : data.translate.Get("Minigame.Score_" + ((color == Color.Crimson) ? 0 : (color == Color.Yellow || color == Color.GreenYellow) ? 2 : (color == Color.LimeGreen) ? 3 : 1));
                DrawStringWithBorder(batch, Game1.smallFont, score, screenMid + new Vector2(0f, -0.28f * height), color * (opacity / 3f), 0f, Game1.smallFont.MeasureString(score) / 2f, scale * 0.4f, SpriteEffects.None, 0.4f);
                DrawStringWithBorder(batch, Game1.smallFont, score2, screenMid + new Vector2(0f, -0.14f * height), color * (opacity / 3f), 0f, Game1.smallFont.MeasureString(score2) / 2f, scale * 0.3f, SpriteEffects.None, 0.4f);
                DrawStringWithBorder(batch, Game1.smallFont, scoreX, screenMid + new Vector2(0f, 0.02f * height), color * (opacity / 3f), 0f, Game1.smallFont.MeasureString(scoreX) / 2f, scale * 0.3f, SpriteEffects.None, 0.4f);

                //bar
                Rectangle whitePixel = new Rectangle(36, 1875, 1, 1);
                batch.Draw(Game1.mouseCursors, new Rectangle((int)(screenMid.X + (width * -0.403f)), (int)(screenMid.Y + (height * 0.184f)), (int)(0.806f * width), (int)(7f * scale)),
                    whitePixel, Color.Black * (opacity / 3f), 0f, Vector2.Zero, SpriteEffects.None, 0.4f);

                Rectangle bounds = new Rectangle((int)(screenMid.X + (width * -0.4f)), (int)(screenMid.Y + (height * 0.19f)), (int)(0.38f * 0.4f * width), (int)(6f * scale));
                batch.Draw(Game1.mouseCursors, bounds, whitePixel, Color.Crimson * (opacity / 3f), 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                bounds.X += bounds.Width;
                batch.Draw(Game1.mouseCursors, bounds, whitePixel, Color.DarkOrange * (opacity / 3f), 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                bounds.X += bounds.Width;
                batch.Draw(Game1.mouseCursors, new Rectangle(bounds.X, (int)(screenMid.Y + (height * 0.185f)), (int)(0.3f * scale), (int)(6.8f * scale)),
                    whitePixel, Color.LimeGreen * (opacity / 3f), 0f, Vector2.Zero, SpriteEffects.None, 0.4f);
                batch.Draw(Game1.mouseCursors, bounds, whitePixel, Color.Yellow * (opacity / 3f), 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                bounds.X += bounds.Width;
                batch.Draw(Game1.mouseCursors, bounds, whitePixel, Color.GreenYellow * (opacity / 3f), 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                bounds.X += bounds.Width;
                batch.Draw(Game1.mouseCursors, bounds, whitePixel, Color.LimeGreen * (opacity / 3f), 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                bounds.X += bounds.Width;
                bounds.Width = (int)(0.1f * 0.4f * width);
                batch.Draw(Game1.mouseCursors, bounds, whitePixel, Color.Purple * (opacity / 3f), 0f, Vector2.Zero, SpriteEffects.None, 0.5f);

                for (float i = 0; i < 0.799f * width; i += (1f / (minigameArrowData.Length * 2f)) * 0.799f * width)
                {
                    batch.Draw(Game1.mouseCursors, new Rectangle((int)(screenMid.X + (width * -0.4f) + i), (int)(screenMid.Y + (height * 0.19f)), (int)(0.15f * scale), (int)(6f * scale)),
                        whitePixel, Color.Gray * (opacity / 3f), 0f, Vector2.Zero, SpriteEffects.None, 0.4f);
                }
                batch.Draw(Game1.mouseCursors, screenMid + new Vector2(width * -0.4f + ((minigameData[2] < 1) ? 0 : (int)((minigameData[2] / (minigameArrowData.Length * 2f)) * 0.799f * width)), height * 0.2f),
                    new Rectangle(146, 1830, 9, 17), Color.Black * (opacity / 3f), 0f, new Vector2(4.5f, 17), 0.4f * scale, SpriteEffects.None, 0.5f);
                batch.Draw(Game1.mouseCursors, screenMid + new Vector2(width * -0.4f + ((minigameData[2] < 1) ? 0 : (int)((minigameData[2] / (minigameArrowData.Length * 2f)) * 0.799f * width)), height * 0.19f),
                    new Rectangle(146, 1830, 9, 17), color * (opacity / 3f), 0f, new Vector2(4.5f, 17), 0.3f * scale, SpriteEffects.None, 0.6f);

                if (minigameStage < 5) minigameStage = 3;
            }
        }
        private void InputHangman()
        {
            if (minigameStage == 2)
            {
                if (minigameData[0] >= 0)
                {
                    bool passed = false;
                    switch (minigameArrowData[minigameData[0]].Split('/')[0])
                    {
                        case "0"://up
                            if (KeybindList.Parse("W, Up, DPadUp").JustPressed()) passed = true;
                            break;
                        case "1"://right
                            if (KeybindList.Parse("D, Right, DPadRight").JustPressed()) passed = true;
                            break;
                        case "2"://down
                            if (KeybindList.Parse("S, Down, DPadDown").JustPressed()) passed = true;
                            break;
                        case "3"://left
                            if (KeybindList.Parse("A, Left, DPadLeft").JustPressed()) passed = true;
                            break;
                    }
                    if (passed)
                    {
                        if (minigameData[0] == minigameData[4])//treasure arrow?
                        {
                            data.treasureCaught = true;
                            Game1.playSound("coin");
                        }
                        if (minigameData[1] == 1)//perfect?
                        {
                            minigameArrowData[minigameData[0]] = minigameArrowData[minigameData[0]].Replace("/0/", "/2/");
                            Game1.playSound("newArtifact");
                        }
                        else
                        {
                            minigameArrowData[minigameData[0]] = minigameArrowData[minigameData[0]].Replace("/0/", "/1/");
                            Game1.playSound("jingle1");
                        }
                    }
                    else if (data.effects["LIFE"] > 0 && minigameData[0] != minigameData[4] && Game1.random.Next(0, 2) == 0)//saved by tackle?
                    {
                        data.effects["LIFE"]--;
                        minigameArrowData[minigameData[0]] = minigameArrowData[minigameData[0]].Replace("/0/", "/1.1/");
                        Game1.playSound("button1");
                    }
                    else
                    {
                        minigameArrowData[minigameData[0]] = minigameArrowData[minigameData[0]].Replace("/0/", "/-1/");
                        Game1.playSound("crit");
                    }
                }
                //else negative point if hit when arrow outside box? maybe if 2 in a row to avoid spam cheeze? can do it by changing data into list and adding red arrows
            }
            else
            {
                if (debug) minigameData[2] = 0;

                Game1.exitActiveMenu();
                minigameData[5] = 70;

                if (minigameData[2] < minigameArrowData.Length * 0.76f)
                {
                    minigameStage = 5;
                    Game1.playSound("fishEscape");
                }
                else
                {
                    if (minigameData[2] < minigameArrowData.Length * 1.14f) minigameStage = 7;
                    else if (minigameData[2] < minigameArrowData.Length * 1.52f) minigameStage = 8;
                    else if (minigameData[2] < minigameArrowData.Length * 1.9f) minigameStage = 9;
                    else minigameStage = 10;
                }
            }
        }

        /// <summary>Makes text a tiny bit bolder and adds a border behind it. The border uses text colour's alpha for its aplha value. 6 DrawString operations, so 6x less efficient.</summary>
        private void DrawStringWithBorder(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth, Color? borderColor = null)
        {
            if (fishingFestivalMinigame != 1)
            {
                Color border = borderColor.HasValue ? borderColor.Value : Color.Black;
                border.A = color.A;
                batch.DrawString(font, text, position + new Vector2(-1.2f * scale, -1.2f * scale), border, rotation, origin, scale, effects, layerDepth - 0.00001f);
                batch.DrawString(font, text, position + new Vector2(1.2f * scale, -1.2f * scale), border, rotation, origin, scale, effects, layerDepth - 0.00001f);
                batch.DrawString(font, text, position + new Vector2(-1.2f * scale, 1.2f * scale), border, rotation, origin, scale, effects, layerDepth - 0.00001f);
                batch.DrawString(font, text, position + new Vector2(1.2f * scale, 1.2f * scale), border, rotation, origin, scale, effects, layerDepth - 0.00001f);
            }

            batch.DrawString(font, text, position + new Vector2(-0.2f * scale, -0.2f * scale), color, rotation, origin, scale, effects, layerDepth);
            batch.DrawString(font, text, position + new Vector2(0.2f * scale, 0.2f * scale), color, rotation, origin, scale, effects, layerDepth);
        }
    }
}
