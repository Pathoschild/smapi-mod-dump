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

namespace Omegasis.HappyBirthday.Framework.Menus
{
    public class GiftSearchTextBox : StardewValley.Menus.TextBox
    {

        public event EventHandler<string> onTextReceived;

        public GiftSearchTextBox(Texture2D textBoxTexture, Texture2D caretTexture, SpriteFont font, Color textColor) : base(textBoxTexture, caretTexture, font, textColor)
        {

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
