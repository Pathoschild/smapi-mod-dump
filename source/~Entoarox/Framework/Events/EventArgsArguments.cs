/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System;

namespace Entoarox.Framework.Events
{
    public class EventArgsArguments : EventArgs
    {
        /*********
        ** Accessors
        *********/
        public object[] Arguments;


        /*********
        ** Public methods
        *********/
        public EventArgsArguments(object[] arguments = null)
        {
            this.Arguments = arguments ?? new object[0];
        }
    }
}
