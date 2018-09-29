using System;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicAPI.Events
{
    public sealed class ObjectEventArgs : EventArgs
    {
        public Object Object { get; set; }

        public ObjectEventArgs(Object o)
        {
            Object = o;
        }
    }
}