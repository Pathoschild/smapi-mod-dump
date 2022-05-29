/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dem1se/CustomReminders
**
*************************************************/

using Dem1se.CustomReminders.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;

namespace Dem1se.CustomReminders.MobilePhoneModAPI
{
    public interface IMobilePhoneApi
    {
        bool AddApp(string id, string name, Action action, Texture2D icon);
        Vector2 GetScreenPosition();
        Vector2 GetScreenSize();
        Vector2 GetScreenSize(bool rotated);
        Rectangle GetPhoneRectangle();
        Rectangle GetScreenRectangle();
        bool GetPhoneRotated();
        void SetPhoneRotated(bool value);
        bool GetPhoneOpened();
        void SetPhoneOpened(bool value);
        bool GetAppRunning();
        void SetAppRunning(bool value);
        string GetRunningApp();
        void SetRunningApp(string value);
    }

    public static class MobilePhoneMod
    {
        public static void HookToMobilePhoneMod(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            IMobilePhoneApi api = Globals.Helper.ModRegistry.GetApi<IMobilePhoneApi>("aedenthorn.MobilePhone");
            if (api != null)
            {
                Texture2D appIcon = Globals.Helper.ModContent.Load<Texture2D>(System.IO.Path.Combine("assets", "mpmIcon.png"));
                bool success = api.AddApp(Globals.Helper.ModRegistry.ModID, "Custom Reminders", ModEntry.ShowReminderMenu, appIcon);
                Globals.Monitor.Log($"Loaded phone app successfully: {success}", LogLevel.Debug);
            }
        }
    }
}
