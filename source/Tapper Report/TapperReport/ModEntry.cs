/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmod-tapper-report
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using StardewModdingAPI;

using SDVObject = StardewValley.Object;


namespace TapperReport
{ 
    public class ModEntry : Mod
    {
        private TapperTracker Tracker;
        private ModConfigMenu ConfigMenu;

        /*********************************************************
         * MOD ENTRY POINT
         *********************************************************/

        public override void Entry(IModHelper helper)
        {
            ModConfig.Monitor = Monitor;
            Tracker = new TapperTracker(Monitor, helper.ReadConfig<ModConfig>, helper.Translation);
            ConfigMenu = new ModConfigMenu(helper, Monitor, ModManifest, Tracker);

            helper.Events.GameLoop.GameLaunched += ConfigMenu.OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += Tracker.OnSaveLoaded;
            helper.Events.GameLoop.DayEnding += Tracker.OnDayEnding;
            helper.Events.GameLoop.DayStarted += Tracker.OnDayStarted;
            helper.Events.GameLoop.TimeChanged += Tracker.OnTimeChanged;
            helper.Events.Input.ButtonsChanged += Tracker.OnButtonsChanged;
        }

    }
}
