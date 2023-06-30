/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.Framework.Interfaces;
using FishingTrawler.Framework.Utilities;
using FishingTrawler.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FishingTrawler.Framework.Objects.Items.Tools
{
    public enum AnimationState
    {
        Idle,
        Windup,
        Throw,
        Kneel,
        WaitAfterKneel,
        StartPullup,
        FinishPullup,
        ShowFish
    }

    public class Trident
    {
        internal static int caughtFishId;
        internal static double fishSize;
        internal static int fishQuality;
        internal static int fishCount;
        internal static bool isRecordSizeFish;

        internal static double cooldownTimer;
        internal static double animationTimer;
        internal static double displayTimer;

        internal static AnimationState animationState;
        internal static Vector2 targetPosition;
        internal static int oldFacingDirection;

        public static GenericTool CreateInstance()
        {
            var trident = new GenericTool(string.Empty, string.Empty, -1, 6, 6);
            trident.modData[ModDataKeys.TRIDENT_TOOL_KEY] = true.ToString();

            return trident;
        }

        public static bool IsValid(Tool tool)
        {
            if (tool is not null && tool.modData.ContainsKey(ModDataKeys.TRIDENT_TOOL_KEY))
            {
                return true;
            }

            return false;
        }

        private static void Reset(Farmer who)
        {
            caughtFishId = -1;
            animationState = AnimationState.Idle;

            who.forceCanMove();
        }

        public static bool Use(GameLocation location, int x, int y, Farmer who)
        {
            if (animationState is not AnimationState.Idle)
            {
                return false;
            }
            Reset(who);

            var standingPosition = who.getTileLocation();
            switch (who.FacingDirection)
            {
                case Game1.up:
                    standingPosition.Y += -1f;
                    break;
                case Game1.right:
                    standingPosition.X += 1f;
                    break;
                case Game1.down:
                    standingPosition.Y += 1f;
                    break;
                case Game1.left:
                    standingPosition.X += -1f;
                    break;
            }

            if (location.canFishHere() is false || string.IsNullOrEmpty(location.doesTileHaveProperty((int)standingPosition.X, (int)standingPosition.Y, "Water", "Back")) is true)
            {
                who.currentLocation.playSound("cancel");
                Game1.addHUDMessage(new HUDMessage(FishingTrawler.i18n.Get("game_message.trident.face_water"), 3) { timeLeft = 1000f });
                return false;
            }
            targetPosition = standingPosition;

            // Check to see if there are fish in this location
            var fishObject = location.getFish(-1f, -1, Game1.random.Next(0, 6), who, -1f, targetPosition);
            if (fishObject is null)
            {
                who.currentLocation.playSound("cancel");
                Game1.addHUDMessage(new HUDMessage(FishingTrawler.i18n.Get("game_message.trident.no_fish"), 3) { timeLeft = 1000f });
                return false;
            }
            else if (FishingRod.isFishBossFish(fishObject.ParentSheetIndex) is true)
            {
                who.currentLocation.playSound("cancel");
                Game1.addHUDMessage(new HUDMessage(FishingTrawler.i18n.Get("game_message.trident.boss_fish"), 2) { timeLeft = 1000f });
                return false;
            }
            caughtFishId = fishObject.ParentSheetIndex;

            // Handle exhaustion
            if (who.Stamina <= 1f)
            {
                if (!who.isEmoting)
                {
                    who.doEmote(36);
                }

                return false;
            }

            float oldStamina = who.Stamina;
            who.Stamina -= 12f - (float)who.FishingLevel * 0.1f;
            who.checkForExhaustion(oldStamina);

            // Set required flags
            who.Halt();
            who.canReleaseTool = false;
            who.UsingTool = true;
            who.CanMove = false;

            // Set animation
            animationState = AnimationState.Idle;
            oldFacingDirection = who.FacingDirection;

            // Get the fish
            fishSize = fishObject.HasContextTag("category_fish") ? GetFishSize(who, caughtFishId) : -1;
            fishQuality = fishObject.HasContextTag("category_fish") ? GetFishQuality(who) : 0;
            fishCount = Game1.random.NextDouble() < who.FishingLevel / 100f ? 2 : 1;
            isRecordSizeFish = who.caughtFish(caughtFishId, (int)fishSize, false, 1);

            // Give the experience
            who.gainExperience(1, Math.Max(1, (fishQuality + 1) * 3));

            //base.Update(who.FacingDirection, 0, who);
            return false;
        }

        private static int GetFishQuality(Farmer who)
        {
            int quality = 0;
            float fishingLevelOffset = who.FishingLevel / 100f;
            switch (Game1.random.NextDouble())
            {
                case var value when value < 0.15 + fishingLevelOffset:
                    quality = 2;
                    break;
                case var value when value < 0.35 + fishingLevelOffset:
                    quality = 1;
                    break;
            }

            return quality;
        }

        private static double GetFishSize(Farmer who, int whichFish)
        {
            Dictionary<int, string> data = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");

            int minFishSize = 0;
            if (data.ContainsKey(whichFish))
            {
                string[] rawData = data[whichFish].Split('/');
                minFishSize = Convert.ToInt32(rawData[3]);
            }

            return Math.Round(minFishSize + Game1.random.NextDouble(), 2);
        }

        private static int GetRandomFishForLocation(GameLocation location)
        {
            List<int> eligibleFishIds = new List<int>();

            // Iterate through any valid locations to find the fish eligible for rewarding (fish need to be in season and player must have minimum level for it)
            Dictionary<string, string> locationData = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
            if (!locationData.ContainsKey(location.Name))
            {
                return -1;
            }

            string[] rawFishData = locationData[location.Name].Split('/')[4 + Utility.getSeasonNumber(Game1.currentSeason)].Split(' ');
            Dictionary<int, string> rawFishDataWithLocation = new Dictionary<int, string>();
            if (rawFishData.Length > 1)
            {
                for (int j = 0; j < rawFishData.Length; j += 2)
                {
                    rawFishDataWithLocation[Convert.ToInt32(rawFishData[j])] = rawFishData[j + 1];
                }
            }
            eligibleFishIds.AddRange(rawFishDataWithLocation.Keys.Where(i => !TrawlerRewards.forbiddenFish.Contains(i)).Distinct());

            if (eligibleFishIds.Count == 0)
            {
                return -1;
            }

            return eligibleFishIds[Game1.random.Next(eligibleFishIds.Count)];
        }

        public static void Update(Tool tool, GameTime gameTime, Farmer who)
        {
            bool isAnimationOver = true;
            if (animationTimer >= 0f)
            {
                isAnimationOver = false;
                animationTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            else
            {
                // Move animation to next state
                switch (animationState)
                {
                    case AnimationState.Idle:
                        animationState = AnimationState.Windup;
                        break;
                    case AnimationState.Windup:
                        animationState = AnimationState.Throw;
                        break;
                    case AnimationState.Throw:
                        animationState = AnimationState.Kneel;
                        break;
                    case AnimationState.Kneel:
                        animationState = AnimationState.WaitAfterKneel;
                        break;
                    case AnimationState.WaitAfterKneel:
                        animationState = AnimationState.StartPullup;
                        break;
                    case AnimationState.StartPullup:
                        animationState = AnimationState.FinishPullup;
                        break;
                    case AnimationState.FinishPullup:
                        animationState = AnimationState.ShowFish;
                        break;
                }
            }

            // Handle current AnimationState
            int frameIndex = -1;
            if (animationState is AnimationState.Windup && isAnimationOver is true)
            {
                animationTimer = 250f;
                switch (who.FacingDirection)
                {
                    case Game1.up:
                        frameIndex = 0;
                        break;
                    case Game1.down:
                        frameIndex = 84;
                        break;
                    case Game1.left:
                    case Game1.right:
                        frameIndex = 30;
                        break;
                }
                who.FarmerSprite.setCurrentFrame(frameIndex, 0, 100, 1, flip: who.FacingDirection == Game1.left, secondaryArm: false);
            }
            else if (animationState is AnimationState.Throw && isAnimationOver is true)
            {
                animationTimer = 125f;
                switch (who.FacingDirection)
                {
                    case Game1.up:
                        frameIndex = 0;
                        break;
                    case Game1.down:
                        frameIndex = 2;
                        break;
                    case Game1.left:
                    case Game1.right:
                        frameIndex = 50;
                        break;
                }
                who.FarmerSprite.setCurrentFrame(frameIndex, 0, 100, 1, flip: who.FacingDirection == Game1.left, secondaryArm: false);
            }
            else if (animationState is AnimationState.Kneel && isAnimationOver is true)
            {
                animationTimer = 125f;
                switch (who.FacingDirection)
                {
                    case Game1.up:
                        frameIndex = 0;
                        break;
                    case Game1.down:
                        frameIndex = 5;
                        break;
                    case Game1.left:
                    case Game1.right:
                        frameIndex = 34;
                        break;
                }
                who.FarmerSprite.setCurrentFrame(frameIndex, 0, 100, 1, flip: who.FacingDirection == Game1.left, secondaryArm: false);

                who.currentLocation.playSound("dropItemInWater");
                who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(28, 100f, 2, 1, targetPosition * 64f, flicker: false, flipped: false));
            }
            else if (animationState is AnimationState.WaitAfterKneel && isAnimationOver is true)
            {
                animationTimer = 500f;
                switch (who.FacingDirection)
                {
                    case Game1.up:
                        frameIndex = 0;
                        break;
                    case Game1.down:
                        frameIndex = 4;
                        break;
                    case Game1.left:
                    case Game1.right:
                        frameIndex = 35;
                        break;
                }
                who.FarmerSprite.setCurrentFrame(frameIndex, 0, 100, 1, flip: who.FacingDirection == Game1.left, secondaryArm: false);
            }
            else if (animationState is AnimationState.StartPullup && isAnimationOver is true)
            {
                animationTimer = 300f;
                switch (who.FacingDirection)
                {
                    case Game1.up:
                        frameIndex = 0;
                        break;
                    case Game1.down:
                        frameIndex = 5;
                        break;
                    case Game1.left:
                    case Game1.right:
                        frameIndex = 34;
                        break;
                }
                who.FarmerSprite.setCurrentFrame(frameIndex, 0, 100, 1, flip: who.FacingDirection == Game1.left, secondaryArm: false);
            }
            else if (animationState is AnimationState.FinishPullup && isAnimationOver is true)
            {
                animationTimer = 300f;
                switch (who.FacingDirection)
                {
                    case Game1.up:
                        frameIndex = 0;
                        break;
                    case Game1.down:
                        frameIndex = 25;
                        break;
                    case Game1.left:
                    case Game1.right:
                        frameIndex = 50;
                        break;
                }
                who.FarmerSprite.setCurrentFrame(frameIndex, 0, 100, 1, flip: who.FacingDirection == Game1.left, secondaryArm: false);
            }
            else if (animationState is AnimationState.ShowFish)
            {
                who.faceDirection(2);
                who.FarmerSprite.setCurrentFrame(84);

                displayTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
                if (displayTimer <= 0f && Game1.input.GetMouseState().LeftButton == ButtonState.Pressed || Game1.didPlayerJustClickAtAll() || Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.useToolButton))
                {
                    who.addItemByMenuIfNecessary(new StardewValley.Object(caughtFishId, fishCount, quality: fishQuality));

                    Reset(who);
                    tool.endUsing(who.currentLocation, who);

                    who.faceDirection(oldFacingDirection);
                }
            }
        }

        public static void Draw(SpriteBatch b, Farmer who)
        {
            if (animationState is AnimationState.ShowFish && caughtFishId > 0)
            {
                ReplicateVanillaFishDisplay(b, who);
            }
            else
            {
                float rotation = 0f;
                var offset = Vector2.Zero;

                // Draw the trident
                switch (animationState)
                {
                    case AnimationState.Windup:
                        switch (who.FacingDirection)
                        {
                            case Game1.up:
                                return;
                            case Game1.down:
                                offset = new Vector2(52f, -48f);
                                rotation = 2.35f;
                                break;
                            case Game1.left:
                                offset = new Vector2(58f, -12f);
                                rotation = 3.5f;
                                break;
                            case Game1.right:
                                offset = new Vector2(58f, -72f);
                                rotation = 1.75f;
                                break;
                        }

                        b.Draw(FishingTrawler.assetManager.TridentTexture, Game1.GlobalToLocal(Game1.viewport, who.Position + offset), new Rectangle(0, 0, 16, 16), Color.White * 0.8f, rotation, Vector2.Zero, 4f, SpriteEffects.None, (float)who.getStandingY() / 10000f + 0.06f);
                        break;
                    case AnimationState.Throw:
                        switch (who.FacingDirection)
                        {
                            case Game1.up:
                                return;
                            case Game1.down:
                                offset = new Vector2(56f, 0f);
                                rotation = 2.35f;
                                break;
                            case Game1.left:
                                offset = new Vector2(24f, 0f);
                                rotation = 3.5f;
                                break;
                            case Game1.right:
                                offset = new Vector2(96f, -58f);
                                rotation = 1.75f;
                                break;
                        }

                        b.Draw(FishingTrawler.assetManager.TridentTexture, Game1.GlobalToLocal(Game1.viewport, who.Position + offset), new Rectangle(0, 0, 16, 16), Color.White * 0.8f, rotation, Vector2.Zero, 4f, SpriteEffects.None, (float)who.getStandingY() / 10000f + 0.06f);
                        break;
                    case AnimationState.Kneel:
                        switch (who.FacingDirection)
                        {
                            case Game1.up:
                                return;
                            case Game1.down:
                                offset = new Vector2(56f, 64f);
                                rotation = 2.35f;
                                break;
                            case Game1.left:
                                offset = new Vector2(20f, 42f);
                                rotation = 3.25f;
                                break;
                            case Game1.right:
                                offset = new Vector2(104f, -16f);
                                rotation = 1.75f;
                                break;
                        }

                        b.Draw(FishingTrawler.assetManager.TridentTexture, Game1.GlobalToLocal(Game1.viewport, who.Position + offset), new Rectangle(16, 0, 16, 16), Color.White * 0.8f, rotation, Vector2.Zero, 4f, SpriteEffects.None, (float)who.getStandingY() / 10000f + 0.06f);
                        break;
                    case AnimationState.WaitAfterKneel:
                        switch (who.FacingDirection)
                        {
                            case Game1.up:
                                return;
                            case Game1.down:
                                offset = new Vector2(56f, 58f);
                                rotation = 2.35f;
                                break;
                            case Game1.left:
                                offset = new Vector2(18f, 48f);
                                rotation = 3.25f;
                                break;
                            case Game1.right:
                                offset = new Vector2(106f, -12f);
                                rotation = 1.75f;
                                break;
                        }

                        b.Draw(FishingTrawler.assetManager.TridentTexture, Game1.GlobalToLocal(Game1.viewport, who.Position + offset), new Rectangle(16, 0, 16, 16), Color.White * 0.8f, rotation, Vector2.Zero, 4f, SpriteEffects.None, (float)who.getStandingY() / 10000f + 0.06f);
                        break;
                    case AnimationState.StartPullup:
                        switch (who.FacingDirection)
                        {
                            case Game1.up:
                                return;
                            case Game1.down:
                                offset = new Vector2(56f, 32f);
                                rotation = 2.35f;
                                break;
                            case Game1.left:
                                offset = new Vector2(22f, 40f);
                                rotation = 3.25f;
                                break;
                            case Game1.right:
                                offset = new Vector2(102f, -20f);
                                rotation = 1.75f;
                                break;
                        }

                        b.Draw(FishingTrawler.assetManager.TridentTexture, Game1.GlobalToLocal(Game1.viewport, who.Position + offset), new Rectangle(16, 0, 16, 16), Color.White * 0.8f, rotation, Vector2.Zero, 4f, SpriteEffects.None, (float)who.getStandingY() / 10000f + 0.06f);
                        break;
                }
            }
        }

        private static void ReplicateVanillaFishDisplay(SpriteBatch b, Farmer who)
        {
            bool caughtDoubleFish = fishCount == 2;

            float yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);

            var dynamicReflectionsApi = FishingTrawler.apiManager.GetDynamicReflectionsInterface();
            if (ShouldSkipForDynamicReflections(dynamicReflectionsApi) is false)
            {
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(-120f, -288f + yOffset)), new Rectangle(31, 1870, 73, 49), Color.White * 0.8f, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)who.getStandingY() / 10000f + 0.06f);
                b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(-124f, -284f + yOffset) + new Vector2(44f, 68f)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, Trident.caughtFishId, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)who.getStandingY() / 10000f + 0.0001f + 0.06f);
            }

            // Draw held fish
            b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(0f, -56f)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, Trident.caughtFishId, 16, 16), Color.White, ((float)Math.PI * 3f / 4f), new Vector2(8f, 8f), 3f, SpriteEffects.None, (float)who.getStandingY() / 10000f + 0.002f + 0.06f);
            if (caughtDoubleFish)
            {
                b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(-8f, -56f)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, Trident.caughtFishId, 16, 16), Color.White, ((float)Math.PI * 4f / 5f), new Vector2(8f, 8f), 3f, SpriteEffects.None, (float)who.getStandingY() / 10000f + 0.002f + 0.058f);
            }

            if (ShouldSkipForDynamicReflections(dynamicReflectionsApi) is false)
            {
                string name = Game1.objectInformation[caughtFishId].Split('/')[4];
                b.DrawString(Game1.smallFont, name, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(26f - Game1.smallFont.MeasureString(name).X / 2f, -278f + yOffset)), Game1.textColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, (float)who.getStandingY() / 10000f + 0.002f + 0.06f);
                if (fishSize != -1)
                {
                    b.DrawString(Game1.smallFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14082"), Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(20f, -214f + yOffset)), Game1.textColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, (float)who.getStandingY() / 10000f + 0.002f + 0.06f);
                    b.DrawString(Game1.smallFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14083", (LocalizedContentManager.CurrentLanguageCode != 0) ? Math.Round((double)fishSize * 2.54) : ((double)fishSize)), Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(85f - Game1.smallFont.MeasureString(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14083", (LocalizedContentManager.CurrentLanguageCode != 0) ? Math.Round((double)fishSize * 2.54) : ((double)fishSize))).X / 2f, -179f + yOffset)), isRecordSizeFish ? (Color.Blue * Math.Min(1f, yOffset / 8f + 1.5f)) : Game1.textColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, (float)who.getStandingY() / 10000f + 0.002f + 0.06f);
                }

                if (caughtDoubleFish)
                {
                    Utility.drawTinyDigits(2, b, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(-120f, -284f + yOffset) + new Vector2(23f, 29f) * 4f), 3f, (float)who.getStandingY() / 10000f + 0.0001f + 0.061f, Color.White);
                }
            }
        }

        private static bool ShouldSkipForDynamicReflections(IDynamicReflectionsAPI api)
        {
            return api is not null && (api.IsFilteringWater() is true || api.IsFilteringPuddles() is true);
        }
    }
}
