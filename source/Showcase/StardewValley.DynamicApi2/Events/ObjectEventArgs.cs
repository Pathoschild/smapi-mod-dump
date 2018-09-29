using System;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicApi2.Events
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