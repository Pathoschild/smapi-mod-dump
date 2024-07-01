/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace Email
{
    public partial class ModEntry
    {
        public bool opening;

        public void OpenEmailApp()
        {
            api.SetAppRunning(true);
            api.SetRunningApp(Helper.ModRegistry.ModID);
            api.OnAfterRenderScreen += Display_RenderedWorld;
            Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            opening = true;
        }

        public void CloseApp()
        {
            api.SetAppRunning(false);
            api.SetRunningApp(null);
            api.OnAfterRenderScreen -= Display_RenderedWorld;
            Helper.Events.Input.ButtonPressed -= Input_ButtonPressed;
        }


        public void ClickRow(Point mousePos)
        {
            int idx = (int)((mousePos.Y - api.GetScreenPosition().Y - Config.MarginY - offsetY - Config.AppHeaderHeight) / (Config.MarginY + Config.AppRowHeight));
            Monitor.Log($"clicked index: {idx}");
            if (idx < Game1.player.mailbox.Count && idx >= 0)
            {
                OpenMail(idx);
            }
        }
    }
}