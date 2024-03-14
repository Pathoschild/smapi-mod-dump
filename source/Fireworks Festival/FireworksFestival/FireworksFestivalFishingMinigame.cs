/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elizabethcd/FireworksFestival
**
*************************************************/

using System;
using FireworksFestival;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley.Extensions;
using StardewValley.Menus;
using StardewValley.Tools;
using xTile.Dimensions;

namespace StardewValley.Minigames
{
    public class FireworksFestivalFishingMinigame : IMinigame
    {
        private GameLocation location;

        private LocalizedContentManager content;

        private int timerToStart = 1000;

        private int gameEndTimer;

        private int showResultsTimer;

        public bool exit;

        public bool gameDone;

        public int score;

        public int fishCaught;

        public int carpWon;

        public int perfections;

        public int perfectionBonus;

        public GameLocation originalLocation;

        public FireworksFestivalFishingMinigame()
        {
            Tool fishingRod = ItemRegistry.Create<Tool>("(T)BambooPole");
            fishingRod.AttachmentSlotsCount = 2;
            fishingRod.attachments[0] = ItemRegistry.Create<Object>("(O)690", 99);
            fishingRod.attachments[1] = ItemRegistry.Create<Object>("(O)687");
            this.content = Game1.content.CreateTemporary();
            this.location = new GameLocation("Maps\\FireworksFestivalFishingGame", "fishingGame");
            this.location.isStructure.Value = true;
            this.location.uniqueName.Value = "fishingGame" + Game1.player.UniqueMultiplayerID;
            this.location.currentEvent = Game1.currentLocation.currentEvent;
            Game1.player.CurrentToolIndex = 0;
            Game1.player.TemporaryItem = fishingRod;
            Game1.player.UsingTool = false;
            Game1.player.CurrentToolIndex = 0;
            Game1.globalFadeToClear(null, 0.01f);
            this.location.Map.LoadTileSheets(Game1.mapDisplayDevice);
            Game1.player.Position = new Vector2(14f, 15f) * 64f;
            Game1.player.currentLocation = this.location;
            this.originalLocation = Game1.currentLocation;
            Game1.currentLocation = this.location;
            this.changeScreenSize();
            this.gameEndTimer = 100000;
            this.showResultsTimer = -1;
            Game1.player.faceDirection(2);
            Game1.player.Halt();

            ModEntry.monitorStatic.Log("Starting minigame!", LogLevel.Trace);
        }

        public bool overrideFreeMouseMovement()
        {
            return Game1.options.SnappyMenus;
        }

        public bool tick(GameTime time)
        {
            Rumble.update(time.ElapsedGameTime.Milliseconds);
            Game1.player.Stamina = Game1.player.MaxStamina;
            if (Game1.activeClickableMenu != null)
            {
                Game1.updateActiveMenu(time);
            }
            if (this.timerToStart > 0)
            {
                Game1.player.faceDirection(3);
                this.timerToStart -= time.ElapsedGameTime.Milliseconds;
                if (this.timerToStart <= 0)
                {
                    Game1.playSound("whistle");
                }
            }
            else if (this.showResultsTimer >= 0)
            {
                int num = this.showResultsTimer;
                this.showResultsTimer -= time.ElapsedGameTime.Milliseconds;
                if (num > 11000 && this.showResultsTimer <= 11000)
                {
                    Game1.playSound("smallSelect");
                }
                if (num > 9000 && this.showResultsTimer <= 9000)
                {
                    Game1.playSound("smallSelect");
                }
                if (num > 7000 && this.showResultsTimer <= 7000)
                {
                    if (this.perfections > 0)
                    {
                        this.score += this.perfections * 10;
                        this.perfectionBonus = this.perfections * 10;
                        if (this.fishCaught >= 3 && this.perfections >= 3)
                        {
                            this.perfectionBonus += this.score;
                            this.score *= 2;
                        }
                        Game1.playSound("newArtifact");
                    }
                    else
                    {
                        Game1.playSound("smallSelect");
                    }
                }
                if (num > 5000 && this.showResultsTimer <= 5000)
                {
                    if (this.score >= 10)
                    {
                        Game1.playSound("reward");
                        this.carpWon = (this.score + 5) / 10 * 6;
                        this.carpWon = this.carpWon / 20;
                        Game1.player.festivalScore += this.carpWon;
                    }
                    else
                    {
                        Game1.playSound("fishEscape");
                    }
                }
                if (this.showResultsTimer <= 0)
                {
                    Game1.globalFadeToClear();
                    return true;
                }
            }
            else if (!this.gameDone)
            {
                this.gameEndTimer -= time.ElapsedGameTime.Milliseconds;
                if (this.gameEndTimer <= 0 && Game1.activeClickableMenu == null && (!Game1.player.UsingTool || (Game1.player.CurrentTool as FishingRod).isFishing))
                {
                    (Game1.player.CurrentTool as FishingRod).doneFishing(Game1.player);
                    (Game1.player.CurrentTool as FishingRod).tickUpdate(time, Game1.player);
                    Game1.player.completelyStopAnimatingOrDoingAction();
                    Game1.playSound("whistle");
                    this.gameEndTimer = 1000;
                    this.gameDone = true;
                }
            }
            else if (this.gameDone && this.gameEndTimer > 0)
            {
                this.gameEndTimer -= time.ElapsedGameTime.Milliseconds;
                if (this.gameEndTimer <= 0)
                {
                    Game1.globalFadeToBlack(gameDoneAfterFade, 0.01f);
                    Game1.exitActiveMenu();
                    Game1.player.forceCanMove();
                }
            }
            return this.exit;
        }

