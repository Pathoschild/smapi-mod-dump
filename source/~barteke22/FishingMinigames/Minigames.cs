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
using StardewValley.Extensions;
using StardewValley.Objects;
using StardewValley.SpecialOrders;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;

namespace FishingMinigames
{
    public class Minigames
    {
        public int nexusKey = 9345;
        private ModEntry entry;
        private IMonitor Monitor;
        public IModHelper Helper;
        private IManifest ModManifest;
        public ITranslationHelper translate;

        private SpriteBatch batch;
        private SparklingText sparklingText;
        private Farmer who;
        //private int fishingLevel;
        private int screen;
        private int x;
        private int y;
        private float modifier;

        private int caughtExtraFish;
        public string whichFish;
        private int minFishSize;
        private int maxFishSize;
        public float fishSize;
        private bool recordSize;
        private bool perfect;
        private int fishQuality;
        private bool fishCaught;
        public bool bossFish;
        private int difficulty;
        public string behavior;
        public bool treasureCaught;
        private bool showPerfect;
        private bool fromFishPond;
        private int clearWaterDistance;
        public Item item;


        private bool hereFishying;
        private bool itemIsInstantCatch;
        private int maxDistance;
        private float voiceVolumePersonal;
        private int oldFacingDirection;
        private float itemSpriteSize;
        private Rectangle sourceRect;
        private bool drawAttachments;

        private Vector2 aimTile;
        private bool keybindHeld;
        private string stage;
        private int stageTimer = -1;

        public bool debug;
        private FishingRod rodDummy;
        public int fishingFestivalMinigame;        //0 none, 1 fall16, 2 winter8
        private int festivalTimer;

        //private int startMinigameStage;             //0 before, 1 fade in, 2 playing, 3 show score, 4 cancel, 5 fail, 7-10 = score
        //private int[] startMinigameData;            //0 current arrow, 1 perfect area?, 2 score, 3 last arrow to vanish, 4 treasure arrow, 5 fade

        private int endMinigameStage;               //0 before, 1 fish flying, 2 input can fail, 3 input can succeed/time out, 8 failed, 9 success, 10 perfect
        private string endMinigameKey;
        private int endMinigameTimer;
        private int endMinigameDiff;
        private int infoTimer;

        private Dictionary<long, MinigameMessage> messages = new Dictionary<long, MinigameMessage>();
        public Dictionary<string, float> effects; //AREA, DAMAGE, DIFFICULTY, DOUBLE, LIFE, QUALITY, SIZE, SPEED, TREASURE, UNBREAKING0, UNBREAKING1


        //config values
        public static Dictionary<string, SoundEffect> voices;
        public static KeybindList[] keyBinds = new KeybindList[4];
        public static Dictionary<string, Dictionary<string, int>> itemData;
        public static float voiceVolume;
        public static string[] voiceType = new string[4];
        public static float[] voicePitch = new float[4];
        public static float[] minigameDamage = new float[4];
        public static bool[] freeAim = new bool[4];
        public static int[] startMinigameStyle = new int[4];
        public static int[] endMinigameStyle = new int[4];
        public static bool[] endCanLoseTreasure = new bool[4];
        public static bool realisticSizes;
        public static bool metricSizes;
        public static bool fishTankSprites;
        public static int[] festivalMode = new int[4];
        public static float[] minigameDifficulty = new float[4];
        public static Color minigameColor;

        public static readonly string[] JUNK = ["(O)167", "(O)168", "(O)169", "(O)170", "(O)171", "(O)172"];
        public const string TRASH = "(O)168";


        private MinigamesStart start;



        public Minigames(ModEntry entry)
        {
            this.entry = entry;
            this.Helper = entry.Helper;
            this.Monitor = entry.Monitor;
            this.ModManifest = entry.ModManifest;
            this.translate = entry.Helper.Translation;

            screen = Context.ScreenId;
            who = Game1.player;
        }

        public void Input_ButtonsChanged(object sender, ButtonsChangedEventArgs e)  //this.Monitor.Log(locationName, LogLevel.Debug);
        {
            if (!Context.IsWorldReady || Game1.emoteMenu != null) return;


            if (infoTimer > 0 && infoTimer < 1001)//reset from bubble
            {
                hereFishying = false;
                infoTimer = 0;
                ClearAnimations(who);
                SendMessage(who, "ClearAnim");
                SendMessage(who, "Clear");
                who.completelyStopAnimatingOrDoingAction();
                who.faceDirection(oldFacingDirection);
            }

            if (hereFishying || keybindHeld)
            {
                SuppressAll(e);
            }
            else if (keyBinds[screen].JustPressed()) //cancel regular rod use, if it's a shared keybind
            {
                if (!Context.IsPlayerFree) return;
                if ((Game1.activeClickableMenu == null || Game1.activeClickableMenu is DummyMenu) && who.CurrentItem is FishingRod)
                {
                    if (Game1.isFestival() && fishingFestivalMinigame > 0 && festivalMode[screen] == 0) return;//fishing + other festivals
                    SuppressAll(e);
                }
            }

            if (e.Pressed.Contains(SButton.F5))
            {
                EmergencyCancel();
                return;
            }
            if (e.Pressed.Contains(SButton.Z))//-----------------------------debug
            {
                if (start == null) DebugConsoleStartMinigameTest(new string[] { "139" });
                return;
            }


            if (endMinigameStage == 2 || endMinigameStage == 3) //already in endMinigame
            {
                SuppressAll(e);
                EndMinigame(1);
            }
            else if (start != null)//already in startMinigame
            {
                SuppressAll(e);
                start.MainInput(e);
                //if (start.MainInput(e)) //cancel
                //{
                //    DrawAndEmote(who, 2);
                //    EmergencyCancel();
                //    return;
                //}
            }
            else if (keyBinds[screen].JustPressed() && freeAim[screen] && (fishingFestivalMinigame == 0 || festivalTimer > 3000)) TryFishingHere(aimTile);//start attempt
        }

        public void GameLoop_UpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            fishingFestivalMinigame = 0;
            if (Game1.isFestival())
            {
                string data = Helper.Reflection.GetField<Dictionary<string, string>>(Game1.CurrentEvent, "festivalData").GetValue()["file"];
                if (data != null)
                {
                    festivalTimer = 0;
                    if (data.Equals("fall16") && Game1.currentMinigame is StardewValley.Minigames.FishingGame)
                    {
                        festivalTimer = Helper.Reflection.GetField<int>(Game1.currentMinigame as StardewValley.Minigames.FishingGame, "gameEndTimer").GetValue();
                        if (festivalTimer < 100000 && festivalTimer > 0) fishingFestivalMinigame = 1;
                    }
                    else if (data.Equals("winter8"))
                    {
                        festivalTimer = Game1.CurrentEvent.festivalTimer;
                        if (festivalTimer < 120000 && festivalTimer > 0) fishingFestivalMinigame = 2;
                    }
                    if (festivalTimer <= 1000 && fishingFestivalMinigame > 0 && festivalMode[screen] > 0) EmergencyCancel();
                }
            }
            if (start != null)
            {
                start.fishingFestivalMinigame = fishingFestivalMinigame;
                start.festivalTimer = festivalTimer;
            }

            if (sparklingText != null && sparklingText.update(Game1.currentGameTime))
            {
                sparklingText = null;
            }

            //start fade-out
            if (start != null && start.minigameStage > 3 && start.minigameData[5] > 0)//fade out and continue - test if keeping it here fixes transition glitches
            {
                //if (start.minigameData[5] > 0 && start.minigameData[5] < 70)
                //{
                //    float f = (start.minigameData[5] / 250f) + 0.01f;
                //    if (Context.IsSplitScreen) Game1.options.localCoopBaseZoomLevel -= f;
                //    else Game1.options.singlePlayerBaseZoomLevel -= f;

                //    int y = who.getStandingY() - Math.Min(start.minigameData[5] * 2, 80) - (Game1.viewport.Height / 2);
                //    if (y + Game1.viewport.Height < who.currentLocation.Map.DisplayHeight) Game1.viewport.Y = y;

                //    if (Game1.viewport.X < 0) Game1.viewport.X = 0;
                //    if (Game1.viewport.Y < 0) Game1.viewport.Y = 0;
                //}

                start.minigameData[5]--;

                if (start.minigameData[5] == 0)
                {
                    //who.currentLocation.forceViewportPlayerFollow = start.oldViewportFollow;
                    //Game1.viewportFreeze = false;
                    //if (Context.IsSplitScreen) Game1.options.localCoopBaseZoomLevel = start.oldZoom;
                    //else Game1.options.singlePlayerBaseZoomLevel = start.oldZoom;
                    stage = null;
                    if (start.minigameStage < 6)//4= cancel, 5= fail
                    {
                        DrawAndEmote(who, 2);
                        EmergencyCancel();
                    }
                    else HereFishyAnimation(who, x, y);
                    return;
                }
            }
            //fish flying
            else if (endMinigameStage > 0)
            {
                foreach (var anim in who.currentLocation.TemporarySprites)
                {
                    if (anim.id == nexusKey && anim.owner == who)
                    {
                        int size = (int)(itemSpriteSize * ((item is Furniture) ? 32 : 16));
                        if (endMinigameStage == 1)//can miss area
                        {
                            Rectangle area = new Rectangle((int)who.Position.X - 200, (int)who.Position.Y - 450, 400, 400);
                            if (anim.Position != anim.initialPosition &&
                                (area.Contains((int)anim.Position.X, (int)anim.Position.Y) ||
                                area.Contains((int)anim.Position.X, (int)anim.Position.Y + size) ||
                                area.Contains((int)anim.Position.X + size, (int)anim.Position.Y) ||
                                area.Contains((int)anim.Position.X + size, (int)anim.Position.Y + size)))
                            {
                                Game1.activeClickableMenu = new DummyMenu();
                                endMinigameStage = 2;
                            }
                        }
                        if (endMinigameStage == 2)//can succeed area
                        {
                            Rectangle area = new Rectangle((int)who.Position.X - 70, (int)who.Position.Y - 115, 140, 220);
                            if (anim.Position != anim.initialPosition &&
                                (area.Contains((int)anim.Position.X, (int)anim.Position.Y) ||
                                area.Contains((int)anim.Position.X, (int)anim.Position.Y + size) ||
                                area.Contains((int)anim.Position.X + size, (int)anim.Position.Y) ||
                                area.Contains((int)anim.Position.X + size, (int)anim.Position.Y + size)))
                            {
                                PlayPause(who);
                                SendMessage(who, "Pause");
                                EndMinigame(0);
                            }
                        }
                        else if (endMinigameStage == 3)//too late timer
                        {
                            endMinigameTimer++;
                            //Monitor.Log($"End: {endMinigameTimer}   " + DateTime.Now.ToString("HH:mm:ss"), LogLevel.Error);

                            if (endMinigameTimer > endMinigameDiff)//too late/missed/failed
                            {
                                if (effects["LIFE"] > 0 && Game1.random.Next(0, 3) == 0)//saved by tackle?
                                {
                                    if ((who.CurrentTool as FishingRod).attachments[1].Quality == 0 && effects["UNBREAKING1"] < Game1.random.Next(1, 101))
                                    {
                                        (who.CurrentTool as FishingRod).attachments[1].uses.Value++;
                                    }
                                    Game1.playSound("button1");
                                    endMinigameStage = 9;
                                    ClearAnimations(who);
                                    SendMessage(who, "ClearAnim");
                                    DrawAndEmote(who, 0);
                                    SendMessage(who, "Swing");
                                    endMinigameTimer = 0;
                                    continue;
                                }

                                PlayPause(who);
                                SendMessage(who, "Fail");

                                endMinigameTimer = 0;
                                endMinigameStage = 8;
                                who.completelyStopAnimatingOrDoingAction();
                                who.UsingTool = true;
                                List<FarmerSprite.AnimationFrame> animationFrames = new List<FarmerSprite.AnimationFrame>(){
                                        new FarmerSprite.AnimationFrame(94, 500, false, false, null, false) { flip =  oldFacingDirection == 3 }.AddFrameAction(delegate (Farmer f) { f.jitterStrength = 2f; }) };
                                who.FarmerSprite.setCurrentAnimation(animationFrames.ToArray());
                                who.FarmerSprite.PauseForSingleAnimation = true;
                                who.FarmerSprite.loop = true;
                                who.FarmerSprite.loopThisAnimation = true;
                                who.Sprite.currentFrame = 94;
                                stage = "Caught1";
                                stageTimer = 20;
                            }
                        }
                    }
                }
            }

