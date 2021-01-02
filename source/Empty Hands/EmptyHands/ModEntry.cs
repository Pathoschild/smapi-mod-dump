/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/quicksilverfox/StardewMods
**
*************************************************/

using System;
using EmptyHands.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace EmptyHands
{
    /// <summary>The mod entry class.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The mod configuration.</summary>
        private ModConfig Config;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<RawModConfig>().GetParsed(Monitor);

            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player presses a keyboard button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Game1.hasLoadedGame)
                return;

            if (!ReceiveKeyPress(e.Button, Config.Keyboard))
                ReceiveKeyPress(e.Button, Config.Controller);
        }

        /// <summary>The method invoked when the player presses an input button.</summary>
        /// <typeparam name="TSButton"></typeparam>
        /// <param name="key">The pressed input.</param>
        /// <param name="map">The configured input mapping.</param>
        /// <returns>Returns whether the key was handled.</returns>
        private bool ReceiveKeyPress<TSButton>(TSButton key, InputMapConfiguration<TSButton> map)
        {
            if (!map.IsValidKey(key))
                return false;

            // perform bound action
            try
            {
                if (key.Equals(map.SetToNothing))
                {
                    UnsetActiveItem();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Something went wrong handling input '{key}':\n{ex}", LogLevel.Error);
                Game1.addHUDMessage(new HUDMessage("Huh. Something went wrong handling your input. The error log has the technical details.", HUDMessage.error_type));
                return true;
            }
        }

        /// <summary>Sets active item to an impossible value.</summary>
        private void UnsetActiveItem()
        {
            if (!Game1.player.IsBusyDoingSomething())
                Game1.player.CurrentToolIndex = int.MaxValue;
        }
    }
}
