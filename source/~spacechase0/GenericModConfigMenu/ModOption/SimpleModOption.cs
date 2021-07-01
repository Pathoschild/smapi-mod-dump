/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using StardewModdingAPI;

namespace GenericModConfigMenu.ModOption
{
    internal class SimpleModOption<T> : BaseModOption
    {
        public Type Type { get; }
        protected Func<T> Getter;
        protected Action<T> Setter;

        private T State;
        public virtual T Value
        {
            get => this.State;
            set
            {
                if (!this.State.Equals(value))
                    Mod.Instance.Configs[this.Owner].Options[Mod.Instance.Configs[this.Owner].ActiveDisplayPage.Name].ChangeHandler.ForEach(c => c.Invoke(this.Id, value));

                this.State = value;
            }
        }

        public override void SyncToMod()
        {
            this.State = this.Getter.Invoke();
        }

        public override void Save()
        {
            SpaceShared.Log.Trace("saving " + this.Name + " " + this.Description);
            this.Setter.Invoke(this.State);
        }

        public SimpleModOption(string name, string desc, Type type, Func<T> theGetter, Action<T> theSetter, string id, IManifest mod)
            : base(name, desc, id, mod)
        {
            this.Type = type;
            this.Getter = theGetter;
            this.Setter = theSetter;

            this.State = this.Getter.Invoke();
        }
    }
}
