/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/unlockable-bundles
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
using static Unlockable_Bundles.ModEntry;
using Unlockable_Bundles.NetLib;

namespace Unlockable_Bundles.Lib
{
    //ShopObject is the bundle object the player interacts with
    public class ShopObject : StardewValley.Object
    {
        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.SetOwner(this)
                 .AddField(_unlockable, "_unlockable")
                 .AddField(Mutex.NetFields, "Mutex.NetFields")
                 .AddField(_speechBubble, "_speechBubble")
                 .AddField(_wasDiscovered, "WasDiscovered");
        }

        public readonly NetRef<Unlockable> _unlockable = new();
        public readonly NetMutex Mutex = new();
        public Unlockable Unlockable { get => _unlockable.Value; }
        public ShopType ShopType { get => _unlockable.Value.ShopType; }
        private NetRef<SpeechBubble> _speechBubble = new();
        public SpeechBubble SpeechBubble { get => _speechBubble.Value; set => _speechBubble.Value = value; }

        public AnimatedTexture ShopTexture;
        public AnimatedTexture OverviewTexture;

        public bool IsPlayerNearby;
        private NetBool _wasDiscovered = new NetBool();
        public bool WasDiscovered
        {
            get {
                return _wasDiscovered.Value;
            }
            set => _wasDiscovered.Value = value;
        }
        public static Texture2D BundleDiscoveredAnimation;
        public static TemporaryAnimatedSpriteList TemporaryAnimatedSprites = new();
        private bool TexturesWereSet = false; //Relevant for multiplayer where Unlockable isn't set when creating the object
        public static void Initialize()
        {
            BundleDiscoveredAnimation = Helper.GameContent.Load<Texture2D>("UnlockableBundles/UI/BundleDiscoveredAnimation");
            Helper.Events.Display.Rendered += drawTemporaryAnimatedSprites;
        }
        public ShopObject()
        {
            ItemId = "DLX.Bundles.ShopObject";
            setEvents();
        }

        public ShopObject(Unlockable unlockable)
        {
            ItemId = "DLX.Bundles.ShopObject";
            _unlockable.Set(unlockable);
            setTextures();
        }

        public ShopObject(Vector2 tileLocation, Unlockable unlockable)
        {
            ItemId = "DLX.Bundles.ShopObject";
            _unlockable.Set(unlockable);
            if (ShopType == ShopType.SpeechBubble || ShopType == ShopType.ParrotPerch)
                SpeechBubble = new SpeechBubble(this);

            IsSpawnedObject = false;

            TileLocation = tileLocation;
            boundingBox.Value = new Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64);

            bigCraftable.Value = true;
            Name = "UnlockableBundles Shop";
            Type = "Crafting";

