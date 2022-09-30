/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.StardustCore.Compatibility.SpaceCore
{
    public interface SpaceCoreAPI
    {
        /// Must take (Event, GameLocation, GameTime, string[])
        void AddEventCommand(string command, MethodInfo info);

        /// Must have [XmlType("Mods_SOMETHINGHERE")] attribute (required to start with "Mods_")
        void RegisterSerializerType(Type type);
    }
}
