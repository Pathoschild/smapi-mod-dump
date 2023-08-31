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
using StardewModdingAPI;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley.Tools;
using Unlockable_Bundles.Lib.ShopTypes;
using Netcode;
using System.Xml.Serialization;
using Unlockable_Bundles.Lib.Enums;
using StardewValley.Network;

namespace Unlockable_Bundles.Lib
{
    public class ShopObject : StardewValley.Object
    {
        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.SetOwner(this)
                 .AddField(_unlockable, "_unlockable")
                 .AddField(Mutex.NetFields, "Mutex.NetFields")
                 .AddField(_speechBubble, "_speechBubble");
        }

        public readonly NetRef<Unlockable> _unlockable = new NetRef<Unlockable>();
        public readonly NetMutex Mutex = new NetMutex();
        public Unlockable Unlockable { get => _unlockable.Value; }
        public ShopType ShopType { get => _unlockable.Value.ShopType; }
        private NetRef<SpeechBubble> _speechBubble = new NetRef<SpeechBubble>();
        public SpeechBubble SpeechBubble { get => _speechBubble.Value; set => _speechBubble.Value = value; }
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;
        private Texture2D Texture;

        private int AnimationFrame = 0;
        private long AnimationTimer = 0;
        private List<KeyValuePair<int, int>> AnimationSequence = new List<KeyValuePair<int, int>>();  //ImageIndex, Tempo

        public bool IsPlayerNearby;

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;
        }

        public ShopObject() { }

        public ShopObject(Vector2 tileLocation, Unlockable unlockable)
        {
            _unlockable.Set(unlockable);
            if (ShopType == ShopType.SpeechBubble || ShopType == ShopType.ParrotPerch)
                SpeechBubble = new SpeechBubble(this);

            IsSpawnedObject = false;

            TileLocation = tileLocation;
            boundingBox.Value = new Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64);

            bigCraftable.Value = true;
            Name = "UnlockableBundles Shop";
            Type = "Crafting";
        }

        public override bool isPassable() => isTemporarilyInvisible;
        public override bool isActionable(Farmer who) => !isTemporarilyInvisible;
        public override bool onExplosion(Farmer who) => false;

        public override void actionOnPlayerEntry()
        {
            if (SpeechBubble != null && SpeechBubble.Shop == null) {
                SpeechBubble.Shop = this;
                SpeechBubble.assignNextItem();
            }
        }

        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            Mutex.RequestLock(delegate { openMenu(who); });

            return false;
        }

        public void openMenu(Farmer who)
        {
            switch (ShopType) {
                case ShopType.Dialogue:
                    if (Unlockable.allRequirementsPaid()) {
                        Mutex.ReleaseLock();
                        break;
                    }                   

                    Game1.activeClickableMenu = new DialogueShopMenu(who, Unlockable);
                    Game1.activeClickableMenu.exitFunction = delegate { Mutex.ReleaseLock(); };
                    break;

                case ShopType.CCBundle or ShopType.AltCCBundle:
                    Game1.activeClickableMenu = new BundleMenu(who, Unlockable, ShopType);
                    Game1.activeClickableMenu.exitFunction = delegate { Mutex.ReleaseLock(); };
                    break;

                case ShopType.SpeechBubble or ShopType.ParrotPerch:
                    SpeechBubble.interact(who);
                    break;

            }
        }

        public override void draw(SpriteBatch b, int x, int y, float alpha = 1)
        {
            Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + (float)(shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), y * 64 - 64));

            var sourceRectangle = new Rectangle(0, 0, 32, 64);
            Vector2 origin = new Vector2(0f, 0f);

            if (Texture == null && Unlockable.ShopTexture.ToLower() != "none") {
                Texture = Helper.GameContent.Load<Texture2D>(Unlockable.ShopTexture);
                resetAnimationFrames();
            }

            if (AnimationSequence.Count > 0)
                sourceRectangle.X = sourceRectangle.Width * AnimationSequence.ElementAt(AnimationFrame).Key;

            if (Texture != null)
                b.Draw(Texture, position, sourceRectangle, Color.White, 0f, origin, 2f, Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, boundingBox.Bottom / 10000f);

            if (SpeechBubble != null)
                SpeechBubble.draw(b);

            if (Unlockable.DrawQuestionMark)
                drawQuestionMark(b, position);
        }

        public void drawQuestionMark(SpriteBatch b, Vector2 position)
        {
            if (Unlockable._completed.Value)
                return;

            float yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2) - 32f;

            b.Draw(Game1.mouseCursors2, position + Unlockable.QuestionMarkOffset + new Vector2(0, yOffset), new Rectangle(114, 53, 6, 10), Color.White, 0f, new Vector2(1f, 4f), 4f, SpriteEffects.None, 1f);

        }

        public void resetAnimationFrames()
        {
            if (Unlockable.ShopAnimation == null || Unlockable.ShopAnimation == "")
                return;

            AnimationFrame = 0;
            AnimationSequence.Clear();

            var currentTempo = 100;
            foreach (var entry in Unlockable.ShopAnimation.Split(",")) {
                var tempoSplit = entry.Split("@");

                if (tempoSplit.Count() > 1)
                    currentTempo = int.Parse(tempoSplit.Last());

                var framesSplit = tempoSplit.First().Split("-");

                var from = int.Parse(framesSplit.First());
                var to = int.Parse(framesSplit.Last());

                if (from < to)
                    for (int i = from; i <= to; i++)
                        AnimationSequence.Add(new KeyValuePair<int, int>(i, currentTempo));
                else
                    for (int i = from; i >= to; i--)
                        AnimationSequence.Add(new KeyValuePair<int, int>(i, currentTempo));

            }
        }

        public override void updateWhenCurrentLocation(GameTime time)
        {
            if (shakeTimer > 0)
                shakeTimer -= time.ElapsedGameTime.Milliseconds;

            updateAnimation(time);
            if (SpeechBubble != null)
                SpeechBubble.updateWhenCurrentLocation(time);

            Mutex.Update(Game1.getOnlineFarmers());
            checkPlayerNearby();
        }

        public void checkPlayerNearby()
        {
            bool player_nearby = false;
            if (Math.Abs(Game1.player.TilePoint.X - TileLocation.X) <= 1 && Math.Abs(Game1.player.TilePoint.Y - TileLocation.Y) <= 1)
                player_nearby = true;

            if (player_nearby == IsPlayerNearby)
                return;

            IsPlayerNearby = player_nearby;

            if (SpeechBubble != null || !IsPlayerNearby || Mutex.IsLocked())
                return;

            if (Unlockable.InteractionSound != "")
                Game1.playSound(Unlockable.InteractionSound);

            if (Unlockable.InteractionShake)
                shakeTimer = 500;

        }

        public void updateAnimation(GameTime time)
        {
            if (AnimationSequence.Count == 0)
                return;

            if (AnimationTimer > 0)
                AnimationTimer -= time.ElapsedGameTime.Milliseconds;

            if (AnimationTimer <= 0) {
                AnimationFrame++;
                if (AnimationFrame >= AnimationSequence.Count)
                    AnimationFrame = 0;

                AnimationTimer = AnimationSequence.ElementAt(AnimationFrame).Value;
            }
        }

        public override bool performToolAction(Tool t)
        {
            if (t is Pickaxe or Axe) {
                shakeTimer = 100;
                Game1.playSound("hammer");
            }
            return false;
        }
    }
}
