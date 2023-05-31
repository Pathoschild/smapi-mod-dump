/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace Omegasis.Revitalize.Framework.Menus.MenuComponents
{
    public class SearchTextBox : StardewValley.Menus.TextBox
    {

        public event EventHandler<string> onTextReceived;

        public virtual Rectangle Bounds
        {
            get
            {
                return new Rectangle(this.X, this.Y, this.Width, this.Height);
            }
            set
            {
                this.X = value.X;
                this.Y = value.Y;
                this.Width = value.Width;
                this.Height = value.Height;
                this.underlyingComponent.bounds = value;
            }
        }
        public ClickableComponent underlyingComponent;


        public SearchTextBox(Texture2D textBoxTexture, Texture2D caretTexture, SpriteFont font, Color textColor, Rectangle Bounds) : base(textBoxTexture, caretTexture, font, textColor)
        {
            this.underlyingComponent = new ClickableComponent(Bounds, "");
        }

        public override void RecieveTextInput(char inputChar)
        {
            base.RecieveTextInput(inputChar);
            if (onTextReceived != null)
            {
                onTextReceived.Invoke(this, inputChar.ToString());
            }
        }

        public override void RecieveTextInput(string text)
        {
            base.RecieveTextInput(text);
            if (onTextReceived != null)
            {
                onTextReceived.Invoke(this, text);
            }
        }

        public virtual void backSpacePressed()
        {
            this.Text = this.Text.Remove(this.Text.Length - 1);

        }
    }
}
