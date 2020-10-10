/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

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