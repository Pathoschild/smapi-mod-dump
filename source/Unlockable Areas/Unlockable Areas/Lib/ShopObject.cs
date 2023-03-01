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
using StardewModdingAPI;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley.Tools;
using Unlockable_Areas.Menus;
using Netcode;
using System.Xml.Serialization;

namespace Unlockable_Areas.Lib
{
    public class ShopObject : StardewValley.Object
    {
        protected override void initNetFields()
        {
            base.initNetFields();
            base.NetFields.AddField(this._unlockable);
        }

        public readonly NetRef<Unlockable> _unlockable = new NetRef<Unlockable>();
        public Unlockable Unlockable { get => _unlockable.Value; }
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;
        private Texture2D Texture;

        //private int CurrentAnimationFrame = 0;
        private int AnimationFrames = 0;
        private int AnimationTicks = 0;

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
            IsSpawnedObject = false;

            TileLocation = tileLocation;
            boundingBox.Value = new Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64);

            bigCraftable.Value = true;
            Name = "Unlockable Shop";
            Type = "Crafting";
        }

        public override bool isPassable() => isTemporarilyInvisible;
        public override bool isActionable(Farmer who) => !isTemporarilyInvisible;
        public override bool onExplosion(Farmer who, GameLocation location) => false;
        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            Game1.activeClickableMenu = new ShopObjectMenu(who, Unlockable);

            return false;
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + (float)(shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), y * 64 - 64));

            var sourceRectangle = new Rectangle(0, 0, 32, 64);
            Vector2 origin = new Vector2(0f, 0f);

            if (Texture == null) {
                Texture = Helper.GameContent.Load<Texture2D>(Unlockable.ShopTexture);
                if (Unlockable.ShopAnimation != null && Unlockable.ShopAnimation != "") {
                    var split = Unlockable.ShopAnimation.Split("@");
                    AnimationFrames = int.Parse(split.First());
                    AnimationTicks = int.Parse(split.Last());
                }
            }

            if (Unlockable.ShopAnimation != null && Unlockable.ShopAnimation != "") {
                var currentFrame = (DateTime.Now.Ticks / 10000) % (AnimationTicks * AnimationFrames) / AnimationTicks;

                sourceRectangle.X = sourceRectangle.Width * (int)currentFrame;
            }

            spriteBatch.Draw(Texture, position, sourceRectangle, Color.White, 0f, origin, 2f, this.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float)this.getBoundingBox(new Vector2(x, y)).Bottom / 10000f);
        }

        public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
        {
            if (shakeTimer > 0)
                shakeTimer -= time.ElapsedGameTime.Milliseconds;
        }

        public override bool performToolAction(Tool t, GameLocation location)
        {
            if (t is Pickaxe or Axe) {
                shakeTimer = 100;
                Game1.playSound("hammer");
            }


            return false;
        }
    }
}
