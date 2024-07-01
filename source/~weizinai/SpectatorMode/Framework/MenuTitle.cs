/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Menus;

namespace weizinai.StardewValleyMod.SpectatorMode.Framework;

internal class MenuTitle
{
    private readonly string text;
    private readonly Vector2 position;

    private readonly SpriteFont font = Game1.dialogueFont;

    public MenuTitle(string text)
    {
        this.text = text;
        this.position = new Vector2((Game1.uiViewport.Width - this.GetActualSize().X) / 2, 32);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        IClickableMenu.drawTextureBox(spriteBatch, (int)this.position.X, (int)this.position.Y, (int)this.GetActualSize().X, (int)this.GetActualSize().Y, Color.White);
        Utility.drawTextWithShadow(spriteBatch, this.text, this.font, this.position + new Vector2(16, 16), Game1.textColor);
    }

    private Vector2 GetActualSize()
    {
        return this.font.MeasureString(this.text) + new Vector2(32, 32);
    }
}