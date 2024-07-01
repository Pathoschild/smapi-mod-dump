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
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.IO;

namespace MobileCatalogues
{
    public class HelperEvents
    {
        private static IMonitor Monitor;
        private static IModHelper Helper;
        private static ModConfig Config;
        private static IMobilePhoneApi api;

        // call this method from your Entry class
        public static void Initialize(IModHelper helper, IMonitor monitor, ModConfig config)
        {
            Monitor = monitor;
            Helper = helper;
            Config = config;
        }

        public static void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ModEntry.api = Helper.ModRegistry.GetApi<IMobilePhoneApi>("JoXW.MobilePhone");
            if (ModEntry.api != null)
            {
                api = ModEntry.api;
                Texture2D appIcon;
                bool success;

                appIcon = Helper.ModContent.Load<Texture2D>(Path.Combine("assets", "app_icon.png"));
                success = ModEntry.api.AddApp(Helper.ModRegistry.ModID + "Catalogues", Helper.Translation.Get("catalogues"), CataloguesApp.OpenCatalogueApp, appIcon);
                Monitor.Log($"loaded catalogues app successfully: {success}", LogLevel.Debug);

                Visuals.MakeTextures();
            }
        }

        public static void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (api.IsCallingNPC() || api.GetRunningApp() != Helper.ModRegistry.ModID)
            {
                return;
            }

            if (e.Button == SButton.MouseLeft)
            {
                Point mousePos = Game1.getMousePosition();
                if (!api.GetScreenRectangle().Contains(mousePos))
                {
                    return;
                }

                Helper.Input.Suppress(SButton.MouseLeft);
                Vector2 screenPos = api.GetScreenPosition();
                Vector2 screenSize = api.GetScreenSize();
                if (!CataloguesApp.opening && new Rectangle((int)(screenPos.X + screenSize.X - Config.AppHeaderHeight), (int)(screenPos.Y), (int)Config.AppHeaderHeight, (int)Config.AppHeaderHeight).Contains(mousePos))
                {
                    Monitor.Log($"Closing app");
                    CataloguesApp.CloseApp();
                    return;
                }

                CataloguesApp.opening = false;
                Visuals.clicking = true;
                Visuals.lastMousePosition = mousePos;
            }
        }
    }
}
