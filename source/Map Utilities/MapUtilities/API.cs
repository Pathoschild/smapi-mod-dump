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
