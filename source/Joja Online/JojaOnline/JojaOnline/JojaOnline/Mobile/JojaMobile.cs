/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/JojaOnline
**
*************************************************/

using JojaOnline.JojaOnline.API;
using JojaOnline.JojaOnline.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JojaOnline.JojaOnline.Mobile
{
    public class JojaMobile
    {
        private static string modID;
        private static IMobileApi mobileApi;
        private static IMonitor monitor = JojaResources.GetMonitor();

        public static void LoadApp(IModHelper helper)
        {
            // Get modID
            modID = helper.ModRegistry.ModID;

            // Attempt to hook into the IMobileApi interface
            mobileApi = helper.ModRegistry.GetApi<IMobileApi>("aedenthorn.MobilePhone");

            if (mobileApi is null)
            {
                monitor.Log("Failed to hook into aedenthorn.MobilePhone!", LogLevel.Error);
                return;
            }

            monitor.Log("Successfully hooked into aedenthorn.MobilePhone, attempting app load.", LogLevel.Debug);

            if (!mobileApi.AddApp(modID, "JojaOnline", ShowMobile, JojaResources.GetJojaAppIcon()))
            {
                monitor.Log("Unable to load JojaStore for aedenthorn.MobilePhone!", LogLevel.Error);
                return;
            }

            monitor.Log($"Loaded JojaStore mobile app for aedenthorn.MobilePhone.", LogLevel.Debug);

            // Hook into the MenuChanged event
            helper.Events.Display.MenuChanged += OnMenuChanged;

            // Hook into RenderingActiveMenu event
            helper.Events.Display.RenderingActiveMenu += OnRenderingActiveMenu;
        }

        public static async void ShowMobile()
        {
            if (mobileApi.GetAppRunning() || mobileApi.GetRunningApp() == modID)
            {
                return;
            }

            // Let the phone know we're running the app now
            monitor.Log("Claimed running time for Mobile Phone.", LogLevel.Trace);
            mobileApi.SetAppRunning(true);
            mobileApi.SetRunningApp(modID);

            // Give a slight delay
            await Task.Delay(50);

            monitor.Log("Rendering site via Mobile Phone.", LogLevel.Debug);
            Game1.activeClickableMenu = JojaResources.GetScaledJojaSite();
        }

        private static void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu is JojaSite)
            {
                monitor.Log("Released running time for Mobile Phone.", LogLevel.Trace);
                mobileApi.SetAppRunning(false);
            }
        }

        private static void OnRenderingActiveMenu(object sender, RenderingActiveMenuEventArgs e)
        {
            if (mobileApi.GetRunningApp() != modID)
            {
                return;
            }

            // Draw the background on the phone while the app is running
            if (mobileApi.GetPhoneRotated())
            {
                // Horizontal phone 
                e.SpriteBatch.Draw(JojaResources.GetJojaMobileHorzBackground(), mobileApi.GetScreenRectangle(), new Rectangle(0, 0, 200, 150), Color.White);
            }
            else
            {
                // Vertical phone
                e.SpriteBatch.Draw(JojaResources.GetJojaMobileVertBackground(), mobileApi.GetScreenRectangle(), new Rectangle(0, 0, 150, 200), Color.White);
            }
        }
    }
}
