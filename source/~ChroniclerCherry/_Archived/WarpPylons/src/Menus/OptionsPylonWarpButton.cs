/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace WarpPylons.Menus
{
    class OptionsPylonWarpButton : PylonButton
    {
        private readonly int xOffset = 600;
        public OptionsPylonWarpButton(IMonitor monitor, string label, PylonData pylon)
            : base(monitor, label, pylon)
        {
            this.bounds = new Rectangle(32 + xOffset, 15, (int)Game1.dialogueFont.MeasureString(label).X + 64, 50);
        }

        public override void receiveLeftClick(int x, int y)
        {
            if (!this.bounds.Contains(x, y))
                return;

            _monitor.Log($"Warping to {_pylon.MapName} {_pylon.Coordinates}");
        }

    }
}
