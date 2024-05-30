/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using HelpWanted.Framework.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace HelpWanted.Framework.Menu;

internal class QuestNote : ClickableComponent
{
    public readonly QuestData QuestData;

    public QuestNote(QuestData questData, Rectangle bounds) : base(bounds, "")
    {
        QuestData = questData;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(QuestData.Pad, bounds, QuestData.PadSource, QuestData.PadColor);
        spriteBatch.Draw(QuestData.Pin, bounds, QuestData.PinSource, QuestData.PinColor);
        spriteBatch.Draw(QuestData.Icon, new Vector2(bounds.X + QuestData.IconOffset.X, bounds.Y + QuestData.IconOffset.Y), QuestData.IconSource, QuestData.IconColor,
            0, Vector2.Zero, QuestData.IconScale, SpriteEffects.None, 0);
    }
}