/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige.Framework.Menus.Elements.Buttons
{
    internal class Checkbox : Button
    {
        private const int PixelsWide = 9;
        private static int Width => PixelsWide * Game1.pixelZoom;
        private bool IsChecked;
        private readonly ClickCallback OnClick;

        protected override string HoverText => string.Empty;
        protected override string Text { get; }

        public delegate void ClickCallback(bool isChecked);

        public Checkbox(bool isChecked, string text, Rectangle bounds, ClickCallback onClickCallback)
        {
            this.IsChecked = isChecked;
            this.OnClick = onClickCallback;
            this.Bounds = bounds;
            this.Text = text;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="e">The event data.</param>
        /// <param name="isClick">Whether the button press is a click.</param>
        public override void OnButtonPressed(ButtonPressedEventArgs e, bool isClick)
        {
            base.OnButtonPressed(e, isClick);
            if (isClick && this.IsHovered)
            {
                Game1.playSound("drumkit6");
                this.IsChecked = !this.IsChecked;
                this.OnClick.Invoke(this.IsChecked);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 location = new Vector2(this.Bounds.X, this.Bounds.Y);
            spriteBatch.Draw(Game1.mouseCursors, location, this.IsChecked ? OptionsCheckbox.sourceRectChecked : OptionsCheckbox.sourceRectUnchecked, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.4f);
            Utility.drawTextWithShadow(spriteBatch, this.Text, Game1.dialogueFont, new Vector2(location.X + Width + Game1.pixelZoom * 2, location.Y), Game1.textColor, 1f, 0.1f);
        }
    }
}
