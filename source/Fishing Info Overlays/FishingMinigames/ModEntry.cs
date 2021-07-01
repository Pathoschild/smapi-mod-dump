/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/barteke22/StardewMods
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace FishingMinigames
{
    public class ModEntry : Mod
    {
        public static ModEntry context;

        public static ModConfig Config;
        private static SoundEffect fishySound;
        private static List<TemporaryAnimatedSprite> animations = new List<TemporaryAnimatedSprite>();
        private static SparklingText sparklingText;
        private static bool caughtDoubleFish;
        private static Farmer who;
        private static int whichFish;
        private static float fishSize;
        private static bool recordSize;
        private static bool perfect;
        private static int fishQuality;
        private static bool fishCaught;
        private static bool bossFish;
        private static int difficulty;
        private static bool treasureCaught;
        private static bool canPerfect;
        private static bool hereFishying;
        private static Object item;
        private static bool itemIsInstantCatch;
        private static bool fromFishPond;
        private static int oldFacingDirection;


        private static int startMinigameStyle = 1;

        private static int endMinigameStyle = 1;
        private static int endMinigameStage;
        private static int endMinigameTimer;
        private static bool endMinigameAnimate;
        private static int endMinigameScore;



        /*  Stop other player interaction while 'fishing'
         *  fix desert
         *  instead of where clicked, soundwave anim ahead? would be hard to aim at pools, could use swing effect anim?
         */


        public override void Entry(IModHelper helper)
        {
            context = this;
            Config = Helper.ReadConfig<ModConfig>();
            if (!Config.EnableMod)
                return;

            try
            {
                fishySound = SoundEffect.FromStream(new FileStream(Path.Combine(Helper.DirectoryPath, "assets", "fishy.wav"), FileMode.Open));
            }
            catch (Exception ex)
            {
                context.Monitor.Log($"error loading fishy.wav: {ex}");
            }
            Helper.Events.Display.RenderedWorld += Display_RenderedWorld;
            Helper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
            Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }


        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)  //this.Monitor.Log(locationName, LogLevel.Debug);
        {
            who = Game1.player;
            if (e.Button == SButton.Z)
            {
                
            }

            if (endMinigameStage == 2)
            {
                who.freezePause = 25;
                if (endMinigameStyle == 1 && e.Button == Game1.options.useToolButton[0].ToSButton() || (Game1.options.useToolButton.Length > 1 && e.Button == Game1.options.useToolButton[1].ToSButton()))
                {
                    if (endMinigameTimer < 20)
                    {
                        endMinigameStage = 10;
                        canPerfect = true;
                    }
                    else endMinigameStage = 9;
                }
                _ = CatchFishAfterMinigame(who);
            }
            else
            {
                if (e.Button != Game1.options.useToolButton[0].ToSButton() && (Game1.options.useToolButton.Length > 1 && e.Button != Game1.options.useToolButton[1].ToSButton())) return;

                if (Context.IsWorldReady && Context.CanPlayerMove && (who.CurrentTool is FishingRod))
                {
                    who.freezePause = 1; //stops fishing rod
                    try
                    {
                        endMinigameStage = 0;
                        perfect = false;
                        Vector2 mousePos = Game1.currentCursorTile;
                        oldFacingDirection = who.getGeneralDirectionTowards(new Vector2(mousePos.X * 64, mousePos.Y * 64));
                        who.faceDirection(oldFacingDirection);


                        if (who.currentLocation.canFishHere() && who.currentLocation.isTileFishable((int)mousePos.X, (int)mousePos.Y))
                        {
                            context.Monitor.Log($"here fishy fishy {mousePos.X},{mousePos.Y}");
                            HereFishyFishy(who, (int)mousePos.X * 64, (int)mousePos.Y * 64);
                        }
                    }
                    catch
                    {
                        context.Monitor.Log($"error getting water tile");
                    }
                }
            }
        }

        private void GameLoop_UpdateTicking(object sender, UpdateTickingEventArgs e) //adds item to inv
        {
            for (int i = animations.Count - 1; i >= 0; i--)
            {
                if (endMinigameStage != 2 && animations[i].update(Game1.currentGameTime))
                {
                    animations.RemoveAt(i);
                }
            }
            if (sparklingText != null && sparklingText.update(Game1.currentGameTime))
            {
                sparklingText = null;
            }
            if (fishCaught && !Game1.isFestival())
            {
                FishingRod rod = who.CurrentTool as FishingRod;
                Helper.Reflection.GetField<Farmer>(rod, "lastUser").SetValue(who);
                Helper.Reflection.GetField<int>(rod, "whichFish").SetValue(whichFish);
                Helper.Reflection.GetField<bool>(rod, "caughtDoubleFish").SetValue(caughtDoubleFish);
                Helper.Reflection.GetField<int>(rod, "fishQuality").SetValue(fishQuality);
                Helper.Reflection.GetField<Farmer>(who.CurrentTool, "lastUser").SetValue(who);

                if (!treasureCaught || itemIsInstantCatch)
                {
                    if (!fromFishPond) rod.doneFishing(who, true);
                    who.addItemByMenuIfNecessary(item);
                }
                else
                {
                    rod.openTreasureMenuEndFunction((!who.addItemToInventoryBool(item)) ? 1 : 0);
                }
                who.faceDirection(oldFacingDirection);
                Game1.freezeControls = false;
                fishCaught = false;
            }
        }

        private void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            for (int i = animations.Count - 1; i >= 0; i--)
            {
                if (endMinigameStage < 9) animations[i].draw(e.SpriteBatch, false, 0, 0, 1f);
                if (endMinigameStage > 0 && i == 0)
                {
                    who = Game1.player;
                    if (endMinigameStage == 1)
                    {
                        Rectangle area = new Rectangle((int)who.Position.X - 225, (int)who.Position.Y - 250, 425, 350);
                        if (area.Contains((int)animations[0].Position.X, (int)animations[0].Position.Y))
                        {
                            endMinigameStage = 2;
                            endMinigameTimer = 0;
                            who.PlayFishBiteChime();
                            Game1.screenOverlayTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(395, 497, 3, 8), new Vector2(who.getStandingX() - Game1.viewport.X, who.getStandingY() - 128 - 8 - Game1.viewport.Y), flipped: false, 0.02f, Color.White)
                            {
                                scale = 5f,
                                scaleChange = -0.01f,
                                motion = new Vector2(0f, -0.5f),
                                shakeIntensityChange = -0.005f,
                                shakeIntensity = 1f
                            });
                        }
                    }
                    else if (endMinigameStage == 2)
                    {
                        endMinigameTimer++;

                        if (endMinigameTimer > 100 - (difficulty / 2))
                        {
                            who.completelyStopAnimatingOrDoingAction();
                            List<FarmerSprite.AnimationFrame> animationFrames = new List<FarmerSprite.AnimationFrame>(){
                                new FarmerSprite.AnimationFrame(94, 500, false, false, null, false).AddFrameAction(delegate (Farmer f) { f.jitterStrength = 2f; }) };
                            who.FarmerSprite.setCurrentAnimation(animationFrames.ToArray());
                            who.FarmerSprite.PauseForSingleAnimation = true;
                            who.FarmerSprite.loop = true;
                            who.FarmerSprite.loopThisAnimation = true;
                            who.Sprite.currentFrame = 94;
                            endMinigameStage = 8;
                        }
                    }
                    else if (endMinigameAnimate) Game1.drawTool(who);
                }
            }

            

            if (canPerfect)
            {
                perfect = true;
                sparklingText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\UI:BobberBar_Perfect"), Color.Yellow, Color.White, false, 0.1, 1500, -1, 500, 1f);
                Game1.playSound("jingle1");
                canPerfect = false;
            }

            if (sparklingText != null && who != null && !itemIsInstantCatch)
            {
                sparklingText.draw(e.SpriteBatch, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(-64f, -352f)));
            }
        }


        private async static void HereFishyFishy(Farmer who, int x, int y)
        {
            await HereFishyStartingAnimation(who);




            CatchFish(who, x, y);




            if (endMinigameStyle > 0) endMinigameStage = 1;
            else await CatchFishAfterMinigame(who);

            await HereFishyFlyingAnimation(who, x, y);
        }

        public async static void PlayerCaughtFishEndFunction(int extraData)
        {
            if (endMinigameStage > 8) await Task.Delay(1000);
            context.Monitor.Log($"caught fish end");
            fishCaught = true;
            who.Halt();
            who.armOffset = Vector2.Zero;

            recordSize = who.caughtFish(whichFish, (int)fishSize, false, caughtDoubleFish ? 2 : 1);
            if (FishingRod.isFishBossFish(whichFish))
            {
                Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14068"));
                string name = Game1.objectInformation[whichFish].Split('/')[4];

                //context.Helper.Reflection.GetField<Multiplayer>(Game1.game1, "multiplayer").GetValue().globalChatInfoMessage("CaughtLegendaryFish", new string[] { who.Name, name }); //multiplayer class is not protected
                if (Game1.IsMultiplayer || Game1.multiplayerMode != 0)
                {
                    if (Game1.IsClient) Game1.client.sendMessage(15, "CaughtLegendaryFish", new string[] { who.Name, name });
                    else
                    {
                        if (!Game1.IsServer) return;

                        foreach (long id in Game1.otherFarmers.Keys)
                        {
                            Game1.server.sendMessage(id, 15, who, "CaughtLegendaryFish", new string[] { who.Name, name });
                        }
                    }
                }
                return;
            }
            if (recordSize)
            {
                sparklingText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14069"), Color.LimeGreen, Color.Azure, false, 0.1, 2500, -1, 500, 1f);
                who.currentLocation.localSound("newRecord");
                return;
            }
            who.currentLocation.localSound("fishSlap");
        }


        private static void CatchFish(Farmer who, int x, int y)
        {
            FishingRod rod = who.CurrentTool as FishingRod;
            Vector2 bobberTile = new Vector2(x / 64, y / 64);
            fromFishPond = who.currentLocation.isTileBuildingFishable((int)bobberTile.X, (int)bobberTile.Y);

            int clearWaterDistance = FishingRod.distanceToLand((int)bobberTile.X, (int)bobberTile.Y, who.currentLocation);
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

            if (item != null) whichFish = item.ParentSheetIndex;//fix here for fishpond

            if (item == null || whichFish <= 0)
            {
                item = new Object(Game1.random.Next(167, 173), 1);//trash
                whichFish = item.ParentSheetIndex;
            }

            Dictionary<int, string> data = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            string[] fishData = null;
            if (data.ContainsKey(whichFish)) fishData = data[whichFish].Split('/');


            itemIsInstantCatch = false;
            if (item is Furniture) itemIsInstantCatch = true;
            else if (Utility.IsNormalObjectAtParentSheetIndex(item, whichFish) && data.ContainsKey(whichFish))
            {
                difficulty = -1;
                if (!int.TryParse(fishData[1], out difficulty)) itemIsInstantCatch = true;
            }
            else itemIsInstantCatch = true;

            if (itemIsInstantCatch || item.Category == -20 || item.ParentSheetIndex == 152 || item.ParentSheetIndex == 153 || item.parentSheetIndex == 157 || item.parentSheetIndex == 797 || item.parentSheetIndex == 79 || item.parentSheetIndex == 73 || item.ParentSheetIndex == 842 || (item.ParentSheetIndex >= 820 && item.ParentSheetIndex <= 828) || item.parentSheetIndex == GameLocation.CAROLINES_NECKLACE_ITEM || item.ParentSheetIndex == 890 || fromFishPond)
            {
                itemIsInstantCatch = true;
            }

            //special item handling
            if (whichFish == GameLocation.CAROLINES_NECKLACE_ITEM) item.questItem.Value = true;
            if (whichFish == 79 || whichFish == 842)
            {
                item = who.currentLocation.tryToCreateUnseenSecretNote(who);
            }
            if (!Game1.isFestival() && !(item is Furniture) && !fromFishPond && who.team.specialOrders != null)
            {
                foreach (SpecialOrder order in who.team.specialOrders)
                {
                    order.onFishCaught?.Invoke(who, item);
                }
            }




            //data calculations: size, quality, double, exp, treasure
            fishSize = 1f;
            fishQuality = 0;
            if (!itemIsInstantCatch)
            {
                fishSize *= Math.Min((float)clearWaterDistance / 5f, 1f);
                int minimumSizeContribution = 1 + who.FishingLevel / 2;
                fishSize *= (float)Game1.random.Next(minimumSizeContribution, Math.Max(6, minimumSizeContribution)) / 5f;

                if (rod.getBaitAttachmentIndex() != -1) fishSize *= 1.2f;
                fishSize *= 1f + (float)Game1.random.Next(-10, 11) / 100f;
                fishSize = Math.Max(0f, Math.Min(1f, fishSize));

                if (fishData != null)
                {
                    int minFishSize = int.Parse(fishData[3]);
                    int maxFishSize = int.Parse(fishData[4]);
                    fishSize = (int)((float)minFishSize + (float)(maxFishSize - minFishSize) * fishSize);
                    fishSize++;

                    fishQuality = (fishSize * (0.9 + (who.FishingLevel / 5.0)) < 0.33) ? 0 : ((fishSize * (0.9 + (who.FishingLevel / 5.0)) < 0.66) ? 1 : 2);
                    if (rod.getBobberAttachmentIndex() == 877) fishQuality++;

                    if (perfect) fishQuality++;

                    if (rod.Name.Equals("Training Rod", StringComparison.Ordinal))
                    {
                        fishQuality = 0;
                        fishSize = minFishSize;
                    }
                    if (fishQuality < 0) fishQuality = 0;
                    if (fishQuality > 2) fishQuality = 4;
                }

                bossFish = FishingRod.isFishBossFish(whichFish);
                caughtDoubleFish = !bossFish && !fromFishPond && Game1.random.NextDouble() < 0.1 + who.DailyLuck / 2.0;

                context.Monitor.Log($"pulling fish {whichFish} {fishSize} {who.Name} {x},{y}");

                if (who.IsLocalPlayer)
                {
                    if (fishData != null) difficulty = int.Parse(fishData[1]);
                    else difficulty = 0;

                    int experience = Math.Max(1, (fishQuality + 1) * 3 + difficulty / 3);
                    if (bossFish) experience *= 5;

                    if (perfect) experience += (int)((float)experience * 1.4f);

                    who.gainExperience(1, experience);
                }
            }
            else if (who.IsLocalPlayer)
            {
                difficulty = 0;
                who.gainExperience(1, 3);
            }


            treasureCaught = false;
            if (!itemIsInstantCatch)
            {
                treasureCaught = !Game1.isFestival() && who.fishCaught != null && who.fishCaught.Count() > 1 && Game1.random.NextDouble() < FishingRod.baseChanceForTreasure + (double)who.LuckLevel * 0.005 + ((rod.getBaitAttachmentIndex() == 703) ? FishingRod.baseChanceForTreasure : 0.0) + ((rod.getBobberAttachmentIndex() == 693) ? (FishingRod.baseChanceForTreasure / 3.0) : 0.0) + who.DailyLuck / 2.0 + (who.professions.Contains(9) ? FishingRod.baseChanceForTreasure : 0.0);
            }


            if (!itemIsInstantCatch)
            {
                item.Quality = fishQuality;
                if (caughtDoubleFish) item.Stack = 2;
            }

            if (fishData == null) context.Monitor.Log(item.DisplayName + ", Quality: " + fishQuality, LogLevel.Debug);
            else context.Monitor.Log(item.DisplayName + ", Size: " + fishData[3] + "-" + fishData[4] + " (" + fishSize + "), Quality: " + fishQuality, LogLevel.Debug);
        }

        private async static Task CatchFishAfterMinigame(Farmer who) //move quality etc here
        {
            if (endMinigameStyle > 0) {
                who.completelyStopAnimatingOrDoingAction();
                endMinigameAnimate = true;
                (who.CurrentTool as FishingRod).setTimingCastAnimation(who);
                switch (oldFacingDirection)
                {
                    case 0:
                        who.FarmerSprite.animateOnce(295, 1f, 1);
                        who.CurrentTool.Update(0, 0, who);
                        break;
                    case 1:
                        who.FarmerSprite.animateOnce(296, 1f, 1);
                        who.CurrentTool.Update(1, 0, who);
                        break;
                    case 2:
                        who.FarmerSprite.animateOnce(297, 1f, 1);
                        who.CurrentTool.Update(2, 0, who);
                        break;
                    case 3:
                        who.FarmerSprite.animateOnce(298, 1f, 1);
                        who.CurrentTool.Update(3, 0, who);
                        break;
                }
                await Task.Delay(300);
                endMinigameAnimate = false;
            }
        }

        //fish flying from xy to player
        private async static Task HereFishyStartingAnimation(Farmer who)
        {
            Game1.freezeControls = true;
            hereFishying = true;
            if (fishySound != null)
            {
                fishySound.Play();
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


            await Task.Delay(1793);

            if (startMinigameStyle + endMinigameStyle == 0 && Game1.random.Next(who.FishingLevel, 20) > 16)
            {
                canPerfect = true;
            }

            who.synchronizedJump(8f);

            await Task.Delay(1000);

            who.stopJittering();
            who.completelyStopAnimatingOrDoingAction();
            who.forceCanMove();

            hereFishying = false;

            await Task.Delay(Game1.random.Next(500, 1000));

            animations.Clear();
        }

        //fish flying from xy to player
        private async static Task HereFishyFlyingAnimation(Farmer who, int x, int y)
        {
            float t;
            if (who.FacingDirection == 1 || who.FacingDirection == 3)
            {
                float distance = Vector2.Distance(new Vector2(x, y), who.Position);
                float gravity = 0.001f;
                float height = 128f - (who.Position.Y - y + 10f);
                double angle = 1.1423973285781066;
                float yVelocity = (float)((double)(distance * gravity) * Math.Tan(angle) / Math.Sqrt((double)(2f * distance * gravity) * Math.Tan(angle) - (double)(2f * gravity * height)));

                if (float.IsNaN(yVelocity))
                {
                    yVelocity = 0.6f;
                }
                float xVelocity = (float)((double)yVelocity * (1.0 / Math.Tan(angle)));
                t = distance / xVelocity;
                animations.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, whichFish, 16, 16), t, 1, 0, new Vector2(x, y), false, false, y / 10000f, 0f, Color.White, 4f, 0f, 0f, 0f, false)
                {
                    motion = new Vector2((float)((who.FacingDirection == 3) ? -1 : 1) * -xVelocity, -yVelocity),
                    acceleration = new Vector2(0f, gravity),
                    timeBasedMotion = true,
                    endFunction = new TemporaryAnimatedSprite.endBehavior(PlayerCaughtFishEndFunction),
                    extraInfoForEndBehavior = whichFish,
                    endSound = "tinyWhip"
                });
                for (int i = 1; i < item.Stack; i++)
                {
                    distance = Vector2.Distance(new Vector2(x, y), who.Position);
                    gravity = 0.0008f;
                    height = 128f - (who.Position.Y - y + 10f);
                    angle = 1.1423973285781066;
                    yVelocity = (float)((double)(distance * gravity) * Math.Tan(angle) / Math.Sqrt((double)(2f * distance * gravity) * Math.Tan(angle) - (double)(2f * gravity * height)));
                    if (float.IsNaN(yVelocity))
                    {
                        yVelocity = 0.6f;
                    }
                    xVelocity = (float)((double)yVelocity * (1.0 / Math.Tan(angle)));
                    t = distance / xVelocity;
                    animations.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, whichFish, 16, 16), t, 1, 0, new Vector2(x, y), false, false, y / 10000f, 0f, Color.White, 4f, 0f, 0f, 0f, false)
                    {
                        motion = new Vector2((float)((who.FacingDirection == 3) ? -1 : 1) * -xVelocity, -yVelocity),
                        acceleration = new Vector2(0f, gravity),
                        timeBasedMotion = true,
                        endSound = "fishSlap",
                        Parent = who.currentLocation
                    });
                    if (item.Stack > 2) await Task.Delay(100);
                }
            }
            else
            {
                float distance2 = y - (float)(who.getStandingY() - 64);
                float height2 = Math.Abs(distance2 + 256f + 32f);
                if (who.FacingDirection == 0)
                {
                    height2 += 96f;
                }
                float gravity2 = 0.003f;
                float velocity = (float)Math.Sqrt((double)(2f * gravity2 * height2));
                t = (float)(Math.Sqrt((double)(2f * (height2 - distance2) / gravity2)) + (double)(velocity / gravity2));
                float xVelocity2 = 0f;
                if (t != 0f)
                {
                    xVelocity2 = (who.Position.X - x) / t;
                }
                animations.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, whichFish, 16, 16), t, 1, 0, new Vector2(x, y), false, false, y / 10000f, 0f, Color.White, 4f, 0f, 0f, 0f, false)
                {
                    motion = new Vector2(xVelocity2, -velocity),
                    acceleration = new Vector2(0f, gravity2),
                    timeBasedMotion = true,
                    endFunction = new TemporaryAnimatedSprite.endBehavior(PlayerCaughtFishEndFunction),
                    extraInfoForEndBehavior = whichFish,
                    endSound = "tinyWhip"
                });
                for (int i = 1; i < item.Stack; i++)
                {
                    distance2 = y - (float)(who.getStandingY() - 64);
                    height2 = Math.Abs(distance2 + 256f + 32f);
                    if (who.FacingDirection == 0)
                    {
                        height2 += 96f;
                    }
                    gravity2 = 0.004f;
                    velocity = (float)Math.Sqrt((double)(2f * gravity2 * height2));
                    t = (float)(Math.Sqrt((double)(2f * (height2 - distance2) / gravity2)) + (double)(velocity / gravity2));
                    xVelocity2 = 0f;
                    if (t != 0f)
                    {
                        xVelocity2 = (who.Position.X - x) / t;
                    }
                    animations.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, whichFish, 16, 16), t, 1, 0, new Vector2(x, y), false, false, y / 10000f, 0f, Color.White, 4f, 0f, 0f, 0f, false)
                    {
                        motion = new Vector2(xVelocity2, -velocity),
                        acceleration = new Vector2(0f, gravity2),
                        timeBasedMotion = true,
                        endSound = "fishSlap",
                        Parent = who.currentLocation
                    });
                    if (item.Stack > 2) await Task.Delay(100);
                }
            }
            Game1.freezeControls = true;
        }
    }
}