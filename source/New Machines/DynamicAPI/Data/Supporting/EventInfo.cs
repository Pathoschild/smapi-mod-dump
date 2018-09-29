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