/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

namespace Entoarox.Framework.Events
{
    public class EventArgsEditable<TReturn> : EventArgsArguments
    {
        /*********
        ** Accessors
        *********/
        public TReturn Value;


        /*********
        ** Public methods
        *********/
        public EventArgsEditable(TReturn value, object[] arguments = null)
            : base(arguments)
        {
            this.Value = value;
        }
    }
}
