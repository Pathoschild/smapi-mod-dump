/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dwayneten/AutoFarmScreenshot
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace AutoFarmScreenshot
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;
        float scale = 0.25f;
        string screenshot_name = null;
        string screenshot_format = null;
        IReflectedMethod takeMapscreenshot = null;
        IReflectedMethod addMessage = null;
        bool isScreenshottedToday = false;
        bool nowInFarm = false;
        int tryTimes = 0;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // read config
            Config = Helper.ReadConfig<ModConfig>();
            scale = Config.ScaleNumber;
            screenshot_format = Config.ScreenshotFormat;
            // attach event
            Helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            Helper.Events.Player.Warped += Player_Warped;
            Helper.Events.GameLoop.TimeChanged += GameLoop_TimeChanged;
            Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
        }

        private void initValue()
        {
            tryTimes = 0;
            nowInFarm = false;
            isScreenshottedToday = false;
            var SToday = SDate.Now();
            screenshot_name = Format_Screenshot_Name();
        }

        private string Format_Screenshot_Name()
        {
            string _screenshot_name = screenshot_format;
            var screenshot_dict = new System.Collections.Generic.Dictionary<string, string> { 
                { "{PlayerName}", Game1.player.name }, 
                { "{Season}", SDate.Now().Season.ToString() },
                { "{Day}", SDate.Now().Day.ToString("00") },
                { "{Year}", SDate.Now().Year.ToString("00") },
                { "{TotalDays}", SDate.Now().DaysSinceStart.ToString("0000") },
                { "{FarmName}", Game1.player.farmName}
            };
            foreach(System.Collections.Generic.KeyValuePair<string, string> format_item in screenshot_dict)
            {
                _screenshot_name = _screenshot_name.Replace(format_item.Key, format_item.Value);
            }
            return _screenshot_name;
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            initValue();
        }

        private void GameLoop_TimeChanged(object sender, TimeChangedEventArgs e)
        {
            // ignore if the screenshot had been captured or the player is not in farm
            if (isScreenshottedToday || !nowInFarm)
                return;

            string fullName = takeMapscreenshot.Invoke<string>(scale, screenshot_name, null);
            if (fullName != null)
            {
                // take screenshot
                addMessage.Invoke("Saved screenshot as '" + fullName + "'.", Color.Green);
                isScreenshottedToday = true;
            }
            else
            {
                if (tryTimes++ > 3)
                {
                    isScreenshottedToday = true;
                    addMessage.Invoke("Failed taking screenshot over 3 times, won't try agian today!", Color.Red);
                }
                else
                    addMessage.Invoke("Failed taking screenshot!", Color.Red);
            }
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            takeMapscreenshot = Helper.Reflection.GetMethod(Game1.game1, "takeMapScreenshot");
            addMessage = Helper.Reflection.GetMethod(Game1.chatBox, "addMessage");
        }

        private void Player_Warped(object sender, WarpedEventArgs e)
        {
            // ignore if the affected player is not the local one
            if (!e.IsLocalPlayer)
                return;
            // ignore if the screenshot had been captured
            if (isScreenshottedToday)
                return;
            // set nowInFarm
            if (e.NewLocation.name.Value.Equals("Farm"))
                nowInFarm = true;
            if (!nowInFarm)
                return;
            if (e.OldLocation.name.Value.Equals("Farm"))
                nowInFarm = false;
        }
    }
}
