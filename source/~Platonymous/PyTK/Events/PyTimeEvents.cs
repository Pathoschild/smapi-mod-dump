/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System;
using PyTK.Overrides;
using PyTK.Types;
using StardewValley;

namespace PyTK.Events
{
    public static class PyTimeEvents
    {
        public static event EventHandler<EventArgsBeforeSleep> BeforeSleepEvents;

        public class EventArgsBeforeSleep : EventArgs
        {
            public EventArgsBeforeSleep(STime sleepTime, bool passedOut, ref Response response)
            {
                SleepTime = sleepTime;
                PassedOut = passedOut;
                Response = response;
            }

            public bool PassedOut { get; }
            public STime SleepTime { get; }
            public Response Response { get; set; }

        }

        internal static void CallBeforeSleepEvents(object sender, EventArgsBeforeSleep e)
        {
            BeforeSleepEvents?.Invoke(sender, e);
        }

    }
}