            WasDiscovered = ModData.getDiscovered(unlockable.ID, Unlockable.LocationUnique);
            setEvents();
            setTextures();
        }

        private void setEvents()
        {
            _wasDiscovered.fieldChangeEvent += _wasDiscovered_fieldChangeEvent;

            Helper.Events.GameLoop.ReturnedToTitle += returnedToTitle;
            Helper.Events.GameLoop.DayEnding += dayEnding;
        }

        private void setTextures()
        {
            TexturesWereSet = true;

            if (Unlockable.ShopTexture.Trim().ToLower() != "none") {
                var texture = Helper.GameContent.Load<Texture2D>(Unlockable.ShopTexture);
                ShopTexture = new AnimatedTexture(texture, Unlockable.ShopAnimation, Unlockable.ShopTextureWidth, Unlockable.ShopTextureHeight);
            }

            if (Unlockable.OverviewTexture is not null && Unlockable.OverviewTexture.Trim() != "") {
                var texture = Helper.GameContent.Load<Texture2D>(Unlockable.OverviewTexture);
                OverviewTexture = new AnimatedTexture(texture, Unlockable.OverviewAnimation, Unlockable.OverviewTextureWidth, Unlockable.OverviewTextureWidth * 2);
            }

            if (Unlockable.ParrotTexture != "" && Unlockable.ShopType == ShopType.ParrotPerch)
                SpeechBubble.ParrotPerch.texture = Helper.GameContent.Load<Texture2D>(Unlockable.ParrotTexture);
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
            if (Game1.activeClickableMenu is not null || justCheckingForActivity)
                return false;

            if (!WasDiscovered)
                WasDiscovered = true;

            Mutex.RequestLock(delegate { openMenu(who); });

            return true;
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

                case ShopType.CCBundle:
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

            position += Unlockable.ShopDrawOffset;

            if (!TexturesWereSet)
                setTextures();

            if (ShopTexture is not null)
                b.Draw(ShopTexture.Texture, new Rectangle(position.ToPoint(), Unlockable.ShopDrawDimensions.ToPoint()), ShopTexture.getOffsetRectangle(), Unlockable.ShopColor, 0f, new Vector2(), Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, boundingBox.Bottom / 10000f);

            if (SpeechBubble != null)
                SpeechBubble.draw(b);

            if (Unlockable.DrawQuestionMark)
                drawQuestionMark(b, position);
        }

        public new void drawInMenu(SpriteBatch b, Vector2 position, float scale)
        {
            Rectangle sourceRectangle;
            Texture2D texture;
            var color = Color.White;

            if (!TexturesWereSet)
                setTextures();

            if (OverviewTexture is not null) {
                texture = OverviewTexture.Texture;
                sourceRectangle = OverviewTexture.getOffsetRectangle();
                OverviewTexture.update(Game1.currentGameTime);
                scale *= 32f / Unlockable.OverviewTextureWidth;
                color = Unlockable.OverviewColor;

            } else if (ShopType == ShopType.ParrotPerch) {
                texture = SpeechBubble.ParrotPerch.texture;
                sourceRectangle = new Rectangle(0, 24 * Unlockable.ParrotIndex, 24, 24);
                position.Y += 8 * scale;
                position.X -= 8 * scale;
                scale *= 2f;

            } else {
                if (ShopTexture is null)
                    return;

                texture = ShopTexture.Texture;
                sourceRectangle = ShopTexture.getOffsetRectangle();
                ShopTexture.update(Game1.currentGameTime);
                scale *= 32f / Unlockable.ShopTextureWidth;
                color = Unlockable.ShopColor;
            }

            if (texture is null)
                return;

            b.Draw(texture, position, sourceRectangle, color, 0f, new Vector2(), scale, Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, boundingBox.Bottom / 10000f);
        }

        public void drawQuestionMark(SpriteBatch b, Vector2 position)
        {
            if (Unlockable._completed.Value)
                return;

            float yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2) - 32f;

            b.Draw(Game1.mouseCursors2, position + Unlockable.QuestionMarkOffset + new Vector2(0, yOffset), new Rectangle(114, 53, 6, 10), Color.White, 0f, new Vector2(1f, 4f), 4f, SpriteEffects.None, 1f);

        }

        public override void updateWhenCurrentLocation(GameTime time)
        {
            if (shakeTimer > 0)
                shakeTimer -= time.ElapsedGameTime.Milliseconds;

            ShopTexture?.update(Game1.currentGameTime);
            SpeechBubble?.updateWhenCurrentLocation(time);

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

        public override bool performToolAction(Tool t)
        {
            if (t is Pickaxe or Axe) {
                shakeTimer = 100;
                Game1.playSound("hammer");
            }
            return false;
        }

        //Only the main players could easily allocate a list of all shop objects, so instead of handling complex caching we minimize the usage of this method and cache results
        //Multiplayer clients may not even have the objects loaded
        public static List<ShopObject> getAll()
        {
            List<ShopObject> list = new();

            Utility.ForEachLocation((loc) => {
                foreach (var obj in loc.Objects.Values.Where(el => el is ShopObject))
                    if (!list.Contains(obj))
                        list.Add(obj as ShopObject);

                return true;
            }, true, false);

            return list;
        }

        private void dayEnding(object sender, StardewModdingAPI.Events.DayEndingEventArgs e) => unsubscribeFromAllEvents();
        private void returnedToTitle(object sender, StardewModdingAPI.Events.ReturnedToTitleEventArgs e) => unsubscribeFromAllEvents();

        public void unsubscribeFromAllEvents()
        {
            //Splitscreen players leaving the game triggers ReturnedToTitle
            if (Context.ScreenId > 0)
                return;
            _wasDiscovered.fieldChangeEvent -= _wasDiscovered_fieldChangeEvent;

            Helper.Events.GameLoop.ReturnedToTitle -= returnedToTitle;
            Helper.Events.GameLoop.DayEnding -= dayEnding; ;
        }


        private void _wasDiscovered_fieldChangeEvent(NetBool field, bool oldValue, bool newValue)
        {
            if (!Context.IsMainPlayer)
                return;

            if (oldValue == newValue)
                return;

            var data = new BundleDiscoveredTransferModel() { id = Unlockable.ID, location = Unlockable.LocationUnique, value = newValue };
            Helper.Multiplayer.SendMessage(data, "BundleDiscovered", modIDs: new[] { ModManifest.UniqueID });
            setDiscovered(Unlockable.ID, Unlockable.LocationUnique, newValue);
        }

        public static void setDiscovered(string id, string location, bool value)
        {
            ModData.setDiscovered(id, location, value);

            if (value && ShopPlacement.HasDayStarted) {
                if (Context.ScreenId == 0)
                    Game1.playSound("ub_pageflip");

                var ts = Game1.game1.GraphicsDevice.Viewport.TitleSafeArea;
                TemporaryAnimatedSprites.Add(
                    new TemporaryAnimatedSprite {
                        initialPosition = new Vector2(ts.X, ts.Y),
                        position = new Vector2(ts.X, ts.Y),
                        sourceRect = new Rectangle(0, 0, 64, 64),
                        animationLength = BundleDiscoveredAnimation.Width / 64,
                        totalNumberOfLoops = 1,
                        interval = 80f,
                        texture = BundleDiscoveredAnimation,
                        scale = 2,
                    });
            }
        }
        private static void drawTemporaryAnimatedSprites(object sender, StardewModdingAPI.Events.RenderedEventArgs e)
        {
            for (int k = TemporaryAnimatedSprites.Count - 1; k >= 0; k--) {
                TemporaryAnimatedSprites[k].draw(e.SpriteBatch, localPosition: true);
                if (TemporaryAnimatedSprites[k].update(Game1.currentGameTime))
                    TemporaryAnimatedSprites.RemoveAt(k);
            }
        }
    }
}
