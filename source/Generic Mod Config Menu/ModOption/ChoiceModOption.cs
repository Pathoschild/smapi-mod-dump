/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/GenericModConfigMenu
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericModConfigMenu.ModOption
{
    internal class ChoiceModOption<T> : SimpleModOption<T>
    {
        public T[] Choices { get; set; }

        public override T Value
        {
            get { return base.Value; }
            set { if (Choices.Contains(value)) base.Value = value; }
        }

        public ChoiceModOption( string name, string desc, Type type, Func<T> theGetter, Action<T> theSetter, T[] choices, string id, IManifest mod )
        :   base( name, desc, type, theGetter, theSetter, id, mod )
        {
            Choices = choices;
        }
    }
}
