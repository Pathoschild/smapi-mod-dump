using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DeepWoodsMod
{
    /// <summary>
    /// This class provides a chest that looks like the treasure chests in the mines,
    /// but can be used like a player chest with an inventory menu.
    /// It cannot be picked up and it won't be deleted when empty.
    /// Allows for nice treasure chests with plenty of loot that players can access without hassle.
    /// </summary>
    class TreasureChest : Chest
    {
        public readonly NetBool isTrashCan = new NetBool();
        private Texture2D texture;
        private const int TRASHCAN_TOP_TILE_INDEX = 46;
        private const int TRASHCAN_BOTTOM_TILE_INDEX = 78;

        public TreasureChest()
            : base()
        {
        }

        protected override void initNetFields()
        {
            base.initNetFields();
            this.NetFields.AddFields(this.isTrashCan);
        }

        public TreasureChest(Vector2 location, List<Item> items, bool isTrashCan = false)
            : base(true)
        {
            this.name = nameof(Chest);
            this.type.Value = "interactive";
            this.giftbox.Value = false;
            this.items.Set(items);
            this.coins.Value = 0;
            this.tileLocation.Value = location;
            this.Tint = Color.Pink;
            this.isTrashCan.Value = isTrashCan;
            this.boundingBox.Value = new Rectangle((int)this.tileLocation.X * 64, (int)this.tileLocation.Y * 64, 64, 64);
        }

        public override bool performToolAction(Tool t, GameLocation location)
        {
            return false;
        }

        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            if (justCheckingForActivity)
                return true;

            this.mutex.RequestLock((Action)(() =>
            {
                if (this.isTrashCan)
                    SetCurrentLidFrame(135);
                this.frameCounter.Value = 5;
                Game1.playSound(this.isTrashCan ? "trashcan" : "openChest");
                Game1.player.Halt();
                Game1.player.freezePause = 1000;
            }), null);

            return true;
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (this.isTrashCan)
            {
                if (this.texture == null)
                    this.texture = Game1.content.Load<Texture2D>("Maps\\spring_town");

                Vector2 localTop = Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64, (tileLocation.Y - 1) * 64));
                Vector2 localBottom = Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64, tileLocation.Y * 64));

                Rectangle destinationRectangleTop = new Rectangle((int)localTop.X, (int)localTop.Y, 64, 64);
                Rectangle sourceRectangleTop = Game1.getSourceRectForStandardTileSheet(this.texture, TRASHCAN_TOP_TILE_INDEX, 16, 16);

                Rectangle destinationRectangleBottom = new Rectangle((int)localBottom.X, (int)localBottom.Y, 64, 64);
                Rectangle sourceRectangleBottom = Game1.getSourceRectForStandardTileSheet(this.texture, TRASHCAN_BOTTOM_TILE_INDEX, 16, 16);

                float wdagrgy = tileLocation.Y * 64;

                spriteBatch.Draw(this.texture, destinationRectangleTop, sourceRectangleTop, Color.White, 0, Vector2.Zero, SpriteEffects.None, (float)(wdagrgy / 10000.0));
                spriteBatch.Draw(this.texture, destinationRectangleBottom, sourceRectangleBottom, Color.White);
            }
            else
            {
                this.playerChest.Value = false;

                int currentPlayerChestLidFrame = GetCurrentLidFrame();
                int currentTreasureChestLidFrame = PlayerChestLidFrameToTreasureChestLidFrame(currentPlayerChestLidFrame);
                SetCurrentLidFrame(currentTreasureChestLidFrame);

                BaseDraw(spriteBatch, x, y, alpha);

                SetCurrentLidFrame(currentPlayerChestLidFrame);

                this.playerChest.Value = true;
            }
        }

        private int GetCurrentLidFrame()
        {
            var field = typeof(Chest).GetField("currentLidFrame", BindingFlags.NonPublic | BindingFlags.Instance);
            return (int)field.GetValue(this);
        }

        private void SetCurrentLidFrame(int lidFrame)
        {
            var field = typeof(Chest).GetField("currentLidFrame", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(this, lidFrame);
        }

        private int PlayerChestLidFrameToTreasureChestLidFrame(int lidFrame)
        {
            if (isTrashCan)
            {
                return 354;
            }
            else
            {
                return Math.Max(501, Math.Min(503, lidFrame - 131 + 501));
            }
        }

        private Action<SpriteBatch, int, int, float> BaseDraw
        {
            get
            {
                var baseDrawPtr = typeof(Chest).GetMethod("draw", new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }).MethodHandle.GetFunctionPointer();
                return (Action<SpriteBatch, int, int, float>)Activator.CreateInstance(typeof(Action<SpriteBatch, int, int, float>), this, baseDrawPtr);
            }
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f, bool local = false)
        {
        }
    }
}
