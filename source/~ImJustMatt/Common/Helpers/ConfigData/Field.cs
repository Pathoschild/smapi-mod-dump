/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System.Reflection;
using System.Text.RegularExpressions;

namespace Helpers.ConfigData
{
    internal class Field : IField
    {
        public string DisplayName => Regex.Replace(Name, "(\\B[A-Z])", " $1");
        public string Name { get; set; }
        public string Description { get; set; }
        public PropertyInfo Info { get; set; }
        public object DefaultValue { get; set; }
    }
}