/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mjSurber/Map-Utilities
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace MapUtilities
{
    public class API
    {

        public bool registerCritterPack(Type t)
        {
            return registerCritterPack(t.Assembly);
        }

        public bool registerCritterPack(System.Reflection.Assembly assembly)
        {
            if (Critters.CritterSpawnData.assemblies.Contains(assembly))
                return false;
            Critters.CritterSpawnData.assemblies.Add(assembly);
            Logger.log("Registered assembly \"" + assembly.FullName + "\"");
            return true;
        }
    }
}
