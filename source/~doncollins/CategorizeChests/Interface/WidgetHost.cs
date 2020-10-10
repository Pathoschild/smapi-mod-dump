/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/doncollins/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValleyMods.CategorizeChests.Interface.Widgets;

namespace StardewValleyMods.CategorizeChests.Interface
{
    class WidgetHost : InterfaceHost
    {
        public readonly Widget RootWidget;
        public readonly ITooltipManager TooltipManager;

        public WidgetHost()
        {
            RootWidget = new Widget() {Width = Game1.viewport.Width, Height = Game1.viewport.Height};
            TooltipManager = new TooltipManager();
        }

        protected override void Draw(SpriteBatch batch)
        {
            RootWidget.Draw(batch);
            DrawCursor();
            TooltipManager.Draw(batch);
        }

        protected override bool ReceiveKeyPress(Keys input) => RootWidget.ReceiveKeyPress(input);
        protected override bool ReceiveLeftClick(int x, int y) => RootWidget.ReceiveLeftClick(new Point(x, y));
        protected override bool ReceiveCursorHover(int x, int y) => RootWidget.ReceiveCursorHover(new Point(x, y));
        protected override bool ReceiveScrollWheelAction(int amount) => RootWidget.ReceiveScrollWheelAction(amount);
    }
}