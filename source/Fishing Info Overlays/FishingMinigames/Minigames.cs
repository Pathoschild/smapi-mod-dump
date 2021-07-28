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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;

namespace FishingMinigames
{
    public class Minigames
    {
        private IMonitor Monitor;
        private IModHelper Helper;
        private IManifest ModManifest;

        private List<KeyValuePair<long, TemporaryAnimatedSprite>> animations = new List<KeyValuePair<long, TemporaryAnimatedSprite>>();
        private SpriteBatch batch;
        private SparklingText sparklingText;
        private Farmer who;
        private int screen;
        private int x;
        private int y;

        private bool caughtDoubleFish;
        private int whichFish;
        private int minFishSize;
        private int maxFishSize;
        private float fishSize;
        private bool recordSize;
        private bool perfect;
        private int fishQuality;
        private bool fishCaught;
        private bool bossFish;
        private int difficulty;
        private bool treasureCaught;
        private bool showPerfect;
        private bool fromFishPond;
        private int clearWaterDistance;
        private Object item;


        private bool hereFishying;
        private bool itemIsInstantCatch;
        private int oldFacingDirection;
        private int oldGameTimeInterval;
        private float itemSpriteSize;
        private Rectangle sourceRect;
        private bool drawTool;
        private int fishingFestivalMinigame;        //0 none, 1 fall16, 2 winter8

        private int startMinigameStage;             //0 before, 1 fade in, 2 playing, 3 show score, 5 fail, 6-10 = score
        private int startMinigameTimer;
        private string[] startMinigameArrowData;    //0 arrow direction, 1 colour, 2 offset, 3 current distance
        private int[] startMinigameData;            //0 current arrow, 1 perfect area?, 2 score, 3 last arrow to vanish, 4 treasure arrow, 5 fade
        private Texture2D[] startMinigameTextures;
        private List<string> startMinigameText;

        private int endMinigameStage;               //0 before, 1 fish flying, 2 input can fail, 3 input can succeed/time out, 8 failed, 9 success, 10 perfect
        private string endMinigameKey;
        private int endMinigameTimer;
        private int infoTimer;

        private string stage;
        private int stageTimer = -1;

        private Dictionary<long, MinigameMessage> messages = new Dictionary<long, MinigameMessage>();

        //config values
        public static SoundEffect fishySound;
        public static KeybindList[] keyBinds = new KeybindList[4];
        public static float voiceVolume;
        public static float[] voicePitch = new float[4];
        public static float[] minigameDamage = new float[4];
        public static int[] startMinigameStyle = new int[4];
        public static int[] endMinigameStyle = new int[4];
        public static float startMinigameScale;
        public static bool realisticSizes;
        public static bool metricSizes;
        public static int[] festivalMode = new int[4];
        public static float[] minigameDifficulty = new float[4];



        /*  
         *  instead of where clicked, soundwave anim ahead? would be hard to aim at pools, could use swing effect anim?
         */

        public Minigames(ModEntry entry)
        {
            this.Helper = entry.Helper;
            this.Monitor = entry.Monitor;
            this.ModManifest = entry.ModManifest;
        }



        public void Input_ButtonsChanged(object sender, ButtonsChangedEventArgs e)  //this.Monitor.Log(locationName, LogLevel.Debug);
        {
            if (Game1.emoteMenu != null) return;
            who = Game1.player;
            if (infoTimer > 0 && infoTimer < 1001)//reset from bubble
            {
                who.CanMove = true;
                hereFishying = false;
                infoTimer = 0;
                SendMessage(who, "Clear");
                who.completelyStopAnimatingOrDoingAction();
                who.faceDirection(oldFacingDirection);
            }
            screen = Context.ScreenId;
            if (keyBinds[screen].JustPressed()) //cancel regular rod use, if it's a shared keybind
            {
                if (!Context.IsPlayerFree) return;
                if (Game1.activeClickableMenu == null && who.CurrentItem is FishingRod)
                {
                    if (fishingFestivalMinigame > 0 && festivalMode[screen] == 0) return;
                    SuppressAll(e.Pressed);
                }
            }


            if (e.Pressed.Contains(SButton.F5) && Context.IsWorldReady)
            {
                EmergencyCancel(who);
                return;
            }
            if (!Context.IsWorldReady) return;


            if (startMinigameStage > 0 && startMinigameStage < 5)//already in startMinigame
            {
                SuppressAll(e.Pressed);
                if (e.Pressed.Contains(SButton.Escape) || e.Pressed.Contains(SButton.ControllerB)) //cancel
                {
                    Helper.Input.Suppress(SButton.Escape);
                    Helper.Input.Suppress(SButton.ControllerB);
                    startMinigameStage = 5;
                    SwingAndEmote(who, 2);
                    EmergencyCancel(who);
                    return;
                }

                if (startMinigameStage > 1) StartMinigameInput(e);
            }
            else if (endMinigameStage == 2 || endMinigameStage == 3) //already in endMinigame
            {
                SuppressAll(e.Pressed);
                EndMinigame(1);
            }
            else//start attempt
            {
                if (keyBinds[screen].JustPressed())
                {
                    if (Context.IsWorldReady && Context.CanPlayerMove && who.CurrentItem is FishingRod)
                    {
                        if (Game1.isFestival() && (fishingFestivalMinigame == 0 || festivalMode[screen] == 0)) return;
                        if (fishingFestivalMinigame == 1)
                        {
                            FestivalGameSkip(who);
                            return;
                        }

                        if (!hereFishying)
                        {
                            try
                            {
                                perfect = false;
                                Vector2 mouse = Game1.currentCursorTile;
                                oldFacingDirection = who.getGeneralDirectionTowards(new Vector2(mouse.X * 64, mouse.Y * 64));
                                who.faceDirection(oldFacingDirection);


                                if (who.currentLocation.canFishHere() && who.currentLocation.isTileFishable((int)mouse.X, (int)mouse.Y))
                                {
                                    Monitor.Log($"here fishy fishy {mouse.X},{mouse.Y}");
                                    x = (int)mouse.X * 64;
                                    y = (int)mouse.Y * 64;
                                    Game1.stats.timesFished++;
                                    HereFishyFishy(who);
                                }
                            }
                            catch (Exception ex)
                            {
                                Monitor.Log("Canceled fishing because: " + ex.Message, LogLevel.Error);
                                EmergencyCancel(who);
                            }
                        }
                    }
                }
            }
        }

        public void GameLoop_UpdateTicking(object sender, UpdateTickingEventArgs e) //adds item to inv
        {
            if (Game1.isFestival())
            {
                fishingFestivalMinigame = 0;
                string data = Helper.Reflection.GetField<Dictionary<string, string>>(Game1.CurrentEvent, "festivalData").GetValue()["file"];
                if (data != null)
                {
                    who = Game1.player;
                    int timer = 0;
                    if (data.Equals("fall16") && Game1.currentMinigame is StardewValley.Minigames.FishingGame)
                    {
                        timer = Helper.Reflection.GetField<int>(Game1.currentMinigame as StardewValley.Minigames.FishingGame, "gameEndTimer").GetValue();
                        if (timer < 100000 && timer >= 500) fishingFestivalMinigame = 1;
                    }
                    else if (data.Equals("winter8"))
                    {
                        timer = Game1.CurrentEvent.festivalTimer;
                        if (timer < 120000 && timer >= 500) fishingFestivalMinigame = 2;
                    }
                    if (timer <= 500 && fishingFestivalMinigame > 0 && festivalMode[screen] > 0) EmergencyCancel(who);
                }
            }

            for (int i = animations.Count - 1; i >= 0; i--)
            {
                if (!animations[i].Value.paused && animations[i].Value.update(Game1.currentGameTime))
                {
                    animations.RemoveAt(i);
                }
            }

            if (sparklingText != null && sparklingText.update(Game1.currentGameTime))
            {
                sparklingText = null;
            }

            if (fishCaught)
            {
                if (fishingFestivalMinigame == 0)
                {
                    infoTimer = 1000;
                    SendMessage(who, "CaughtBubble");
                }
                stageTimer = -1;
                stage = null;
                fishCaught = false;
            }

            if (infoTimer > 0 && Game1.activeClickableMenu != null) infoTimer = 1020;//bubble logic
            else if (infoTimer > 0) infoTimer--;
            if (infoTimer == 1)
            {
                who.CanMove = true;
                hereFishying = false;
                SendMessage(who, "Clear");
                infoTimer--;
                who.faceDirection(oldFacingDirection);
            }


            if (stageTimer > 0) stageTimer--;//animation await delay control, remember stage needs a single digit at the end to pass here
            else if (stageTimer == 0)
            {
                stageTimer = -1;
                switch (stage.Remove(stage.Length - 1))
                {
                    case "Starting":
                        HereFishyAnimation(who, x, y);
                        break;
                    case "Caught":
                        PlayerCaughtFishEndFunction();
                        break;
                    case "Festival":
                        FestivalGameSkip(who);
                        break;
                }
            }
            if (hereFishying) who.CanMove = false;
        }

        public void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (batch == null) batch = e.SpriteBatch;
            who = Game1.player;
            foreach (Farmer other in Game1.getAllFarmers())//draw tool for tool anims & bubble for other players
            {
                if (messages.ContainsKey(other.UniqueMultiplayerID))
                {
                    if (messages[other.UniqueMultiplayerID].drawTool) Game1.drawTool(other);
                    if (messages[other.UniqueMultiplayerID].stage != null)
                    {
                        if (messages[other.UniqueMultiplayerID].stage.Equals("CaughtBubble", StringComparison.Ordinal)) CaughtBubble(other);
                        else if (messages[other.UniqueMultiplayerID].stage.Equals("Playing", StringComparison.Ordinal)) SwingAndEmote(other, 3);
                    }
                }
            }


