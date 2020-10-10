/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System;

namespace Igorious.StardewValley.DynamicAPI.Data.Supporting
{
    public sealed class EventInfo
    {
        public EventInfo(Type type, string eventName)
        {
            Type = type;
            EventName = eventName;
        }

        public Type Type { get; }
        public string EventName { get; }
    }
}