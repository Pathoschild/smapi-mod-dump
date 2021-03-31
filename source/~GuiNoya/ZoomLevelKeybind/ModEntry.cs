/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GuiNoya/SVMods
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace ZoomLevelKeybind
{
    // ReSharper disable once UnusedMember.Global
    public class ModEntry : Mod
    {
        private ModConfig _config;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();

            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.TryGetKeyboard(out Keys _))
            {
                if (e.Button == _config.IncreaseZoomKey)
                    IncreaseZoom();
                else if (e.Button == _config.DecreaseZoomKey)
                    DecreaseZoom();
            }
            else if (e.Button.TryGetController(out Buttons _))
            {
                var wasZoom = false;

                if (e.Button == _config.IncreaseZoomButton)
                {
                    IncreaseZoom();
                    wasZoom = true;
                }
                else if (e.Button == _config.DecreaseZoomButton)
                {
                    DecreaseZoom();
                    wasZoom = true;
                }

                if (_config.SuppressControllerButton && wasZoom)
                    Helper.Input.Suppress(e.Button);
            }
        }

        private void IncreaseZoom()
        {
            if (_config.UnlimitedZoom)
                Game1.options.desiredBaseZoomLevel = Game1.options.zoomLevel >= 3f ? 3f : (float)Math.Round(Game1.options.zoomLevel + 0.05, 2);
            else
                Game1.options.desiredBaseZoomLevel = Game1.options.zoomLevel >= 1.25f ? 1.25f : (float)Math.Round(Game1.options.zoomLevel + 0.05, 2);

            Program.gamePtr.refreshWindowSettings();
        }

        private void DecreaseZoom()
        {
            if (_config.UnlimitedZoom)
                Game1.options.desiredBaseZoomLevel = Game1.options.zoomLevel <= 0.05f ? 0.05f : (float)Math.Round(Game1.options.zoomLevel - 0.05, 2);
            else if (_config.MoreZoom)
                Game1.options.desiredBaseZoomLevel = Game1.options.zoomLevel <= 0.35f ? 0.35f : (float)Math.Round(Game1.options.zoomLevel - 0.05, 2);
            else
                Game1.options.desiredBaseZoomLevel = Game1.options.zoomLevel <= 0.75f ? 0.75f : (float)Math.Round(Game1.options.zoomLevel - 0.05, 2);

            Program.gamePtr.refreshWindowSettings();
        }
    }
}