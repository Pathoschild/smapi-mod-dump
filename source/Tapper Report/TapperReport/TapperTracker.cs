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

using System.Collections.Generic;
using System.Text;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

using SDVObject = StardewValley.Object;

namespace TapperReport
{
    class TapperTracker
    {
        /* CONSTants */
        const string TAPPER = "Tapper";
        const string LINE1_PRE = "> ";
        const string LINE2_PRE = "   "; // indent
        const string NEWLINE = "^";

        /* Interface to rest of Assembly */
        public delegate ModConfig ConfigReaderDelegate();
        private readonly ConfigReaderDelegate ConfigReader;
        private readonly IMonitor Monitor;
        private readonly ITranslationHelper Translation;
        private ModConfig Config;

        /* Internal States */
        private readonly Dictionary<GameLocation, List<SDVObject>> AllTappers = new Dictionary<GameLocation, List<SDVObject>>();
        private int BeforeSleepDoneCount;
        private int AfterWakeupDoneCount;
        private int PreviousDoneCount = 0;
        private int CountdownUntilCheck = 999;
        private bool BlockCheck = false;
        private readonly StringBuilder sb = new StringBuilder();

        public TapperTracker(IMonitor monitor, ConfigReaderDelegate configReader, ITranslationHelper translation)
        {
            Monitor = monitor;
            ConfigReader = configReader;
            Translation = translation;

            UpdateConfig();
        }

        /*********************************************************
         * EVENT HANDLERS
         *********************************************************/

        internal void OnSaveLoaded(object sender, SaveLoadedEventArgs args)
        {
            UpdateConfig();
        }

        internal void OnDayEnding(object sender, DayEndingEventArgs args)
        {
            BeforeSleepDoneCount = GetDoneCount(refreshState: true);
            Monitor.Log($"BeforeSleepDoneCount = {BeforeSleepDoneCount}", LogLevel.Info);
        }

        internal void OnDayStarted(object sender, DayStartedEventArgs args)
        {
            AfterWakeupDoneCount = GetDoneCount(refreshState: true);
            Monitor.Log($"AfterWakeupDoneCount = {AfterWakeupDoneCount}", LogLevel.Info);
            if (AfterWakeupDoneCount > BeforeSleepDoneCount)
            {
                int new_done = AfterWakeupDoneCount - BeforeSleepDoneCount;
                Monitor.Log("Preparing DayStarted HUD pop-up...");
                object tokens = new { tapperword = TapperWord(new_done) };
                sb.Clear()
                  .Append(new_done)
                  .Append(" ")
                  .Append(Translation.Get("hud.done_while_sleep", tokens));
                // HUDMessage(string, string) is the autofade pop-up that does not have the icon square
                Game1.addHUDMessage(new HUDMessage(sb.ToString(), ""));
                Monitor.Log("DayStarted HUD had just been shown.");
            }
            PreviousDoneCount = AfterWakeupDoneCount;
        }

        internal void OnTimeChanged(object sender, TimeChangedEventArgs args)
        {
            // This is only useful for Tappers on Mushroom Trees
            // Tappers on other trees seem to be hardcoded to trigger when sleeping

            // Guards
            if (BlockCheck) return;
            if (!Config.CheckNewFinishedContinually) return;
            if (--CountdownUntilCheck > 0) return;

            Monitor.Log("Continual (periodic) check triggered", LogLevel.Info);
            CountdownUntilCheck = Config.CheckEvery;

            int current_done_count = GetDoneCount(refreshState: true);
            Monitor.Log($"Done count was {PreviousDoneCount} now {current_done_count}");
            int new_done = current_done_count - PreviousDoneCount;
            PreviousDoneCount = current_done_count;
            if (new_done <= 0) return;  // can be negative if tapper harvested then removed from tree

            object tokens = new { tapperword = TapperWord(new_done), hotkey = Config.HotKey };
            sb.Clear()
              .Append(new_done)
              .Append(" ")
              .Append(Translation.Get("hud.done_while_sleep", tokens));
            Game1.addHUDMessage(new HUDMessage(sb.ToString(), ""));
            Monitor.Log("HUD message popped");
        }

