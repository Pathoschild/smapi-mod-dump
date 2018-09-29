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