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
    public class EventArgsReturnable<TReturn> : EventArgsArguments
    {
        /*********
        ** Fields
        *********/
        private TReturn _Value;


        /*********
        ** Accessors
        *********/
        internal bool ReturnSet;

        public TReturn Value
        {
            set
            {
                this.ReturnSet = true;
                this._Value = value;
            }
            get => this._Value;
        }


        /*********
        ** Public methods
        *********/
        public EventArgsReturnable(object[] arguments = null)
            : base(arguments) { }
    }
}