        internal void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (Config.HotKey.JustPressed() || Config.GamePadHotKey.JustPressed())
            {
                Monitor.Log("Hotkey detected", LogLevel.Info);
                if (!Context.IsPlayerFree) {
                    Monitor.Log("Player not Free, skipping report");
                    return;
                    }
                BlockCheck = true;
                Monitor.Log("Refreshing mod config", LogLevel.Info);
                UpdateConfig();
                Game1.drawLetterMessage(MakeReport().ToString());
                CountdownUntilCheck = Config.CheckEvery;
                BlockCheck = false;
            }
        }

        /*********************************************************
         * HELPERS
         *********************************************************/

        private string TapperWord(int num)
        {
            return num == 1 ? Translation.Get("tapper.single") : Translation.Get("tapper.plural");
        }

        internal void UpdateConfig()
        {
            Config = ConfigReader();
            if (Config.CheckEvery < CountdownUntilCheck) CountdownUntilCheck = Config.CheckEvery;
        }

        /*********************************************************
         * HEAVY DUTY LOGIC
         *********************************************************/

        private void GatherTapperStates()
        {
            Monitor.Log("Gathering tappers...");
            AllTappers.Clear();
            foreach (GameLocation loc in Game1.locations)
            {
                // loc.objects is, surprisingly, an IEnumerable
                // To actually get the objects, we must first iterate through it ...
                foreach (var objdict in loc.objects)
                {
                    // ... THEN we iterate over the 'inner' dictionary's values
                    // (we don't care about the Vector2 coordinates in .Keys)
                    var tappers = objdict.Values.Where(o => ((Item)o).Name == TAPPER).ToList();
                    if (tappers.Count == 0) continue;
                    AllTappers.Add(loc, tappers);
                }
            }
            Monitor.Log($"{AllTappers.Count} locations with tappers");
        }

        private int GetDoneCount(bool refreshState = true)
        {
            if (refreshState) GatherTapperStates();
            int done_count = AllTappers.Values.Sum(
                tappers => tappers.Count(
                    tapper => tapper.minutesUntilReady <= 0
                    )
                );
            Monitor.Log($"{done_count} tappers are done");
            return done_count;
        }

        private StringBuilder MakeReport()
        {
            Monitor.Log("A Report is requested", LogLevel.Info);
            GatherTapperStates();

            sb.Clear()
              .Append("===== ")
              .Append(Translation.Get("report.title"))
              .Append(" =====^");

            if (AllTappers.Count == 0)
            {
                sb.Append(NEWLINE)
                  .Append(Translation.Get("report.no_tapper"));
                return sb;
            }

            string separator = Config.Separator;

            var done_onloc_bytype = new Dictionary<string, int>();
            var ratios_oftypes = new List<string>();
            string tapper_type;
            int total_of_thistype;
            string localized_type;
            foreach (GameLocation loc in AllTappers.Keys)
            {
                done_onloc_bytype.Clear();
                ratios_oftypes.Clear();

                var tappers_onloc_bytype =
                    from tapper in AllTappers[loc]
                    group tapper by ((Item)tapper.heldObject).Name into tapper_groups
                    select tapper_groups;

                // tappers_onloc_bytype is an IEnumerate(IGroup<>)
                // This IGroup thing is interesting ... itself is enumerable, containing the members of the group,
                // but it has a .Key property containing the "grouped by" object as specified in Linq
                foreach (var tapper_group in tappers_onloc_bytype)
                {
                    tapper_type = tapper_group.Key;
                    done_onloc_bytype[tapper_type] = tapper_group.Where(x => x.minutesUntilReady <= 0).Count();
                    total_of_thistype = tapper_group.Count();
                    // Localize+shorten
                    localized_type = Translation.Get($"report.{tapper_type}");
                    ratios_oftypes.Add(
                        $"{localized_type} {done_onloc_bytype[tapper_type]}/{total_of_thistype}"
                        );
                }
                // Sort by localized string
                ratios_oftypes.Sort();

                // First line
                sb.Append($"{LINE1_PRE}{loc.Name}: ")
                  .Append($"{done_onloc_bytype.Values.Sum()}/{AllTappers[loc].Count} ")
                  .Append(Translation.Get("report.done_suffix"))
                  .Append(NEWLINE);

                // Second line
                sb.Append(LINE2_PRE)
                  .Append(string.Join(separator, ratios_oftypes))
                  .Append(NEWLINE);
            }
            sb.Append(NEWLINE)
              .Append(Translation.Get("report.total_onworld"))
              .Append(" ")
              .Append(AllTappers.Values.Sum(tappers => tappers.Count()));
            return sb;
        }

    }
}
