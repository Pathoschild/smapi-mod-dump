/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Deflaktor/KeySuppressor
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace KeySuppressor
{
    public class ModEntry : Mod
    {

        #region Properties
        public static IMonitor MonitorObject { get; private set; }
        private ModConfig Config;

        #endregion


        #region Entry
        public override void Entry(IModHelper helper)
        {
            MonitorObject = Monitor;
            Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.Input.ButtonsChanged += SuppressKeys;

        }

        // Set very low (even lower than low) event priority so that this event is handled after all other mods have handled that event. That way the keys are only suppressed for the base game functionality.
        #endregion
        [EventPriority(EventPriority.Low - 10000)]
        private void SuppressKeys(object sender, ButtonsChangedEventArgs e)
        {
            if (Config != null)
            {
                foreach (var keyValue in Config.SuppressedKeys)
                {
                    switch (keyValue.Value)
                    {
                        case ModConfig.SuppressMode.SuppressOnlyInMenu when Context.IsWorldReady && Game1.activeClickableMenu != null:
                        case ModConfig.SuppressMode.SuppressOnlyWhenPlayerFree when Context.IsPlayerFree:
                        case ModConfig.SuppressMode.SuppressOnlyWhenPlayerCanMove when Context.CanPlayerMove:
                        case ModConfig.SuppressMode.Suppress:
                            this.Helper.Input.SuppressActiveKeybinds(KeybindList.ForSingle(keyValue.Key));
                            break;
                    }
                    
                }
            }
        }
    }
}
