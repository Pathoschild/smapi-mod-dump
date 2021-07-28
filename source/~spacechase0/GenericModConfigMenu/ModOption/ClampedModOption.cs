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
using GenericModConfigMenu.Framework;
using SpaceShared;

namespace GenericModConfigMenu.ModOption
{
    internal class ClampedModOption<T> : SimpleModOption<T>
    {
        public T Minimum { get; set; }
        public T Maximum { get; set; }
        public T Interval { get; set; }

        public override T Value
        {
            get => base.Value;
            set => base.Value = Util.Adjust(Util.Clamp(this.Minimum, value, this.Maximum), this.Interval);
        }

        public ClampedModOption(string name, string desc, Type type, Func<T> theGetter, Action<T> theSetter, T theMin, T theMax, T interval, string id, ModConfig mod)
            : base(name, desc, type, theGetter, theSetter, id, mod)
        {
            this.Minimum = theMin;
            this.Maximum = theMax;
            this.Interval = interval;
        }
    }
}
