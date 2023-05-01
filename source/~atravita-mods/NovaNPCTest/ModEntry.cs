/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using AtraBase.Toolkit.Extensions;
using AtraBase.Toolkit.StringHandler;
using AtraCore.Framework.Caches;
using AtraShared.Schedules;
using AtraShared.Utils.Extensions;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace NovaNPCTest;

internal sealed class ModEntry : Mod
{
    internal static IMonitor ModMonitor { get; private set; } = null!;

    internal static ScheduleUtilityFunctions Scheduler { get; set; } = null!;

    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        ModMonitor = this.Monitor;
        Scheduler = new ScheduleUtilityFunctions(this.Monitor, this.Helper.Translation);

        helper.Events.GameLoop.DayStarted += this.OnDayStart;
    }

    [EventPriority(EventPriority.Low - 10000)]
    private void OnDayStart(object? sender, DayStartedEventArgs e)
    {
        this.CheckSchedule(NPCCache.GetByVillagerName("Nova.Dylan"));
        this.CheckSchedule(NPCCache.GetByVillagerName("Nova.Eli"));
    }

    private void CheckSchedule(NPC? npc)
    {
        if (npc is null)
        {
            return;
        }

        if (Game1.IsVisitingIslandToday(npc.Name))
        {
            ModMonitor.Log($"{npc.Name} is going to the island, have fun!");
            return;
        }

        ModMonitor.Log($"Checking {npc.Name}, is currently at {npc.currentLocation.NameOrUniqueName}", LogLevel.Info);

        var scheduleKey = npc.dayScheduleName.Value;

        if (!npc.TryGetScheduleEntry(scheduleKey, out var entry))
        {
            ModMonitor.Log($"With schedule key {scheduleKey} that apparently doesn't correspond to a schedule. What.", LogLevel.Info);
            return;
        }

        if (!Scheduler.TryFindGOTOschedule(npc, SDate.Now(), entry, out var schedule))
        {
            ModMonitor.Log($"With schedule key {scheduleKey} that apparently doesn't correspond to a schedule that could be resolved: {entry}.", LogLevel.Info);
            return;
        }

        var claimedMap = Game1.getLocationFromName(npc.currentLocation.NameOrUniqueName);
        if (claimedMap is not null)
        {
            if (!ReferenceEquals(claimedMap, npc.currentLocation))
            {
                ModMonitor.Log($"What the hell? NPC claims to be on map {claimedMap.NameOrUniqueName} but is on the map {npc.currentLocation.NameOrUniqueName}, which has failed a reference equality check.", LogLevel.Warn);
            }

            if (claimedMap.characters.Contains(npc))
            {
                ModMonitor.Log($"{npc.Name} correctly in the characters list of {claimedMap}", LogLevel.Info);
            }
        }

        if (schedule.StartsWith("0 "))
        {
            ModMonitor.Log($"Appears to have a zero schedule: {schedule}.");
            var locstring = schedule.GetNthChunk(' ', 1).ToString();

            if (npc.currentLocation.NameOrUniqueName != locstring)
            {

                ModMonitor.Log($"Performing hard warp", LogLevel.Info);
                var target = schedule.GetNthChunk('/').StreamSplit();
                _ = target.MoveNext(); // time

                if (!target.MoveNext())
                {
                    ModMonitor.Log($"Location could not be parsed from schedule {schedule}", LogLevel.Warn);
                    return;
                }

                var loc = Game1.getLocationFromName(target.Current.ToString());
                if (loc is null)
                {
                    ModMonitor.Log($"Location could not be found from schedule {schedule}", LogLevel.Warn);
                    return;
                }

                if (!target.MoveNext() || !int.TryParse(target.Current, out var x))
                {
                    ModMonitor.Log($"X coords could not be parsed from schedule {schedule}", LogLevel.Warn);
                    return;
                }

                if (!target.MoveNext() || !int.TryParse(target.Current, out var y))
                {
                    ModMonitor.Log($"Y coords could not be parsed from schedule {schedule}", LogLevel.Warn);
                    return;
                }

                Game1.warpCharacter(npc, loc.NameOrUniqueName, new Point(x, y));
            }
        }
        else
        {
            ModMonitor.Log($"Does not have 0 schedule", LogLevel.Info);
            if (npc.currentLocation.NameOrUniqueName != npc.DefaultMap)
            {
                ModMonitor.Log($"Seems to be on the wrong map?", LogLevel.Info);
                Game1.warpCharacter(npc, npc.DefaultMap, new Point((int)npc.DefaultPosition.X / Game1.tileSize, (int)npc.DefaultPosition.Y / Game1.tileSize));
            }
        }
    }
}