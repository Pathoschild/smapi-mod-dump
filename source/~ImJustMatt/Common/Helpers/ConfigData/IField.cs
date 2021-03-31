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

namespace Helpers.ConfigData
{
    internal interface IField
    {
        string DisplayName { get; }
        string Name { get; set; }
        string Description { get; set; }
        PropertyInfo Info { get; set; }
        object DefaultValue { get; set; }
    }
}