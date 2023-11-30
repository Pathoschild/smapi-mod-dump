/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using BirbCore.Attributes;
using StardewValley;
using StardewValley.GameData;

namespace BetterFestivalNotifications;

[SEvent]
internal class Events
{

    private string FestivalName;
    private int StartTime;
    private int EndTime;

    [SEvent.DayEnding]
    private void GameLoop_DayEnding(object sender, StardewModdingAPI.Events.DayEndingEventArgs e)
    {
        ModEntry.Instance.Helper.Events.GameLoop.TimeChanged -= this.GameLoop_TimeChanged;
    }

    [SEvent.DayStarted]
    private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
    {
        if (!Utility.isFestivalDay() && !Utility.IsPassiveFestivalDay())
        {
            return;
        }

        if (Utility.isFestivalDay())
        {
            Dictionary<string, string> festivalData = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + Game1.currentSeason + Game1.dayOfMonth);

            this.FestivalName = festivalData["name"];
            string startAndEnd = festivalData["conditions"].Split('/')[1];
            this.StartTime = Convert.ToInt32(ArgUtility.SplitBySpaceAndGet(startAndEnd, 0, "-1"));
            this.EndTime = Convert.ToInt32(ArgUtility.SplitBySpaceAndGet(startAndEnd, 1, "-1"));

            if (this.StartTime < 600 || this.StartTime >= 2600 || this.EndTime < 600 || this.EndTime > 2600)
            {
                Log.Warn("Festival start or end time is invalid");
                return;
            }
        }
        else
        {
            if (!Utility.TryGetPassiveFestivalDataForDay(Game1.dayOfMonth, Game1.season, out string id, out PassiveFestivalData data))
            {
                Log.Warn("Could not load passive festival name, start, and end time");
                return;

            }

            this.FestivalName = data.DisplayName;
            this.StartTime = data.StartTime;
            this.EndTime = 2600;
        }

        ModEntry.Instance.Helper.Events.GameLoop.TimeChanged += this.GameLoop_TimeChanged;
    }

    private void GameLoop_TimeChanged(object sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
    {
        if (e.NewTime == this.StartTime)
        {
            if (ModEntry.Config.PlayStartSound)
            {
                Game1.playSound(ModEntry.Config.StartSound);
            }
        }
        else if (e.NewTime == this.EndTime - (100 * ModEntry.Config.WarnHoursAheadOfTime))
        {
            if (ModEntry.Config.PlayWarnSound)
            {
                Game1.playSound(ModEntry.Config.WarnSound);
            }
            if (ModEntry.Config.ShowWarnNotification)
            {
                Game1.showGlobalMessage(ModEntry.Instance.I18n.Get("festivalWarn", new { festival = this.FestivalName }));
            }
        }
        else if (e.NewTime == this.EndTime)
        {
            if (ModEntry.Config.PlayOverSound)
            {
                Game1.playSound(ModEntry.Config.OverSound);
            }
            if (ModEntry.Config.ShowOverNotification)
            {
                Game1.showGlobalMessage(ModEntry.Instance.I18n.Get("festivalOver", new { festival = this.FestivalName }));
            }
        }
    }
}
