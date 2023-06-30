/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Unlockable_Bundles.Lib.Enums;
using static StardewValley.BellsAndWhistles.ParrotUpgradePerch;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace Unlockable_Bundles.Lib.ShopTypes
{
    public class SpeechBubble : INetObject<NetFields>
    {
        public enum UpgradeState
        {
            Idle,
            PerformingAnimation,
            StartBuilding,
            Building,
            Complete
        }

        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        private ShopObject Shop;
        private Unlockable Unlockable;
        private bool ShowYesNoMenu { get => Shop.ShopType == ShopType.YesNoSpeechBubble || Shop.ShopType == ShopType.ParrotPerch; }
        public bool IsPlayerNearby;

        //TODO: Assing NetFields
        public NetFields NetFields => new NetFields();

        public float StateTimer;
        public float ShakeTime;
        public float CostShakeTime;
        public float SquawkTime;
        public float TimeUntilChomp;
        public float TimeUntilSqwawk;

        public NetEnum<UpgradeState> CurrentState = new NetEnum<UpgradeState>(UpgradeState.Idle);

        public ParrotUpgradePerch ParrotPerch = null;
        public List<Parrot> Parrots = new List<Parrot>();
        public bool ParrotPresent = true;
        public const int PARROT_COUNT = 24;

        public NetEvent0 AnimationEvent = new NetEvent0();
        public NetEvent0 UpgradeCompleteEvent = new NetEvent0();

        public KeyValuePair<string, int> NextRequirement;
        public string NextId;
        public int NextQuality;
        public StardewValley.Object NextObject;

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;
        }

        public SpeechBubble(ShopObject shop)
        {
            Shop = shop;
            Unlockable = shop.Unlockable;

            if (Shop.ShopType == ShopType.ParrotPerch)
                ParrotPerch = new ParrotUpgradePerch(Unlockable.getGameLocation(), Shop.TileLocation.ToPoint(), Unlockable.ParrotTarget, 1, null, null); ;

            assignNextItem();

            NetFields.AddFields(CurrentState, AnimationEvent.NetFields, UpgradeCompleteEvent.NetFields);
            AnimationEvent.onEvent += PerformAnimation;
            UpgradeCompleteEvent.onEvent += PerformCompleteAnimation;

            Helper.Events.GameLoop.ReturnedToTitle += returnedToTitle;
            Helper.Events.GameLoop.DayEnding += dayEnding;
            Helper.Events.GameLoop.OneSecondUpdateTicked += oneSecondUpdateTicked;
        }

        private void oneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (Game1.currentLocation.NameOrUniqueName == Unlockable.LocationUnique)
                UpdateEvenIfFarmerIsntHere();
        }

        private void dayEnding(object sender, DayEndingEventArgs e) => unsubscribeFromAllEvents();
        private void returnedToTitle(object sender, ReturnedToTitleEventArgs e) => unsubscribeFromAllEvents();

        private void unsubscribeFromAllEvents()
        { //If events aren't unsubscribed this object and associates will probably not be properly garbage collected. This is to prevent a expected memory leak
            Helper.Events.GameLoop.ReturnedToTitle -= returnedToTitle;
            Helper.Events.GameLoop.DayEnding -= dayEnding;
            Helper.Events.GameLoop.OneSecondUpdateTicked -= oneSecondUpdateTicked;
        }

        private void assignNextItem()
        {
            foreach (var req in Unlockable.Price)
                if (!Unlockable._alreadyPaid.ContainsKey(req.Key)) {
                    NextRequirement = req;
                    NextId = Unlockable.getFirstIDFromReqKey(req.Key);
                    NextQuality = Unlockable.getFirstQualityFromReqKey(req.Key);
                    NextObject = NextId == "money" ? null : new StardewValley.Object(int.Parse(NextId), req.Value, quality: NextQuality);
                    return;
                }

            ModData.setPurchased(Unlockable.ID, Unlockable.LocationUnique);
            //TODO: All requirements paid up
        }

        public bool IsAtTile(int x, int y)
        {
            if (Shop.TileLocation.X == x && Shop.TileLocation.Y == y)
                return CurrentState.Value == UpgradeState.Idle;

            return false;
        }
        public virtual void UpdateEvenIfFarmerIsntHere()
        {
            AnimationEvent.Poll();
            UpgradeCompleteEvent.Poll();
            if (!Game1.IsMasterGame)
                return;

            if (StateTimer > 0f)
                StateTimer--;

            if (CurrentState.Value == UpgradeState.StartBuilding && StateTimer <= 0f) {
                CurrentState.Value = UpgradeState.Building;
                StateTimer = 5f;
            }
            if (CurrentState.Value == UpgradeState.Building && this.StateTimer <= 0f) {
                //this.ApplyUpgrade();
                //TODO: Apply Map Patch Here
                CurrentState.Value = UpgradeState.Complete;
                Shop.Mutex.ReleaseLock();
                UpgradeCompleteEvent.Fire();
            }
        }

        public void updateWhenCurrentLocation(GameTime time, GameLocation environment)
        {
            updateTimers(time);
        }

        public void updateTimers(GameTime time)
        {
            if (ShakeTime > 0f)
                ShakeTime -= (float)time.ElapsedGameTime.TotalSeconds;

            if (CostShakeTime > 0f)
                CostShakeTime -= (float)time.ElapsedGameTime.TotalSeconds;

            if (SquawkTime > 0f)
                SquawkTime -= (float)time.ElapsedGameTime.TotalSeconds;

            if (TimeUntilSqwawk > 0f) {
                TimeUntilSqwawk -= (float)time.ElapsedGameTime.TotalSeconds;
                updateSquawk();
            }

            if (TimeUntilChomp > 0f) {
                TimeUntilChomp -= (float)time.ElapsedGameTime.TotalSeconds;
                updateChomp();
            }

            transformParrot();
            doPlayerNearbySqwawk();
        }

        public void doPlayerNearbySqwawk()
        {
            bool player_nearby = false;
            if (Math.Abs(Game1.player.getTileLocationPoint().X - Shop.TileLocation.X) <= 1 && Math.Abs(Game1.player.getTileLocationPoint().Y - Shop.TileLocation.Y) <= 1)
                player_nearby = true;

            if (player_nearby != IsPlayerNearby) {
                IsPlayerNearby = player_nearby;

                if (IsPlayerNearby && !Shop.Mutex.IsLocked() && CurrentState.Value == UpgradeState.Idle) {
                    SquawkTime = 0.5f;

                    if (Unlockable.InteractionSound != "")
                        Game1.playSound(Unlockable.InteractionSound);

                    if (Unlockable.InteractionShake) {
                        ShakeTime = 0.5f;
                        CostShakeTime = 0.5f;
                    }

                    if (NextId == "73")
                        Game1.specialCurrencyDisplay.ShowCurrency("walnuts");
                    else if (NextId == "858")
                        Game1.specialCurrencyDisplay.ShowCurrency("qiGems");
                } else {
                    Game1.specialCurrencyDisplay.ShowCurrency(null);
                }
            }
        }

        public void transformParrot()
        {
            if (!ParrotPresent || CurrentState.Value <= UpgradeState.StartBuilding)
                return;

            if (CurrentState.Value == UpgradeState.Building) {
                Parrot flying_parrot = new Parrot(ParrotPerch, Shop.TileLocation);
                flying_parrot.isPerchedParrot = true;
                Parrots.Add(flying_parrot);
            }
            ParrotPresent = false;
        }

        public void updateChomp()
        {
            if (TimeUntilChomp > 0f)
                return;

            Game1.playSound("eat");
            TimeUntilChomp = 0f;

            ShakeTime = 0.25f;
            if (ParrotPerch == null)
                Shop.shakeTimer = 500;

            if (Game1.currentLocation.getTemporarySpriteByID(98765) != null) {
                for (int j = 0; j < 6; j++) {
                    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(9, 252, 3, 3), Game1.currentLocation.getTemporarySpriteByID(98765).position + new Vector2(8f, 8f) * 4f, Game1.random.NextDouble() < 0.5, 0f, Color.White) {
                        motion = new Vector2(Game1.random.Next(-1, 2), -6f),
                        acceleration = new Vector2(0f, 0.25f),
                        rotationChange = (float)Game1.random.Next(-4, 5) * 0.05f,
                        scale = 4f,
                        animationLength = 1,
                        totalNumberOfLoops = 1,
                        interval = 500 + Game1.random.Next(500),
                        layerDepth = 1f,
                        drawAboveAlwaysFront = true
                    });
                }
            }
            Game1.currentLocation.removeTemporarySpritesWithID(98765f);
            TimeUntilSqwawk = 1f;
        }

        public void updateSquawk()
        {
            if (TimeUntilSqwawk > 0)
                return;

            //TODO: Check if all items were paid and the CurrentState needs to be changed to StartBuilding
            CurrentState.Value = UpgradeState.Idle;

            TimeUntilSqwawk = 0f;
            if (Unlockable.InteractionSound != "")
                Game1.playSound(Unlockable.InteractionSound);

            SquawkTime = 0.5f;
            ShakeTime = 0.5f;
            if (ParrotPerch == null)
                Shop.shakeTimer = 500;
        }

        public void interact(Farmer who)
        {
            if (StateTimer > 0)
                return; //The game seems to fire this event twice, it won't happen later with the MuteX, but I'll use this workaround for now while developing

            if (Unlockable._price.Pairs.All(e => Unlockable._alreadyPaid.ContainsKey(e.Key)))
                return; //This shop is already completed //TODO: Maybe add shake and NOPE sound?

            //TODO: Handle Mutex stuff, use second requestlock action parameter to set canMove true
            //Game1.player.canMove = false;
            //if (Shop.Mutex.RequestLock())

            if (ShowYesNoMenu)
                showYesNo(who);
            else //TODO: Check if the relevant item is being held
                tryDonate(who);

            //if (this.IsAtTile(tile_location.X, tile_location.Y) && this.IsAvailable()) {
            //    string request_text = Game1.content.LoadStringReturnNullIfNotFound("Strings\\UI:UpgradePerch_" + this.upgradeName.Value);
            //    GameLocation location = this.locationRef.Value;
            //    if (request_text != null && location != null) {
            //        request_text = string.Format(request_text, this.requiredNuts.Value);
            //        this.costShakeTime = 0.5f;
            //        this.squawkTime = 0.5f;
            //        this.shakeTime = 0.5f;
            //        if (this.locationRef.Value == Game1.currentLocation) {
            //            Game1.playSound("parrot_squawk");
            //        }
            //        if ((int)Game1.netWorldState.Value.GoldenWalnuts >= this.requiredNuts.Value) {
            //            location.createQuestionDialogue(request_text, location.createYesNoResponses(), "UpgradePerch_" + this.upgradeName.Value);
            //        } else {
            //            Game1.drawDialogueNoTyping(request_text);
            //        }
            //    } else if ((int)Game1.netWorldState.Value.GoldenWalnuts >= this.requiredNuts.Value) {
            //        this.AttemptConstruction();
            //    } else {
            //        this.ShowInsufficientNuts();
            //    }
            //    return true;
            //}
            //return false;

            Shop.Mutex.ReleaseLock();
        }
        public bool tryDonate(Farmer who)
        {
            if (!Inventory.hasEnoughItems(who, NextRequirement)) {
                CostShakeTime = 0.5f;
                SquawkTime = 0.5f;
                ShakeTime = 0.5f;
                if (ParrotPerch == null)
                    Shop.shakeTimer = 500;

                if (Unlockable.InteractionSound != "")
                    Game1.playSound(Unlockable.InteractionSound);
                return false;
            }

            Inventory.removeItemsOfRequirement(who, NextRequirement);
            Unlockable._alreadyPaid.Add(NextRequirement.Key, NextRequirement.Value);
            AnimationEvent.Fire();
            assignNextItem(); //TODO: Race condition if animationevent is async
            return true;
        }

        public void showYesNo(Farmer who)
        {
            var question = Unlockable.getTranslatedShopDescription();
            question = question.Replace("{{item}}", NextId == "money" ? Helper.Translation.Get("ub_parrot_money") : NextObject.DisplayName);

            Game1.currentLocation.afterQuestion = exitYesNo;
            var yesNo = Game1.currentLocation.createYesNoResponses();
            Game1.activeClickableMenu = new DialogueBox(question, yesNo.ToList());
        }

        public void exitYesNo(Farmer who, string whichAnswer)
        {
            if (whichAnswer == "No")
                return; //TODO: Maybe sqwawk here and release mutex

            tryDonate(who);
        }

        public virtual void PerformAnimation()
        {
            CurrentState.Value = UpgradeState.PerformingAnimation;
            StateTimer = 3f;
            if (Game1.currentLocation.NameOrUniqueName == Unlockable.LocationUnique) {
                if (Unlockable.InteractionSound != "")
                    Game1.playSound(Unlockable.InteractionSound);

                Parrots.Clear();
                ParrotPresent = true;
                var textureName = NextId == "money" ? "LooseSprites\\Cursors" : "Maps\\springobjects";
                var sourceRectangle = NextId == "money" ? new Rectangle(280, 412, 15, 14) : Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, int.Parse(NextId), 16, 16);

                Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(textureName, sourceRectangle, 2000f, 1, 0, (Shop.TileLocation + new Vector2(0.25f, -2.5f)) * 64f, flicker: false, flipped: false, (float)(Shop.TileLocation.Y * 64 + 1) / 10000f, 0f, Color.White, 4f, -0.015f, 0f, 0f) {
                    motion = new Vector2(-0.1f, -7f),
                    acceleration = new Vector2(0f, 0.25f),
                    id = 98765f,
                    drawAboveAlwaysFront = true
                });
                Game1.playSound("dwop");
                if (Shop.Mutex.IsLockHeld())
                    Game1.player.freezePause = 3000;

                TimeUntilChomp = Unlockable.TimeUntilChomp;
                SquawkTime = Unlockable.TimeUntilChomp;
            }
        }

        public virtual void PerformCompleteAnimation()
        {
            //TODO: ?

            /*
            if (this.upgradeName.Contains("Volcano")) {
                for (int j = 0; j < 16; j++) {
                    this.locationRef.Value.temporarySprites.Add(new TemporaryAnimatedSprite(5, Utility.getRandomPositionInThisRectangle(this.upgradeRect, Game1.random) * 64f, Color.White) {
                        motion = new Vector2(Game1.random.Next(-1, 2), -1f),
                        scale = 1f,
                        layerDepth = 1f,
                        drawAboveAlwaysFront = true,
                        delayBeforeAnimationStart = j * 15
                    });
                    TemporaryAnimatedSprite t2 = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(65, 229, 16, 6), Utility.getRandomPositionInThisRectangle(this.upgradeRect, Game1.random) * 64f, Game1.random.NextDouble() < 0.5, 0f, Color.White) {
                        motion = new Vector2(Game1.random.Next(-2, 3), -16f),
                        acceleration = new Vector2(0f, 0.5f),
                        rotationChange = (float)Game1.random.Next(-4, 5) * 0.05f,
                        scale = 4f,
                        animationLength = 1,
                        totalNumberOfLoops = 1,
                        interval = 1000 + Game1.random.Next(500),
                        layerDepth = 1f,
                        drawAboveAlwaysFront = true,
                        yStopCoordinate = (this.upgradeRect.Bottom + 1) * 64
                    };
                    t2.reachedStopCoordinate = t2.bounce;
                    this.locationRef.Value.TemporarySprites.Add(t2);
                    t2 = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(65, 229, 16, 6), Utility.getRandomPositionInThisRectangle(this.upgradeRect, Game1.random) * 64f, Game1.random.NextDouble() < 0.5, 0f, Color.White) {
                        motion = new Vector2(Game1.random.Next(-2, 3), -16f),
                        acceleration = new Vector2(0f, 0.5f),
                        rotationChange = (float)Game1.random.Next(-4, 5) * 0.05f,
                        scale = 4f,
                        animationLength = 1,
                        totalNumberOfLoops = 1,
                        interval = 1000 + Game1.random.Next(500),
                        layerDepth = 1f,
                        drawAboveAlwaysFront = true,
                        yStopCoordinate = (this.upgradeRect.Bottom + 1) * 64
                    };
                    t2.reachedStopCoordinate = t2.bounce;
                    this.locationRef.Value.TemporarySprites.Add(t2);
                }
                if (this.locationRef.Value == Game1.currentLocation) {
                    Game1.flashAlpha = 1f;
                    Game1.playSound("boulderBreak");
                }
            } else if (this.upgradeName == "House") {
                for (int i = 0; i < 16; i++) {
                    this.locationRef.Value.temporarySprites.Add(new TemporaryAnimatedSprite(5, Utility.getRandomPositionInThisRectangle(this.upgradeRect, Game1.random) * 64f, Color.White) {
                        motion = new Vector2(Game1.random.Next(-1, 2), -1f),
                        scale = 1f,
                        layerDepth = 1f,
                        drawAboveAlwaysFront = true,
                        delayBeforeAnimationStart = i * 15
                    });
                    TemporaryAnimatedSprite t = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(49 + 16 * Game1.random.Next(3), 229, 16, 6), Utility.getRandomPositionInThisRectangle(this.upgradeRect, Game1.random) * 64f, Game1.random.NextDouble() < 0.5, 0f, Color.White) {
                        motion = new Vector2(Game1.random.Next(-2, 3), -16f),
                        acceleration = new Vector2(0f, 0.5f),
                        rotationChange = (float)Game1.random.Next(-4, 5) * 0.05f,
                        scale = 4f,
                        animationLength = 1,
                        totalNumberOfLoops = 1,
                        interval = 1000 + Game1.random.Next(500),
                        layerDepth = 1f,
                        drawAboveAlwaysFront = true,
                        yStopCoordinate = (this.upgradeRect.Bottom + 1) * 64
                    };
                    t.reachedStopCoordinate = t.bounce;
                    this.locationRef.Value.TemporarySprites.Add(t);
                    t = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(49 + 16 * Game1.random.Next(3), 229, 16, 6), Utility.getRandomPositionInThisRectangle(this.upgradeRect, Game1.random) * 64f, Game1.random.NextDouble() < 0.5, 0f, Color.White) {
                        motion = new Vector2(Game1.random.Next(-2, 3), -16f),
                        acceleration = new Vector2(0f, 0.5f),
                        rotationChange = (float)Game1.random.Next(-4, 5) * 0.05f,
                        scale = 4f,
                        animationLength = 1,
                        totalNumberOfLoops = 1,
                        interval = 1000 + Game1.random.Next(500),
                        layerDepth = 1f,
                        drawAboveAlwaysFront = true,
                        yStopCoordinate = (this.upgradeRect.Bottom + 1) * 64
                    };
                    t.reachedStopCoordinate = t.bounce;
                    this.locationRef.Value.TemporarySprites.Add(t);
                }
                if (this.locationRef.Value == Game1.currentLocation) {
                    Game1.flashAlpha = 1f;
                    Game1.playSound("boulderBreak");
                }
            } else if ((this.upgradeName == "Resort" || this.upgradeName == "Trader" || this.upgradeName == "Obelisk") && this.locationRef.Value == Game1.currentLocation) {
                Game1.flashAlpha = 1f;
            }
            if (this.locationRef.Value == Game1.currentLocation && this.upgradeName != "Hut") {
                DelayedAction.playSoundAfterDelay("secret1", 800);
            }
            */
        }

        public void draw(SpriteBatch b)
        {
            drawSpeechBubble(b);

            drawPerchParrot(b);

            foreach (var parrot in Parrots)
                parrot.Draw(b);
        }

        public void drawSpeechBubble(SpriteBatch b)
        {
            if (!IsPlayerNearby || CurrentState.Value > UpgradeState.Idle)
                return;

            Vector2 offset = Unlockable.SpeechBubbleOffset;
            if (CostShakeTime > 0f)
                offset += new Vector2(Utility.RandomFloat(-0.5f, 0.5f) * 4f);

            float yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2) - 72f;
            Vector2 draw_position = Shop.TileLocation;
            float draw_layer = draw_position.Y * 64f / 10000f;
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(draw_position.X * 64f, draw_position.Y * 64f - 96f - 48f + yOffset)) + offset, new Rectangle(141, 465, 20, 24), Color.White * 0.75f, 0f, Vector2.Zero, 4f, SpriteEffects.None, draw_layer + 1E-06f);
            Vector2 item_draw_position = Game1.GlobalToLocal(Game1.viewport, new Vector2(draw_position.X * 64f + 8f, draw_position.Y * 64f - 64f - 62f - 8f + yOffset)) + offset;
            if (NextId == "money")
                UtilityMisc.drawMoneyKiloFormat(b, NextRequirement.Value, (int)item_draw_position.X, (int)item_draw_position.Y, Color.White);
            else
                NextObject.drawInMenu(b, item_draw_position, 1f);

        }

        public void drawPerchParrot(SpriteBatch b)
        {
            if (ParrotPerch == null)
                return;

            int num = 0;
            Vector2 zero = Vector2.Zero;
            if (SquawkTime > 0f)
                num = 1;

            if (ShakeTime > 0f)
                zero = new Vector2(Utility.RandomFloat(-0.5f, 0.5f) * 4f);

            b.Draw(ParrotPerch.texture, Game1.GlobalToLocal(Game1.viewport, (Shop.TileLocation + new Vector2(0.5f, -1f)) * 64f) + zero, new Rectangle(num * 24, 0, 24, 24), Color.White, 0f, new Vector2(12f, 16f), 4f, SpriteEffects.None, ((Shop.TileLocation.Y + 1f) * 64f - 1f) / 10000f);
        }
    }

}

