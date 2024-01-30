/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jaredtjahjadi/LogMenu
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace LogMenu
{
    internal class DialogueElement
    {
        public readonly Dialogue charDiag;
        public readonly int portraitIndex;
        public readonly string text;
        private Rectangle bounds;

        public DialogueElement(Dialogue charDiag, int portraitIndex, string text)
        {
            this.charDiag = charDiag;
            this.portraitIndex = portraitIndex;
            text = Game1.parseText(text, Game1.smallFont, 1000 - 128 - IClickableMenu.borderWidth / 2);
            if(charDiag is not null && !Game1.options.showPortraits)
                text = text[(text.IndexOf(":") + 2)..];
            this.text = text;
            bounds = new Rectangle(8 * Game1.pixelZoom, 4 * Game1.pixelZoom, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom);
        }

        public virtual void draw(SpriteBatch b, int slotX, int slotY)
        {
            if(charDiag is not null) // NPC
            {
                // NPC name
                Utility.drawTextWithShadow(b, charDiag.speaker.displayName, Game1.dialogueFont, new Vector2(slotX + bounds.X, slotY + bounds.Y / 2), Game1.textColor, 0.75f);
                // NPC dialogue text
                Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2(slotX + bounds.X, slotY + 42), Game1.textColor);
            }
            else // Non-NPC (object/furniture, etc.)
                Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2(slotX + bounds.X, slotY + bounds.Y), Game1.textColor);
        }
    }
}