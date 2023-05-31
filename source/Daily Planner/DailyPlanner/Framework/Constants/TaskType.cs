/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BuildABuddha/StardewDailyPlanner
**
*************************************************/

namespace DailyPlanner.Framework.Constants
{
    /// <summary>A tab in the daily planner menu.</summary>
    public enum TaskType
    {
        /// <summary>A daily task.</summary>
        Daily,

        /// <summary>A weekly task.</summary>
        Weekly,

        /// <summary>A one-day task on a certain date.</summary>
        OnDate,

        /// <summary>A checklist task. Should ONLY show up on the checklist and not the daily planner.</summary>
        Checklist
    }
}