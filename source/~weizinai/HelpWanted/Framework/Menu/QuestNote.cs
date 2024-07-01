/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using weizinai.StardewValleyMod.HelpWanted.Framework.Data;

namespace weizinai.StardewValleyMod.HelpWanted.Framework.Menu;

internal class QuestNote : ClickableComponent
{
    public readonly QuestData QuestData;

    public QuestNote(QuestData questData, Rectangle bounds) : base(bounds, "")
    {
        this.QuestData = questData;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(this.QuestData.Pad, this.bounds, this.QuestData.PadSource, this.QuestData.PadColor);
        spriteBatch.Draw(this.QuestData.Pin, this.bounds, this.QuestData.PinSource, this.QuestData.PinColor);
        spriteBatch.Draw(this.QuestData.Icon, new Vector2(this.bounds.X + this.QuestData.IconOffset.X, this.bounds.Y + this.QuestData.IconOffset.Y), this.QuestData.IconSource, this.QuestData.IconColor,
            0, Vector2.Zero, this.QuestData.IconScale, SpriteEffects.None, 0);
    }
}