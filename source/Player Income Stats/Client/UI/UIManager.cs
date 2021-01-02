/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/PlayerIncomeStats
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

namespace PlayerIncomeStats.Client.UI
{
    public class UIManager
    {
        public CustomPanel panel;

        public UIManager()
        {
            panel = new CustomPanel(new Rectangle(300, 400, 330, 50));
            ModEntry.instance.Helper.Events.Display.RenderedHud += OnHudRendered;
        }

        public void OnEntriesChanged() => panel.OnEntriesChanged();

        private void OnHudRendered(object sender, RenderedHudEventArgs args)
        {
            panel.Draw();
        }
    }
}