/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chencrstu/TeleportNPCLocation
**
*************************************************/

using System;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using static TeleportNPCLocation.framework.CommonSprites;

namespace TeleportNPCLocation.framework
{
    public class Checkbox
    {
        /*********
        ** Accessors
        *********/
        public Texture2D Texture { get; set; }
        public Rectangle CheckedTextureRect { get; set; }
        public Rectangle UncheckedTextureRect { get; set; }

        public Action<Checkbox> Callback { get; set; }

        public bool Checked { get; set; } = true;

        /// <inheritdoc />
        public int Width => this.CheckedTextureRect.Width * 4;

        /// <inheritdoc />
        public int Height => this.CheckedTextureRect.Height * 4;

        /// <inheritdoc />
        public Vector2 Position { get; set; }
        public Rectangle Bounds => new((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height);

        public bool Hover { get; private set; }
        public string HoveredSound => null;

        public bool ClickGestured { get; private set; }
        public bool Clicked => this.Hover && this.ClickGestured;
        public string ClickedSound => "drumkit6";

        /*********
        ** Public methods
        *********/
        public Checkbox()
        {
            this.Texture = Game1.mouseCursors;
            this.CheckedTextureRect = OptionsCheckbox.sourceRectChecked;
            this.UncheckedTextureRect = OptionsCheckbox.sourceRectUnchecked;
        }

        /// <inheritdoc />
        public void Update(bool isOffScreen = false)
        {
            int mouseX;
            int mouseY;
            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                mouseX = Game1.getMouseX();
                mouseY = Game1.getMouseY();
            }
            else
            {
                mouseX = Game1.getOldMouseX();
                mouseY = Game1.getOldMouseY();
            }

            bool newHover = !isOffScreen && this.Bounds.Contains(mouseX, mouseY);
            if (newHover && !this.Hover && this.HoveredSound != null)
                Game1.playSound(this.HoveredSound);
            this.Hover = newHover;

            this.ClickGestured = (Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released);
            // gamepad support
            this.ClickGestured = this.ClickGestured || (Game1.options.gamepadControls && (Game1.input.GetGamePadState().IsButtonDown(Buttons.A) && !Game1.oldPadState.IsButtonDown(Buttons.A)));
            if (this.Clicked && this.ClickedSound != null)
                Game1.playSound(this.ClickedSound);

            if (this.Clicked && this.Callback != null)
            {
                this.Checked = !this.Checked;
                this.Callback.Invoke(this);
            }
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch b)
        {
            b.Draw(this.Texture, this.Position, this.Checked ? this.CheckedTextureRect : this.UncheckedTextureRect, Color.White, 0, Vector2.Zero, 4, SpriteEffects.None, 0);
        }
    }
}

