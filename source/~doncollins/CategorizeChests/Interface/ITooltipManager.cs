/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/doncollins/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValleyMods.CategorizeChests.Interface.Widgets;

namespace StardewValleyMods.CategorizeChests.Interface
{
    interface ITooltipManager
    {
        void ShowTooltipThisFrame(Widget tooltip);
        void Draw(SpriteBatch batch);
    }
}