        public void gameDoneAfterFade()
        {
            this.showResultsTimer = 11100;
            Game1.player.canMove = false;
            Game1.player.Position = new Vector2(5f, 36f) * 64f;
            Game1.player.TemporaryPassableTiles.Add(new Microsoft.Xna.Framework.Rectangle(1536, 4544, 64, 64));
            Game1.player.currentLocation = this.originalLocation;
            Game1.currentLocation = this.originalLocation;
            Game1.player.faceDirection(2);
            Utility.killAllStaticLoopingSoundCues();
            if (FishingRod.reelSound != null && FishingRod.reelSound.IsPlaying)
            {
                FishingRod.reelSound.Stop(AudioStopOptions.Immediate);
            }
        }

        public void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (!Game1.isAnyGamePadButtonBeingPressed())
            {
                this.handleCastInput();
            }
        }

        public void leftClickHeld(int x, int y)
        {
        }

        public void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public void releaseLeftClick(int x, int y)
        {
            this.handleCastInputReleased();
        }

        public void releaseRightClick(int x, int y)
        {
        }

        public void receiveKeyPress(Keys k)
        {
            if (!this.gameDone)
            {
                if (Game1.player.movementDirections.Count < 2 && !Game1.player.UsingTool && this.timerToStart <= 0)
                {
                    if (Game1.options.doesInputListContain(Game1.options.moveUpButton, k))
                    {
                        Game1.player.setMoving(1);
                    }
                    if (Game1.options.doesInputListContain(Game1.options.moveRightButton, k))
                    {
                        Game1.player.setMoving(2);
                    }
                    if (Game1.options.doesInputListContain(Game1.options.moveDownButton, k))
                    {
                        Game1.player.setMoving(4);
                    }
                    if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, k))
                    {
                        Game1.player.setMoving(8);
                    }
                }
                if (Game1.options.doesInputListContain(Game1.options.useToolButton, k))
                {
                    this.handleCastInput();
                }
                if (k == Keys.Escape)
                {
                    if (this.gameEndTimer <= 0 && !this.gameDone)
                    {
                        this.EmergencyCancel();
                    }
                    else if (Game1.activeClickableMenu == null)
                    {
                        this.gameEndTimer = 1;
                    }
                    else
                    {
                        (Game1.activeClickableMenu as BobberBar)?.receiveKeyPress(k);
                    }
                }
            }
            if (Game1.options.doesInputListContain(Game1.options.runButton, k) || Game1.isGamePadThumbstickInMotion())
            {
                Game1.player.setRunning(isRunning: true);
            }
        }

        public void receiveKeyRelease(Keys k)
        {
            if (Game1.options.doesInputListContain(Game1.options.moveUpButton, k))
            {
                Game1.player.setMoving(33);
            }
            if (Game1.options.doesInputListContain(Game1.options.moveRightButton, k))
            {
                Game1.player.setMoving(34);
            }
            if (Game1.options.doesInputListContain(Game1.options.moveDownButton, k))
            {
                Game1.player.setMoving(36);
            }
            if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, k))
            {
                Game1.player.setMoving(40);
            }
            if (Game1.options.doesInputListContain(Game1.options.runButton, k))
            {
                Game1.player.setRunning(isRunning: false);
            }
            if (Game1.player.movementDirections.Count == 0 && !Game1.player.UsingTool)
            {
                Game1.player.Halt();
            }
            if (Game1.options.doesInputListContain(Game1.options.useToolButton, k))
            {
                this.handleCastInputReleased();
            }
        }

        public virtual void EmergencyCancel()
        {
            Game1.player.Halt();
            Game1.player.isEating = false;
            Game1.player.CanMove = true;
            Game1.player.UsingTool = false;
            Game1.player.usingSlingshot = false;
            Game1.player.FarmerSprite.PauseForSingleAnimation = false;
            (Game1.player.CurrentTool as FishingRod)?.resetState();
        }

        private void handleCastInput()
        {
            if (this.timerToStart <= 0 && this.showResultsTimer < 0 && !this.gameDone && Game1.activeClickableMenu == null && !(Game1.player.CurrentTool as FishingRod).hit && !(Game1.player.CurrentTool as FishingRod).pullingOutOfWater && !(Game1.player.CurrentTool as FishingRod).isCasting && !(Game1.player.CurrentTool as FishingRod).fishCaught && !(Game1.player.CurrentTool as FishingRod).castedButBobberStillInAir)
            {
                Game1.player.lastClick = Vector2.Zero;
                Game1.player.Halt();
                Game1.pressUseToolButton();
            }
            else if (this.showResultsTimer > 11000)
            {
                this.showResultsTimer = 11001;
            }
            else if (this.showResultsTimer > 9000)
            {
                this.showResultsTimer = 9001;
            }
            else if (this.showResultsTimer > 7000)
            {
                this.showResultsTimer = 7001;
            }
            else if (this.showResultsTimer > 5000)
            {
                this.showResultsTimer = 5001;
            }
            else if (this.showResultsTimer < 5000 && this.showResultsTimer > 1000)
            {
                this.showResultsTimer = 1500;
                Game1.playSound("smallSelect");
            }
        }

        private void handleCastInputReleased()
        {
            if (this.showResultsTimer < 0 && Game1.player.CurrentTool != null && !(Game1.player.CurrentTool as FishingRod).isCasting && Game1.activeClickableMenu == null && Game1.player.CurrentTool.onRelease(this.location, 0, 0, Game1.player))
            {
                Game1.player.Halt();
            }
        }

        public void draw(SpriteBatch b)
        {
            if (this.showResultsTimer < 0)
            {
                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
                Game1.mapDisplayDevice.BeginScene(b);
                this.location.Map.RequireLayer("Back").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, wrapAround: false, 4);
                this.location.drawWater(b);
                b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, Game1.player.Position + new Vector2(32f, 24f)), Game1.shadowTexture.Bounds, Color.White, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 4f - ((Game1.player.running || Game1.player.UsingTool) ? ((float)Math.Abs(FarmerRenderer.featureYOffsetPerFrame[Game1.player.FarmerSprite.CurrentFrame]) * 0.8f) : 0f), SpriteEffects.None, Math.Max(0f, (float)Game1.player.StandingPixel.Y / 10000f + 0.00011f) - 1E-07f);
                this.location.Map.RequireLayer("Buildings").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, wrapAround: false, 4);
                this.location.draw(b);
                b.End();
                b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
                Game1.player.draw(b);
                b.End();
                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
                this.location.Map.RequireLayer("Front").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, wrapAround: false, 4);
                if (Game1.activeClickableMenu != null)
                {
                    Game1.activeClickableMenu.draw(b);
                }
                b.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1514", Utility.getMinutesSecondsStringFromMilliseconds(Math.Max(0, this.gameEndTimer))), new Vector2(16f, 64f), Color.White);
                b.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingGame.cs.10444", this.score), new Vector2(16f, 32f), Color.White);
                b.End();
                return;
            }
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            Vector2 position = new Vector2(Game1.viewport.Width / 2 - 128, Game1.viewport.Height / 2 - 64);
            if (this.showResultsTimer <= 11000)
            {
                Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingGame.cs.10444", this.score), Game1.textColor, (this.showResultsTimer <= 7000 && this.perfectionBonus > 0) ? Color.Lime : Color.White, position);
            }
            if (this.showResultsTimer <= 9000)
            {
                position.Y += 48f;
                Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingGame.cs.12010", this.fishCaught), Game1.textColor, Color.White, position);
            }
            if (this.showResultsTimer <= 7000)
            {
                position.Y += 48f;
                if (this.perfectionBonus > 1)
                {
                    Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingGame.cs.12011", this.perfectionBonus), Game1.textColor, Color.Yellow, position);
                }
                else
                {
                    Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingGame.cs.12012"), Game1.textColor, Color.Red, position);
                }
            }
            if (this.showResultsTimer <= 5000)
            {
                position.Y += 64f;
                if (this.carpWon > 0)
                {
                    float fade = Math.Min(1f, (float)(this.showResultsTimer - 2000) / 4000f);
                    Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingGame.cs.12013", this.carpWon), Game1.textColor * 0.2f * fade, Color.SkyBlue * 0.3f * fade, position + new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) * 4f * 2f, 0f, 1f, 1f);
                    Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingGame.cs.12013", this.carpWon), Game1.textColor * 0.2f * fade, Color.SkyBlue * 0.3f * fade, position + new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) * 4f * 2f, 0f, 1f, 1f);
                    Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingGame.cs.12013", this.carpWon), Game1.textColor * 0.2f * fade, Color.SkyBlue * 0.3f * fade, position + new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) * 4f * 2f, 0f, 1f, 1f);
                    Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingGame.cs.12013", this.carpWon), Game1.textColor, Color.SkyBlue, position, 0f, 1f, 1f);
                }
                else
                {
                    Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingGame.cs.12021"), Game1.textColor, Color.Red, position);
                }
            }
            if (this.showResultsTimer <= 1000)
            {
                b.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), Color.Black * (1f - (float)this.showResultsTimer / 1000f));
            }
            b.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle(16, 16, 128 + ((Game1.player.festivalScore > 999) ? 16 : 0), 64), Color.Black * 0.75f);
            b.Draw(Game1.mouseCursors, new Vector2(32f, 32f), new Microsoft.Xna.Framework.Rectangle(338, 400, 8, 8), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            Game1.drawWithBorder(Game1.player.festivalScore.ToString() ?? "", Color.Black, Color.White, new Vector2(72f, 29f), 0f, 1f, 1f, tiny: false);
            b.End();
        }

        public static void startMe()
        {
            Game1.currentMinigame = new FireworksFestivalFishingMinigame();
        }

        public void changeScreenSize()
        {
            Game1.viewport.X = this.location.Map.Layers[0].LayerWidth * 64 / 2 - (int)((float)(Game1.game1.localMultiplayerWindow.Width / 2) / Game1.options.zoomLevel);
            Game1.viewport.Y = this.location.Map.Layers[0].LayerHeight * 64 / 2 - (int)((float)(Game1.game1.localMultiplayerWindow.Height / 2) / Game1.options.zoomLevel);
        }

        public void unload()
        {
            FishingRod obj = (FishingRod)Game1.player.CurrentTool;
            obj.castingEndFunction(Game1.player);
            obj.doneFishing(Game1.player);

            if (carpWon > 0)
            {
                Object carp = new Object(ModEntry.carpIndex, carpWon);
                Game1.player.addItemByMenuIfNecessary(carp);
                Game1.player.festivalScore = 0;
            }

            Game1.player.TemporaryItem = null;
            Game1.player.currentLocation = Game1.currentLocation;
            Game1.player.completelyStopAnimatingOrDoingAction();
            Game1.player.forceCanMove();
            Game1.player.faceDirection(2);
            this.content.Unload();
            this.content.Dispose();
            this.content = null;
        }

        public void receiveEventPoke(int data)
        {
        }

        public string minigameId()
        {
            return "FishingGame";
        }

        public bool doMainGameUpdates()
        {
            return true;
        }

        public bool forceQuit()
        {
            return false;
        }
    }
}