            if (startMinigameStage > 0 && startMinigameStage < 5) StartMinigameDraw(batch);
            else if (startMinigameStage > 4 && startMinigameData[5] > 0) StartMinigameDraw(batch);
            else
            {
                //draw mouse target on water
                if (!Game1.eventUp && !Game1.menuUp && !hereFishying && who.CurrentItem is FishingRod && who.currentLocation.isTileFishable((int)Game1.currentCursorTile.X, (int)Game1.currentCursorTile.Y))
                {
                    e.SpriteBatch.Draw(Game1.mouseCursors, new Vector2(((Game1.getMouseX() / 64) * 64), ((Game1.getMouseY() / 64) * 64)), new Rectangle(652, 204, 44, 44), new Color(0, 255, 0, 0.4f), 0f, Vector2.Zero, 1.45f, SpriteEffects.None, 1f);
                }
                //draw fish flying
                for (int i = animations.Count - 1; i >= 0; i--)
                {
                    animations[i].Value.draw(e.SpriteBatch, false, 0, 0, 1f);
                    if (endMinigameStage > 0 && animations[i].Key == who.UniqueMultiplayerID)
                    {
                        int size = (int)(itemSpriteSize * ((item is Furniture) ? 32 : 16));
                        if (endMinigameStage == 1)//can miss area
                        {
                            Rectangle area = new Rectangle((int)who.Position.X - 200, (int)who.Position.Y - 450, 400, 400);
                            if (animations[i].Value.Position != animations[i].Value.initialPosition &&
                                (area.Contains((int)animations[i].Value.Position.X, (int)animations[i].Value.Position.Y) ||
                                area.Contains((int)animations[i].Value.Position.X, (int)animations[i].Value.Position.Y + size) ||
                                area.Contains((int)animations[i].Value.Position.X + size, (int)animations[i].Value.Position.Y) ||
                                area.Contains((int)animations[i].Value.Position.X + size, (int)animations[i].Value.Position.Y + size)))
                            {
                                endMinigameStage = 2;
                            }
                        }
                        if (endMinigameStage == 2)//can succeed area
                        {
                            Rectangle area = new Rectangle((int)who.Position.X - 70, (int)who.Position.Y - 115, 140, 220);
                            if (animations[i].Value.Position != animations[i].Value.initialPosition &&
                                (area.Contains((int)animations[i].Value.Position.X, (int)animations[i].Value.Position.Y) ||
                                area.Contains((int)animations[i].Value.Position.X, (int)animations[i].Value.Position.Y + size) ||
                                area.Contains((int)animations[i].Value.Position.X + size, (int)animations[i].Value.Position.Y) ||
                                area.Contains((int)animations[i].Value.Position.X + size, (int)animations[i].Value.Position.Y + size)))
                            {
                                PlayPause(who);
                                SendMessage(who, "Pause");
                                EndMinigame(0);
                            }
                        }
                        else if (endMinigameStage == 3)//too late timer
                        {
                            endMinigameTimer++;

                            int totalDifficulty = (int)(100f + ((endMinigameStyle[screen] == 3) ? 25f : 0f) - ((difficulty / 2f) - (fishSize / 10f) * minigameDifficulty[screen]));

                            if (endMinigameTimer > totalDifficulty)//too late/missed/failed
                            {
                                PlayPause(who);
                                drawTool = true;
                                SendMessage(who, "Fail");

                                if (oldFacingDirection == 0) who.armOffset += new Vector2(-10f, 0);
                                endMinigameTimer = 0;
                                endMinigameStage = 8;
                                who.completelyStopAnimatingOrDoingAction();
                                List<FarmerSprite.AnimationFrame> animationFrames = new List<FarmerSprite.AnimationFrame>(){
                                new FarmerSprite.AnimationFrame(94, 500, false, false, null, false).AddFrameAction(delegate (Farmer f) { f.jitterStrength = 2f; }) };
                                who.FarmerSprite.setCurrentAnimation(animationFrames.ToArray());
                                who.FarmerSprite.PauseForSingleAnimation = true;
                                who.FarmerSprite.loop = true;
                                who.FarmerSprite.loopThisAnimation = true;
                                who.Sprite.currentFrame = 94;
                            }
                        }
                    }
                }

                if (showPerfect)//add perfect popup
                {
                    perfect = true;
                    sparklingText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\UI:BobberBar_Perfect"), Color.Yellow, Color.White, false, 0.1, 1500, -1, 500, 1f);
                    Game1.playSound("jingle1");
                    showPerfect = false;
                }
                if (sparklingText != null && who != null && !itemIsInstantCatch)//show perfect/new record popup
                {
                    sparklingText.draw(e.SpriteBatch, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(-64f, -300f)));
                }

                if (endMinigameStyle[screen] == 3 && endMinigameTimer > 0 && endMinigameTimer < 100)//draw letter for end minigame
                {
                    float y_offset = (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 200), 2);
                    Vector2 position = new Vector2(who.getStandingX() - Game1.viewport.X, who.getStandingY() - 156 - Game1.viewport.Y) + new Vector2(y_offset);
                    batch.Draw(Game1.mouseCursors, position + new Vector2(-24, 0), new Rectangle(473, 36, 24, 24), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.98f);//text bg box
                    batch.DrawString(Game1.smallFont, endMinigameKey, position - (Game1.smallFont.MeasureString(endMinigameKey) / 2 * 1.2f) + new Vector2(0f, 28f), Color.Gold, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 1f); //text
                }