            if (fishCaught)
            {
                if (fishingFestivalMinigame == 0)
                {
                    infoTimer = 1000;
                    CaughtBubbleSprite(who);
                    SendMessage(who, "CaughtBubble");
                }
                else infoTimer = 1;
                stageTimer = -1;
                stage = null;
                fishCaught = false;
            }

            if (infoTimer > 0 && Game1.activeClickableMenu != null)//bubble logic
            {
                who.FacingDirection = 2;
                who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(84, 150) });
                infoTimer = 1020;
            }
            else if (infoTimer > 1) infoTimer--;
            else if (infoTimer == 1)
            {
                hereFishying = false;
                ClearAnimations(who);
                SendMessage(who, "ClearAnim");
                infoTimer--;
                who.faceDirection(oldFacingDirection);
            }

            if (start != null) start.MainUpdateTicked(e);


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
                }
            }
        }

        public void Display_RenderingWorld(object sender, RenderingWorldEventArgs e)//bait/tackle only
        {
            foreach (Farmer other in Game1.getAllFarmers())//draw tool for tool anims & bubble for other players
            {
                if (messages.ContainsKey(other.UniqueMultiplayerID) && who.currentLocation == other.currentLocation && messages[other.UniqueMultiplayerID].drawAttachments) DrawAndEmote(other, 4);//draw bait and tackle
            }
            if (drawAttachments) DrawAndEmote(who, 4);//draw bait and tackle
        }
        public void Display_Rendered(SpriteBatch e)
        {
            if (start != null) start.MainRendered(batch);
            else if (fishingFestivalMinigame == 1) Display_RenderedAll(e);
        }
        public void Display_RenderedAll(SpriteBatch e)
        {
            if (!Context.IsWorldReady || (Game1.activeClickableMenu != null && !(Game1.activeClickableMenu is DummyMenu))) return;
            if (batch == null) batch = e;
            who = Game1.player;
            foreach (Farmer other in Game1.getAllFarmers())//draw bubble for other players
            {
                if (messages.ContainsKey(other.UniqueMultiplayerID) && who.currentLocation == other.currentLocation)
                {
                    if (messages[other.UniqueMultiplayerID].stage != null && messages[other.UniqueMultiplayerID].stage.Equals("CaughtBubble", StringComparison.Ordinal)) CaughtBubbleDraw(other);//bubble
                }
            }

            if (start == null)
            {
                //draw mouse target on water
                if ((!Game1.eventUp || (fishingFestivalMinigame != 0 && festivalMode[screen] != 0)) && Game1.activeClickableMenu == null && who.CurrentItem is FishingRod && (!hereFishying || infoTimer > 0)) AimAssist(batch);

                if (showPerfect)//add perfect popup
                {
                    perfect = true;
                    sparklingText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\UI:BobberBar_Perfect"), Color.Yellow, Color.White, false, 0.1, 1500, -1, 500, 1f);
                    Game1.playSound("jingle1");
                    showPerfect = false;
                }
                if (sparklingText != null && who != null && !itemIsInstantCatch)//show perfect/new record popup
                {
                    sparklingText.draw(batch, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(sparklingText.drawnTextWidth / -2f + 32f, treasureCaught ? -300f : -200f)));
                }

                if (endMinigameStyle[screen] == 3 && endMinigameTimer > 0 && endMinigameTimer < 100)//draw letter for end minigame
                {
                    float y_offset = (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 200), 2);
                    Vector2 position = new Vector2(who.getStandingPosition().X - Game1.viewport.X, who.getStandingPosition().Y - 156 - Game1.viewport.Y) + new Vector2(y_offset);
                    batch.Draw(Game1.mouseCursors, position + new Vector2(-24, 0), new Rectangle(473, 36, 24, 24), minigameColor, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.98f);//text bg box
                    batch.DrawString(Game1.smallFont, endMinigameKey, position - (Game1.smallFont.MeasureString(endMinigameKey) / 2 * 1.2f) + new Vector2(0f, 28f), minigameColor, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 1f); //text
                }

                if (infoTimer > 0) CaughtBubbleDraw(who);//bubble
            }
        }


        private void TryFishingHere(Vector2 aimTile)
        {
            if (Context.IsWorldReady && Context.CanPlayerMove && who.CurrentItem is FishingRod)
            {
                if (!hereFishying)
                {
                    try
                    {
                        if (who.currentLocation.isTileFishable((int)aimTile.X, (int)aimTile.Y) || who.currentLocation.getTileIndexAt((int)aimTile.X, (int)aimTile.Y, "Buildings") == 1208 || who.currentLocation.getTileIndexAt((int)aimTile.X, (int)aimTile.Y, "Buildings") == 1260)
                        {
                            perfect = false;
                            debug = false;
                            Game1.displayHUD = false;

                            oldFacingDirection = who.getGeneralDirectionTowards(aimTile * 64);
                            who.faceDirection(oldFacingDirection);

                            x = (int)aimTile.X * 64;
                            y = (int)aimTile.Y * 64;
                            Game1.stats.TimesFished++;

                            HereFishyFishy(who);
                        }
                    }
                    catch (Exception ex)
                    {
                        Monitor.Log("Canceled fishing because: " + ex.Message + " in: " + ModEntry.exception.Match(ex.StackTrace).Value, LogLevel.Error);
                        EmergencyCancel();
                    }
                }
            }
        }

        private void HereFishyFishy(Farmer who)
        {
            voiceVolumePersonal = 0;
            if (voiceVolume > 0f)
            {
                voiceVolumePersonal = Math.Min((voiceVolume * 0.80f) + (who.FishingLevel * voiceVolume * 0.018f), (voiceVolume * 0.98f));
                if (who.FishingLevel > 10) voiceVolumePersonal += Math.Min((who.FishingLevel - 10) * voiceVolume * 0.004f, (voiceVolume * 0.02f));
            }
            effects = new Dictionary<string, float>() { { "AREA", 1f }, { "DAMAGE", 1f }, { "DIFFICULTY", 1f }, { "EXTRA_MAX", 0f }, { "EXTRA_CHANCE", 0f }, { "LIFE", 0f },
                                                        { "QUALITY", 0f }, { "SIZE", 1f }, { "SPEED", 1f }, { "TREASURE", 0f }, { "UNBREAKING0", 0f }, { "UNBREAKING1", 0f } };
            if (fishingFestivalMinigame == 0)
            {
                FishingRod rod = (who.CurrentItem as FishingRod) ?? rodDummy;//debug
                AddEffects(rod.Name, -1);
                if (rod.attachments[0] != null) AddEffects(rod.attachments[0].Name, 0);
                if (rod.attachments[1] != null) AddEffects(rod.attachments[1].Name, 1);
            }
            treasureCaught = false;
            hereFishying = true;
            endMinigameStage = 0;

            if (!debug && who.IsLocalPlayer && fishingFestivalMinigame == 0)
            {
                float oldStamina = who.Stamina;
                who.Stamina -= 8f - (float)who.FishingLevel * 0.1f;
                who.checkForExhaustion(oldStamina);
            }

            CatchFish(who, x, y);

            if (debug || ((!itemIsInstantCatch && fishingFestivalMinigame == 0 && startMinigameStyle[screen] > 0) || (fishingFestivalMinigame != 0 && festivalMode[screen] == 3)))//start minigame
            {
                who.FacingDirection = 2;
                who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(84, 9999999) });
                start = new MinigamesStart(this, Monitor);
                start.EmergencyCancel = EmergencyCancel;
                start.minigameDiff = (int)(modifier * minigameDifficulty[screen] * effects["DIFFICULTY"]);
                start.StartMinigameSetup();
            }
            else HereFishyAnimation(who, x, y);
        }

        private void AddEffects(string itemName, int category)
        {
            if (itemData.ContainsKey(itemName))
            {
                foreach (var effect in itemData[itemName])
                {
                    if (category != -1 && effect.Key.Equals("UNBREAKING", StringComparison.Ordinal)) effects[effect.Key + category] += effect.Value;
                    else
                    {
                        switch (effect.Key)
                        {
                            case "AREA":
                            case "SIZE":
                                effects[effect.Key] *= (effect.Value / 100f) + 1f;
                                break;
                            case "DAMAGE":
                            case "DIFFICULTY":
                            case "SPEED":
                                effects[effect.Key] *= 1f - (effect.Value / 100f);
                                break;
                            case "EXTRA_CHANCE":
                            case "TREASURE":
                                effects[effect.Key] += effect.Value / 100f;
                                break;
                            case "EXTRA_MAX":
                            case "LIFE":
                            case "QUALITY":
                                effects[effect.Key] += effect.Value;
                                break;
                            default:
                                Monitor.LogOnce(itemName + "'s effect skipped, error in its item_data.json entry", LogLevel.Error);
                                break;
                        }
                    }
                }
            }
            else
            {
                if ((who.CurrentTool as FishingRod).Name.Equals(itemName, StringComparison.Ordinal))
                {
                    effects["SPEED"] *= 0.8f;
                    Monitor.LogOnce(itemName + " not found in item_data.json. Modded Rod? Defaulting to SPEED:0.8 multiplier.", LogLevel.Debug);
                }
                else
                {
                    effects["SIZE"] *= 1.1f;
                    Monitor.LogOnce(itemName + " not found in item_data.json. Modded Item? Defaulting to SIZE:1.1 multiplier.", LogLevel.Debug);
                }
            }
        }
        private void CatchFish(Farmer who, int x, int y)
        {
            if (!debug)
            {
                FishingRod rod = who.CurrentTool as FishingRod;
                Vector2 bobberTile = new Vector2(x / 64, y / 64);
                fromFishPond = who.currentLocation.isTileBuildingFishable((int)bobberTile.X, (int)bobberTile.Y);

                clearWaterDistance = FishingRod.distanceToLand((int)bobberTile.X, (int)bobberTile.Y, who.currentLocation);
                Object bait = rod.GetBait();
                double baitPotency = ((bait != null) ? ((float)bait.Price / 10f) : 0f);

                Rectangle fishSplashRect = new Rectangle(who.currentLocation.fishSplashPoint.X * 64, who.currentLocation.fishSplashPoint.Y * 64, 64, 64);
                Rectangle bobberRect = new Rectangle(x - 80, y - 80, 64, 64);
                bool splashPoint = fishSplashRect.Intersects(bobberRect);

                item = who.currentLocation.getFish(0, rod.GetBait()?.QualifiedItemId, clearWaterDistance + (splashPoint ? 1 : 0), who, baitPotency + (splashPoint ? 0.4 : 0.0), bobberTile); //all item data starts here, FishingRod.cs

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

                if (whichFish == "(O)79" || whichFish == "(O)842")//notes
                {
                    item = who.currentLocation.tryToCreateUnseenSecretNote(who);
                }

                if (item != null) whichFish = item.QualifiedItemId;//fix here for fishpond

                if (item == null || whichFish == null || ItemRegistry.GetDataOrErrorItem(item.QualifiedItemId).IsErrorItem)
                {
                    item = ItemRegistry.Create("(O)" + Game1.random.Next(167, 173));//trash
                    whichFish = item.QualifiedItemId;
                    fromFishPond = false;
                }

                fishSize = 0f;
                fishQuality = 0;
                difficulty = 0;
                minFishSize = 0;
                maxFishSize = 0;

                Dictionary<string, string> data = DataLoader.Fish(Game1.content);
                string[] fishData = null;
                if (data.TryGetValue(item.ItemId, out string val))
                {
                    fishData = val.Split('/');
                }

                itemIsInstantCatch = false;
                if (item.HasTypeObject() && fishData != null)
                {
                    if (int.TryParse(fishData[1], out difficulty) && int.TryParse(fishData[3], out minFishSize) && int.TryParse(fishData[4], out maxFishSize))
                    {
                        behavior = fishData[2];
                        itemIsInstantCatch = false;
                    }
                    else itemIsInstantCatch = true;
                }
                else itemIsInstantCatch = true;

                if (!itemIsInstantCatch)
                {
                    switch (item.QualifiedItemId)
                    {
                        case "(O)152":
                        case "(O)153":
                        case "(O)157":
                        case "(O)797":
                        case "(O)79":
                        case "(O)73":
                        case "(O)842":
                        case "(O)890":
                        case "(O)820":
                        case "(O)821":
                        case "(O)822":
                        case "(O)823":
                        case "(O)824":
                        case "(O)825":
                        case "(O)826":
                        case "(O)827":
                        case "(O)828":
                            itemIsInstantCatch = true;
                            break;
                        default:
                            itemIsInstantCatch = item.Category == -20 || item.QualifiedItemId == GameLocation.CAROLINES_NECKLACE_ITEM_QID;
                            break;
                    }
                }
                rod.lastCatchWasJunk = itemIsInstantCatch;

                //special item handling
                if (fishingFestivalMinigame == 0 && !(item is Furniture) && !fromFishPond && who.team.specialOrders != null)
                {
                    foreach (SpecialOrder order in who.team.specialOrders)
                    {
                        order.onFishCaught?.Invoke(who, item);
                    }
                }
                if (whichFish == GameLocation.CAROLINES_NECKLACE_ITEM_QID) item.specialItem = true;



                //sizes
                if (maxFishSize > 0)
                {
                    fishSize = Math.Min(clearWaterDistance / 5f, 1f);
                    int minimumSizeContribution = 1 + who.FishingLevel / 2;
                    fishSize *= Game1.random.Next(minimumSizeContribution, Math.Max(6, minimumSizeContribution)) / 5f;

                    fishSize *= 1f + Game1.random.Next(-10, 11) / 100f;
                    fishSize = Math.Max(0f, Math.Min(1f, fishSize));


                    fishSize = (int)(minFishSize + (maxFishSize - minFishSize) * fishSize);
                    fishSize++;
                    fishSize *= effects["SIZE"];
                }

                if (rod.Name.Equals("Training Rod", StringComparison.Ordinal)) fishSize = minFishSize;

                //extra fish - max 1 if inv has no space, because treasure can do max 2
                caughtExtraFish = 0;
                if (!itemIsInstantCatch && fishingFestivalMinigame == 0 && effects["EXTRA_MAX"] != 0f)
                {
                    bool space = who.couldInventoryAcceptThisItem(item);
                    float chance = Math.Max((float)(who.DailyLuck / 2.0f) + effects["EXTRA_CHANCE"], 0f);
                    for (int i = 0; i < effects["EXTRA_MAX"]; i++)
                    {
                        if ((space || caughtExtraFish == 0) && Game1.random.NextDouble() < chance) caughtExtraFish++;
                    }
                    if (space && bossFish) caughtExtraFish = (int)(caughtExtraFish / 2f);
                }
                if (caughtExtraFish > 0) item.Stack += caughtExtraFish;

                bossFish = (who.CurrentItem as FishingRod).bossFish;

                //bossFish = true;//boss test
                //whichFish = 163;
                //difficulty = 110;
                //fishSize = 51;
            }

            modifier = (who.FishingLevel / 5f) - ((difficulty / 12f + 3) - (fishSize / 40f)) + who.LuckLevel + (Game1.random.Next(0, 50) / 100f);
            endMinigameDiff = (int)((60 + (endMinigameStyle[screen] == 3 ? 35f : endMinigameStyle[screen] == 2 ? 20f : 0f) + modifier) / minigameDifficulty[screen] / effects["DIFFICULTY"]);
        }
        private void CatchFishAfterMinigame(Farmer who)
        {
            //data calculations: quality, exp, treasure
            FishingRod rod = who.CurrentTool as FishingRod;

            float reduction = 0f;

            if (!itemIsInstantCatch)
            {
                if (rod.Name.Equals("Training Rod", StringComparison.Ordinal)) fishQuality = 0;
                else
                {
                    fishQuality = (int)effects["QUALITY"] + ((fishSize * (0.9f + (who.FishingLevel / 5.0)) < 0.33f) ? 0 : ((fishSize * (0.9f + (who.FishingLevel / 5.0f)) < 0.66f) ? 1 : 2));//init quality

                    if (start != null && endMinigameStyle[screen] > 0) //minigame score reductions
                    {
                        if (start.minigameStage == 10) reduction -= 0.4f;
                        else if (start.minigameStage == 9) reduction += 0.33f;
                        else if (start.minigameStage == 8) reduction += 0.6f;
                        else if (start.minigameStage == 7) reduction += 0.8f;
                        if (endMinigameStage == 10) reduction -= 0.4f;
                        else if (endMinigameStage == 9) reduction += 0.6f;
                        else if (endMinigameStage == 8) reduction += 0.8f;
                    }
                    else if (startMinigameStyle[screen] > 0)
                    {
                        if (start.minigameStage == 10) reduction -= 1f;
                        else if (start.minigameStage == 8) reduction += 1f;
                        else if (start.minigameStage == 7) reduction += 2f;
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
                    fishSize = Math.Max(1f, (int)Math.Round((fishSize - reduction * 2)));
                    fishQuality -= (int)Math.Round(reduction);
                }

                if (fishQuality < 0) fishQuality = 0;
                if (fishQuality > 2) fishQuality = 4;


                if (who.IsLocalPlayer && fishingFestivalMinigame == 0)
                {
                    int experience = Math.Max(1, (fishQuality + 1) * 3 + difficulty / 3);
                    if (bossFish) experience *= 5;

                    if (startMinigameStyle[screen] + endMinigameStyle[screen] > 0) experience += (int)(experience - reduction - 0.5f);
                    else if (perfect) experience += (int)((float)experience * 1.4f);

                    who.gainExperience(1, experience);
                    if (minigameDamage[screen] > 0 && endMinigameStyle[screen] > 0 && endMinigameStage == 8)
                    {
                        who.takeDamage((int)((10 + (difficulty / 10) + (int)(fishSize / 10) - who.FishingLevel) * minigameDamage[screen] * effects["DAMAGE"]), true, null);
                        who.temporarilyInvincible = false;
                    }
                }


                if (startMinigameStyle[screen] == 0) treasureCaught = fishingFestivalMinigame == 0 && who.fishCaught != null && who.fishCaught.Count() > 1 && Game1.random.NextDouble() < who.LuckLevel * 0.005 + effects["TREASURE"] + who.DailyLuck / 2.0 + ((who.professions.Contains(9) ? FishingRod.baseChanceForTreasure : 0) - reduction - 0.5f);
                if (endMinigameStage == 8 && endCanLoseTreasure[screen]) treasureCaught = false;
                item.Quality = fishQuality;
            }
            else if (who.IsLocalPlayer && fishingFestivalMinigame == 0)
            {
                who.gainExperience(1, 3);
                if (!fromFishPond && minigameDamage[screen] > 0 && endMinigameStyle[screen] > 0 && endMinigameStage == 8)
                {
                    who.takeDamage((int)((16 - who.FishingLevel) * minigameDamage[screen] * effects["DAMAGE"]), true, null);
                    who.temporarilyInvincible = false;
                }
            }
            fishSize = (int)Math.Round(fishSize);
        }

        private void HereFishyAnimation(Farmer who, int x, int y)
        {
            float layer = (y + 8) / 10000f;
            //player jumping and calling fish
            switch (stage)
            {
                case null:
                    if (!fromFishPond && fishingFestivalMinigame == 0)
                    {
                        Helper.Multiplayer.SendMessage(JUNK.Contains(whichFish) ? "(O)168" : whichFish, "whichFish", modIDs: new[] { "barteke22.FishingInfoOverlays" }, new[] { who.UniqueMultiplayerID });//notify overlay of which fish
                    }
                    if (!Context.IsSplitScreen && voices.TryGetValue(voiceType[screen], out SoundEffect sfx)) sfx.Play(voiceVolumePersonal, voicePitch[screen], 0);

                    if ((who.CurrentTool as FishingRod).GetBait() != null || (who.CurrentTool as FishingRod).GetTackleQualifiedItemIDs().Count > 0) drawAttachments = true;
                    SendMessage(who);

                    who.completelyStopAnimatingOrDoingAction();
                    who.jitterStrength = 2f;
                    List<FarmerSprite.AnimationFrame> animationFrames = new List<FarmerSprite.AnimationFrame>(){
                        new FarmerSprite.AnimationFrame(94, 100, false, false, null, false) { flip =  oldFacingDirection == 3 }.AddFrameAction(delegate (Farmer f) { f.jitterStrength = 2f; }) };
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
                    drawAttachments = false;
                    ClearAnimations(who);
                    SendMessage(who, "ClearAnim");
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
                    who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.emoteSpriteSheet.ToString(), new Rectangle(12 * 16 % Game1.emoteSpriteSheet.Width, 12 * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), 200, 1, 0, new Vector2(x, y - 32), false, false, layer, 0f, Color.White, 4f, 0f, 0f, 0f, false)
                    {
                        owner = who,
                        id = nexusKey
                    });
                    stage = "Starting5";
                    stageTimer = 12;
                    break;
                case "Starting5":
                    SendMessage(who);
                    who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.emoteSpriteSheet.ToString(), new Rectangle(13 * 16 % Game1.emoteSpriteSheet.Width, 12 * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), 200, 1, 0, new Vector2(x, y - 32), false, false, layer, 0f, Color.White, 4f, 0f, 0f, 0f, false)
                    {
                        owner = who,
                        id = nexusKey
                    });
                    stage = "Starting6";
                    stageTimer = 12;
                    break;
                case "Starting6":
                    SendMessage(who);
                    who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.emoteSpriteSheet.ToString(), new Rectangle(14 * 16 % Game1.emoteSpriteSheet.Width, 12 * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), 200, 1, 0, new Vector2(x, y - 32), false, false, layer, 0f, Color.White, 4f, 0f, 0f, 0f, false)
                    {
                        owner = who,
                        id = nexusKey
                    });
                    stage = "Starting7";
                    stageTimer = 12;
                    break;
                case "Starting7":
                    SendMessage(who);
                    who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.emoteSpriteSheet.ToString(), new Rectangle(15 * 16 % Game1.emoteSpriteSheet.Width, 12 * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), 200, 1, 0, new Vector2(x, y - 32), false, false, layer, 0f, Color.White, 4f, 0f, 0f, 0f, false)
                    {
                        owner = who,
                        id = nexusKey
                    });
                    stage = "Starting8";
                    stageTimer = 12;
                    break;


                //fish flying from water to player
                case "Starting8":
                    //realistic: divide fish size by 64 inches (around average human size), sprites are diagonal * 6f (how much you need to multiply item sprite to be player height (6 * 10.3 (diagonal of 16x16) = +-128 = 2 tiles))
                    if (realisticSizes)
                    {
                        itemSpriteSize = 2.5f;
                        if (fishSize > 0)
                        {
                            itemSpriteSize = Math.Max(((fishSize < 3) ? fishSize * 1.2f : fishSize) / 64f, 0.01f) * 6.6f;
                            if (item.Name.Contains("Eel")) itemSpriteSize /= 2f;//eel sprite curls, so half size
                        }
                    }
                    else itemSpriteSize = 4f;
                    if (item is Furniture) itemSpriteSize = 2.2f;
                    var miniData = ItemRegistry.GetDataOrErrorItem(item.QualifiedItemId);
                    sourceRect = miniData.GetSourceRect();
                    SendMessage(who);

                    float t;
                    float distance = y - (float)(who.getStandingPosition().Y - 100);

                    float height = Math.Abs(distance + 170f);
                    if (oldFacingDirection == 0) height -= 130f;
                    else if (oldFacingDirection == 2) height -= 170f;
                    height = Math.Max(height, 0f);

                    float gravity = 0.002f;
                    float velocity = (float)Math.Sqrt((double)(2f * gravity * height));
                    t = (float)(Math.Sqrt((double)(2f * (height - distance) / gravity)) + (double)(velocity / gravity));
                    float xVelocity = 0f;
                    if (t != 0f)
                    {
                        xVelocity = (who.Position.X - x) / t;
                    }
                    who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite((item is Furniture) ? Furniture.furnitureTextureName : "Maps\\springobjects", sourceRect, t, 1, 0, new Vector2(x, y), false, false, layer, 0f, Color.White, itemSpriteSize, 0f, 0f, 0f, false)
                    {
                        motion = new Vector2(xVelocity, -velocity),
                        acceleration = new Vector2(0f, gravity),
                        extraInfoForEndBehavior = 1,
                        endFunction = (endMinigameStyle[screen] == 0 || fromFishPond) ? new TemporaryAnimatedSprite.endBehavior(PlayerCaughtFishEndFunction) : null,
                        timeBasedMotion = true,
                        endSound = "tinyWhip",
                        owner = who,
                        id = nexusKey
                    });
                    int delay = 25;
                    for (int i = 1; i < item.Stack; i++)
                    {
                        who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", sourceRect, t, 1, 0, new Vector2(x, y), false, false, layer, 0f, Color.White, itemSpriteSize, 0f, 0f, 0f, false)
                        {
                            delayBeforeAnimationStart = delay,
                            motion = new Vector2(xVelocity, -velocity),
                            acceleration = new Vector2(0f, gravity),
                            timeBasedMotion = true,
                            endSound = "tinyWhip",
                            id = nexusKey,
                            owner = who,
                            Parent = who.currentLocation
                        });
                        delay += 25;
                    }
                    break;
            }
        }

        private void EndMinigame(int stage)
        {
            if (ModEntry.config.EndMinigameStyle[screen] == 3)
            {
                if (Game1.options.gamepadControls || Game1.options.gamepadMode == Options.GamepadModes.ForceOn) endMinigameStyle[screen] = 2;
                else if (System.Text.Encoding.ASCII.GetString(System.Text.Encoding.ASCII.GetBytes(item.DisplayName)).Replace(" ", "").Replace("?", "").Length < 1) endMinigameStyle[screen] = 2;
                else endMinigameStyle[screen] = 3;
            }

            if (stage == 0) //pick button + show alert/button sprite
            {
                endMinigameStage = 3;
                endMinigameTimer = 0;

                Rectangle rect = new Rectangle(395, 497, 3, 8);
                Vector2 offset = new Vector2(-7.5f, 0);

                who.PlayFishBiteChime();
                Rumble.rumble(0.75f, 250f);
                switch (endMinigameStyle[screen])
                {
                    case 1:
                        break;
                    case 2:
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
                Vector2 position = new Vector2(who.getStandingPosition().X, who.getStandingPosition().Y - 180) + offset;
                if (fishingFestivalMinigame != 1)
                {
                    position.X -= Game1.viewport.X;
                    position.Y -= Game1.viewport.Y;
                }
                Game1.screenOverlayTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", rect, position, flipped: false, 0.02f, minigameColor)
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
                            if (endMinigameKey != null && KeybindList.Parse(endMinigameKey).JustPressed()) passed = true;
                            break;
                    }
                }
                if (passed)
                {
                    if (endMinigameStage == 3 && endMinigameTimer < endMinigameDiff * effects["AREA"] * ((endMinigameStyle[screen] == 3) ? 0.6f : (endMinigameStyle[screen] == 2) ? 0.5f : 0.2f)) endMinigameStage = 10;
                    else endMinigameStage = 9;

                    ClearAnimations(who);
                    SendMessage(who, "ClearAnim");
                    DrawAndEmote(who, 0);
                    SendMessage(who, "Swing");
                    endMinigameTimer = 0;
                }
                else //button press, too early or wrong
                {
                    endMinigameStage = 3;
                    endMinigameTimer = 1000;
                }
            }
        }

        private void PlayerCaughtFishEndFunction(int forceStage = 0)
        {
            try
            {
                if (forceStage == 1) stage = "Caught1";

                switch (stage)
                {
                    case "Caught1":
                        if (Game1.activeClickableMenu is DummyMenu) Game1.exitActiveMenu();
                        if (fishingFestivalMinigame == 0)
                        {
                            rodDummy = who.CurrentTool.getOne() as FishingRod;
                            if (effects["UNBREAKING0"] < Game1.random.Next(1, 101)) rodDummy.attachments[0] = (who.CurrentTool as FishingRod).attachments[0];
                            else effects["UNBREAKING0"] = 9999f;
                            if (effects["UNBREAKING1"] < Game1.random.Next(1, 101)) rodDummy.attachments[1] = (who.CurrentTool as FishingRod).attachments[1];
                            else effects["UNBREAKING1"] = 9999f;
                            rodDummy.lastUser = who;
                            rodDummy.whichFish = ItemRegistry.GetMetadata(whichFish);
                            rodDummy.numberOfFishCaught = caughtExtraFish;
                            rodDummy.fishQuality = fishQuality;
                            rodDummy.clearWaterDistance = clearWaterDistance;
                            who.CurrentTool.lastUser = who;
                        }


                        who.UsingTool = false;
                        SendMessage(who);

                        CatchFishAfterMinigame(who);

                        if (!fromFishPond)
                        {
                            if (endMinigameStage == 8)//water on face
                            {
                                who.currentLocation.playSound("fishSlap", who.Tile);
                                who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(10, who.Position - new Vector2(0, 120), Color.Blue * 0.9f)
                                {
                                    layerDepth = (who.Position.Y + 17.5f) / 10000f,
                                    motion = new Vector2(0f, 0.12f),
                                    timeBasedMotion = true,
                                    owner = who,
                                    id = nexusKey
                                });
                                SendMessage(who, "Water");
                            }
                            if (fishingFestivalMinigame == 0)//not festivals
                            {
                                recordSize = who.caughtFish(whichFish, (metricSizes) ? (int)(fishSize * 2.54f) : (int)fishSize, false, caughtExtraFish + 1);

                                if (start != null && !MinigamesStart.tutorialSkip[screen] && !itemIsInstantCatch)
                                {
                                    MinigamesStart.tutorialSkip[screen] = true;
                                    ModConfig config = Helper.ReadConfig<ModConfig>();
                                    config.TutorialSkip[screen] = true;
                                    Helper.WriteConfig(config);
                                }

                                if (bossFish)
                                {
                                    Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14068"));

                                    if (Game1.IsMultiplayer || Game1.multiplayerMode != 0)
                                    {
                                        if (Game1.IsClient) Game1.client.sendMessage(15, "CaughtLegendaryFish", new string[] { who.Name, item.DisplayName });
                                        else if (Game1.IsServer)
                                        {
                                            foreach (long id in Game1.otherFarmers.Keys)
                                            {
                                                Game1.server.sendMessage(id, 15, who, "CaughtLegendaryFish", new string[] { who.Name, item.DisplayName });
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
                            DrawAndEmote(who, 2);
                        }

                        start = null;
                        who.Halt();
                        Game1.displayHUD = true;

                        stage = "Caught2";
                        if (fishingFestivalMinigame != 0)//festivals
                        {
                            if (!itemIsInstantCatch)
                            {
                                if (fishingFestivalMinigame == 1 && (endMinigameStage == 10 || festivalMode[screen] == 1 || endMinigameStyle[screen] == 0))//fall
                                {
                                    Event ev = Game1.CurrentEvent;
                                    ev.caughtFish("(O)137", Game1.random.Next(0, 20), who);
                                    if (Game1.random.Next(0, 5) == 0) ev.perfectFishing();
                                }
                                else if (fishingFestivalMinigame == 2 && (endMinigameStage == 10 || festivalMode[screen] == 1 || endMinigameStyle[screen] == 0)) Game1.CurrentEvent.caughtFish(whichFish, (int)fishSize, who);//winter
                            }
                            stageTimer = 1;
                        }
                        else//adding items + chest
                        {
                            if (!treasureCaught || itemIsInstantCatch)
                            {
                                if (!itemIsInstantCatch) rodDummy.doneFishing(who, true);

                                //maybe extra checks will help split screen issue where menu sometimes pops up even though there's space in inventory
                                if (!who.couldInventoryAcceptThisItem(item) || !who.addItemToInventoryBool(item)) who.addItemByMenuIfNecessary(item);
                                stageTimer = 1;
                            }
                            else
                            {
                                who.currentLocation.localSound("openChest");

                                who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(64, 1920, 32, 32), 200f, 4, 0, who.Position + new Vector2(-32f, -228f), false, false, 0.5f, 0f, Color.White, 4f, 0f, 0f, 0f)
                                {
                                    motion = new Vector2(0f, -0.128f),
                                    timeBasedMotion = true,
                                    alpha = 0f,
                                    alphaFade = -0.002f,
                                    endFunction = rodDummy.openTreasureMenuEndFunction,
                                    extraInfoForEndBehavior = (!who.addItemToInventoryBool(item)) ? 1 : 0,
                                    owner = who,
                                    id = nexusKey
                                });
                                stageTimer = 60;
                            }
                        }
                        break;
                    case "Caught2":
                        fishCaught = true;
                        if (fishingFestivalMinigame == 0)
                        {
                            if (effects["UNBREAKING0"] != 9999f) (who.CurrentTool as FishingRod).attachments[0] = (rodDummy.attachments[0] == null || rodDummy.attachments[0].Stack == 0) ? null : rodDummy.attachments[0];
                            if (effects["UNBREAKING1"] != 9999f) (who.CurrentTool as FishingRod).attachments[1] = (rodDummy.attachments[1] == null || rodDummy.attachments[1].Stack == 0) ? null : rodDummy.attachments[1];
                            Helper.Multiplayer.SendMessage(-1, "whichFish", modIDs: new[] { "barteke22.FishingInfoOverlays" }, new[] { who.UniqueMultiplayerID });//clear overlay
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Monitor.Log("Handled Exception in PlayerCaughtFishEndFunction. Festival: " + Game1.isFestival() + ", Message: " + ex.Message + " in: " + ModEntry.exception.Match(ex.StackTrace).Value, LogLevel.Trace);
                EmergencyCancel();
            }
        }


        private void DrawAndEmote(Farmer who, int which)
        {

            if (which < 2)
            {
                if (!fromFishPond && endMinigameStyle[screen] > 0) //swing animation
                {
                    if (endMinigameStage == 10) showPerfect = true;
                    who.completelyStopAnimatingOrDoingAction();
                    who.currentLocation.playSound("cast", who.Tile);
                    (who.CurrentTool as FishingRod).setTimingCastAnimation(who);
                    who.UsingTool = true;
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
                    if (start != null)
                    {
                        if (start.minigameStage == 4) //for now 4 = cancel = X
                        {
                            who.doEmote(36);
                            who.netDoEmote("x");
                        }
                        else if (start.minigameStage == 5) //for now 5 = fail = Angry
                        {
                            who.doEmote(12);
                            who.netDoEmote("angry");
                        }
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
            else if (which == 4 && !fromFishPond && fishingFestivalMinigame == 0)//bait & megaphone
            {
                int facingDir = oldFacingDirection;
                if (who != this.who) facingDir = messages[who.UniqueMultiplayerID].oldFacingDirection;
                ClearAnimations(who);
                FishingRod rod = who.CurrentTool as FishingRod;
                Vector2 position = who.Position + new Vector2(0, who.yJumpOffset * 2f) + who.jitter;
                float layer = (who.Position.Y + (facingDir != 0 ? 17.5f : 16.2f)) / 10000f;

                if (rod.GetBait() is Object bait)
                {
                    var data = ItemRegistry.GetDataOrErrorItem(bait.QualifiedItemId);
                    who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(data.GetTextureName(),
                        data.GetSourceRect(),
                        position + new Vector2(facingDir == 3 ? 44f : -10f, -80f), false, 0f, Color.White)
                    {
                        layerDepth = layer,
                        rotation = facingDir == 3 ? 0.3f : -0.3f,
                        scale = 2f,
                        owner = who,
                        id = nexusKey
                    });
                }
                if (rod.GetTackle().FirstOrDefault() is Object tackle)
                {
                    var data = ItemRegistry.GetDataOrErrorItem(tackle.QualifiedItemId);
                    who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(data.GetTextureName(),
                        data.GetSourceRect(),
                        position + new Vector2(facingDir == 3 ? -20f : 36f, -90f), false, 0f, Color.White)
                    {
                        layerDepth = layer,
                        rotation = facingDir == 3 ? 0.6f : -0.6f,
                        scale = 3f,
                        flipped = facingDir == 3,
                        owner = who,
                        id = nexusKey
                    });
                }
            }
        }

        private void CaughtBubbleDraw(Farmer who)
        {
            int fishQuality = this.fishQuality;
            int maxFishSize = this.maxFishSize;
            float fishSize = this.fishSize;
            int fishCount = item.Stack;
            bool recordSize = this.recordSize;
            if (who == this.who)
            {
                if (!(who.CurrentItem is FishingRod))//cancel for scrolling
                {
                    infoTimer = 1;
                    return;
                }
            }
            else
            {
                fishQuality = messages[who.UniqueMultiplayerID].fishQuality;
                maxFishSize = messages[who.UniqueMultiplayerID].maxFishSize;
                fishSize = messages[who.UniqueMultiplayerID].fishSize;
                fishCount = messages[who.UniqueMultiplayerID].stack;
                recordSize = messages[who.UniqueMultiplayerID].recordSize;
            }


            ////arm up
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
                    batch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(-104f, -48f)), quality_rect, Color.White, 0f, new Vector2(4f), 2f, SpriteEffects.None, 0.1f);
                    batch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(-56f, -48f)), quality_rect, Color.White, 0f, new Vector2(4f), 2f, SpriteEffects.None, 0.1f);
                }
                //strings
                float offset = 0f;
                if (fishCount > 1 || fishQuality > 0) offset = 13f;
                if (fishCount > 1)
                {
                    batch.DrawString(Game1.smallFont, "x" + fishCount, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(-91f, -60f)), Game1.textColor, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.1f);
                }
                string sizeString = Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14083", (metricSizes || LocalizedContentManager.CurrentLanguageCode != 0) ? Math.Round(fishSize * 2.54f) : (fishSize));
                batch.DrawString(Game1.smallFont, sizeString, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(-80f - Game1.smallFont.MeasureString(sizeString).X / 2f, -77f - offset)), recordSize ? Color.Blue : Game1.textColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
            }
        }
        private void CaughtBubbleSprite(Farmer who)
        {
            string whichFish = this.whichFish;
            float itemSpriteSize = this.itemSpriteSize;
            int fishCount = item.Stack;
            bool furniture;
            Rectangle sourceRect = this.sourceRect;
            if (who == this.who) furniture = item is Furniture;
            else
            {
                whichFish = messages[who.UniqueMultiplayerID].whichFish;
                itemSpriteSize = messages[who.UniqueMultiplayerID].itemSpriteSize;
                fishCount = messages[who.UniqueMultiplayerID].stack;
                furniture = messages[who.UniqueMultiplayerID].furniture;
                sourceRect = messages[who.UniqueMultiplayerID].sourceRect;
            }
            float layer = (who.Position.Y + 17.5f) / 10000f;

            //fishing net
            if (!fromFishPond) who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.toolSpriteSheetName, Game1.getSourceRectForStandardTileSheet(Game1.toolSpriteSheet, who.CurrentTool.IndexOfMenuItemView, 16, 16), who.Position + new Vector2(-10f, -15f), false, 0f, Color.White)
            { layerDepth = layer, rotation = -3f, scale = 4f, owner = who, id = nexusKey });

            //item(s) in hand
            if (!furniture)
            {
                //whichFish = 152;//test-------------------------------------------------------
                //sourceRect = GameLocation.getSourceRectForObject(whichFish);
                //itemIsInstantCatch = true;

                string textureName = Game1.objectSpriteSheetName;
                float distanceFromMidToFaceCorner = -8f * itemSpriteSize;
                float rotOffset = 0f;
                switch (whichFish)//regular hardcoded sprites
                {
                    case "(O)128"://puff
                    case "(O)151"://squid
                    case "(O)798"://midnight squid
                    case "(O)800"://blob
                        rotOffset = 2.2f;
                        break;
                    case "(O)158"://stonefish
                        rotOffset = 1f;
                        break;
                    case "(O)160"://angler
                    case "(O)838"://discus
                    case "(O)899"://ms angler
                        rotOffset = 0.3f;
                        break;
                }

                Vector2 tankOffset = Vector2.Zero;
                if (fishTankSprites)
                {
                    Item fish = ItemRegistry.Create(whichFish);
                    FishTankFurniture tank = new FishTankFurniture("2322", Vector2.Zero);
                    if (fish.Category == Object.FishCategory && tank.CanBeDeposited(fish))//fishtank sprites
                    {
                        tank.boundingBox.Value = new Rectangle(0, 0, 300, 100);
                        textureName = tank.GetAquariumTexture().Name;
                        TankFish tankFish = new TankFish(tank, fish);
                        tank.tankFish.Add(tankFish);
                        int cols = tank.GetAquariumTexture().Width / 24;
                        int sprite_sheet_x = tank.tankFish[0].currentFrame % cols * 24;
                        int sprite_sheet_y = tank.tankFish[0].currentFrame / cols * 48;
                        rotOffset = 1f;
                        switch (tankFish.fishType)
                        {
                            case TankFish.FishType.Cephalopod:
                                sprite_sheet_x += 72;
                                rotOffset = 2.2f;
                                break;
                            case TankFish.FishType.Float:
                                rotOffset = 3f;
                                break;
                            case TankFish.FishType.Crawl:
                            case TankFish.FishType.Static:
                                rotOffset = 2.2f;
                                break;
                            case TankFish.FishType.Eel:
                                itemSpriteSize *= 2f;
                                rotOffset = 0.7f;
                                break;
                        }
                        switch (whichFish)
                        {
                            case "(O)158"://stonefish
                                rotOffset = 1.2f;
                                break;
                        }
                        if (fishCount == 1) rotOffset -= 0.2f;
                        sourceRect = new Rectangle(sprite_sheet_x, sprite_sheet_y, 24, 24);
                        tankOffset = new Vector2(5f, 5f) * itemSpriteSize;
                        tankOffset.X -= 5f;
                    }
                }

                float rot = itemIsInstantCatch ? -0.2f : fishCount == 1 ? 2.4f - rotOffset : 2.2f - rotOffset + (fishCount < 6 ? ((fishCount - 0.5f) * 0.2f) : ((fishCount - 0.5f) * 0.15f));//rotate by half the amount

                for (int i = 0; i < fishCount; i++)
                {
                    float offsetX = 15f;
                    float offsetY = -30f;
                    if (!itemIsInstantCatch)
                    {
                        offsetX = distanceFromMidToFaceCorner * (float)Math.Sin(rot + 1f + rotOffset) + (7f * itemSpriteSize);
                        offsetY = distanceFromMidToFaceCorner * (float)Math.Cos(rot + 1f + rotOffset) - (12f * itemSpriteSize);
                    }
                    else if (itemSpriteSize != 2.5f)
                    {
                        offsetX = 3f * itemSpriteSize + 6f;
                        offsetY = -5f * itemSpriteSize - 7f;
                    }

                    who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(textureName, sourceRect, who.Position + new Vector2(7f - offsetX, -48f + offsetY) - tankOffset, false, 0f, Color.White)
                    { layerDepth = layer + (i % 3 == 0 ? 0.000001f : 0f), rotation = rot, scale = itemSpriteSize, owner = who, id = nexusKey });
                    layer += 0.00000001f;
                    rot -= fishCount < 6 ? 0.4f : 0.3f;
                }
            }
            else who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Furniture.furnitureTextureName, sourceRect, who.Position + new Vector2(-10f, -80f), false, 0f, Color.White)
            { layerDepth = layer + 0.00000001f, scale = itemSpriteSize, owner = who, id = nexusKey });
        }

        private void SuppressAll(ButtonsChangedEventArgs e)
        {
            if (Game1.activeClickableMenu == null || Game1.activeClickableMenu is DummyMenu)
            {
                foreach (SButton button in e.Pressed)
                    Helper.Input.Suppress(button);
                foreach (SButton button in e.Held)
                    Helper.Input.Suppress(button);
            }
        }
        public void EmergencyCancel()
        {
            if (Game1.activeClickableMenu is DummyMenu) Game1.exitActiveMenu();
            endMinigameStage = 0;
            if (start != null)
            {
                //who.currentLocation.forceViewportPlayerFollow = start.oldViewportFollow;
                //Game1.viewportFreeze = false;
                //if (Context.IsSplitScreen) Game1.options.localCoopBaseZoomLevel = start.oldZoom;
                //else Game1.options.singlePlayerBaseZoomLevel = start.oldZoom;
                start = null;
            }
            who.UsingTool = false;
            who.Halt();
            who.completelyStopAnimatingOrDoingAction();
            ClearAnimations(who);
            SendMessage(who, "ClearAnim");
            SendMessage(who, "Clear");
            hereFishying = false;
            stage = null;
            Game1.displayHUD = true;
            who.canMove = true;
            Helper.Multiplayer.SendMessage(-1, "whichFish", modIDs: new[] { "barteke22.FishingInfoOverlays" }, new[] { who.UniqueMultiplayerID });//clear overlay
            Helper.Multiplayer.SendMessage(false, "hideText", modIDs: new[] { "barteke22.FishingInfoOverlays" }, new[] { who.UniqueMultiplayerID });//clear overlay
        }
        private void ClearAnimations(Farmer who)
        {
            for (int i = 0; i < who.currentLocation.TemporarySprites.Count; i++)
            {
                if (who.currentLocation.TemporarySprites[i].id == nexusKey && who.currentLocation.TemporarySprites[i].owner == who)
                {
                    who.currentLocation.TemporarySprites.RemoveAt(i);
                    i--;
                }
            }
        }
        private void PlayPause(Farmer who)
        {
            foreach (var anim in who.currentLocation.TemporarySprites)
            {
                if (anim.id == nexusKey && anim.owner == who)
                {
                    if (anim.paused) anim.paused = false;
                    else anim.paused = true;
                }
            }
        }

        private void AimAssist(SpriteBatch batch)
        {
            maxDistance = (who.FishingLevel > 14) ? 7 : (who.FishingLevel > 7) ? 6 : (who.FishingLevel > 3) ? 5 : (who.FishingLevel > 0) ? 4 : 3;

            if (!freeAim[screen])//grid aim
            {
                if (Game1.activeClickableMenu != null) return;
                if (keyBinds[screen].IsDown()
                            || Helper.Input.IsSuppressed(Game1.options.useToolButton[0].ToSButton())
                            || Helper.Input.IsSuppressed(Game1.options.useToolButton[1].ToSButton())
                            || Helper.Input.IsSuppressed(SButton.ControllerX))
                {
                    who.completelyStopAnimatingOrDoingAction();

                    Vector2 topLeft = who.Tile + new Vector2((who.FacingDirection == 3) ? -maxDistance - 1 : (who.FacingDirection == 1) ? 1 : -1,
                                                                          (who.FacingDirection == 0) ? -maxDistance : (who.FacingDirection == 2) ? 1 : -1);

                    Vector2 bottomRight = who.Tile + new Vector2((who.FacingDirection == 3) ? 0 : (who.FacingDirection == 1) ? maxDistance + 2 : 2,//left : right
                                                                              (who.FacingDirection == 0) ? 0 : (who.FacingDirection == 2) ? maxDistance + 1 : 2);//up : down

                    List<KeyValuePair<Vector2, bool>> tilesMid = new List<KeyValuePair<Vector2, bool>>();
                    List<KeyValuePair<Vector2, bool>> tilesAll = new List<KeyValuePair<Vector2, bool>>();

                    for (int x = (int)topLeft.X; x < bottomRight.X; x++)
                    {
                        for (int y = (int)topLeft.Y; y < bottomRight.Y; y++)
                        {
                            KeyValuePair<Vector2, bool> tile = new KeyValuePair<Vector2, bool>(new Vector2(x, y), (who.currentLocation.isTileFishable(x, y) || (who.currentLocation.getTileIndexAt(x, y, "Buildings") == 1208) || (who.currentLocation.getTileIndexAt(x, y, "Buildings") == 1260)));

                            if (who.FacingDirection == 3 || who.FacingDirection == 1)
                            {
                                if (y == topLeft.Y + 1f) tilesMid.Add(tile);
                            }
                            else if (x == topLeft.X + 1f) tilesMid.Add(tile);
                            tilesAll.Add(tile);
                        }
                    }
                    if (who.FacingDirection == 1 || who.FacingDirection == 2)
                    {
                        tilesMid.Reverse();
                        tilesAll.Reverse();
                    }

                    if (!tilesAll.Contains(new KeyValuePair<Vector2, bool>(aimTile, true)) && !tilesAll.Contains(new KeyValuePair<Vector2, bool>(aimTile, false)))
                    {
                        aimTile = new Vector2(-9999f);
                        foreach (var tile in tilesMid)
                        {
                            if (tile.Value)
                            {
                                aimTile = tile.Key;
                                break;
                            }
                        }
                        if (aimTile.X == -9999f)
                        {
                            foreach (var tile in tilesAll)
                            {
                                if (tile.Value)
                                {
                                    aimTile = tile.Key;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (KeybindList.Parse("A, Left, DPadLeft, LeftThumbstickLeft").JustPressed())
                        {
                            if (topLeft.X - 1 < aimTile.X - 1) aimTile.X -= 1;
                            else aimTile.X = bottomRight.X - 1;
                        }
                        else if (KeybindList.Parse("D, Right, DPadRight, LeftThumbstickRight").JustPressed())
                        {
                            if (bottomRight.X > aimTile.X + 1) aimTile.X += 1;
                            else aimTile.X = topLeft.X;
                        }
                        else if (KeybindList.Parse("W, Up, DPadUp, LeftThumbstickUp").JustPressed())
                        {
                            if (topLeft.Y - 1 < aimTile.Y - 1) aimTile.Y -= 1;
                            else aimTile.Y = bottomRight.Y - 1;
                        }
                        else if (KeybindList.Parse("S, Down, DPadDown, LeftThumbstickDown").JustPressed())
                        {
                            if (bottomRight.Y > aimTile.Y + 1) aimTile.Y += 1;
                            else aimTile.Y = topLeft.Y;
                        }
                    }

                    foreach (var pair in tilesAll)
                    {
                        batch.Draw(MinigamesStart.minigameTextures[2], new Vector2((int)pair.Key.X * 64 - Game1.viewport.X, (int)pair.Key.Y * 64 - Game1.viewport.Y),
                            new Rectangle((pair.Value) ? 0 : 64, 0, 64, 64), Color.White * 0.5f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    }
                    batch.Draw(Game1.mouseCursors, new Vector2((int)aimTile.X * 64 - Game1.viewport.X, (int)aimTile.Y * 64 - Game1.viewport.Y), new Rectangle(652, 204, 44, 44),
                        (tilesAll.Contains(new KeyValuePair<Vector2, bool>(aimTile, true))) ? new Color(0, 255, 0, 0.5f) : Color.Red * 0.4f, 0f, Vector2.Zero, 1.45f, SpriteEffects.None, 1f);

                    keybindHeld = true;
                }
                else if (keybindHeld)
                {
                    keybindHeld = false;
                    if (fishingFestivalMinigame == 0 || festivalTimer > 3000) TryFishingHere(aimTile);
                }
            }
            else//free aim
            {
                List<Vector2> tiles = new List<Vector2>();
                int endX = (int)who.Tile.X + maxDistance + 2;
                int endY = (int)who.Tile.Y + 2;
                for (int x = (int)who.Tile.X - maxDistance - 1; x < endX; x++)
                {
                    for (int y = (int)who.Tile.Y - 1; y < endY; y++)
                    {
                        if (who.currentLocation.isTileFishable(x, y) || (who.currentLocation.getTileIndexAt(x, y, "Buildings") == 1208) || (who.currentLocation.getTileIndexAt(x, y, "Buildings") == 1260))
                        {
                            batch.Draw(MinigamesStart.minigameTextures[2], new Vector2(x * 64 - Game1.viewport.X, y * 64 - Game1.viewport.Y), new Rectangle(0, 0, 64, 64), Color.White * 0.3f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                            tiles.Add(new Vector2(x, y));
                        }
                    }
                }
                endX = (int)who.Tile.X + 2;
                endY = (int)who.Tile.Y + maxDistance + 1;
                for (int x = (int)who.Tile.X - 1; x < endX; x++)
                {
                    for (int y = (int)who.Tile.Y - maxDistance; y < endY; y++)
                    {
                        if (!tiles.Contains(new Vector2(x, y)) && (who.currentLocation.isTileFishable(x, y) || (who.currentLocation.getTileIndexAt(x, y, "Buildings") == 1208) || (who.currentLocation.getTileIndexAt(x, y, "Buildings") == 1260)))
                        {
                            batch.Draw(MinigamesStart.minigameTextures[2], new Vector2(x * 64 - Game1.viewport.X, y * 64 - Game1.viewport.Y), new Rectangle(0, 0, 64, 64), Color.White * 0.3f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                            tiles.Add(new Vector2(x, y));
                        }
                    }
                }
                endX = (int)(who.Tile.X + maxDistance / 1.8f) + 3;
                endY = (int)(who.Tile.Y + maxDistance / 1.8f) + 2;
                for (int x = (int)(who.Tile.X - maxDistance / 1.8f) - 1; x < endX; x++)
                {
                    for (int y = (int)(who.Tile.Y - maxDistance / 1.8f); y < endY; y++)
                    {
                        if (!tiles.Contains(new Vector2(x, y)) && (who.currentLocation.isTileFishable(x, y) || (who.currentLocation.getTileIndexAt(x, y, "Buildings") == 1208) || (who.currentLocation.getTileIndexAt(x, y, "Buildings") == 1260)))
                        {
                            batch.Draw(MinigamesStart.minigameTextures[2], new Vector2(x * 64 - Game1.viewport.X, y * 64 - Game1.viewport.Y), new Rectangle(0, 0, 64, 64), Color.White * 0.3f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                            tiles.Add(new Vector2(x, y));
                        }
                    }
                }

                //for (float x = who.getTileLocation().X - maxDistance; x <= who.getTileLocation().X + maxDistance; x++)
                //{
                //    for (float y = who.getTileLocation().Y - maxDistance; y <= who.getTileLocation().Y + maxDistance; y++)
                //    {
                //        if (Vector2.DistanceSquared(who.getTileLocation(), new Vector2(x, y)) <= (maxDistance + 1) * (maxDistance + 1) && who.currentLocation.isTileFishable((int)x, (int)y))
                //        {
                //            batch.Draw(startMinigameTextures[2], new Vector2((int)x * 64 - Game1.viewport.X, (int)y * 64 - Game1.viewport.Y), new Rectangle(0, 0, 64, 64), Color.White * 0.3f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                //            tiles.Add(new Vector2(x, y));
                //        }
                //    }
                //}

                aimTile = new Vector2(-9999f);
                if (tiles.Contains(Game1.currentCursorTile)) aimTile = Game1.currentCursorTile;
                else
                {
                    foreach (var tile in tiles)
                    {
                        if (Vector2.DistanceSquared(tile, Game1.currentCursorTile) < Vector2.DistanceSquared(aimTile, Game1.currentCursorTile)) aimTile = tile;
                    }
                }

                if (aimTile.X != -9999f)
                {
                    batch.Draw(Game1.mouseCursors, new Vector2((int)aimTile.X * 64 - Game1.viewport.X, (int)aimTile.Y * 64 - Game1.viewport.Y), new Rectangle(652, 204, 44, 44), new Color(0, 255, 0, 0.5f), 0f, Vector2.Zero, 1.45f, SpriteEffects.None, 1f);
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
            Helper.Multiplayer.SendMessage(new MinigameMessage(who, stageRequested, voiceType[screen], voicePitch[screen], drawAttachments, whichFish, fishQuality, maxFishSize, fishSize, itemSpriteSize,
                stack, recordSize, furniture, sourceRect, x, y, oldFacingDirection), "Animation", modIDs: new[] { ModManifest.UniqueID }, IDs);
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
                    // || who.currentLocation != this.who.currentLocation

                    float voiceVolumePersonal = 0;
                    if (voiceVolume > 0f)
                    {
                        voiceVolumePersonal = Math.Min((voiceVolume * 0.80f) + (who.FishingLevel * voiceVolume * 0.018f), (voiceVolume * 0.98f));
                        if (who.FishingLevel > 10) voiceVolumePersonal += Math.Min((who.FishingLevel - 10) * voiceVolume * 0.004f, (voiceVolume * 0.02f));
                    }

                    int x = message.x;
                    int y = message.y;

                    float layer = (y + 8) / 10000f;
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
                            who.UsingTool = true;
                            switch (message.oldFacingDirection)
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
                            else if (voices.TryGetValue(message.voiceType, out SoundEffect sfx))//volume based on distance? will be iffy in split... play at the same time?
                            {
                                float volume = 0f;
                                volume = Math.Min(Math.Max((Vector2.Distance(who.Position, this.who.Position) - 2560f) / -2560f, 0f), 1f);//distance based volume within 40ish tiles
                                if (Context.IsSplitScreen)
                                {
                                    int screen = Context.ScreenId;
                                    if (Helper.Multiplayer.GetConnectedPlayer(message.multiplayerID).IsSplitScreen)
                                    {
                                        int screen2 = (int)Helper.Multiplayer.GetConnectedPlayer(message.multiplayerID).ScreenID;
                                        sfx.Play(voiceVolumePersonal, message.voicePitch, (screen2 == 0 || screen2 == 2) ? -1f : 1f); //if local split screen, screen 0 and 2 play on left, 1, 3 on right, * distance on other side
                                        if (((screen == 0 || screen == 2) && (screen2 == 1 || screen2 == 3)) ||
                                            ((screen == 1 || screen == 3) && (screen2 == 0 || screen2 == 2))) sfx.Play(volume * voiceVolumePersonal, message.voicePitch, (screen == 0 || screen == 2) ? -1f : 1f);
                                    }
                                    else sfx.Play(volume * voiceVolumePersonal, message.voicePitch, (screen == 0 || screen == 2) ? -1f : 1f); //if split screen, but sender is not split screen (same as above * distance)
                                }
                                else sfx.Play(volume * voiceVolumePersonal, message.voicePitch, 0); //not scplit screen, won't do directional sound as it can be annoying
                            }
                            who.completelyStopAnimatingOrDoingAction();
                            who.jitterStrength = 2f;
                            if (message.stage != null) who.UsingTool = true;
                            List<FarmerSprite.AnimationFrame> animationFrames = new List<FarmerSprite.AnimationFrame>(){
                                new FarmerSprite.AnimationFrame(94, 100, false, false, null, false) { flip =  message.oldFacingDirection == 3 }.AddFrameAction(delegate (Farmer f) { f.jitterStrength = 2f; }) };
                            who.FarmerSprite.setCurrentAnimation(animationFrames.ToArray());
                            who.FarmerSprite.PauseForSingleAnimation = true;
                            who.FarmerSprite.loop = true;
                            who.FarmerSprite.loopThisAnimation = true;
                            who.Sprite.currentFrame = 94;
                            break;
                        case "Water":
                            who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(10, who.Position - new Vector2(0, 120), Color.Blue * 0.9f)
                            {
                                layerDepth = (who.Position.Y + 17.5f) / 10000f,
                                motion = new Vector2(0f, 0.12f),
                                timeBasedMotion = true,
                                owner = who,
                                id = nexusKey
                            });
                            who.completelyStopAnimatingOrDoingAction();
                            break;
                        case "Starting4":
                            SendMessage(who);
                            who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.emoteSpriteSheet.ToString(), new Rectangle(12 * 16 % Game1.emoteSpriteSheet.Width, 12 * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), 200, 1, 0, new Vector2(x, y - 32), false, false, layer, 0f, Color.White, 4f, 0f, 0f, 0f, false)
                            {
                                owner = who,
                                id = nexusKey
                            });
                            break;
                        case "Starting5":
                            SendMessage(who);
                            who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.emoteSpriteSheet.ToString(), new Rectangle(13 * 16 % Game1.emoteSpriteSheet.Width, 12 * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), 200, 1, 0, new Vector2(x, y - 32), false, false, layer, 0f, Color.White, 4f, 0f, 0f, 0f, false)
                            {
                                owner = who,
                                id = nexusKey
                            });
                            break;
                        case "Starting6":
                            SendMessage(who);
                            who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.emoteSpriteSheet.ToString(), new Rectangle(14 * 16 % Game1.emoteSpriteSheet.Width, 12 * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), 200, 1, 0, new Vector2(x, y - 32), false, false, layer, 0f, Color.White, 4f, 0f, 0f, 0f, false)
                            {
                                owner = who,
                                id = nexusKey
                            });
                            break;
                        case "Starting7":
                            SendMessage(who);
                            who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.emoteSpriteSheet.ToString(), new Rectangle(15 * 16 % Game1.emoteSpriteSheet.Width, 12 * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), 200, 1, 0, new Vector2(x, y - 32), false, false, layer, 0f, Color.White, 4f, 0f, 0f, 0f, false)
                            {
                                owner = who,
                                id = nexusKey
                            });
                            break;
                        case "Starting8":
                            float t;
                            float distance = y - (float)(who.getStandingPosition().Y - 100);

                            float height = Math.Abs(distance + 170f);
                            if (message.oldFacingDirection == 0) height -= 130f;
                            else if (message.oldFacingDirection == 2) height -= 170f;
                            height = Math.Max(height, 0f);

                            float gravity = 0.002f;
                            float velocity = (float)Math.Sqrt((double)(2f * gravity * height));
                            t = (float)(Math.Sqrt((double)(2f * (height - distance) / gravity)) + (double)(velocity / gravity));
                            float xVelocity = 0f;
                            if (t != 0f)
                            {
                                xVelocity = (who.Position.X - x) / t;
                            }
                            who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite((item is Furniture) ? Furniture.furnitureTextureName : "Maps\\springobjects", message.sourceRect, t, 1, 0, new Vector2(x, y), false, false, layer, 0f, Color.White, message.itemSpriteSize, 0f, 0f, 0f, false)
                            {
                                motion = new Vector2(xVelocity, -velocity),
                                acceleration = new Vector2(0f, gravity),
                                timeBasedMotion = true,
                                endSound = "tinyWhip",
                                owner = who,
                                id = nexusKey
                            });
                            int delay = 25;
                            for (int i = 1; i < message.stack; i++)
                            {
                                who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", message.sourceRect, t, 1, 0, new Vector2(x, y), false, false, layer, 0f, Color.White, message.itemSpriteSize, 0f, 0f, 0f, false)
                                {
                                    delayBeforeAnimationStart = delay,
                                    motion = new Vector2(xVelocity, -velocity),
                                    acceleration = new Vector2(0f, gravity),
                                    timeBasedMotion = true,
                                    endSound = "tinyWhip",
                                    id = nexusKey,
                                    owner = who,
                                    Parent = who.currentLocation
                                });
                                delay += 25;
                            }
                            break;
                        case "Caught1":
                            who.UsingTool = false;
                            break;
                        case "CaughtBubble":
                            CaughtBubbleSprite(who);
                            break;
                    }
                }
            }
        }
        //public void OnWarped(object sender, WarpedEventArgs e)//maybe remove ---------------------------
        //{
        //    //if (e.Player == who && e.OldLocation != e.NewLocation)
        //    //{
        //    //    foreach (Farmer other in Game1.getAllFarmers())
        //    //    {
        //    //        ClearAnimations(other);
        //    //    }
        //    //}
        //}


        /// <summary>Makes text a tiny bit bolder and adds a border behind it. The border uses text colour's alpha for its aplha value. 6 DrawString operations, so 6x less efficient.</summary>
        private void DrawStringWithBorder(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth, Color? borderColor = null)
        {
            Color border = borderColor.HasValue ? borderColor.Value : Color.Black;
            border.A = color.A;
            batch.DrawString(font, text, position + new Vector2(-1.2f * scale, -1.2f * scale), border, rotation, origin, scale, effects, layerDepth - 0.00001f);
            batch.DrawString(font, text, position + new Vector2(1.2f * scale, -1.2f * scale), border, rotation, origin, scale, effects, layerDepth - 0.00001f);
            batch.DrawString(font, text, position + new Vector2(-1.2f * scale, 1.2f * scale), border, rotation, origin, scale, effects, layerDepth - 0.00001f);
            batch.DrawString(font, text, position + new Vector2(1.2f * scale, 1.2f * scale), border, rotation, origin, scale, effects, layerDepth - 0.00001f);

            batch.DrawString(font, text, position + new Vector2(-0.2f * scale, -0.2f * scale), color, rotation, origin, scale, effects, layerDepth);
            batch.DrawString(font, text, position + new Vector2(0.2f * scale, 0.2f * scale), color, rotation, origin, scale, effects, layerDepth);
        }


        public void DebugConsoleStartMinigameTest(string[] args)
        {
            rodDummy = (who.CurrentTool as FishingRod) ?? new FishingRod();

            try
            {
                if (args.Length == 1)
                {
                    if (ItemRegistry.GetData(args[0]) == null)
                    {
                        Item item2 = Utility.fuzzyItemSearch(args[0]);
                        if (item2 != null) whichFish = item2.QualifiedItemId;
                    }
                    Dictionary<string, string> data = Game1.content.Load<Dictionary<string, string>>("Data\\Fish");
                    if (data.ContainsKey(whichFish))
                    {
                        string[] rawData = data[whichFish].Split('/');
                        if (!int.TryParse(rawData[1], out difficulty) || !int.TryParse(rawData[3], out minFishSize) || !int.TryParse(rawData[4], out maxFishSize)) Monitor.Log("Missing Data\\Fish values, try a different fish.", LogLevel.Debug);
                        fishSize = Game1.random.Next(minFishSize, maxFishSize + 1);
                        bossFish = (who.CurrentItem as FishingRod).bossFish;

                        Monitor.Log("Starting minigame for " + rawData[0] + ". Difficulty: " + difficulty + ", size: " + fishSize + ", boss: " + bossFish, LogLevel.Debug);
                    }
                    else
                    {
                        Monitor.Log("No such fish in Data\\Fish", LogLevel.Debug);
                        return;
                    }
                }
                else
                {
                    difficulty = int.Parse(args[0]);
                    fishSize = int.Parse(args[1]);
                    if (args.Length < 3 || !bool.TryParse(args[2], out bossFish)) bossFish = false;

                    Monitor.Log("Starting minigame for difficulty: " + difficulty + ", size: " + fishSize + ", boss: " + bossFish, LogLevel.Debug);
                }
                debug = true;
                HereFishyFishy(who);
            }
            catch (Exception)
            {
                Monitor.Log("Invalid Input.", LogLevel.Debug);
            }
        }
    }
}
