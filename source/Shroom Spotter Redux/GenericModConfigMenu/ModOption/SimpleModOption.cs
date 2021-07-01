/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using StardewModdingAPI;

namespace GenericModConfigMenu.ModOption
{
    internal class SimpleModOption<T> : BaseModOption
    {
        public Type Type { get; }
        protected Func<T> getter;
        protected Action<T> setter;

        private T state;
        public virtual T Value
        {
            get { return state; }
            set {
                if (!state.Equals(value))
                    Mod.instance.configs[Owner].Options[Mod.instance.configs[Owner].ActiveDisplayPage.Name].ChangeHandler.ForEach(c => c.Invoke(Id, value));

                state = value; 
            }
        }

        public override void SyncToMod()
        {
            state = getter.Invoke();
        }

        public override void Save()
        {
            SpaceShared.Log.trace( "saving " + Name + " " + Description );
            setter.Invoke(state);
        }

        public SimpleModOption( string name, string desc, Type type, Func<T> theGetter, Action<T> theSetter, string id, IManifest mod )
        :   base( name, desc, id, mod )
        {
            Type = type;
            getter = theGetter;
            setter = theSetter;

            state = getter.Invoke();
        }
    }
}