                if (drawTool) Game1.drawTool(who);//draw tool for tool animations
                if (infoTimer > 0) CaughtBubble(who);//bubble
            }
        }


        private void HereFishyFishy(Farmer who)
        {
            treasureCaught = false;
            hereFishying = true;
            startMinigameStage = 0;
            endMinigameStage = 0;
            oldGameTimeInterval = Game1.gameTimeInterval;
            if (!Game1.IsMultiplayer && !Game1.isFestival()) Game1.gameTimeInterval = 0;

            if (who.IsLocalPlayer && fishingFestivalMinigame != 2)
            {
                float oldStamina = who.Stamina;
                who.Stamina -= 8f - (float)who.FishingLevel * 0.1f;
                who.checkForExhaustion(oldStamina);
            }

            CatchFish(who, x, y);

            if (!itemIsInstantCatch) Helper.Multiplayer.SendMessage(whichFish, "whichFish", modIDs: new[] { "barteke22.FishingInfoOverlays" }, new[] { who.UniqueMultiplayerID });//notify overlay of which fish

            if (!fromFishPond && fishingFestivalMinigame != 2 && startMinigameStyle[screen] > 0) //add !instant + test if works in festival
            {
                //starting minigame init
                ITranslationHelper translate = Helper.Translation;
                startMinigameTextures = new Texture2D[] { Game1.content.Load<Texture2D>("LooseSprites\\boardGameBorder"), Game1.content.Load<Texture2D>("LooseSprites\\CraneGame") };
                startMinigameText = new List<string>() { translate.Get("Minigame.Score") };
                foreach (string s in translate.Get("Minigame.InfoDDR").ToString().Split(new string[] { "\n" }, StringSplitOptions.None)) startMinigameText.Add(s);

                startMinigameData = new int[6];
                startMinigameArrowData = new string[5 + (int)Math.Ceiling((difficulty + fishSize) / 3f * minigameDifficulty[screen])]; //make it minimum X (20?) and then apply diff/size - max 99
                int offset = 0;
                for (int i = 0; i < startMinigameArrowData.Length; i++)
                {
                    startMinigameArrowData[i] = Game1.random.Next(0, 4) + "/0/" + offset + "/9999";//0 arrow direction, 1 colour, 2 offset, 3 current distance
                    if (Game1.random.Next(0, 3) == 0) offset += (int)(250 * startMinigameScale);
                    else offset += (int)(140 * startMinigameScale);
                }
                //vanilla treasure chance calculation
                if (fishingFestivalMinigame != 2 && who.fishCaught != null && who.fishCaught.Count() > 1 && Game1.random.NextDouble() < FishingRod.baseChanceForTreasure + who.LuckLevel * 0.005 + (((who.CurrentTool as FishingRod).getBaitAttachmentIndex() == 703) ? FishingRod.baseChanceForTreasure : 0.0) + (((who.CurrentTool as FishingRod).getBobberAttachmentIndex() == 693) ? (FishingRod.baseChanceForTreasure / 3.0) : 0.0) + who.DailyLuck / 2.0 + ((who.professions.Contains(9) ? FishingRod.baseChanceForTreasure : 0.0)))
                {
                    startMinigameData[4] = Game1.random.Next(startMinigameArrowData.Length / 2, startMinigameArrowData.Length - 1);
                }
                else startMinigameData[4] = -1;
                startMinigameData[0] = -2;
                startMinigameData[5] = 0;

                startMinigameStage = 1;
                startMinigameTimer = 0;
                SendMessage(who, "Playing");
            }
            else HereFishyAnimation(who, x, y);
        }


        private void CatchFish(Farmer who, int x, int y)
        {
            FishingRod rod = who.CurrentTool as FishingRod;
            Vector2 bobberTile = new Vector2(x / 64, y / 64);
            fromFishPond = who.currentLocation.isTileBuildingFishable((int)bobberTile.X, (int)bobberTile.Y);

            clearWaterDistance = FishingRod.distanceToLand((int)bobberTile.X, (int)bobberTile.Y, who.currentLocation);
            double baitPotency = ((rod.attachments[0] != null) ? ((float)rod.attachments[0].Price / 10f) : 0f);

            Rectangle fishSplashRect = new Rectangle(who.currentLocation.fishSplashPoint.X * 64, who.currentLocation.fishSplashPoint.Y * 64, 64, 64);
            Rectangle bobberRect = new Rectangle(x - 80, y - 80, 64, 64);
            bool splashPoint = fishSplashRect.Intersects(bobberRect);

            item = who.currentLocation.getFish(0, (rod.attachments[0] != null) ? rod.attachments[0].ParentSheetIndex : (-1), clearWaterDistance + (splashPoint ? 1 : 0), who, baitPotency + (splashPoint ? 0.4 : 0.0), bobberTile); //all item data starts here, FishingRod.cs

            if (fromFishPond) //get whole fishpond stage in one go: 6-3-1 fish
            {
                foreach (Building b in Game1.getFarm().buildings)
                {
                    if (b is FishPond && b.isTileFishable(bobberTile))
                    {
                        for (int i = 0; i < (b as FishPond).currentOccupants.Value; i++)
                        {
                            (b as FishPond).CatchFish();
                            item.Stack++;
                        }
                        break;
                    }
                }
            }

            if (whichFish == 79 || whichFish == 842)//notes
            {
                item = who.currentLocation.tryToCreateUnseenSecretNote(who);
            }

            if (item != null) whichFish = item.ParentSheetIndex;//fix here for fishpond

            if (item == null || whichFish <= 0)
            {
                item = new Object(Game1.random.Next(167, 173), 1);//trash
                whichFish = item.ParentSheetIndex;
            }

            fishSize = 0f;
            fishQuality = 0;
            difficulty = 0;
            minFishSize = 0;
            maxFishSize = 0;

            Dictionary<int, string> data = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            string[] fishData = null;
            if (data.ContainsKey(whichFish)) fishData = data[whichFish].Split('/');


            itemIsInstantCatch = false;
            if (item is Furniture) itemIsInstantCatch = true;
            else if (Utility.IsNormalObjectAtParentSheetIndex(item, whichFish) && data.ContainsKey(whichFish))
            {
                if (int.TryParse(fishData[1], out difficulty) && int.TryParse(fishData[3], out minFishSize) && int.TryParse(fishData[4], out maxFishSize)) itemIsInstantCatch = false;
                else itemIsInstantCatch = true;
            }
            else itemIsInstantCatch = true;

            if (itemIsInstantCatch || item.Category == -20 || item.ParentSheetIndex == 152 || item.ParentSheetIndex == 153 || item.parentSheetIndex == 157 || item.parentSheetIndex == 797 || item.parentSheetIndex == 79 || item.parentSheetIndex == 73 || item.ParentSheetIndex == 842 || (item.ParentSheetIndex >= 820 && item.ParentSheetIndex <= 828) || item.parentSheetIndex == GameLocation.CAROLINES_NECKLACE_ITEM || item.ParentSheetIndex == 890 || fromFishPond)
            {
                itemIsInstantCatch = true;
            }

            //special item handling
            if (fishingFestivalMinigame != 2 && !(item is Furniture) && !fromFishPond && who.team.specialOrders != null)
            {
                foreach (SpecialOrder order in who.team.specialOrders)
                {
                    order.onFishCaught?.Invoke(who, item);
                }
            }
            if (whichFish == GameLocation.CAROLINES_NECKLACE_ITEM) item.questItem.Value = true;
            


            //sizes
            if (maxFishSize > 0)
            {
                fishSize = Math.Min((float)clearWaterDistance / 5f, 1f);
                int minimumSizeContribution = 1 + who.FishingLevel / 2;
                fishSize *= (float)Game1.random.Next(minimumSizeContribution, Math.Max(6, minimumSizeContribution)) / 5f;

                if (rod.getBaitAttachmentIndex() != -1) fishSize *= 1.2f;
                fishSize *= 1f + (float)Game1.random.Next(-10, 11) / 100f;
                fishSize = Math.Max(0f, Math.Min(1f, fishSize));


                fishSize = (int)((float)minFishSize + (float)(maxFishSize - minFishSize) * fishSize);
                fishSize++;
            }

            if (rod.Name.Equals("Training Rod", StringComparison.Ordinal)) fishSize = minFishSize;


            caughtDoubleFish = !itemIsInstantCatch && fishingFestivalMinigame == 0 && !bossFish && rod.getBaitAttachmentIndex() == 774 && Game1.random.NextDouble() < 0.1 + who.DailyLuck / 2.0;
            if (caughtDoubleFish) item.Stack = 2;

            bossFish = FishingRod.isFishBossFish(whichFish);
        }

        private void CatchFishAfterMinigame(Farmer who)
        {
            //data calculations: quality, exp, treasure
            FishingRod rod = who.CurrentTool as FishingRod;

            float reduction = 0f;

            if (!itemIsInstantCatch)
            {
                if (rod.Name.Equals("Training Rod", StringComparison.Ordinal))
                {
                    fishQuality = 0;
                    fishSize = minFishSize;
                }
                else
                {
                    fishQuality = (fishSize * (0.9 + (who.FishingLevel / 5.0)) < 0.33) ? 0 : ((fishSize * (0.9 + (who.FishingLevel / 5.0)) < 0.66) ? 1 : 2);//init quality
                    if (rod.getBobberAttachmentIndex() == 877) fishQuality++;

                    if (startMinigameStyle[screen] > 0 && endMinigameStyle[screen] > 0) //minigame score reductions
                    {
                        if (startMinigameStage == 10) reduction -= 0.4f;
                        else if (startMinigameStage == 9) reduction += 0.3f;
                        else if (startMinigameStage == 8) reduction += 0.5f;
                        else if (startMinigameStage == 7) reduction += 0.7f;
                        else if (startMinigameStage == 6) reduction += 0.8f;
                        if (endMinigameStage == 10) reduction -= 0.4f;
                        else if (endMinigameStage == 9) reduction += 0.6f;
                        else if (endMinigameStage == 8) reduction += 0.8f;
                    }
                    else if (startMinigameStyle[screen] > 0)
                    {
                        if (startMinigameStage == 10) reduction -= 1f;
                        else if (startMinigameStage == 9) reduction += 0f;
                        else if (startMinigameStage < 9) reduction += 1f;
                        else if (startMinigameStage == 6) reduction += 2f;
                    }
                    else if (endMinigameStyle[screen] > 0)
                    {
                        if (endMinigameStage == 10) reduction -= 1f;
                        else if (endMinigameStage == 9) reduction += (Game1.random.Next(0, 2) == 0) ? 0f : 1f;
                        else if (endMinigameStage < 8) reduction += 2f;
                    }
                    else
                    {
                        if (perfect) fishQuality++;
                    }
                    fishSize -= (int)Math.Round(reduction * 2);
                    fishQuality -= (int)Math.Round(reduction);
                }

                if (fishQuality < 0) fishQuality = 0;
                if (fishQuality > 2) fishQuality = 4;


                if (who.IsLocalPlayer && fishingFestivalMinigame != 2)
                {
                    int experience = Math.Max(1, (fishQuality + 1) * 3 + difficulty / 3);
                    if (bossFish) experience *= 5;

                    if (startMinigameStyle[screen] + endMinigameStyle[screen] > 0) experience += (int)(experience - reduction - 0.5f);
                    else if (perfect) experience += (int)((float)experience * 1.4f);

                    who.gainExperience(1, experience);
                    if (minigameDamage[screen] > 0 && endMinigameStyle[screen] > 0 && endMinigameStage == 8) who.takeDamage((int)((10 + (difficulty / 10) + (int)(fishSize / 5) - who.FishingLevel) * minigameDamage[screen]), true, null);
                }


                if (startMinigameStyle[screen] == 0) treasureCaught = fishingFestivalMinigame != 2 && who.fishCaught != null && who.fishCaught.Count() > 1 && Game1.random.NextDouble() < FishingRod.baseChanceForTreasure + (double)who.LuckLevel * 0.005 + ((rod.getBaitAttachmentIndex() == 703) ? FishingRod.baseChanceForTreasure : 0.0) + ((rod.getBobberAttachmentIndex() == 693) ? (FishingRod.baseChanceForTreasure / 3.0) : 0.0) + who.DailyLuck / 2.0 + ((who.professions.Contains(9) ? FishingRod.baseChanceForTreasure : 0.0) - reduction - 0.5f);
                item.Quality = fishQuality;
            }
            else if (who.IsLocalPlayer && fishingFestivalMinigame != 2)
            {
                who.gainExperience(1, 3);
                if (!fromFishPond && minigameDamage[screen] > 0 && endMinigameStyle[screen] > 0 && endMinigameStage == 8) who.takeDamage((int)((16 - who.FishingLevel) * minigameDamage[screen]), true, null);
            }
        }

        private void HereFishyAnimation(Farmer who, int x, int y)
        {
            //player jumping and calling fish
            switch (stage)
            {
                case null:
                    if (fishySound != null && !Context.IsSplitScreen) fishySound.Play(voiceVolume, voicePitch[screen], 0);
                    SendMessage(who);

                    who.completelyStopAnimatingOrDoingAction();
                    who.jitterStrength = 2f;
                    List<FarmerSprite.AnimationFrame> animationFrames = new List<FarmerSprite.AnimationFrame>(){
                        new FarmerSprite.AnimationFrame(94, 100, false, false, null, false).AddFrameAction(delegate (Farmer f) { f.jitterStrength = 2f; }) };
                    who.FarmerSprite.setCurrentAnimation(animationFrames.ToArray());
                    who.FarmerSprite.PauseForSingleAnimation = true;
                    who.FarmerSprite.loop = true;
                    who.FarmerSprite.loopThisAnimation = true;
                    who.Sprite.currentFrame = 94;

                    stage = "Starting1";
                    stageTimer = 110;
                    break;

                case "Starting1":
                    if (startMinigameStyle[screen] + endMinigameStyle[screen] == 0 && Game1.random.Next(who.FishingLevel, 20) > 16)
                    {
                        showPerfect = true;
                    }

                    who.synchronizedJump(8f);

                    stage = "Starting2";
                    stageTimer = 60;
                    break;

                case "Starting2":
                    SendMessage(who, "Clear");

                    who.stopJittering();
                    who.completelyStopAnimatingOrDoingAction();
                    who.forceCanMove();

                    stage = "Starting3";
                    stageTimer = Game1.random.Next(30, 60);
                    break;

                case "Starting3":
                    if (!fromFishPond && endMinigameStyle[screen] > 0) endMinigameStage = 1;

                    ClearAnimations(who);
                    SendMessage(who, "ClearAnim");

                    if (itemIsInstantCatch && !fromFishPond) stage = "Starting4"; //angory fish, emote workaround
                    else stage = "Starting8";
                    stageTimer = 1;
                    break;

                case "Starting4":
                    SendMessage(who);
                    animations.Add(new KeyValuePair<long, TemporaryAnimatedSprite>(who.UniqueMultiplayerID, new TemporaryAnimatedSprite(Game1.emoteSpriteSheet.ToString(), new Rectangle(12 * 16 % Game1.emoteSpriteSheet.Width, 12 * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), 200, 1, 0, new Vector2(x, y - 32), false, false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f, false)));
                    stage = "Starting5";
                    stageTimer = 12;
                    break;
                case "Starting5":
                    SendMessage(who);
                    animations.Add(new KeyValuePair<long, TemporaryAnimatedSprite>(who.UniqueMultiplayerID, new TemporaryAnimatedSprite(Game1.emoteSpriteSheet.ToString(), new Rectangle(13 * 16 % Game1.emoteSpriteSheet.Width, 12 * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), 200, 1, 0, new Vector2(x, y - 32), false, false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f, false)));
                    stage = "Starting6";
                    stageTimer = 12;
                    break;
                case "Starting6":
                    SendMessage(who);
                    animations.Add(new KeyValuePair<long, TemporaryAnimatedSprite>(who.UniqueMultiplayerID, new TemporaryAnimatedSprite(Game1.emoteSpriteSheet.ToString(), new Rectangle(14 * 16 % Game1.emoteSpriteSheet.Width, 12 * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), 200, 1, 0, new Vector2(x, y - 32), false, false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f, false)));
                    stage = "Starting7";
                    stageTimer = 12;
                    break;
                case "Starting7":
                    SendMessage(who);
                    animations.Add(new KeyValuePair<long, TemporaryAnimatedSprite>(who.UniqueMultiplayerID, new TemporaryAnimatedSprite(Game1.emoteSpriteSheet.ToString(), new Rectangle(15 * 16 % Game1.emoteSpriteSheet.Width, 12 * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), 200, 1, 0, new Vector2(x, y - 32), false, false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f, false)));
                    stage = "Starting8";
                    stageTimer = 12;
                    break;


                //fish flying from water to player
                case "Starting8":
                    //realistic: divide fish size by 64 inches (around average human size) * 10f (how much you need to multiply item sprite to be player height (8*16 = 128 = 2 tiles + 20% for perspective))
                    if (realisticSizes)
                    {
                        itemSpriteSize = 2.5f;
                        if (fishSize > 0) itemSpriteSize = Math.Max(fishSize / 64f, 0.05f) * 10f;
                    }
                    else itemSpriteSize = 4f;
                    if (item is Furniture) itemSpriteSize = 2.2f;
                    sourceRect = (item is Furniture) ? (item as Furniture).defaultSourceRect : Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, whichFish, 16, 16);
                    SendMessage(who);

                    float t;
                    float distance = y - (float)(who.getStandingY() - 100);

                    float height = Math.Abs(distance + 170f);
                    if (who.FacingDirection == 0) height -= 130f;
                    else if (who.FacingDirection == 2) height -= 170f;
                    height = Math.Max(height, 0f);

                    float gravity = 0.002f;
                    float velocity = (float)Math.Sqrt((double)(2f * gravity * height));
                    t = (float)(Math.Sqrt((double)(2f * (height - distance) / gravity)) + (double)(velocity / gravity));
                    float xVelocity = 0f;
                    if (t != 0f)
                    {
                        xVelocity = (who.Position.X - x) / t;
                    }
                    animations.Add(new KeyValuePair<long, TemporaryAnimatedSprite>(who.UniqueMultiplayerID, new TemporaryAnimatedSprite((item is Furniture) ? Furniture.furnitureTexture.ToString() : "Maps\\springobjects", sourceRect, t, 1, 0, new Vector2(x, y), false, false, y / 10000f, 0f, Color.White, itemSpriteSize, 0f, 0f, 0f, false)
                    {
                        motion = new Vector2(xVelocity, -velocity),
                        acceleration = new Vector2(0f, gravity),
                        extraInfoForEndBehavior = 1,
                        endFunction = new TemporaryAnimatedSprite.endBehavior(PlayerCaughtFishEndFunction),
                        timeBasedMotion = true,
                        endSound = "tinyWhip"
                    }));
                    int delay = 25;
                    for (int i = 1; i < item.Stack; i++)
                    {
                        animations.Add(new KeyValuePair<long, TemporaryAnimatedSprite>(who.UniqueMultiplayerID, new TemporaryAnimatedSprite("Maps\\springobjects", sourceRect, t, 1, 0, new Vector2(x, y), false, false, y / 10000f, 0f, Color.White, itemSpriteSize, 0f, 0f, 0f, false)
                        {
                            delayBeforeAnimationStart = delay,
                            motion = new Vector2(xVelocity, -velocity),
                            acceleration = new Vector2(0f, gravity),
                            timeBasedMotion = true,
                            endSound = "tinyWhip",
                            Parent = who.currentLocation
                        }));
                        delay += 25;
                    }
                    break;
            }
        }


        private void StartMinigameDraw(SpriteBatch batch) //limit to non-insta catch - for now testing with any
        {
            if (startMinigameStage == 1)//fade in (opacity calc)
            {
                startMinigameData[5]++;
                if (startMinigameData[5] == 300) startMinigameStage = 2;
            }
            else if (startMinigameStage > 4)//fade out and continue
            {
                startMinigameData[5]--;
                if (startMinigameData[5] == 0)
                {
                    stage = null;
                    if (startMinigameStage == 5)
                    {
                        SwingAndEmote(who, 2);
                        EmergencyCancel(who);
                    }
                    else HereFishyAnimation(who, x, y);
                    return;
                }
            }
            float opacity = startMinigameData[5] / 100f;

            //scale/middle/bounds calculation
            float scale = 7f * startMinigameScale;
            int width = (int)Math.Round(138f * scale);
            int height = (int)Math.Round(74f * scale);
            Vector2 screenMid = new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width / 2, Game1.graphics.GraphicsDevice.Viewport.Height / 2);

            //board
            batch.Draw(Game1.mouseCursors, screenMid, new Rectangle(31, 1870, 73, 49), Color.Cyan * (opacity / 3f), 0f, new Vector2(36.5f, 22.5f), scale * 1.84f, SpriteEffects.None, 0.2f);
            batch.Draw(startMinigameTextures[0], screenMid, null, Color.Cyan * opacity, 0f, new Vector2(69f, 37f), scale, SpriteEffects.None, 0.3f);


            if (startMinigameData[3] != startMinigameArrowData.Length)//minigame not done
            {
                //info
                Vector2 textLoc = new Vector2(0f, height * -0.44f);
                for (int i = 1; i < startMinigameText.Count; i++)
                {
                    batch.DrawString(Game1.tinyFont, startMinigameText[i], screenMid + (textLoc += new Vector2(0f, height * 0.05f)), Color.AntiqueWhite * opacity, 0f, new Vector2(Game1.tinyFont.MeasureString(startMinigameText[i]).X / 2f, 0f), scale * 0.1f, SpriteEffects.None, 0.4f);
                }

                //if paused/out of focus:
                if ((Game1.paused || (!Game1.game1.IsActiveNoOverlay && Program.releaseBuild)) && (Game1.options == null || Game1.options.pauseWhenOutOfFocus || Game1.paused) && Game1.multiplayerMode == 0)
                {
                    batch.Draw(Game1.mouseCursors, screenMid, new Rectangle(322, 498, 12, 12), Color.Brown, 0f, new Vector2(6f), scale * 2f, SpriteEffects.None, 0.4f);
                    return;
                }
                //hit area rings
                Vector2 hitAreaMid = screenMid + new Vector2((int)(width * -0.2f), height * 0.18f);
                batch.Draw(startMinigameTextures[1], hitAreaMid, new Rectangle(355, 86, 26, 26), Color.Yellow * (opacity / 3f), 0f, new Vector2(13f), scale * 0.7f, SpriteEffects.None, 0.4f);
                batch.Draw(startMinigameTextures[1], hitAreaMid, new Rectangle(355, 86, 26, 26), Color.Brown * (opacity / 3f), 0f, new Vector2(13f), scale * 0.5f, SpriteEffects.None, 0.41f);

                //arrows
                Vector2 firstArrowLoc = new Vector2((int)(screenMid.X + (width / 2f)) + startMinigameTimer, screenMid.Y + (height * 0.18f));

                int speed = (int)(startMinigameScale * ((who.fishCaught.Count() == 0) ? 2 : 2 + (((difficulty - who.FishingLevel) / 10f) + (fishSize / 5)) * minigameDifficulty[screen]));
                if (startMinigameStage == 2) startMinigameTimer -= speed;


                startMinigameData[0] = -2;//-2 to not clash with treasure arrow
                startMinigameData[1] = 0;
                int arrowsLeft = startMinigameArrowData.Length;

                for (int i = 0; i < startMinigameArrowData.Length; i++)
                {
                    if (i == startMinigameData[4] && startMinigameTimer % (speed * 200) == 0)//flip treasure arrow clockwise
                    {
                        if (int.Parse(startMinigameArrowData[i][0].ToString()) == 3) startMinigameArrowData[i] = startMinigameArrowData[i].Remove(0, 1).Insert(0, "0");
                        else startMinigameArrowData[i] = startMinigameArrowData[i].Remove(0, 1).Insert(0, (int.Parse(startMinigameArrowData[i][0].ToString()) + 1).ToString());
                    }

                    int[] data = startMinigameArrowData[i].Split('/').Select(int.Parse).ToArray();//data = 0 direction, 1 colour, 2 offset from first, 3 current loc

                    if (data[1] == 0)//if empty arrow
                    {
                        if (hitAreaMid.X - (13f * scale * 0.5f) > firstArrowLoc.X + data[2])//too late - fail
                        {
                            data[1] = 3;
                            startMinigameArrowData[i] = startMinigameArrowData[i].Replace("/0/", "/-1/");
                        }
                        else if (hitAreaMid.X - (13f * scale * 0.5f) <= firstArrowLoc.X + data[2] &&
                                 hitAreaMid.X + (13f * scale * 0.5f) >= firstArrowLoc.X + data[2])                          //in regular hit area
                        {
                            startMinigameData[0] = i;

                            if (hitAreaMid.X - (13f * scale * 0.1f) <= firstArrowLoc.X + data[2] &&
                                hitAreaMid.X + (13f * scale * 0.1f) >= firstArrowLoc.X + data[2]) startMinigameData[1] = 1; //+ in perfect area
                        }
                    }


                    if (firstArrowLoc.X + data[2] + (6f * scale) <= screenMid.X + (width * 0.464f))//arrow passed start
                    {
                        arrowsLeft--;

                        if (firstArrowLoc.X + data[2] - (6f * scale) >= screenMid.X - (width * 0.464f))//arrow didn't pass end
                        {
                            Color color = (data[1] == 2) ? Color.LimeGreen : (data[1] == 1) ? Color.Orange : (data[1] == -1) ? Color.Red : (i == startMinigameData[4]) ? Color.LightPink : Color.Cyan;
                            batch.Draw(startMinigameTextures[1], firstArrowLoc + new Vector2((data[2]), 0), new Rectangle((data[0] == 0 || data[0] == 2) ? 338 : 322, 82, 12, 12),
                                color, 0f, new Vector2(6f), scale, (data[0] == 0) ? SpriteEffects.FlipVertically : (data[0] == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0.4f);
                            //treasure arrow?
                            if (i == startMinigameData[4]) batch.Draw(Game1.mouseCursors, firstArrowLoc + new Vector2((data[2]), 0), new Rectangle((treasureCaught) ? 104 : (data[1] == -1) ? 167 : 71, 1926, 20, 26),
                                Color.White, 0f, new Vector2(9f, 14f), scale * 0.2f, SpriteEffects.None, 0.41f);
                        }
                        else if (i + 1 > startMinigameData[3])//update score on new arrow passed end
                        {
                            startMinigameData[3] = i + 1;
                            startMinigameData[2] = 0;
                            for (int j = 0; j < i + 1; j++)
                            {
                                startMinigameData[2] += int.Parse(startMinigameArrowData[j].Split('/')[1]);
                            }
                        }
                    }
                }

                //arrow 'dispensers'
                batch.Draw(Game1.mouseCursors, screenMid + new Vector2(width * 0.464f, height * 0.18f), new Rectangle(301, 288, 15, 15), Color.DodgerBlue * ((scale < 0.95f) ? scale : 0.95f), 0f, new Vector2(15f, 7.5f), scale, SpriteEffects.None, 0.5f);
                batch.Draw(Game1.mouseCursors, screenMid + new Vector2(width * -0.464f, height * 0.18f), new Rectangle(301, 288, 15, 15), Color.DodgerBlue * ((scale < 0.95f) ? scale : 0.95f), 0f, new Vector2(0f, 7.5f), scale, SpriteEffects.FlipHorizontally, 0.5f);
                //score count
                batch.DrawString(Game1.smallFont, startMinigameData[2].ToString(), screenMid + new Vector2(width * -0.41f, height * 0.19f),
                    ((startMinigameData[2] < startMinigameArrowData.Length * 0.4f) ? Color.Crimson :
                    (startMinigameData[2] < startMinigameArrowData.Length * 0.8f) ? Color.Red :
                    (startMinigameData[2] < startMinigameArrowData.Length) ? Color.Orange :
                    (startMinigameData[2] < startMinigameArrowData.Length * 1.2f) ? Color.Yellow :
                    (startMinigameData[2] < startMinigameArrowData.Length * 1.5f) ? Color.LimeGreen :
                    (startMinigameData[2] < startMinigameArrowData.Length * 1.9f) ? Color.Green : Color.Purple) * opacity,
                    0f, Game1.smallFont.MeasureString(startMinigameData[2].ToString()) / 2f, scale * 0.28f, SpriteEffects.None, 0.51f);
                //arrows left count
                batch.DrawString(Game1.smallFont, arrowsLeft.ToString(), screenMid + new Vector2(width * 0.41f, height * 0.19f), Color.DarkTurquoise * opacity, 0f, Game1.smallFont.MeasureString(arrowsLeft.ToString()) / 2f, scale * 0.28f, SpriteEffects.None, 0.51f);
            }
            else
            {
                batch.DrawString(Game1.smallFont, startMinigameText[0] + ": " + startMinigameData[2], screenMid, Color.AntiqueWhite * opacity, 0f, Game1.smallFont.MeasureString(startMinigameText[0] + ": " + startMinigameData[2]) / 2f, scale * 0.5f, SpriteEffects.None, 0.4f);
                if (startMinigameStage < 5) startMinigameStage = 3;
            }
        }
        private void StartMinigameInput(ButtonsChangedEventArgs e)
        {
            if (startMinigameStage == 2)
            {
                if (startMinigameData[0] >= 0)
                {
                    bool passed = false;
                    switch (startMinigameArrowData[startMinigameData[0]].Split('/')[0])
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
                        if (startMinigameData[0] == startMinigameData[4])//treasure arrow?
                        {
                            who.currentLocation.localSound("coin");
                            treasureCaught = true;
                        }
                        if (startMinigameData[1] == 1) startMinigameArrowData[startMinigameData[0]] = startMinigameArrowData[startMinigameData[0]].Replace("/0/", "/2/");//perfect?
                        else startMinigameArrowData[startMinigameData[0]] = startMinigameArrowData[startMinigameData[0]].Replace("/0/", "/1/");
                    }
                    else startMinigameArrowData[startMinigameData[0]] = startMinigameArrowData[startMinigameData[0]].Replace("/0/", "/-1/");
                }
                //else negative point if hit when arrow outside box? maybe if 2 in a row to avoid spam cheeze? can do it by changing data into list and adding red arrows
            }
            else
            {
                startMinigameData[5] = 60;

                if (startMinigameData[2] < startMinigameArrowData.Length * 0.8f) startMinigameStage = 5;
                else
                {
                    if (startMinigameData[2] < startMinigameArrowData.Length) startMinigameStage = 6;
                    else if (startMinigameData[2] < startMinigameArrowData.Length * 1.2f) startMinigameStage = 7;
                    else if (startMinigameData[2] < startMinigameArrowData.Length * 1.5f) startMinigameStage = 8;
                    else if (startMinigameData[2] < startMinigameArrowData.Length * 1.9f) startMinigameStage = 9;
                    else startMinigameStage = 10;
                }
            }
        }
        private void EndMinigame(int stage)
        {
            if (ModEntry.config.EndMinigameStyle[screen] == 3)
            {
                if (Game1.options.gamepadControls || Game1.options.gamepadMode == Options.GamepadModes.ForceOn || Game1.options.gamepadMode != Options.GamepadModes.ForceOff) endMinigameStyle[screen] = 2;
                else if (System.Text.Encoding.ASCII.GetString(System.Text.Encoding.ASCII.GetBytes(item.DisplayName)).Replace(" ", "").Replace("?", "").Length < 1) endMinigameStyle[screen] = 2;
                else endMinigameStyle[screen] = 3;
            }

            if (stage == 0) //pick button + show alert/button sprite
            {
                endMinigameStage = 3;
                endMinigameTimer = 0;
                who.PlayFishBiteChime();

                string sprite = "LooseSprites\\Cursors";
                Rectangle rect = new Rectangle(395, 497, 3, 8);
                Vector2 offset = new Vector2(-7.5f, 0);
                Color color = Color.White;

                switch (endMinigameStyle[screen])
                {
                    case 1:
                        break;
                    case 2:
                        color = Color.Gold;
                        offset = new Vector2(-25f, -20f);
                        int direction = Game1.random.Next(0, 4);
                        endMinigameKey = direction.ToString();
                        switch (direction)
                        {
                            case 0://up
                                rect = new Rectangle(407, 1651, 10, 10);
                                break;
                            case 1://right
                                rect = new Rectangle(416, 1660, 10, 10);
                                break;
                            case 2://down
                                rect = new Rectangle(407, 1660, 10, 10);
                                break;
                            case 3://left
                                rect = new Rectangle(398, 1660, 10, 10);
                                break;
                        }
                        if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.zh) rect.Y--; //language texture adjustment
                        else if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko)
                        {
                            rect.X += 3;
                            rect.Y--;
                        }
                        break;
                    case 3:
                        string temp = System.Text.Encoding.ASCII.GetString(System.Text.Encoding.ASCII.GetBytes(item.DisplayName)).Replace(" ", "").Replace("?", "");
                        endMinigameKey = temp[Game1.random.Next(0, temp.Length)].ToString().ToUpper();
                        return;
                }
                Game1.screenOverlayTempSprites.Add(new TemporaryAnimatedSprite(sprite, rect, new Vector2(who.getStandingX() - Game1.viewport.X, who.getStandingY() - 136 - Game1.viewport.Y) + offset, flipped: false, 0.02f, color)
                {
                    scale = 5f,
                    scaleChange = -0.01f,
                    motion = new Vector2(0f, -0.5f),
                    shakeIntensityChange = -0.005f,
                    shakeIntensity = 1f
                });
            }
            else
            {
                drawTool = true;
                SendMessage(who, "");
                if (oldFacingDirection == 0) who.armOffset += new Vector2(-10f, 0);
                bool passed = false;
                if (endMinigameStage == 2)//too early
                {
                    PlayPause(who);
                    SendMessage(who, "Fail");
                }
                else //button press, if sprite appeared
                {
                    switch (endMinigameStyle[screen])
                    {
                        case 1:
                            if (keyBinds[screen].JustPressed()) passed = true;
                            break;
                        case 2:
                            switch (endMinigameKey)
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
                            break;
                        case 3:
                            if (KeybindList.Parse(endMinigameKey).JustPressed()) passed = true;
                            break;
                    }
                }
                if (passed)
                {
                    int totalDifficulty = (int)((30f + ((endMinigameStyle[screen] == 3) ? 20f : (endMinigameStyle[screen] == 2) ? 10f : 0f)) - ((difficulty / 33f) - (fishSize / 33f) * minigameDifficulty[screen]));

                    if (endMinigameStage == 3 && endMinigameTimer < totalDifficulty) endMinigameStage = 10;
                    else endMinigameStage = 9;

                    ClearAnimations(who);
                    SendMessage(who, "ClearAnim");
                    SwingAndEmote(who, 0);
                    SendMessage(who, "Swing");
                    endMinigameTimer = 0;
                }
                else //button press, too early or wrong
                {
                    Game1.playSound("fishEscape");
                    endMinigameStage = 3;
                    endMinigameTimer = 1000;
                }
            }
        }

        private void PlayerCaughtFishEndFunction(int forceStage = 0)
        {
            if (forceStage == 1) stage = "Caught1";

            FishingRod rod = who.CurrentTool as FishingRod;
            Helper.Reflection.GetField<Farmer>(rod, "lastUser").SetValue(who);
            Helper.Reflection.GetField<int>(rod, "whichFish").SetValue(whichFish);
            Helper.Reflection.GetField<bool>(rod, "caughtDoubleFish").SetValue(caughtDoubleFish);
            Helper.Reflection.GetField<int>(rod, "fishQuality").SetValue(fishQuality);
            Helper.Reflection.GetField<int>(rod, "clearWaterDistance").SetValue(clearWaterDistance);
            Helper.Reflection.GetField<Farmer>(who.CurrentTool, "lastUser").SetValue(who);

            switch (stage)
            {
                case "Caught1":
                    drawTool = false;
                    SendMessage(who);

                    CatchFishAfterMinigame(who);

                    if (!fromFishPond)
                    {
                        if (endMinigameStage == 8)//water on face
                        {
                            animations.Add(new KeyValuePair<long, TemporaryAnimatedSprite>(who.UniqueMultiplayerID, new TemporaryAnimatedSprite(10, who.Position - new Vector2(0, 120), Color.Blue)
                            {
                                motion = new Vector2(0f, 0.12f),
                                timeBasedMotion = true,
                            }));
                            SendMessage(who, "Water");
                        }
                        if (fishingFestivalMinigame == 0)
                        {


                            recordSize = who.caughtFish(whichFish, (metricSizes) ? (int)(fishSize * 2.54f) : (int)fishSize, false, caughtDoubleFish ? 2 : 1);
                            if (bossFish)
                            {
                                Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14068"));
                                string name = Game1.objectInformation[whichFish].Split('/')[4];

                                //Helper.Reflection.GetField<Multiplayer>(Game1.game1, "multiplayer").GetValue().globalChatInfoMessage("CaughtLegendaryFish", new string[] { who.Name, name }); //multiplayer class is not protected
                                if (Game1.IsMultiplayer || Game1.multiplayerMode != 0)
                                {
                                    if (Game1.IsClient) Game1.client.sendMessage(15, "CaughtLegendaryFish", new string[] { who.Name, name });
                                    else if (Game1.IsServer)
                                    {
                                        foreach (long id in Game1.otherFarmers.Keys)
                                        {
                                            Game1.server.sendMessage(id, 15, who, "CaughtLegendaryFish", new string[] { who.Name, name });
                                        }
                                    }
                                }
                            }
                            else if (recordSize)
                            {
                                sparklingText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14069"), Color.LimeGreen, Color.Azure, false, 0.1, 2500, -1, 500, 1f);
                                who.currentLocation.localSound("newRecord");
                            }
                        }
                        SwingAndEmote(who, 2);
                    }


                    Monitor.Log($"caught fish end");
                    who.Halt();
                    who.armOffset = Vector2.Zero;

                    stage = "Caught2";
                    if (fishingFestivalMinigame == 2)
                    {
                        if (!itemIsInstantCatch)
                        {
                            if (endMinigameStage == 10 || festivalMode[screen] == 1) Game1.CurrentEvent.caughtFish(whichFish, (int)fishSize, who);
                        }
                        stageTimer = 1;
                    }
                    else//adding items + chest
                    {
                        if (!treasureCaught || itemIsInstantCatch)
                        {
                            if (!itemIsInstantCatch) rod.doneFishing(who, true);

                            //maybe extra checks will help split screen issue where menu sometimes pops up even though there's space in inventory
                            if (!who.couldInventoryAcceptThisItem(item) || !who.addItemToInventoryBool(item)) who.addItemByMenuIfNecessary(item);
                            stageTimer = 1;
                        }
                        else
                        {
                            who.currentLocation.localSound("openChest");

                            animations.Add(new KeyValuePair<long, TemporaryAnimatedSprite>(who.UniqueMultiplayerID, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(64, 1920, 32, 32), 200f, 4, 0, who.Position + new Vector2(-32f, -228f), flicker: false, flipped: false, (float)who.getStandingY() / 10000f + 0.001f, 0f, Color.White, 4f, 0f, 0f, 0f)
                            {
                                motion = new Vector2(0f, -0.128f),
                                timeBasedMotion = true,
                                alpha = 0f,
                                alphaFade = -0.002f,
                                endFunction = rod.openTreasureMenuEndFunction,
                                extraInfoForEndBehavior = (!who.addItemToInventoryBool(item)) ? 1 : 0
                            }));
                            stageTimer = 60;
                        }
                    }
                    break;
                case "Caught2":
                    if (!Game1.IsMultiplayer && !Game1.isFestival()) Game1.gameTimeInterval = oldGameTimeInterval;
                    fishCaught = true;

                    Helper.Multiplayer.SendMessage(-1, "whichFish", modIDs: new[] { "barteke22.FishingInfoOverlays" }, new[] { who.UniqueMultiplayerID });//clear overlay
                    break;
            }
        }


        private void SwingAndEmote(Farmer who, int which) //could maybe move this to animations
        {

            if (which < 2)
            {
                if (!fromFishPond && endMinigameStyle[screen] > 0) //swing animation
                {
                    if (endMinigameStage == 10) showPerfect = true;
                    who.completelyStopAnimatingOrDoingAction();
                    (who.CurrentTool as FishingRod).setTimingCastAnimation(who);
                    switch (oldFacingDirection)
                    {
                        case 0://up
                            who.FarmerSprite.animateOnce(295, 1f, 1);
                            who.CurrentTool.Update(0, 0, who);
                            break;
                        case 1://right
                            who.FarmerSprite.animateOnce(296, 1f, 1);
                            who.CurrentTool.Update(1, 0, who);
                            break;
                        case 2://down
                            who.armOffset += new Vector2(-10f, 0);
                            who.FarmerSprite.animateOnce(297, 1f, 1);
                            who.CurrentTool.Update(2, 0, who);
                            break;
                        case 3://left
                            who.FarmerSprite.animateOnce(298, 1f, 1);
                            who.CurrentTool.Update(3, 0, who);
                            break;
                    }
                    stage = "Caught1";
                    stageTimer = 18;
                }
            }
            else if (which == 2)//emotes
            {
                if (startMinigameStyle[screen] + endMinigameStyle[screen] > 0)
                {
                    if (startMinigameStage == 4) //for now 4 = cancel = X
                    {
                        who.doEmote(36);
                        who.netDoEmote("x");
                    }
                    else if (startMinigameStage == 5) //for now 5 = fail = Angry
                    {
                        who.doEmote(12);
                        who.netDoEmote("angry");
                    }
                    else if (endMinigameStage == 8) //8 = hit = Uh
                    {
                        who.doEmote(10);
                        who.netDoEmote("angry");
                    }
                    else //otherwise = happy
                    {
                        who.doEmote(32);
                        who.netDoEmote("happy");
                    }
                }
            }
            else if (which == 3)//controller above head while startminigame
            {
                batch.Draw(Game1.emoteSpriteSheet, who.getLocalPosition(Game1.viewport) + new Vector2(0f, -160f),
                    new Rectangle(52 * 16 % Game1.emoteSpriteSheet.Width, 52 * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16),
                    Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0f);
            }
        }

        private void CaughtBubble(Farmer who)
        {
            int whichFish = this.whichFish;
            int fishQuality = this.fishQuality;
            int maxFishSize = this.maxFishSize;
            float fishSize = this.fishSize;
            float itemSpriteSize = this.itemSpriteSize;
            bool caughtDoubleFish = this.caughtDoubleFish;
            bool recordSize = this.recordSize;
            bool furniture;
            Rectangle sourceRect = this.sourceRect;
            if (who == this.who)
            {
                if (!(who.CurrentItem is FishingRod))//cancel for scrolling
                {
                    infoTimer = 0;
                    return;
                }
                furniture = item is Furniture;
            }
            else
            {
                whichFish = messages[who.UniqueMultiplayerID].whichFish;
                fishQuality = messages[who.UniqueMultiplayerID].fishQuality;
                maxFishSize = messages[who.UniqueMultiplayerID].maxFishSize;
                fishSize = messages[who.UniqueMultiplayerID].fishSize;
                itemSpriteSize = messages[who.UniqueMultiplayerID].itemSpriteSize;
                caughtDoubleFish = messages[who.UniqueMultiplayerID].count > 1;
                recordSize = messages[who.UniqueMultiplayerID].recordSize;
                furniture = messages[who.UniqueMultiplayerID].furniture;
                sourceRect = messages[who.UniqueMultiplayerID].sourceRect;
            }
            //arm up
            who.FacingDirection = 2;
            who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(84, 150) });

            if (maxFishSize > 0)
            {
                //bubble
                batch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(-130f, -75f)), new Rectangle(156, 465, 5, 20), Color.White * 0.8f, -1.57f, Vector2.Zero, 5f, SpriteEffects.None, 0f);
                batch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(-130f, -50f)), new Rectangle(149, 465, 5, 24), Color.White * 0.8f, -1.57f, Vector2.Zero, 5f, SpriteEffects.None, 0f);
                batch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(-130f, -25f)), new Rectangle(141, 465, 5, 20), Color.White * 0.8f, -1.57f, Vector2.Zero, 5f, SpriteEffects.None, 0f);
                //stars
                if (fishQuality > 0)
                {
                    Rectangle quality_rect = (fishQuality < 4) ? new Rectangle(338 + (fishQuality - 1) * 8, 400, 8, 8) : new Rectangle(346, 392, 8, 8);
                    batch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(-104f, -48f)), quality_rect, Color.White, 0f, new Vector2(4f, 4f), 2f, SpriteEffects.None, 0.1f);
                    batch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(-56f, -48f)), quality_rect, Color.White, 0f, new Vector2(4f, 4f), 2f, SpriteEffects.None, 0.1f);
                }
                //strings
                float offset = 0f;
                if (caughtDoubleFish || fishQuality > 0) offset = 13f;
                if (caughtDoubleFish)
                {
                    batch.DrawString(Game1.smallFont, "x2", Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(-91f, -60f)), Game1.textColor, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.1f);
                }
                string sizeString = Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14083", (metricSizes || LocalizedContentManager.CurrentLanguageCode != 0) ? Math.Round(fishSize * 2.54f) : (fishSize));
                batch.DrawString(Game1.smallFont, sizeString, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(-80f - Game1.smallFont.MeasureString(sizeString).X / 2f, -77f - offset)), recordSize ? Color.Blue : Game1.textColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
            }
            //fishing net
            batch.Draw(Game1.toolSpriteSheet, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(+22f, +17f)), Game1.getSourceRectForStandardTileSheet(Game1.toolSpriteSheet, who.CurrentTool.IndexOfMenuItemView, 16, 16), Color.White, -3f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 0f);

            //item(s) in hand
            if (!furniture)
            {
                batch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(12f, -44f)), sourceRect, Color.White, (fishSize == -1 || whichFish == 800 || whichFish == 798 || whichFish == 149 || whichFish == 151) ? 0.5f : (caughtDoubleFish) ? 2.2f : 2.4f, new Vector2(8f, 8f), itemSpriteSize, SpriteEffects.None, 0.1f);

                if (caughtDoubleFish) batch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(4f, -44f)), sourceRect, Color.White, (fishSize == -1 || whichFish == 800 || whichFish == 798 || whichFish == 149 || whichFish == 151) ? 1f : 2.6f, new Vector2(8f, 8f), itemSpriteSize, SpriteEffects.None, 0.1f);
            }
            else batch.Draw(Furniture.furnitureTexture, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(0f, -56f)), sourceRect, Color.White, 0f, new Vector2(8f, 8f), 2.25f, SpriteEffects.None, 0.1f);
        }

        private void FestivalGameSkip(Farmer who)
        {

            if (!hereFishying)
            {
                hereFishying = true;
                who.FacingDirection = 3;

                if (fishySound != null) fishySound.Play(voiceVolume, voicePitch[screen], 0);
                who.synchronizedJump(8f);

                stage = "Festival1";
                stageTimer = Game1.random.Next(120, 180);
                return;
            }
            switch (stage)
            {
                case "Festival1":
                    Event ev = Game1.CurrentEvent;
                    ev.caughtFish(137, Game1.random.Next(0, 20), who);
                    if (Game1.random.Next(0, 5) == 0) ev.perfectFishing();

                    stage = "Festival2";
                    stageTimer = Game1.random.Next(180, 240);
                    break;

                case "Festival2":
                    who.synchronizedJump(6f);
                    who.faceDirection(0);

                    stage = "Festival3";
                    stageTimer = 12;
                    break;
                case "Festival3":
                    who.synchronizedJump(6f);
                    who.faceDirection(1);

                    stage = "Festival4";
                    stageTimer = 12;
                    break;
                case "Festival4":
                    who.synchronizedJump(6f);
                    who.faceDirection(2);

                    stage = "Festival5";
                    stageTimer = 12;
                    break;
                case "Festival5":
                    who.synchronizedJump(6f);
                    who.faceDirection(3);

                    hereFishying = false;

                    if (keyBinds[screen].IsDown()
                        || Helper.Input.IsSuppressed(Game1.options.useToolButton[0].ToSButton())
                        || Helper.Input.IsSuppressed(Game1.options.useToolButton[1].ToSButton())
                        || Helper.Input.IsSuppressed(SButton.ControllerX)) FestivalGameSkip(who);
                    break;
            }
        }

        protected void SuppressAll(IEnumerable<SButton> buttons)
        {
            foreach (SButton button in buttons)
                Helper.Input.Suppress(button);
        }
        private void EmergencyCancel(Farmer who)
        {
            endMinigameStage = 0;
            startMinigameStage = 0;
            who.UsingTool = false;
            who.Halt();
            drawTool = false;
            who.completelyStopAnimatingOrDoingAction();
            ClearAnimations(who);
            SendMessage(who, "ClearAnim");
            SendMessage(who, "Clear");
            hereFishying = false;
            stage = null;
            if (oldGameTimeInterval > 0 && !Game1.IsMultiplayer && !Game1.isFestival()) Game1.gameTimeInterval = oldGameTimeInterval;
            who.CanMove = true;
        }
        private void ClearAnimations(Farmer who)
        {
            for (int i = 0; i < animations.Count; i++)
            {
                if (animations[i].Key == who.UniqueMultiplayerID)
                {
                    animations.RemoveAt(i);
                    i--;
                }
            }
        }
        private void PlayPause(Farmer who)
        {
            foreach (var anim in animations)
            {
                if (anim.Key == who.UniqueMultiplayerID)
                {
                    if (anim.Value.paused) anim.Value.paused = false;
                    else anim.Value.paused = true;
                }
            }
        }


        private void SendMessage(Farmer who, string stageRequested = null)
        {
            long[] IDs = Helper.Multiplayer.GetConnectedPlayers().Select(x => x.PlayerID).ToArray();

            if (IDs == null) return;

            if (stageRequested == null) stageRequested = stage;
            int stack = 1;
            bool furniture = false;
            if (item != null)
            {
                stack = item.Stack;
                furniture = item is Furniture;
            }
            Helper.Multiplayer.SendMessage(new MinigameMessage(who, stageRequested, voicePitch[screen], drawTool, whichFish, fishQuality, maxFishSize, fishSize, itemSpriteSize,
                stack, recordSize, furniture, sourceRect, x, y), "Animation", modIDs: new[] { ModManifest.UniqueID }, IDs);
        }

        /// <summary>Other players' animations.</summary>
        public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == ModManifest.UniqueID && e.Type == "Animation")
            {
                MinigameMessage message = e.ReadAs<MinigameMessage>();
                messages[message.multiplayerID] = message;

                if (message.multiplayerID == who.UniqueMultiplayerID) return;
                else
                {
                    Farmer who = null;
                    foreach (Farmer other in Game1.getAllFarmers())
                    {
                        if (other.UniqueMultiplayerID == message.multiplayerID) who = other;
                    }
                    if (who == null) return;

                    int x = message.x;
                    int y = message.y;

                    switch (message.stage)
                    {
                        case "Clear":
                            who.completelyStopAnimatingOrDoingAction();
                            break;
                        case "ClearAnim":
                            ClearAnimations(who);
                            break;
                        case "Pause":
                            PlayPause(who);
                            break;
                        case "Swing":
                            (who.CurrentTool as FishingRod).setTimingCastAnimation(who);
                            switch (who.FacingDirection)
                            {
                                case 0://up
                                    who.FarmerSprite.animateOnce(295, 1f, 1);
                                    who.CurrentTool.Update(0, 0, who);
                                    break;
                                case 1://right
                                    who.FarmerSprite.animateOnce(296, 1f, 1);
                                    who.CurrentTool.Update(1, 0, who);
                                    break;
                                case 2://down
                                    who.armOffset += new Vector2(-10f, 0);
                                    who.FarmerSprite.animateOnce(297, 1f, 1);
                                    who.CurrentTool.Update(2, 0, who);
                                    break;
                                case 3://left
                                    who.FarmerSprite.animateOnce(298, 1f, 1);
                                    who.CurrentTool.Update(3, 0, who);
                                    break;
                            }
                            break;
                        case "Fail":
                        case null:
                            if (message.stage != null) PlayPause(who);
                            else if (fishySound != null)//volume based on distance? will be iffy in split... play at the same time?
                            {
                                float volume = 0f;
                                if (who.currentLocation == this.who.currentLocation) volume = Math.Min(Math.Max((Vector2.Distance(who.Position, this.who.Position) - 2560f) / -2560f, 0f), 1f);//distance based volume within 40ish tiles
                                if (Context.IsSplitScreen)
                                {
                                    int screen = Context.ScreenId;
                                    if (Helper.Multiplayer.GetConnectedPlayer(message.multiplayerID).IsSplitScreen)
                                    {
                                        int screen2 = (int)Helper.Multiplayer.GetConnectedPlayer(message.multiplayerID).ScreenID;
                                        fishySound.Play(voiceVolume, message.voicePitch, (screen2 == 0 || screen2 == 2) ? -1f : 1f); //if local split screen, screen 0 and 2 play on left, 1, 3 on right, * distance on other side
                                        if (((screen == 0 || screen == 2) && (screen2 == 1 || screen2 == 3)) ||
                                            ((screen == 1 || screen == 3) && (screen2 == 0 || screen2 == 2))) fishySound.Play(volume * voiceVolume, message.voicePitch, (screen == 0 || screen == 2) ? -1f : 1f);
                                    }
                                    else fishySound.Play(volume * voiceVolume, message.voicePitch, (screen == 0 || screen == 2) ? -1f : 1f); //if split screen, but sender is not split screen (same as above * distance)
                                }
                                else fishySound.Play(volume * voiceVolume, message.voicePitch, 0); //not scplit screen, won't do directional sound as it can be annoying
                            }
                            who.completelyStopAnimatingOrDoingAction();
                            who.jitterStrength = 2f;
                            List<FarmerSprite.AnimationFrame> animationFrames = new List<FarmerSprite.AnimationFrame>(){
                                new FarmerSprite.AnimationFrame(94, 100, false, false, null, false).AddFrameAction(delegate (Farmer f) { f.jitterStrength = 2f; }) };
                            who.FarmerSprite.setCurrentAnimation(animationFrames.ToArray());
                            who.FarmerSprite.PauseForSingleAnimation = true;
                            who.FarmerSprite.loop = true;
                            who.FarmerSprite.loopThisAnimation = true;
                            who.Sprite.currentFrame = 94;
                            break;
                        case "Water":
                            animations.Add(new KeyValuePair<long, TemporaryAnimatedSprite>(who.UniqueMultiplayerID, new TemporaryAnimatedSprite(10, who.Position - new Vector2(0, 120), Color.Blue)
                            {
                                motion = new Vector2(0f, 0.12f),
                                timeBasedMotion = true,
                            }));
                            who.completelyStopAnimatingOrDoingAction();
                            break;
                        case "Starting4":
                            animations.Add(new KeyValuePair<long, TemporaryAnimatedSprite>(who.UniqueMultiplayerID, new TemporaryAnimatedSprite(Game1.emoteSpriteSheet.ToString(), new Rectangle(12 * 16 % Game1.emoteSpriteSheet.Width, 12 * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), 200, 1, 0, new Vector2(x, y - 32), false, false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f, false)));
                            break;
                        case "Starting5":
                            animations.Add(new KeyValuePair<long, TemporaryAnimatedSprite>(who.UniqueMultiplayerID, new TemporaryAnimatedSprite(Game1.emoteSpriteSheet.ToString(), new Rectangle(13 * 16 % Game1.emoteSpriteSheet.Width, 12 * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), 200, 1, 0, new Vector2(x, y - 32), false, false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f, false)));
                            break;
                        case "Starting6":
                            animations.Add(new KeyValuePair<long, TemporaryAnimatedSprite>(who.UniqueMultiplayerID, new TemporaryAnimatedSprite(Game1.emoteSpriteSheet.ToString(), new Rectangle(14 * 16 % Game1.emoteSpriteSheet.Width, 12 * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), 200, 1, 0, new Vector2(x, y - 32), false, false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f, false)));
                            break;
                        case "Starting7":
                            animations.Add(new KeyValuePair<long, TemporaryAnimatedSprite>(who.UniqueMultiplayerID, new TemporaryAnimatedSprite(Game1.emoteSpriteSheet.ToString(), new Rectangle(15 * 16 % Game1.emoteSpriteSheet.Width, 12 * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), 200, 1, 0, new Vector2(x, y - 32), false, false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f, false)));
                            break;
                        case "Starting8":
                            float t;
                            float distance = y - (float)(who.getStandingY() - 100);

                            float height = Math.Abs(distance + 170f);
                            if (who.FacingDirection == 0) height -= 130f;
                            else if (who.FacingDirection == 2) height -= 170f;
                            height = Math.Max(height, 0f);

                            float gravity = 0.002f;
                            float velocity = (float)Math.Sqrt((double)(2f * gravity * height));
                            t = (float)(Math.Sqrt((double)(2f * (height - distance) / gravity)) + (double)(velocity / gravity));
                            float xVelocity = 0f;
                            if (t != 0f)
                            {
                                xVelocity = (who.Position.X - x) / t;
                            }
                            animations.Add(new KeyValuePair<long, TemporaryAnimatedSprite>(who.UniqueMultiplayerID, new TemporaryAnimatedSprite((message.furniture) ? Furniture.furnitureTexture.ToString() : "Maps\\springobjects", message.sourceRect, t, 1, 0, new Vector2(x, y), false, false, y / 10000f, 0f, Color.White, 4f, 0f, 0f, 0f, false)
                            {
                                motion = new Vector2(xVelocity, -velocity),
                                acceleration = new Vector2(0f, gravity),
                                timeBasedMotion = true,
                                endSound = "tinyWhip"
                            }));
                            int delay = 25;
                            for (int i = 1; i < message.count; i++)
                            {
                                animations.Add(new KeyValuePair<long, TemporaryAnimatedSprite>(who.UniqueMultiplayerID, new TemporaryAnimatedSprite("Maps\\springobjects", message.sourceRect, t, 1, 0, new Vector2(x, y), false, false, y / 10000f, 0f, Color.White, 4f, 0f, 0f, 0f, false)
                                {
                                    delayBeforeAnimationStart = delay,
                                    motion = new Vector2(xVelocity, -velocity),
                                    acceleration = new Vector2(0f, gravity),
                                    timeBasedMotion = true,
                                    endSound = "tinyWhip",
                                    Parent = who.currentLocation
                                }));
                                delay += 25;
                            }
                            break;
                    }
                }
            }
        }
    }
}

