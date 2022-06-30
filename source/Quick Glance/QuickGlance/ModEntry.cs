/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/adverserath/QuickGlance
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;

namespace QuickGlance
{
    public class ModEntry : Mod
    {
        private ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            if (Config.ToggleZoom)
            {
                helper.Events.Input.ButtonPressed += this.OnButtonPressedToggle;
            }
            else
            {
                helper.Events.Input.ButtonPressed += this.OnButtonPressed;
                helper.Events.Input.ButtonReleased += this.OnButtonReleased;
                helper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
            }
        }

        private Dictionary<long, float> zoomMemory = new Dictionary<long, float>();

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (zoomMemory.ContainsKey(Game1.player.UniqueMultiplayerID) && ((int)e.Button == Config.ZoomKey1 || (int)e.Button == Config.ZoomKey2))
            {
                Game1.options.desiredBaseZoomLevel = zoomMemory.GetValueOrDefault(Game1.player.UniqueMultiplayerID, 1f);
                zoomMemory.Remove(Game1.player.UniqueMultiplayerID);
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!zoomMemory.ContainsKey(Game1.player.UniqueMultiplayerID) && ((int)e.Button == Config.ZoomKey1 || (int)e.Button == Config.ZoomKey2))
            {
                if (Game1.options.desiredBaseZoomLevel != Config.ZoomLevel)
                    zoomMemory.Add(Game1.player.UniqueMultiplayerID, Game1.options.desiredBaseZoomLevel);
                Game1.options.desiredBaseZoomLevel = Config.ZoomLevel;
            }
        }

        private void OnButtonPressedToggle(object sender, ButtonPressedEventArgs e)
        {
            if (!zoomMemory.ContainsKey(Game1.player.UniqueMultiplayerID) && ((int)e.Button == Config.ZoomKey1 || (int)e.Button == Config.ZoomKey2))
            {
                if (Game1.options.desiredBaseZoomLevel != Config.ZoomLevel)
                    zoomMemory.Add(Game1.player.UniqueMultiplayerID, Game1.options.desiredBaseZoomLevel);
                Game1.options.desiredBaseZoomLevel = Config.ZoomLevel;
            }
            else if (zoomMemory.ContainsKey(Game1.player.UniqueMultiplayerID) && ((int)e.Button == Config.ZoomKey1 || (int)e.Button == Config.ZoomKey2))
            {
                Game1.options.desiredBaseZoomLevel = zoomMemory.GetValueOrDefault(Game1.player.UniqueMultiplayerID, 1f);
                zoomMemory.Remove(Game1.player.UniqueMultiplayerID);
            }
        }

        private void GameLoop_OneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (zoomMemory.ContainsKey(Game1.player.UniqueMultiplayerID) && (!Helper.Input.IsDown((SButton)Config.ZoomKey1) && !Helper.Input.IsDown((SButton)Config.ZoomKey2)))
            {
                Game1.options.desiredBaseZoomLevel = zoomMemory.GetValueOrDefault(Game1.player.UniqueMultiplayerID, 1f);
                zoomMemory.Remove(Game1.player.UniqueMultiplayerID);
            }
        }

    }
}