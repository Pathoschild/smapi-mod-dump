/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using FarmVisitors.Datamodels;
using StardewValley;
using StardewValley.Pathfinding;
using System.Collections.Generic;

namespace FarmVisitors.Models;

internal class VisitData
{
    public ScheduleData CustomData { get; set; }
    public bool IsOutside { get; set; }
    public int ControllerTime { get; set; }
    public int DurationSoFar { get; set; }
    public int TimeOfArrival { get; set; }
    public bool CustomVisiting { get; set; }
    public bool IsGoingToSleep { get; set; }
    public bool Idle { get; set; }
    public Dictionary<int, SchedulePathDescription> Scheduledata { get; private set; }

    public VisitData(NPC who)
    {
        Scheduledata = who.Schedule;
        TimeOfArrival = Game1.timeOfDay;

        CustomData = null;
        IsOutside = false;
        ControllerTime = 0;
        DurationSoFar = 0;
        CustomVisiting = false;
        IsGoingToSleep = false;
        Idle = false;
    }

    public VisitData(NPC who, bool isCustom, ScheduleData data)
    {
        CustomData = data;
        Scheduledata = who.Schedule;
        TimeOfArrival = Game1.timeOfDay;
        CustomVisiting = isCustom;

        IsOutside = false;
        ControllerTime = 0;
        DurationSoFar = 0;
        IsGoingToSleep = false;
        Idle = false;
    }

    internal void Restart(Dictionary<int, SchedulePathDescription> schedule)
    {
        DurationSoFar = 0;
        TimeOfArrival = Game1.timeOfDay;
        ControllerTime = 0;
        Scheduledata = schedule;
        IsGoingToSleep = false;
        Idle = false;
    }
}
