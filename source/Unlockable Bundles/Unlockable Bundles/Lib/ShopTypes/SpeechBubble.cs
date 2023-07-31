/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-bundles
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
        public NetFields NetFields { get; } = new NetFields();

        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        private NetRef<ParrotUpgradePerch> _parrotPerch = new NetRef<ParrotUpgradePerch>();
        public NetEnum<UpgradeState> CurrentState = new NetEnum<UpgradeState>(UpgradeState.Idle);

        public ShopObject Shop;
        public Unlockable Unlockable { get => Shop.Unlockable; }

        public bool IsPlayerNearby;

        public float StateTimer;
        public float ShakeTime;
        public float CostShakeTime;
        public float SquawkTime;
        public float TimeUntilChomp;
        public float TimeUntilSqwawk;
        public float NextParrotSpawn;

        public ParrotUpgradePerch ParrotPerch { get => _parrotPerch.Value; set => _parrotPerch.Value = value; }
        public List<Parrot> Parrots = new List<Parrot>();
        public bool ParrotPresent = true;
        public const int PARROT_COUNT = 24;

        public NetEvent0 AnimationEvent = new NetEvent0();
        public NetEvent0 UpgradeCompleteEvent = new NetEvent0();
        public bool WaitingForProcessShopEvent = false;

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
        private void addNetFieldsAndEvents()
        {
            NetFields.AddFields(CurrentState, AnimationEvent, UpgradeCompleteEvent, _parrotPerch);

            Helper.Events.GameLoop.ReturnedToTitle += returnedToTitle;
            Helper.Events.GameLoop.DayEnding += dayEnding;
            Helper.Events.GameLoop.UpdateTicked += updateTicked;

            AnimationEvent.onEvent += PerformAnimation;
            UpgradeCompleteEvent.onEvent += PerformCompleteAnimation;
        }

        private void updateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Shop != null)
                UpdateEvenIfFarmerIsntHere(Game1.currentGameTime);
        }

        public SpeechBubble() => addNetFieldsAndEvents();

        public SpeechBubble(ShopObject shop)
        {
            Shop = shop;

            addNetFieldsAndEvents();
            NetFields.Parent = shop.NetFields;

            if (shop.ShopType == ShopType.ParrotPerch) {
                ParrotPerch = new ParrotUpgradePerch(shop.Unlockable.getGameLocation(), shop.TileLocation.ToPoint(), shop.Unlockable.ParrotTarget, 1, null, null);

                if(Unlockable.ParrotTexture != "")
                    ParrotPerch.texture = Helper.GameContent.Load<Texture2D>(Unlockable.ParrotTexture);
            }
                


            assignNextItem();
        }

        private void dayEnding(object sender, DayEndingEventArgs e) => unsubscribeFromAllEvents();
        private void returnedToTitle(object sender, ReturnedToTitleEventArgs e) => unsubscribeFromAllEvents();

        private void unsubscribeFromAllEvents()
        { //If events aren't unsubscribed this object and associates will probably not be properly garbage collected. This is to prevent a expected memory leak
            Helper.Events.GameLoop.ReturnedToTitle -= returnedToTitle;
            Helper.Events.GameLoop.DayEnding -= dayEnding;
            Helper.Events.GameLoop.UpdateTicked -= updateTicked;
        }

        public void assignNextItem()
        {
            foreach (var req in Unlockable._price.Pairs)
                if (!Unlockable._alreadyPaid.ContainsKey(req.Key)) {
                    NextRequirement = req;
                    NextId = Unlockable.getFirstIDFromReqKey(req.Key);
                    NextQuality = Unlockable.getFirstQualityFromReqKey(req.Key);
                    NextObject = NextId == "money" ? null : new StardewValley.Object(Unlockable.intParseID(NextId), req.Value, quality: NextQuality);
                    return;
                }

            ModData.setPurchased(Unlockable.ID, Unlockable.LocationUnique);
        }

        public bool IsAtTile(int x, int y)
        {
            if (Shop.TileLocation.X == x && Shop.TileLocation.Y == y)
                return CurrentState.Value == UpgradeState.Idle;

            return false;
        }
        public virtual void UpdateEvenIfFarmerIsntHere(GameTime time)
        {
            AnimationEvent.Poll();
            UpgradeCompleteEvent.Poll();

            if (Game1.IsMasterGame)
                UpdateState(time);

            if (WaitingForProcessShopEvent && CurrentState.Value == UpgradeState.Complete) {
                WaitingForProcessShopEvent = false;
                Unlockable.processShopEvent();

                Shop.Mutex.ReleaseLock();
                UpgradeCompleteEvent.Fire();
            }
        }

        private void UpdateState(GameTime time)
        {
            if (StateTimer > 0f)
                StateTimer -= (float)time.ElapsedGameTime.TotalSeconds;

            if (CurrentState.Value == UpgradeState.StartBuilding && StateTimer <= 0f) {
                CurrentState.Value = UpgradeState.Building;
                StateTimer = Unlockable.ShopType == ShopType.ParrotPerch ? 5f : 0f;
            }

            if (CurrentState.Value == UpgradeState.Building && StateTimer <= 0f) {
                if (ParrotPerch != null)
                    ParrotPerch.currentState.Value = UpgradeState.Complete;

                CurrentState.Value = UpgradeState.Complete;
            }
        }

        public void updateWhenCurrentLocation(GameTime time, GameLocation environment)
        {
            updateTimers(time);
            updateParrots(time);
        }

        public void updateParrots(GameTime time)
        {
            if (Unlockable.ShopType != ShopType.ParrotPerch)
                return;

            if (CurrentState == UpgradeState.Building && Parrots.Count < 24) {
                if (NextParrotSpawn > 0f)
                    NextParrotSpawn -= (float)time.ElapsedGameTime.TotalSeconds;

                if (NextParrotSpawn <= 0f) {
                    NextParrotSpawn = 0.05f;
                    Rectangle spawn_rectangle = Unlockable.ParrotTarget;
                    spawn_rectangle.Inflate(5, 0);
                    Parrots.Add(new Parrot(ParrotPerch, Utility.getRandomPositionInThisRectangle(spawn_rectangle, Game1.random), Parrots.Count % 10 == 0));
                }
            }
            for (int i = 0; i < Parrots.Count; i++)
                if (Parrots[i].Update(time))
                    Parrots.RemoveAt(i--);
                else if (Parrots[i].isPerchedParrot)
                    //The game resets the birdType to 0 at every update for no reason
                    Helper.Reflection.GetField<int>(Parrots[i], "birdType").SetValue(Unlockable.ParrotIndex);
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
            if (Unlockable.ShopType != ShopType.ParrotPerch || !ParrotPresent || CurrentState.Value <= UpgradeState.StartBuilding)
                return;

            if (CurrentState.Value == UpgradeState.Building) {
                Parrot flying_parrot = new Parrot(ParrotPerch, Shop.TileLocation);
                flying_parrot.isPerchedParrot = true;
                Helper.Reflection.GetField<int>(flying_parrot, "birdType").SetValue(Unlockable.ParrotIndex);
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
                Shop.shakeTimer = 250;

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

            CurrentState.Value = UpgradeState.Idle;

            var allRequirementsPaid = Unlockable.allRequirementsPaid();

            if (allRequirementsPaid && Shop.Mutex.IsLockHeld()) {
                Unlockable.processPurchase();
                WaitingForProcessShopEvent = true;
            }

            if (Shop.Mutex.IsLockHeld())
                prepareExit();

            if (allRequirementsPaid)
                CurrentState.Value = UpgradeState.StartBuilding;

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
            if (Unlockable.allRequirementsPaid())
                return;

            Game1.player.canMove = false;
            showYesNo(who);
        }

        public void prepareExit()
        {
            Shop.Mutex.ReleaseLock();
            Game1.player.canMove = true;
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

                prepareExit();
                return false;
            }

            Inventory.removeItemsOfRequirement(who, NextRequirement);
            Unlockable.processContribution(NextRequirement);
            var displayName = NextId == "money" ? NextRequirement.Value.ToString("# ### ##0").TrimStart() + "g" : NextObject.DisplayName;
            Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue().globalChatInfoMessage("BundleDonate", Game1.player.displayName, displayName);
            AnimationEvent.Fire();
            return true;
        }

        public void showYesNo(Farmer who)
        {
            var question = Unlockable.getTranslatedShopDescription();
            var moneyString = Unlockable.ShopType == ShopType.ParrotPerch ? Helper.Translation.Get("ub_parrot_money") : Helper.Translation.Get("ub_speech_money");

            question = question.Replace("{{item}}", NextId == "money" ? moneyString : NextObject.DisplayName);

            Game1.currentLocation.afterQuestion = exitYesNo;
            var yesNo = Game1.currentLocation.createYesNoResponses();
            Game1.activeClickableMenu = new DialogueBox(question, yesNo.ToList());
        }

        public void exitYesNo(Farmer who, string whichAnswer)
        {
            if (whichAnswer == "No") {
                prepareExit();
                return;
            }

            tryDonate(who);
        }

        public virtual void PerformAnimation()
        {
            StateTimer = 3f;

            PerformAnimationLocal();
            assignNextItem();
        }

        private void PerformAnimationLocal()
        {
            if (Game1.currentLocation.NameOrUniqueName != Unlockable.LocationUnique)
                return;

            if (Unlockable.InteractionSound != "")
                Game1.playSound(Unlockable.InteractionSound);

            Parrots.Clear();
            ParrotPresent = true;
            var textureName = NextId == "money" ? "LooseSprites\\Cursors" : "Maps\\springobjects";
            var sourceRectangle = NextId == "money" ? new Rectangle(280, 412, 15, 14) : Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, Unlockable.intParseID(NextId), 16, 16);

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

        public virtual void PerformCompleteAnimation()
        {
            //No flash, because, honestly, it is just annoying
            if (Unlockable.LocationUnique == Game1.currentLocation.NameOrUniqueName)
                DelayedAction.playSoundAfterDelay("secret1", 800);
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

            //Speechbubble
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(draw_position.X * 64f, draw_position.Y * 64f - 96f - 48f + yOffset)) + offset, new Rectangle(141, 465, 20, 24), Color.White * 0.75f, 0f, Vector2.Zero, 4f, SpriteEffects.None, draw_layer + 1E-06f);

            Vector2 item_draw_position = Game1.GlobalToLocal(Game1.viewport, new Vector2(draw_position.X * 64f + 8f, draw_position.Y * 64f - 64f - 62f - 8f + yOffset)) + offset;
            if (NextId == "money")
                UtilityMisc.drawMoneyKiloFormat(b, NextRequirement.Value, (int)item_draw_position.X, (int)item_draw_position.Y, Color.White);
            else
                NextObject.drawInMenu(b, item_draw_position, 1f);

        }

        public void drawPerchParrot(SpriteBatch b)
        {
            if (ParrotPerch == null || !ParrotPresent || CurrentState == UpgradeState.Complete)
                return;

            int num = 0;
            Vector2 zero = Vector2.Zero;
            if (SquawkTime > 0f)
                num = 1;

            if (ShakeTime > 0f)
                zero = new Vector2(Utility.RandomFloat(-0.5f, 0.5f) * 4f);

            b.Draw(ParrotPerch.texture, Game1.GlobalToLocal(Game1.viewport, (Shop.TileLocation + new Vector2(0.5f, -1f)) * 64f) + zero, new Rectangle(num * 24, 24 * Unlockable.ParrotIndex, 24, 24), Color.White, 0f, new Vector2(12f, 16f), 4f, SpriteEffects.None, ((Shop.TileLocation.Y + 1f) * 64f - 1f) / 10000f);
        }
    }

}

