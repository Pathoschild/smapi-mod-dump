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
using System.Linq;
using GenericModConfigMenu.Framework;

namespace GenericModConfigMenu.ModOption
{
    internal class ChoiceModOption<T> : SimpleModOption<T>
    {
        public T[] Choices { get; set; }

        public override T Value
        {
            get => base.Value;
            set { if (this.Choices.Contains(value)) base.Value = value; }
        }

        public ChoiceModOption(string name, string desc, Type type, Func<T> theGetter, Action<T> theSetter, T[] choices, string id, ModConfig mod)
            : base(name, desc, type, theGetter, theSetter, id, mod)
        {
            this.Choices = choices;
        }
    }
}
