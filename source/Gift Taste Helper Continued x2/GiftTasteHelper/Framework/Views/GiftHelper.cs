/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/GiftTasteHelper
**
*************************************************/

using System;
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
        private SVector2 OrigHoverTextSize = SVector2.Zero;
        protected GiftDrawData? CurrentGiftDrawData { get; private set; }
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
        public static float UiScale => 1f; // SMAPI's draw call will handle scale


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

        public virtual void OnCursorMoved(CursorMovedEventArgs e)
        {
            // Empty
        }

        public virtual bool CanDraw()
        {
            // Double check here since we may not be unsubscribed from post render right away when the calendar closes
            if (!this.DrawCurrentFrame || this.CurrentGiftDrawData is null)
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
            if (this.CurrentGiftDrawData is not null)
            {
                this.DrawGiftTooltip(this.CurrentGiftDrawData, this.TooltipTitle());
            }
        }

        public virtual bool WantsUpdateEvent()
        {
            return false;
        }

        public virtual void OnPostUpdate(UpdateTickedEventArgs e)
        {
            // Empty
        }

        protected bool SetSelectedNPC(string npcName)
        {
            // TODO: cache these so we only create them once per npc.
            this.CurrentGiftDrawData = this.GiftDrawDataProvider.GetDrawData(npcName, GetTastesToDisplay(), this.GiftConfig.ShowUniversalGifts);
            return this.CurrentGiftDrawData != null;
        }

        private static GiftTaste[] GetTastesToDisplay()
        {
            // TODO: load these from what is set in config.
            return new GiftTaste[] { GiftTaste.Love };
        }

        #region Drawing
        protected string TooltipTitle()
        {
            return this.CurrentGiftDrawData is not null && this.CurrentGiftDrawData.Gifts.Length > 0
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
            int numItemsPerColumn = numItemsToDraw;
            SVector2 maxNameSize = new(0, 0);
            for (int i = 0; i < numItemsToDraw; ++i)
            {
                maxNameSize = Utils.CreateMax(maxNameSize, drawData.Gifts[i].Item.NameSize);
            }

            float spriteScale = 2.0f * UiScale; // 16x16 is pretty small
            SVector2 spriteSize = numItemsToDraw > 0 ? GiftInfo.IconSize : SVector2.Zero; // We just need the dimensions which we assume are all the same
            SVector2 scaledSpriteSize = spriteSize * spriteScale;

            // The longest length of text will help us determine how wide the tooltip box should be 
            SVector2 titleSize = SVector2.MeasureString(title, Game1.smallFont);
            SVector2 maxTextSize = (titleSize.X - scaledSpriteSize.X > maxNameSize.X) ? titleSize : maxNameSize;

            SVector2 mouse = new(Game1.getOldMouseX(), Game1.getOldMouseY());

            int padding = 4; // Chosen by fair dice roll
            int rowHeight = (int)Math.Max(maxTextSize.Y * UiScale, scaledSpriteSize.YInt) + padding;
            int columnWidth = AdjustForTileSize((maxTextSize.X * UiScale) + scaledSpriteSize.XInt) + padding;
            int width = AdjustForTileSize((maxTextSize.X * UiScale) + scaledSpriteSize.XInt) + padding;
            int height = AdjustForTileSize(rowHeight * (numItemsToDraw + 1)); // Add one to make room for the title
            int x = AdjustForTileSize(mouse.X, 0.5f, UiScale);
            int y = AdjustForTileSize(mouse.Y, 0.5f, UiScale);

            int viewportW = (int)(Game1.viewport.Width * Game1.options.zoomLevel / Game1.options.uiScale);
            int viewportH = (int)(Game1.viewport.Height * Game1.options.zoomLevel / Game1.options.uiScale);

            // Create new columns of items shown if they will go off screen.
            if (height > viewportH)
            {
                numItemsPerColumn = ((viewportH - spriteSize.YInt) / rowHeight) - 1; // Remove an item to make space for the title
                height = AdjustForTileSize(rowHeight * (numItemsPerColumn + 1));

                int columnsToDraw = (numItemsToDraw - 1) / numItemsPerColumn + 1;
                width = (AdjustForTileSize((maxTextSize.X * UiScale) + scaledSpriteSize.XInt) + padding) * columnsToDraw;
            }

            // Let derived classes adjust the positioning
            this.AdjustTooltipPosition(ref x, ref y, width, height, viewportW, viewportH);

            // Approximate where the original tooltip will be positioned if there is an existing one we need to account for
            this.OrigHoverTextSize = SVector2.MeasureString(originalTooltipText, Game1.dialogueFont);
            int origTToffsetX = this.OrigHoverTextSize.X > 0
                ? Math.Max(0, AdjustForTileSize(this.OrigHoverTextSize.X + mouse.X, UiScale) - viewportW) + width
                : 0;

            // Consider the position of the original tooltip and ensure we don't cover it up
            SVector2 tooltipPos = this.ClampToViewport(x - origTToffsetX, y, width, height, viewportW, viewportH, mouse);

            // Draw the background of the tooltip
            SpriteBatch spriteBatch = Game1.spriteBatch;

            // Part of the spritesheet containing the texture we want to draw
            Rectangle menuTextureSourceRect = new(0, 256, 60, 60);
            IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, menuTextureSourceRect, tooltipPos.XInt, tooltipPos.YInt, width, height, Color.White, UiScale);

            // Offset the sprite from the corner of the bg, and the text to the right and centered vertically of the sprite
            SVector2 spriteOffset = new(AdjustForTileSize(tooltipPos.X, 0.25f), AdjustForTileSize(tooltipPos.Y, 0.25f));
            SVector2 textOffset = new(spriteOffset.X, spriteOffset.Y + (spriteSize.YInt / 2));

            // TODO: fix weird title y offset when there are > 0 items.
            // Draw the title then set up the offset for the remaining text
            DrawText(title, textOffset);
            textOffset.X += scaledSpriteSize.X + padding;
            textOffset.Y += rowHeight;
            spriteOffset.Y += rowHeight;

            float initialSpriteY = spriteOffset.Y;
            float initialTextY = textOffset.Y;

            // Draw all the items
            for (int i = 0; i < numItemsToDraw; ++i)
            {
                GiftInfo gift = drawData.Gifts[i];
                ItemData item = gift.Item;

                // Draw the sprite for the item then the item text
                var textColor = gift.Universal && this.GiftConfig.ColorizeUniversalGiftNames ? Color.Blue : Game1.textColor;
                var texture = ItemRegistry.GetData(item.ID).GetTexture();
                var tileSheetSourceRect = Game1.getSourceRectForStandardTileSheet(texture, item.SpriteIndex, GiftInfo.IconSize.XInt, GiftInfo.IconSize.YInt);
                DrawText(item.DisplayName, textOffset, textColor);
                DrawTexture(texture, spriteOffset, tileSheetSourceRect, spriteScale);
                

                if ((i + 1) % numItemsPerColumn == 0)
                {
                    // Move to the next column
                    spriteOffset.X += columnWidth;
                    textOffset.X += columnWidth;
                    spriteOffset.Y = initialSpriteY;
                    textOffset.Y = initialTextY;
                }
                else
                {
                    // Move to the next row
                    spriteOffset.Y += rowHeight;
                    textOffset.Y += rowHeight;
                }
            }
        }

        protected virtual void AdjustTooltipPosition(ref int x, ref int y, int width, int height, int viewportW, int viewportHeight)
        {
            // Empty
        }

        private static int AdjustForTileSize(float v, float tileSizeMod = 0.5f, float zoom = 1.0f)
        {
            float tileSize = Game1.tileSize * tileSizeMod;
            return (int)((v + tileSize) * zoom);
        }

        private static void DrawText(string text, SVector2 pos)
        {
            DrawText(text, pos, Game1.textColor);
        }

        private static void DrawText(string text, SVector2 pos, Color textColor)
        {
            Game1.spriteBatch.DrawString(Game1.smallFont, text, pos.ToVector2(), textColor, 0.0f, Vector2.Zero, UiScale, SpriteEffects.None, 0.0f);
        }

        private static void DrawTexture(Texture2D texture, SVector2 pos, Rectangle source, float scale = 1.0f)
        {
            Game1.spriteBatch.Draw(texture, pos.ToVector2(), source, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        }

        private SVector2 ClampToViewport(int x, int y, int w, int h, int viewportW, int viewportH, SVector2 mouse)
        {
            SVector2 p = new(x, y);

            p.X = ClampToViewportAxis(p.XInt, w, viewportW);
            p.Y = ClampToViewportAxis(p.YInt, h, viewportH);

            // Only adjust the position if there's another tooltip that we need to adjust for.
            if (!this.OrigHoverTextSize.IsZero())
            {
                // Only adjust x if the original tooltip isn't right up against the right side and the mouse is between them.
                bool adjustX = (mouse.X <= (viewportW - AdjustForTileSize(this.OrigHoverTextSize.X, 1.0f)));

                // This mimics the regular tooltip behaviour; moving them out of the cursor's way slightly
                int halfTileSize = AdjustForTileSize(0.0f);
                //p.Y = Math.Max(0, p.Y - ((p.X != x) ? halfTileSize : 0));
                p.X = Math.Max(0, p.X - ((p.Y != y && adjustX) ? halfTileSize : 0));
            }
            return p;
        }

        private static int ClampToViewportAxis(int a, int l1, int l2)
        {
            int ca = Utils.Clamp(a, 0, a);
            if (ca + l1 > l2)
            {
                // Offset by how much it extends past the viewport
                int diff = (ca + l1) - l2;
                ca -= diff;
            }
            return Math.Max(0, ca);
        }
        #endregion Drawing

        protected static SVector2 GetAdjustedCursorPosition(float x, float y)
            => new SVector2(x, y) * Game1.options.zoomLevel / Game1.options.uiScale;
    }

}