public class MinigameMessage
{
    public long multiplayerID;
    public string stage;
    public float voicePitch;
    public bool drawTool;
    public int whichFish;
    public int fishQuality;
    public int maxFishSize;
    public float fishSize;
    public float itemSpriteSize;
    public int count;
    public bool recordSize;
    public bool furniture;
    public Rectangle sourceRect;
    public int x;
    public int y;


    public MinigameMessage()
    {
        this.multiplayerID = -1L;
        this.stage = null;
        this.sourceRect = new Rectangle();
    }

    public MinigameMessage(Farmer whichPlayer, string stage, float voice, bool drawTool, int whichFish, int fishQuality, int maxFishSize, float fishSize, float itemSpriteSize, int count, bool recordSize, bool furniture, Rectangle sourceRec, int x, int y)
    {
        this.multiplayerID = whichPlayer.UniqueMultiplayerID;
        this.stage = stage;
        this.voicePitch = voice;
        this.drawTool = drawTool;
        this.whichFish = whichFish;
        this.fishQuality = fishQuality;
        this.maxFishSize = maxFishSize;
        this.fishSize = fishSize;
        this.itemSpriteSize = itemSpriteSize;
        this.count = count;
        this.recordSize = recordSize;
        this.furniture = furniture;
        this.sourceRect = sourceRec;
        this.x = x;
        this.y = y;
    }
}
