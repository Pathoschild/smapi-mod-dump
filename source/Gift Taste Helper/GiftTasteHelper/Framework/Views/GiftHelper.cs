using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace GiftTasteHelper.Framework
{
    internal abstract class GiftHelper : IGiftHelper
    {
        /*********
        ** Properties
        *********/
        private readonly GiftConfig GiftConfig;
        private SVector2 OrigHoverTextSize;
        protected GiftDrawData CurrentGiftDrawData { get; private set; }
        protected bool DrawCurrentFrame;

        protected readonly IReflectionHelper Reflection;
        protected readonly ITranslationHelper Translation;
        protected readonly IGiftDrawDataProvider GiftDrawDataProvider;

        /*********
        ** Accessors
        *********/
        public bool IsInitialized { get; private set; }
        public bool IsOpen { get; private set; }        
        public GiftHelperType GiftHelperType { get; }
        public float ZoomLevel => 1.0f; // SMAPI's draw call will handle zoom


        /*********
        ** Public methods
        *********/
        public GiftHelper(GiftHelperType helperType, IGiftDataProvider dataProvider, GiftConfig config, IReflectionHelper reflection, ITranslationHelper translation)
        {
            this.GiftHelperType = helperType;
            this.GiftConfig = config;
            this.Reflection = reflection;
            this.Translation = translation;
            this.GiftDrawDataProvider = new GiftDrawDataProvider(dataProvider);
        }

        public virtual void Init(IClickableMenu menu)
        {
            if (this.IsInitialized)
            {
                Utils.DebugLog("BaseGiftHelper already initialized; skipping");
                return;
            }
            this.IsInitialized = true;
        }

        public virtual void Reset()
        {
            // Empty
        }

        public virtual bool OnOpen(IClickableMenu menu)
        {
            this.CurrentGiftDrawData = null;
            this.IsOpen = true;

            return true;
        }

        public virtual void OnResize(IClickableMenu menu)
        {
            // Empty
        }

        public virtual void OnClose()
        {
            this.CurrentGiftDrawData = null;
            this.DrawCurrentFrame = false;
            this.IsOpen = false;
        }

        public virtual bool CanTick()
        {
            return true;
        }

        public virtual void OnMouseStateChange(EventArgsMouseStateChanged e)
        {
            // Empty
        }

        public virtual bool CanDraw()
        {
            // Double check here since we may not be unsubscribed from post render right away when the calendar closes
            if (!this.DrawCurrentFrame || this.CurrentGiftDrawData == null)
            {
                return false;
            }

            if (!this.GiftConfig.ShowGiftsForUnmetNPCs && !Game1.player.friendshipData.ContainsKey(this.CurrentGiftDrawData.NpcName))
            {
                return false;
            }
            return true;
        }

        public virtual void OnDraw()
        {
            this.DrawGiftTooltip(this.CurrentGiftDrawData, this.TooltipTitle());
        }

        protected bool SetSelectedNPC(string npcName)
        {
            // TODO: cache these so we only create them once per npc.
            this.CurrentGiftDrawData = this.GiftDrawDataProvider.GetDrawData(npcName, GetTastesToDisplay(), this.GiftConfig.ShowUniversalGifts);
            return this.CurrentGiftDrawData != null;
        }

        private GiftTaste[] GetTastesToDisplay()
        {
            // TODO: load these from what is set in config.
            return new GiftTaste[] { GiftTaste.Love };
        }

        #region Drawing
        protected string TooltipTitle()
        {
            return this.CurrentGiftDrawData.Gifts.Length > 0
                ? this.Translation.Get("tooltip.title.favorite")
                : this.Translation.Get("tooltip.title.none");
        }

        private static int CalculateNumberOfGiftsToDisplay(int numGifts, int maxGiftsToDisplay)
        {
            // 0 or less means no limit
            return maxGiftsToDisplay <= 0 ? numGifts : Math.Min(numGifts, maxGiftsToDisplay);
        }

        protected void DrawGiftTooltip(GiftDrawData drawData, string title, string originalTooltipText = "")
        {
            int numItemsToDraw = CalculateNumberOfGiftsToDisplay(drawData.Gifts.Length, this.GiftConfig.MaxGiftsToDisplay);
            if (numItemsToDraw == 0 && this.GiftConfig.HideTooltipWhenNoGiftsKnown)
            {
                return;
            }

            SVector2 maxNameSize = new SVector2(0, 0);
            for (int i = 0; i < numItemsToDraw; ++i)
            {
                maxNameSize = Utils.CreateMax(maxNameSize, drawData.Gifts[i].Item.NameSize);
            }

            float spriteScale = 2.0f * this.ZoomLevel; // 16x16 is pretty small
            Rectangle spriteRect = numItemsToDraw > 0 ? drawData.IconSize : new Rectangle(0,0,0,0); // We just need the dimensions which we assume are all the same
            SVector2 scaledSpriteSize = new SVector2(spriteRect.Width * spriteScale, spriteRect.Height * spriteScale);

            // The longest length of text will help us determine how wide the tooltip box should be 
            SVector2 titleSize = SVector2.MeasureString(title, Game1.smallFont);
            SVector2 maxTextSize = (titleSize.X - scaledSpriteSize.X > maxNameSize.X) ? titleSize : maxNameSize;

            SVector2 mouse = new SVector2(Game1.getOldMouseX(), Game1.getOldMouseY());

            int padding = 4; // Chosen by fair dice roll
            int rowHeight = (int)Math.Max(maxTextSize.Y * this.ZoomLevel, scaledSpriteSize.YInt) + padding;
            int width = this.AdjustForTileSize((maxTextSize.X * this.ZoomLevel) + scaledSpriteSize.XInt) + padding;
            int height = this.AdjustForTileSize(rowHeight * (numItemsToDraw + 1)); // Add one to make room for the title
            int x = this.AdjustForTileSize(mouse.X, 0.5f, this.ZoomLevel);
            int y = this.AdjustForTileSize(mouse.Y, 0.5f, this.ZoomLevel);

            int viewportW = Game1.viewport.Width;
            int viewportH = Game1.viewport.Height;

            // Let derived classes adjust the positioning
            this.AdjustTooltipPosition(ref x, ref y, width, height, viewportW, viewportH);

            // Approximate where the original tooltip will be positioned if there is an existing one we need to account for
            this.OrigHoverTextSize = SVector2.MeasureString(originalTooltipText, Game1.dialogueFont);
            int origTToffsetX = this.OrigHoverTextSize.X > 0
                ? Math.Max(0, this.AdjustForTileSize(this.OrigHoverTextSize.X + mouse.X, 1.0f) - viewportW) + width
                : 0;

            // Consider the position of the original tooltip and ensure we don't cover it up
            SVector2 tooltipPos = this.ClampToViewport(x - origTToffsetX, y, width, height, viewportW, viewportH, mouse);

            // Reduce the number items shown if it will go off screen.
            // TODO: perhaps add a second column
            if (height > viewportH)
            {
                numItemsToDraw = (viewportH / rowHeight) - 1; // Remove an item to make space for the title
                height = this.AdjustForTileSize(rowHeight * numItemsToDraw);
            }

            // Draw the background of the tooltip
            SpriteBatch spriteBatch = Game1.spriteBatch;

            // Part of the spritesheet containing the texture we want to draw
            Rectangle menuTextureSourceRect = new Rectangle(0, 256, 60, 60);
            IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, menuTextureSourceRect, tooltipPos.XInt, tooltipPos.YInt, width, height, Color.White, this.ZoomLevel);

            // Offset the sprite from the corner of the bg, and the text to the right and centered vertically of the sprite
            SVector2 spriteOffset = new SVector2(this.AdjustForTileSize(tooltipPos.X, 0.25f), this.AdjustForTileSize(tooltipPos.Y, 0.25f));
            SVector2 textOffset = new SVector2(spriteOffset.X, spriteOffset.Y + (spriteRect.Height / 2));

            // TODO: fix weird title y offset when there are > 0 items.
            // Draw the title then set up the offset for the remaining text
            this.DrawText(title, textOffset);
            textOffset.X += scaledSpriteSize.X + padding;
            textOffset.Y += rowHeight;
            spriteOffset.Y += rowHeight;

            // Draw all the items
            for (int i = 0; i < numItemsToDraw; ++i)
            {
                GiftInfo gift = drawData.Gifts[i];
                ItemData item = gift.Item;

                // Draw the sprite for the item then the item text
                var textColor = gift.Universal && this.GiftConfig.ColorizeUniversalGiftNames ? Color.Blue : Game1.textColor;
                this.DrawText(item.DisplayName, textOffset, textColor);
                this.DrawTexture(Game1.objectSpriteSheet, spriteOffset, item.TileSheetSourceRect, spriteScale);

                // Move to the next row
                spriteOffset.Y += rowHeight;
                textOffset.Y += rowHeight;
            }
        }

        protected virtual void AdjustTooltipPosition(ref int x, ref int y, int width, int height, int viewportW, int viewportHeight)
        {
            // Empty
        }

        private int AdjustForTileSize(float v, float tileSizeMod = 0.5f, float zoom = 1.0f)
        {
            float tileSize = Game1.tileSize * tileSizeMod;
            return (int)((v + tileSize) * zoom);
        }

        private void DrawText(string text, SVector2 pos)
        {
            DrawText(text, pos, Game1.textColor);
        }

        private void DrawText(string text, SVector2 pos, Color textColor)
        {
            Game1.spriteBatch.DrawString(Game1.smallFont, text, pos.ToVector2(), textColor, 0.0f, Vector2.Zero, this.ZoomLevel, SpriteEffects.None, 0.0f);
        }

        private void DrawTexture(Texture2D texture, SVector2 pos, Rectangle source, float scale = 1.0f)
        {
            Game1.spriteBatch.Draw(texture, pos.ToVector2(), source, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        }

        private SVector2 ClampToViewport(int x, int y, int w, int h, int viewportW, int viewportH, SVector2 mouse)
        {
            SVector2 p = new SVector2(x, y);

            p.X = this.ClampToViewportAxis(p.XInt, w, viewportW);
            p.Y = this.ClampToViewportAxis(p.YInt, h, viewportH);

            // Only adjust the position if there's another tooltip that we need to adjust for.
            if (!this.OrigHoverTextSize.IsZero())
            {
                // Only adjust x if the original tooltip isn't right up against the right side and the mouse is between them.
                bool adjustX = (mouse.X <= (viewportW - this.AdjustForTileSize(this.OrigHoverTextSize.X, 1.0f)));

                // This mimics the regular tooltip behaviour; moving them out of the cursor's way slightly
                int halfTileSize = this.AdjustForTileSize(0.0f);
                p.Y = Math.Max(0, p.Y - ((p.X != x) ? halfTileSize : 0));
                p.X = Math.Max(0, p.X - ((p.Y != y && adjustX) ? halfTileSize : 0));
            }
            return p;
        }

        private int ClampToViewportAxis(int a, int l1, int l2)
        {
            int ca = Utils.Clamp(a, 0, a);
            if (ca + l1 > l2)
            {
                // Offset by how much it extends past the viewport
                int diff = (ca + l1) - l2;
                ca -= diff;
            }
            return ca;
        }
        #endregion Drawing
    }

}

