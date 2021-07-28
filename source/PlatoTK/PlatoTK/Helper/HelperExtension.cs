/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;
using System.Linq;

namespace PlatoTK
{
    public static class HelperExtension
    {
        public static HashSet<IPlatoHelper> Helper = new HashSet<IPlatoHelper>();

        public static IPlatoHelper GetPlatoHelper(this IModHelper helper)
        {
            IPlatoHelper platoHelper = Helper.FirstOrDefault(p => p.ModHelper.ModRegistry.ModID == helper.ModRegistry.ModID);
            if (platoHelper is IPlatoHelper)
                return platoHelper;

            platoHelper = new PlatoHelper(helper);
            Helper.Add(platoHelper);

            return platoHelper;
        }

        public static IPlatoHelper GetPlatoHelper(this Mod mod) => mod.Helper.GetPlatoHelper();
    }
}
